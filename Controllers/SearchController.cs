using System.Linq.Expressions;
using Biblioteka.Data;
using Biblioteka.Infrastructure;
using Biblioteka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Advanced([FromQuery] AdvancedSearchQuery query)
    {
        var model = new AdvancedSearchViewModel
        {
            Query = query,
            Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
            Tags = await _context.Tags.OrderBy(t => t.Name).ToListAsync(),
            HasSearched = query.HasAnyCriteria(),
        };

        var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        model.CartCount = cart.Sum(x => x.Quantity);

        if (!model.HasSearched)
        {
            return View(model);
        }

        var booksQuery = _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
            .AsQueryable();

        var predicate = BuildPredicate(query);
        if (predicate != null)
        {
            booksQuery = booksQuery.Where(predicate);
        }

        if (query.CategoryId.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.CategoryId == query.CategoryId.Value);
        }

        if (query.TagIds != null && query.TagIds.Length > 0)
        {
            var tagIds = query.TagIds;
            booksQuery = booksQuery.Where(b => b.BookTags.Any(bt => tagIds.Contains(bt.TagId)));
        }

        model.Results = await booksQuery
            .OrderBy(b => b.Title)
            .ToListAsync();

        return View(model);
    }

    private static Expression<Func<Book, bool>>? BuildPredicate(AdvancedSearchQuery query)
    {
        var includeConditions = new List<Expression<Func<Book, bool>>>();
        var excludeConditions = new List<Expression<Func<Book, bool>>>();

        // Warunki włączające (szukaj)
        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            includeConditions.Add(MakeStringCondition(nameof(Book.Title), query.Title.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(query.Author))
        {
            includeConditions.Add(MakeNullableStringCondition(nameof(Book.Author), query.Author.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(query.Isbn))
        {
            includeConditions.Add(MakeNullableStringCondition(nameof(Book.Isbn), query.Isbn.Trim()));
        }

        // Warunki wykluczające (NOT)
        if (!string.IsNullOrWhiteSpace(query.ExcludeTitle))
        {
            excludeConditions.Add(MakeStringCondition(nameof(Book.Title), query.ExcludeTitle.Trim(), negate: true));
        }

        if (!string.IsNullOrWhiteSpace(query.ExcludeAuthor))
        {
            excludeConditions.Add(MakeNullableStringCondition(nameof(Book.Author), query.ExcludeAuthor.Trim(), negate: true));
        }

        Expression<Func<Book, bool>>? result = null;

        // Łączenie warunków włączających według trybu
        if (includeConditions.Count > 0)
        {
            result = includeConditions[0];
            for (int i = 1; i < includeConditions.Count; i++)
            {
                result = query.Mode == SearchMode.All
                    ? CombineAnd(result, includeConditions[i])
                    : CombineOr(result, includeConditions[i]);
            }
        }

        // Warunki wykluczające zawsze łączone przez AND
        foreach (var exclude in excludeConditions)
        {
            result = result == null ? exclude : CombineAnd(result, exclude);
        }

        return result;
    }

    private static Expression<Func<Book, bool>> CombineAnd(
        Expression<Func<Book, bool>> left,
        Expression<Func<Book, bool>> right)
    {
        var param = Expression.Parameter(typeof(Book), "b");
        var leftBody = new ReplaceParameterVisitor(left.Parameters[0], param).Visit(left.Body)!;
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], param).Visit(right.Body)!;
        var body = Expression.AndAlso(leftBody, rightBody);
        return Expression.Lambda<Func<Book, bool>>(body, param);
    }

    private static Expression<Func<Book, bool>> CombineOr(
        Expression<Func<Book, bool>> left,
        Expression<Func<Book, bool>> right)
    {
        var param = Expression.Parameter(typeof(Book), "b");
        var leftBody = new ReplaceParameterVisitor(left.Parameters[0], param).Visit(left.Body)!;
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], param).Visit(right.Body)!;
        var body = Expression.OrElse(leftBody, rightBody);
        return Expression.Lambda<Func<Book, bool>>(body, param);
    }

    private static Expression<Func<Book, bool>> MakeStringCondition(string propertyName, string value, bool negate = false)
    {
        var param = Expression.Parameter(typeof(Book), "b");
        var prop = Expression.Property(param, propertyName);
        var contains = Expression.Call(prop, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(value));
        Expression body = negate ? Expression.Not(contains) : contains;
        return Expression.Lambda<Func<Book, bool>>(body, param);
    }

    private static Expression<Func<Book, bool>> MakeNullableStringCondition(string propertyName, string value, bool negate = false)
    {
        var param = Expression.Parameter(typeof(Book), "b");
        var prop = Expression.Property(param, propertyName); // string?
        var notNull = Expression.NotEqual(prop, Expression.Constant(null, typeof(string)));
        var contains = Expression.Call(prop, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(value));
        var safeContains = Expression.AndAlso(notNull, contains);
        Expression body = negate ? Expression.Not(safeContains) : safeContains;
        return Expression.Lambda<Func<Book, bool>>(body, param);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _from ? _to : base.VisitParameter(node);
        }
    }
}
