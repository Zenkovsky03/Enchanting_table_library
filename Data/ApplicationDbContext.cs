using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BookTag> BookTags { get; set; }
    public DbSet<BookFile> BookFiles { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<SavedSearch> SavedSearches { get; set; }
    public DbSet<News> News { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Klucz złożony dla BookTag (wiele-do-wielu)
        builder.Entity<BookTag>()
            .HasKey(bt => new { bt.BookId, bt.TagId });
    }
}
