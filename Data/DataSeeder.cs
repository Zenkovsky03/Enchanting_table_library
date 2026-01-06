using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Models; // Upewnij się, że masz odpowiednie namespace'y

namespace Biblioteka.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Sprawdź czy dane już istnieją
        if (await context.Categories.AnyAsync())
        {
            return; // Dane już zaseedowane
        }

        // ========== KATEGORIE ==========
        var categories = new List<Category>
        {
            // Główne kategorie
            new() { Id = 1, Name = "Literatura piękna", ParentCategoryId = null },
            new() { Id = 2, Name = "Literatura faktu", ParentCategoryId = null },
            new() { Id = 3, Name = "Nauka i edukacja", ParentCategoryId = null },
            new() { Id = 4, Name = "Dla dzieci i młodzieży", ParentCategoryId = null },
            new() { Id = 5, Name = "Poradniki", ParentCategoryId = null },

            // Podkategorie - Literatura piękna
            new() { Id = 6, Name = "Powieść obyczajowa", ParentCategoryId = 1 },
            new() { Id = 7, Name = "Kryminał i thriller", ParentCategoryId = 1 },
            new() { Id = 8, Name = "Fantasy i sci-fi", ParentCategoryId = 1 },
            new() { Id = 9, Name = "Romans", ParentCategoryId = 1 },
            new() { Id = 10, Name = "Klasyka literatury", ParentCategoryId = 1 },
            new() { Id = 11, Name = "Horror", ParentCategoryId = 1 },

            // Podkategorie - Literatura faktu
            new() { Id = 12, Name = "Biografie i wspomnienia", ParentCategoryId = 2 },
            new() { Id = 13, Name = "Historia", ParentCategoryId = 2 },
            new() { Id = 14, Name = "Reportaż", ParentCategoryId = 2 },
            new() { Id = 15, Name = "Publicystyka", ParentCategoryId = 2 },

            // Podkategorie - Nauka i edukacja
            new() { Id = 16, Name = "Informatyka", ParentCategoryId = 3 },
            new() { Id = 17, Name = "Psychologia", ParentCategoryId = 3 },
            new() { Id = 18, Name = "Ekonomia i biznes", ParentCategoryId = 3 },
            new() { Id = 19, Name = "Przyrodnicze", ParentCategoryId = 3 },
            new() { Id = 20, Name = "Matematyka", ParentCategoryId = 3 },

            // Podkategorie - Dla dzieci i młodzieży
            new() { Id = 21, Name = "Bajki i baśnie", ParentCategoryId = 4 },
            new() { Id = 22, Name = "Powieści dla młodzieży", ParentCategoryId = 4 },
            new() { Id = 23, Name = "Książki edukacyjne", ParentCategoryId = 4 },
            new() { Id = 24, Name = "Komiksy", ParentCategoryId = 4 },

            // Podkategorie - Poradniki
            new() { Id = 25, Name = "Rozwój osobisty", ParentCategoryId = 5 },
            new() { Id = 26, Name = "Zdrowie i sport", ParentCategoryId = 5 },
            new() { Id = 27, Name = "Kuchnia", ParentCategoryId = 5 },
            new() { Id = 28, Name = "Dom i ogród", ParentCategoryId = 5 },
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // ========== TAGI ==========
        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "Bestseller" },
            new() { Id = 2, Name = "Nagroda literacka" },
            new() { Id = 3, Name = "Ekranizacja" },
            new() { Id = 4, Name = "Polska literatura" },
            new() { Id = 5, Name = "Klasyka" },
            new() { Id = 6, Name = "Współczesna" },
            new() { Id = 7, Name = "Seria" },
            new() { Id = 8, Name = "Debiut" },
            new() { Id = 9, Name = "Lektura szkolna" },
            new() { Id = 10, Name = "Audiobook dostępny" },
            new() { Id = 11, Name = "E-book dostępny" },
            new() { Id = 12, Name = "Nowość" },
        };

        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();

        // ========== KSIĄŻKI ==========
        var books = new List<Book>
        {
            // Kryminał i thriller
            new() { Id = 1, Title = "Dziewczyna z pociągu", Author = "Paula Hawkins", Isbn = "978-83-287-0123-4", CategoryId = 7, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-120), IsNew = false,
                Description = "Rachel codziennie dojeżdża pociągiem do Londynu. Zna już trasę na pamięć: te same domy przy torach, te same ogrody. Obserwuje szczególnie jedno miejsce – dom, gdzie mieszka para, którą nazwała Jess i Jason. Pewnego dnia zauważa coś szokującego. I wkrótce Rachel zostaje wciągnięta w morderstwo.",
                TableOfContentsExcerpt = "Część pierwsza: Rachel • Megan • Anna\nCzęść druga: Śledztwo\nCzęść trzecia: Prawda" },

            new() { Id = 2, Title = "Milczenie owiec", Author = "Thomas Harris", Isbn = "978-83-287-0124-5", CategoryId = 7, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-200), IsNew = false,
                Description = "Młoda agentka FBI Clarice Starling musi skonsultować się z genialnym psychiatrą i kanibalem Hannibalem Lecterem, by złapać seryjnego mordercę Buffalo Billa. Wciągająca gra psychologiczna między łowcą a bestią.",
                TableOfContentsExcerpt = "Rozdział 1-10: Pierwsze spotkanie\nRozdział 11-25: Polowanie\nRozdział 26-40: Konfrontacja" },

            new() { Id = 3, Title = "Zaginiona dziewczyna", Author = "Gillian Flynn", Isbn = "978-83-287-0125-6", CategoryId = 7, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-90), IsNew = false,
                Description = "W dniu piątej rocznicy ślubu Nick Dunne wraca do domu i odkrywa, że jego żona Amy zaginęła. W miarę jak śledztwo się rozwija, wszystkie poszlaki zaczynają wskazywać na niego jako głównego podejrzanego.",
                TableOfContentsExcerpt = "Część I: Chłopiec traci dziewczynę\nCzęść II: Chłopiec spotyka dziewczynę\nCzęść III: Chłopiec odzyskuje dziewczynę" },

            new() { Id = 4, Title = "Kobieta w oknie", Author = "A.J. Finn", Isbn = "978-83-287-0126-7", CategoryId = 7, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-60), IsNew = false,
                Description = "Anna Fox cierpi na agorafobię i nie wychodzi z domu od dziesięciu miesięcy. Obserwuje sąsiadów przez okno. Pewnego wieczoru widzi coś, czego nie powinna zobaczyć – i jej życie zmienia się na zawsze.", TableOfContentsExcerpt = "" },

            // Fantasy i sci-fi
            new() { Id = 5, Title = "Wiedźmin. Ostatnie życzenie", Author = "Andrzej Sapkowski", Isbn = "978-83-287-0127-8", CategoryId = 8, StockCount = 5, AddedDate = DateTime.UtcNow.AddDays(-365), IsNew = false,
                Description = "Geralt z Rivii to wiedźmin – zabójca potworów za pieniądze. W świecie, gdzie ludzie bywają gorsi od bestii, musi odnaleźć swoją drogę. Pierwszy tom kultowej sagi, która podbiła cały świat.",
                TableOfContentsExcerpt = "Wiedźmin\nDroga, z której się nie wraca\nZiarno prawdy\nMniejsze zło\nKwestia ceny\nKraniec świata\nOstatnie życzenie" },

            new() { Id = 6, Title = "Władca Pierścieni. Drużyna Pierścienia", Author = "J.R.R. Tolkien", Isbn = "978-83-287-0128-9", CategoryId = 8, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-500), IsNew = false,
                Description = "Hobbit Frodo Baggins wyrusza w niebezpieczną podróż, by zniszczyć Jedyny Pierścień. Pierwszy tom epickiej trylogii, która zdefiniowała gatunek fantasy na całe pokolenia.",
                TableOfContentsExcerpt = "Księga I: Droga w nieznane\nKsięga II: Drużyna Pierścienia" },

            new() { Id = 7, Title = "Diuna", Author = "Frank Herbert", Isbn = "978-83-287-0129-0", CategoryId = 8, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-400), IsNew = false,
                Description = "Na pustynnej planecie Arrakis, jedynym źródle najcenniejszej substancji we wszechświecie, rozgrywa się epicka historia młodego Paula Atrydy. Arcydzieło science fiction.",
                TableOfContentsExcerpt = "Księga I: Diuna\nKsięga II: Muad'Dib\nKsięga III: Prorok" },

            new() { Id = 8, Title = "Gra o tron", Author = "George R.R. Martin", Isbn = "978-83-287-0130-6", CategoryId = 8, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-300), IsNew = false,
                Description = "W Siedmiu Królestwach trwa bezwzględna walka o władzę. W grze o tron wygrywasz albo giniesz. Pierwszy tom bestsellerowej sagi Pieśń Lodu i Ognia.", TableOfContentsExcerpt = "" },

            new() { Id = 9, Title = "Metro 2033", Author = "Dmitry Glukhovsky", Isbn = "978-83-287-0131-7", CategoryId = 8, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-250), IsNew = false,
                Description = "Po wojnie atomowej ocaleli ludzie schronili się w moskiewskim metrze. Artiom musi wyruszyć w niebezpieczną podróż przez tunele pełne zagrożeń, by ocalić swoją stację.", TableOfContentsExcerpt = "" },

            // Klasyka literatury
            new() { Id = 10, Title = "Zbrodnia i kara", Author = "Fiodor Dostojewski", Isbn = "978-83-287-0132-8", CategoryId = 10, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-600), IsNew = false,
                Description = "Student Raskolnikow popełnia morderstwo, przekonany o swojej wyższości moralnej. Psychologiczne arcydzieło o winie, karze i odkupieniu.",
                TableOfContentsExcerpt = "Część I: Morderstwo\nCzęść II-V: Konsekwencje\nCzęść VI: Epilog" },

            new() { Id = 11, Title = "Duma i uprzedzenie", Author = "Jane Austen", Isbn = "978-83-287-0133-9", CategoryId = 10, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-550), IsNew = false,
                Description = "Historia Elizabeth Bennet i pana Darcy'ego – dwojga ludzi, którzy muszą pokonać własną dumę i uprzedzenia, by odnaleźć prawdziwą miłość. Ponadczasowa klasyka.", TableOfContentsExcerpt = "" },

            new() { Id = 12, Title = "1984", Author = "George Orwell", Isbn = "978-83-287-0134-0", CategoryId = 10, StockCount = 5, AddedDate = DateTime.UtcNow.AddDays(-450), IsNew = false,
                Description = "W totalitarnym państwie Oceania Winston Smith pracuje w Ministerstwie Prawdy, fałszując historię. Ale jego myśli są nadal wolne – jeszcze. Wizjonerska antyutopia.", TableOfContentsExcerpt = "" },

            new() { Id = 13, Title = "Mistrz i Małgorzata", Author = "Michaił Bułhakow", Isbn = "978-83-287-0135-1", CategoryId = 10, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-380), IsNew = false,
                Description = "Szatan przybywa do ateistycznej Moskwy lat 30. wraz ze swoją świtą. Równolegle toczy się historia Mistrza i jego ukochanej. Fantastyczna satyra na sowiecką rzeczywistość.", TableOfContentsExcerpt = "" },

            // Biografie i wspomnienia
            new() { Id = 14, Title = "Steve Jobs", Author = "Walter Isaacson", Isbn = "978-83-287-0136-2", CategoryId = 12, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-280), IsNew = false,
                Description = "Autoryzowana biografia współtwórcy Apple'a. Historia wizjonera, który zmienił świat technologii, oparta na ponad 40 wywiadach z Jobsem i setkami rozmów z rodziną i współpracownikami.", TableOfContentsExcerpt = "" },

            new() { Id = 15, Title = "Becoming. Moja historia", Author = "Michelle Obama", Isbn = "978-83-287-0137-3", CategoryId = 12, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-180), IsNew = false,
                Description = "Inspirująca autobiografia byłej Pierwszej Damy USA. Od dzieciństwa na South Side Chicago, przez Princeton i Harvard, po Biały Dom.", TableOfContentsExcerpt = "" },

            // Historia
            new() { Id = 16, Title = "Sapiens. Od zwierząt do bogów", Author = "Yuval Noah Harari", Isbn = "978-83-287-0138-4", CategoryId = 13, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-320), IsNew = false,
                Description = "Fascynująca opowieść o tym, jak gatunek Homo sapiens stał się władcą Ziemi. Od rewolucji poznawczej, przez rolniczą i naukową, po współczesność.", TableOfContentsExcerpt = "" },

            new() { Id = 17, Title = "Krótka historia prawie wszystkiego", Author = "Bill Bryson", Isbn = "978-83-287-0139-5", CategoryId = 13, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-400), IsNew = false,
                Description = "Dowcipna i przystępna podróż przez historię nauki – od Wielkiego Wybuchu po DNA. Bryson wyjaśnia, jak dowiedzieliśmy się tego wszystkiego, co wiemy o wszechświecie.", TableOfContentsExcerpt = "" },

            // Informatyka
            new() { Id = 18, Title = "Czysty kod. Podręcznik dobrego programisty", Author = "Robert C. Martin", Isbn = "978-83-287-0140-1", CategoryId = 16, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-150), IsNew = false,
                Description = "Biblia programisty o pisaniu czytelnego, łatwego w utrzymaniu kodu. Praktyczne wskazówki i przykłady, które zmienią twoje podejście do programowania.",
                TableOfContentsExcerpt = "1. Czysty kod\n2. Znaczące nazwy\n3. Funkcje\n4. Komentarze\n5. Formatowanie\n6. Obiekty i struktury danych" },

            new() { Id = 19, Title = "Pragmatyczny programista", Author = "David Thomas, Andrew Hunt", Isbn = "978-83-287-0141-2", CategoryId = 16, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-100), IsNew = false,
                Description = "Klasyk literatury IT. Ponadczasowe porady dotyczące rzemiosła programistycznego, od architektury po pracę zespołową.", TableOfContentsExcerpt = "" },

            new() { Id = 20, Title = "Algorytmy. Ilustrowany przewodnik", Author = "Aditya Bhargava", Isbn = "978-83-287-0142-3", CategoryId = 16, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-80), IsNew = true,
                Description = "Przystępne wprowadzenie do algorytmów z ilustracjami. Sortowanie, wyszukiwanie, grafy, programowanie dynamiczne – wszystko wyjaśnione krok po kroku.", TableOfContentsExcerpt = "" },

            // Psychologia
            new() { Id = 21, Title = "Pułapki myślenia", Author = "Daniel Kahneman", Isbn = "978-83-287-0143-4", CategoryId = 17, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-220), IsNew = false,
                Description = "Laureat Nagrody Nobla pokazuje, jak nasz umysł nas oszukuje. Dwa systemy myślenia i ich wpływ na nasze decyzje. Fundamentalna książka o psychologii poznawczej.", TableOfContentsExcerpt = "" },

            new() { Id = 22, Title = "Człowiek w poszukiwaniu sensu", Author = "Viktor E. Frankl", Isbn = "978-83-287-0144-5", CategoryId = 17, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-350), IsNew = false,
                Description = "Psychiatra, który przeżył obozy koncentracyjne, opisuje jak znaleźć sens życia nawet w najcięższych okolicznościach. Podstawy logoterapii.", TableOfContentsExcerpt = "" },

            // Rozwój osobisty
            new() { Id = 23, Title = "Atomowe nawyki", Author = "James Clear", Isbn = "978-83-287-0145-6", CategoryId = 25, StockCount = 5, AddedDate = DateTime.UtcNow.AddDays(-45), IsNew = true,
                Description = "Jak budować dobre nawyki i pozbywać się złych? Praktyczny system małych zmian, które prowadzą do wielkich rezultatów. Bestsellerowy poradnik.",
                TableOfContentsExcerpt = "I. Fundamenty\nII. Prawo 1: Uczyń to oczywistym\nIII. Prawo 2: Uczyń to atrakcyjnym\nIV. Prawo 3: Uczyń to łatwym\nV. Prawo 4: Uczyń to satysfakcjonującym" },

            new() { Id = 24, Title = "Głębia. Jak pracować mądrze", Author = "Cal Newport", Isbn = "978-83-287-0146-7", CategoryId = 25, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-70), IsNew = true,
                Description = "W świecie ciągłych rozpraszaczy głęboka praca staje się supermocą. Newport pokazuje jak się skupić i osiągać więcej w krótszym czasie.", TableOfContentsExcerpt = "" },

            // Dla młodzieży
            new() { Id = 25, Title = "Harry Potter i Kamień Filozoficzny", Author = "J.K. Rowling", Isbn = "978-83-287-0147-8", CategoryId = 22, StockCount = 6, AddedDate = DateTime.UtcNow.AddDays(-700), IsNew = false,
                Description = "Jedenastoletni Harry dowiaduje się, że jest czarodziejem i zostaje przyjęty do Szkoły Magii i Czarodziejstwa w Hogwarcie. Początek magicznej przygody.", TableOfContentsExcerpt = "" },

            new() { Id = 26, Title = "Igrzyska śmierci", Author = "Suzanne Collins", Isbn = "978-83-287-0148-9", CategoryId = 22, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-450), IsNew = false,
                Description = "W dystopijnym Panem nastolatki walczą na śmierć i życie w telewizyjnym show. Katniss Everdeen zgłasza się na ochotnika zamiast swojej siostry.", TableOfContentsExcerpt = "" },

            new() { Id = 27, Title = "Ferdynand Wspaniały", Author = "Ludwik Jerzy Kern", Isbn = "978-83-287-0149-0", CategoryId = 21, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-500), IsNew = false,
                Description = "Kultowa polska książka dla dzieci o niezwykłym psie Ferdynandzie i jego przygodach. Klasyka polskiej literatury dziecięcej.", TableOfContentsExcerpt = "" },

            // Reportaż
            new() { Id = 28, Title = "Kapuściński non-fiction", Author = "Artur Domosławski", Isbn = "978-83-287-0150-6", CategoryId = 14, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-280), IsNew = false,
                Description = "Biografia legendarnego reportera Ryszarda Kapuścińskiego. Kontrowersyjna książka o granicach między reportażem a literaturą.", TableOfContentsExcerpt = "" },

            new() { Id = 29, Title = "Wielki Post", Author = "Mariusz Szczygieł", Isbn = "978-83-287-0151-7", CategoryId = 14, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-60), IsNew = true,
                Description = "Nowy zbiór reportaży mistrza polskiego reportażu literackiego. Szczygieł wraca do Czech i opowiada o ludziach, którzy odeszli od Kościoła.", TableOfContentsExcerpt = "" },

            // Ekonomia i biznes
            new() { Id = 30, Title = "Bogaty ojciec, biedny ojciec", Author = "Robert Kiyosaki", Isbn = "978-83-287-0152-8", CategoryId = 18, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-400), IsNew = false,
                Description = "Bestsellerowa książka o finansach osobistych. Kiyosaki dzieli się lekcjami od dwóch ojców – własnego (biednego) i ojca przyjaciela (bogatego).", TableOfContentsExcerpt = "" },

            new() { Id = 31, Title = "Myśl i bogać się", Author = "Napoleon Hill", Isbn = "978-83-287-0153-9", CategoryId = 18, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-600), IsNew = false,
                Description = "Klasyk literatury motywacyjnej z 1937 roku. 13 zasad sukcesu opartych na wywiadach z 500 milionerami, w tym Andrew Carnegie i Henrym Fordem.", TableOfContentsExcerpt = "" },

            // Zdrowie i sport
            new() { Id = 32, Title = "Nie jem śmietnika", Author = "Anna Lewandowska", Isbn = "978-83-287-0154-0", CategoryId = 26, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-180), IsNew = false,
                Description = "Poradnik zdrowego odżywiania od trenerki personalnej i dietetyczki. Przepisy, plany żywieniowe i wskazówki dotyczące aktywnego stylu życia.", TableOfContentsExcerpt = "" },

            // Kuchnia
            new() { Id = 33, Title = "Kuchnia polska. Tradycyjne przepisy", Author = "Jan Czernikowski", Isbn = "978-83-287-0155-1", CategoryId = 27, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-250), IsNew = false,
                Description = "Kompendium polskich przepisów kulinarnych. Od bigosu i pierogów po sernik i makowiec. Przepisy babć i prababć w nowoczesnym wydaniu.", TableOfContentsExcerpt = "" },

            // Nowe książki - dodane w ostatnim miesiącu
            new() { Id = 34, Title = "Projekt: Hail Mary", Author = "Andy Weir", Isbn = "978-83-287-0156-2", CategoryId = 8, StockCount = 4, AddedDate = DateTime.UtcNow.AddDays(-15), IsNew = true,
                Description = "Ryland Grace budzi się sam na statku kosmicznym, nie pamiętając kim jest ani jak się tu znalazł. Jedyne co wie – to że od niego zależy przyszłość ludzkości. Od autora „Marsjanina\"" },

            new() { Id = 35, Title = "Czwarta strona", Author = "Zygmunt Miłoszewski", Isbn = "978-83-287-0157-3", CategoryId = 7, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-10), IsNew = true,
                Description = "Najnowszy thriller mistrza polskiego kryminału. Prokurator Teodor Szacki powraca w najbardziej skomplikowanej sprawie swojej kariery.", TableOfContentsExcerpt = "" },

            new() { Id = 36, Title = "Homo Deus. Krótka historia jutra", Author = "Yuval Noah Harari", Isbn = "978-83-287-0158-4", CategoryId = 13, StockCount = 3, AddedDate = DateTime.UtcNow.AddDays(-30), IsNew = true,
                Description = "Co czeka ludzkość w przyszłości? Harari, autor Sapiens, analizuje dokąd zmierzamy jako gatunek. Sztuczna inteligencja, inżynieria genetyczna i boskość.", TableOfContentsExcerpt = "" },

            new() { Id = 37, Title = "Cała prawda o nas", Author = "Colleen Hoover", Isbn = "978-83-287-0159-5", CategoryId = 9, StockCount = 5, AddedDate = DateTime.UtcNow.AddDays(-20), IsNew = true,
                Description = "Emocjonalna opowieść o miłości, wyborach i konsekwencjach. Hoover po raz kolejny sięga do serca czytelników w swojej najnowszej powieści.", TableOfContentsExcerpt = "" },

            new() { Id = 38, Title = "System. Audiobook", Author = "James Clear", Isbn = "978-83-287-0160-1", CategoryId = 25, StockCount = 2, AddedDate = DateTime.UtcNow.AddDays(-5), IsNew = true,
                Description = "Kontynuacja bestsellerowych „Atomowych nawyków\". Clear pokazuje jak stworzyć system, który będzie pracował na twój sukces każdego dnia." },
        };

        context.Books.AddRange(books);
        await context.SaveChangesAsync();

        // ========== POWIĄZANIA BOOK-TAG ==========
        var bookTags = new List<BookTag>
        {
            // Dziewczyna z pociągu
            new() { BookId = 1, TagId = 1 }, // Bestseller
            new() { BookId = 1, TagId = 3 }, // Ekranizacja
            new() { BookId = 1, TagId = 6 }, // Współczesna

            // Milczenie owiec
            new() { BookId = 2, TagId = 3 }, // Ekranizacja
            new() { BookId = 2, TagId = 5 }, // Klasyka

            // Zaginiona dziewczyna
            new() { BookId = 3, TagId = 1 }, // Bestseller
            new() { BookId = 3, TagId = 3 }, // Ekranizacja

            // Wiedźmin
            new() { BookId = 5, TagId = 4 }, // Polska literatura
            new() { BookId = 5, TagId = 3 }, // Ekranizacja
            new() { BookId = 5, TagId = 7 }, // Seria
            new() { BookId = 5, TagId = 1 }, // Bestseller

            // Władca Pierścieni
            new() { BookId = 6, TagId = 5 }, // Klasyka
            new() { BookId = 6, TagId = 3 }, // Ekranizacja
            new() { BookId = 6, TagId = 7 }, // Seria

            // Diuna
            new() { BookId = 7, TagId = 5 }, // Klasyka
            new() { BookId = 7, TagId = 3 }, // Ekranizacja
            new() { BookId = 7, TagId = 2 }, // Nagroda literacka

            // Gra o tron
            new() { BookId = 8, TagId = 1 }, // Bestseller
            new() { BookId = 8, TagId = 3 }, // Ekranizacja
            new() { BookId = 8, TagId = 7 }, // Seria

            // Metro 2033
            new() { BookId = 9, TagId = 7 }, // Seria
            new() { BookId = 9, TagId = 3 }, // Ekranizacja (gra)

            // Klasyki
            new() { BookId = 10, TagId = 5 }, // Klasyka
            new() { BookId = 10, TagId = 9 }, // Lektura szkolna
            new() { BookId = 11, TagId = 5 }, // Klasyka
            new() { BookId = 11, TagId = 3 }, // Ekranizacja
            new() { BookId = 12, TagId = 5 }, // Klasyka
            new() { BookId = 12, TagId = 9 }, // Lektura szkolna
            new() { BookId = 13, TagId = 5 }, // Klasyka

            // Biografie
            new() { BookId = 14, TagId = 1 }, // Bestseller
            new() { BookId = 15, TagId = 1 }, // Bestseller

            // Historia
            new() { BookId = 16, TagId = 1 }, // Bestseller
            new() { BookId = 16, TagId = 2 }, // Nagroda literacka

            // IT
            new() { BookId = 18, TagId = 1 }, // Bestseller
            new() { BookId = 20, TagId = 12 }, // Nowość

            // Psychologia
            new() { BookId = 21, TagId = 2 }, // Nagroda literacka
            new() { BookId = 21, TagId = 1 }, // Bestseller

            // Rozwój osobisty
            new() { BookId = 23, TagId = 1 }, // Bestseller
            new() { BookId = 23, TagId = 12 }, // Nowość
            new() { BookId = 24, TagId = 12 }, // Nowość

            // Młodzieżowe
            new() { BookId = 25, TagId = 1 }, // Bestseller
            new() { BookId = 25, TagId = 3 }, // Ekranizacja
            new() { BookId = 25, TagId = 7 }, // Seria
            new() { BookId = 26, TagId = 3 }, // Ekranizacja
            new() { BookId = 26, TagId = 7 }, // Seria
            new() { BookId = 27, TagId = 4 }, // Polska literatura

            // Reportaż
            new() { BookId = 29, TagId = 4 }, // Polska literatura
            new() { BookId = 29, TagId = 12 }, // Nowość

            // Nowe książki
            new() { BookId = 34, TagId = 12 }, // Nowość
            new() { BookId = 34, TagId = 1 }, // Bestseller
            new() { BookId = 35, TagId = 12 }, // Nowość
            new() { BookId = 35, TagId = 4 }, // Polska literatura
            new() { BookId = 36, TagId = 12 }, // Nowość
            new() { BookId = 36, TagId = 1 }, // Bestseller
            new() { BookId = 37, TagId = 12 }, // Nowość
            new() { BookId = 37, TagId = 1 }, // Bestseller
            new() { BookId = 38, TagId = 12 }, // Nowość
            new() { BookId = 38, TagId = 10 }, // Audiobook dostępny
        };

        context.BookTags.AddRange(bookTags);
        await context.SaveChangesAsync();

        // ========== AKTUALNOŚCI ==========
        var news = new List<News>
        {
            new() { Id = 1, Title = "Nowe godziny otwarcia w okresie świątecznym",
                Content = "Informujemy, że w okresie od 23 grudnia do 6 stycznia biblioteka będzie czynna w zmienionych godzinach:\n\n" +
                    "• 23-24 grudnia: 8:00 - 14:00\n" +
                    "• 25-26 grudnia: ZAMKNIĘTE\n" +
                    "• 27-30 grudnia: 9:00 - 17:00\n" +
                    "• 31 grudnia - 1 stycznia: ZAMKNIĘTE\n" +
                    "• 2-6 stycznia: 9:00 - 17:00\n\n" +
                    "Od 7 stycznia wracamy do standardowych godzin otwarcia. Życzymy wszystkim czytelnikom spokojnych świąt!",
                PublishDate = DateTime.UtcNow.AddDays(-7), IsPublished = true },

            new() { Id = 2, Title = "Spotkanie autorskie z Zygmuntem Miłoszewskim",
                Content = "Z przyjemnością zapraszamy na spotkanie z Zygmuntem Miłoszewskim, autorem bestsellerowych thrillerów o prokuratorze Teodorze Szackim.\n\n" +
                    "Autor opowie o swojej najnowszej książce „Czwarta strona\" i odpowie na pytania czytelników.\n\n" +
                    "**Kiedy:** 15 stycznia 2026, godz. 18:00\n" +
                    "**Gdzie:** Sala konferencyjna biblioteki\n" +
                    "**Wstęp:** Wolny, liczba miejsc ograniczona\n\n" +
                    "Zapisy w recepcji biblioteki lub telefonicznie pod numerem 12 345 67 89.",
                PublishDate = DateTime.UtcNow.AddDays(-3), IsPublished = true },

            new() { Id = 3, Title = "Konkurs „Moja ulubiona książka 2025\"",
                Content = "Zapraszamy do udziału w naszym dorocznym konkursie czytelniczym!\n\n" +
                    "Napisz krótką recenzję (do 500 słów) swojej ulubionej książki przeczytanej w 2025 roku. " +
                    "Na autorów trzech najciekawszych recenzji czekają atrakcyjne nagrody:\n\n" +
                    "🥇 I miejsce: Roczna karta biblioteczna premium + zestaw książek o wartości 200 zł\n" +
                    "🥈 II miejsce: Półroczna karta premium + zestaw książek o wartości 100 zł\n" +
                    "🥉 III miejsce: Zestaw książek o wartości 50 zł\n\n" +
                    "**Termin nadsyłania prac:** 31 stycznia 2026\n" +
                    "Recenzje można składać osobiście lub wysłać mailem na adres konkurs@biblioteka.pl",
                PublishDate = DateTime.UtcNow.AddDays(-1), IsPublished = true },

            new() { Id = 4, Title = "Nowe książki w zbiorach – grudzień 2025",
                Content = "Do naszych zbiorów trafiło prawie 50 nowych tytułów! Wśród nich znajdziecie:\n\n" +
                    "**Beletrystyka:**\n" +
                    "• „Projekt: Hail Mary\" - Andy Weir\n" +
                    "• „Czwarta strona\" - Zygmunt Miłoszewski\n" +
                    "• „Cała prawda o nas\" - Colleen Hoover\n\n" +
                    "**Literatura faktu:**\n" +
                    "• „Homo Deus\" - Yuval Noah Harari\n" +
                    "• „Wielki Post\" - Mariusz Szczygieł\n\n" +
                    "**Poradniki:**\n" +
                    "• „System\" - James Clear\n" +
                    "• „Algorytmy. Ilustrowany przewodnik\" - Aditya Bhargava\n\n" +
                    "Pełna lista dostępna w katalogu online. Zapraszamy!",
                PublishDate = DateTime.UtcNow, IsPublished = true },

            new() { Id = 5, Title = "Warsztaty dla dzieci – Ferie zimowe 2026",
                Content = "W czasie ferii zimowych zapraszamy dzieci w wieku 6-12 lat na bezpłatne warsztaty kreatywne!\n\n" +
                    "**Program:**\n" +
                    "• Poniedziałek: Tworzenie własnych zakładek do książek\n" +
                    "• Środa: Czytanie bajek + konkurs plastyczny\n" +
                    "• Piątek: Teatrzyk cieni – inscenizacja ulubionej bajki\n\n" +
                    "Zajęcia odbywają się w godz. 10:00-12:00 w sali dziecięcej.\n" +
                    "Zapisy u bibliotekarzy. Liczba miejsc ograniczona do 15 osób na każde zajęcia.",
                PublishDate = DateTime.UtcNow.AddDays(-14), IsPublished = true },
        };

        context.News.AddRange(news);
        await context.SaveChangesAsync();

        // ========== UŻYTKOWNICY ==========
        // Pracownicy
        var employees = new List<(string email, string name, string role)>
        {
            ("anna.kowalska@biblioteka.pl", "Anna Kowalska", "Employee"),
            ("jan.nowak@biblioteka.pl", "Jan Nowak", "Employee"),
            ("maria.wisniewska@biblioteka.pl", "Maria Wiśniewska", "Employee"),
        };

        foreach (var (email, name, role) in employees)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    IsApproved = true,
                    IsActive = true,
                };
                var result = await userManager.CreateAsync(user, "Pracownik123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        // Czytelnicy
        var readers = new List<(string email, string name, bool isApproved)>
        {
            ("czytelnik1@example.com", "Tomasz Zieliński", true),
            ("czytelnik2@example.com", "Katarzyna Dąbrowska", true),
            ("czytelnik3@example.com", "Michał Lewandowski", true),
            ("czytelnik4@example.com", "Agnieszka Kamińska", true),
            ("czytelnik5@example.com", "Piotr Szymański", true),
            ("czytelnik6@example.com", "Ewa Woźniak", true),
            ("czytelnik7@example.com", "Adam Kozłowski", true),
            ("czytelnik8@example.com", "Magdalena Jankowska", true),
            ("czytelnik9@example.com", "Krzysztof Mazur", false), // Niezatwierdzony
            ("czytelnik10@example.com", "Monika Krawczyk", false), // Niezatwierdzony
        };

        var readerUsers = new List<ApplicationUser>();
        foreach (var (email, name, isApproved) in readers)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    IsApproved = isApproved,
                    IsActive = true,
                };
                var result = await userManager.CreateAsync(user, "Czytelnik123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Reader");
                    readerUsers.Add(user);
                }
            }
            else
            {
                readerUsers.Add(existingUser);
            }
        }

        await context.SaveChangesAsync();

        // ========== HISTORIA WYPOŻYCZEŃ ==========
        var approvedReaders = readerUsers.Where(u => u.IsApproved).ToList();

        if (approvedReaders.Any())
        {
            var loans = new List<Loan>();

            // Wypożyczenia zakończone (zwrócone)
            var returnedLoans = new[]
            {
                (BookId: 5, UserIdx: 0, BorrowedDaysAgo: 60, ReturnedDaysAgo: 45),
                (BookId: 6, UserIdx: 0, BorrowedDaysAgo: 40, ReturnedDaysAgo: 25),
                (BookId: 1, UserIdx: 1, BorrowedDaysAgo: 50, ReturnedDaysAgo: 35),
                (BookId: 2, UserIdx: 1, BorrowedDaysAgo: 30, ReturnedDaysAgo: 15),
                (BookId: 23, UserIdx: 2, BorrowedDaysAgo: 45, ReturnedDaysAgo: 30),
                (BookId: 18, UserIdx: 3, BorrowedDaysAgo: 35, ReturnedDaysAgo: 20),
                (BookId: 12, UserIdx: 4, BorrowedDaysAgo: 55, ReturnedDaysAgo: 40),
                (BookId: 16, UserIdx: 5, BorrowedDaysAgo: 70, ReturnedDaysAgo: 50),
                (BookId: 25, UserIdx: 6, BorrowedDaysAgo: 25, ReturnedDaysAgo: 10),
                (BookId: 7, UserIdx: 7, BorrowedDaysAgo: 80, ReturnedDaysAgo: 60),
                (BookId: 10, UserIdx: 0, BorrowedDaysAgo: 90, ReturnedDaysAgo: 75),
                (BookId: 14, UserIdx: 2, BorrowedDaysAgo: 65, ReturnedDaysAgo: 50),
                (BookId: 21, UserIdx: 3, BorrowedDaysAgo: 40, ReturnedDaysAgo: 25),
                (BookId: 30, UserIdx: 4, BorrowedDaysAgo: 85, ReturnedDaysAgo: 70),
                (BookId: 8, UserIdx: 5, BorrowedDaysAgo: 100, ReturnedDaysAgo: 85),
            };

            foreach (var loan in returnedLoans)
            {
                if (loan.UserIdx < approvedReaders.Count)
                {
                    loans.Add(new Loan
                    {
                        BookId = loan.BookId,
                        UserId = approvedReaders[loan.UserIdx].Id,
                        Status = LoanStatus.Returned,
                        CreatedAt = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo - 2),
                        BorrowedAt = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo),
                        ReturnedAt = DateTime.UtcNow.AddDays(-loan.ReturnedDaysAgo),
                        DueDate = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo + 30),
                    });
                }
            }

            // Aktywne wypożyczenia
            var activeLoans = new[]
            {
                (BookId: 3, UserIdx: 0, BorrowedDaysAgo: 10),
                (BookId: 34, UserIdx: 1, BorrowedDaysAgo: 5),
                (BookId: 35, UserIdx: 2, BorrowedDaysAgo: 7),
                (BookId: 24, UserIdx: 3, BorrowedDaysAgo: 12),
                (BookId: 37, UserIdx: 4, BorrowedDaysAgo: 8),
                (BookId: 19, UserIdx: 5, BorrowedDaysAgo: 15),
                (BookId: 9, UserIdx: 6, BorrowedDaysAgo: 20),
            };

            foreach (var loan in activeLoans)
            {
                if (loan.UserIdx < approvedReaders.Count)
                {
                    loans.Add(new Loan
                    {
                        BookId = loan.BookId,
                        UserId = approvedReaders[loan.UserIdx].Id,
                        Status = LoanStatus.Borrowed,
                        CreatedAt = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo - 1),
                        BorrowedAt = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo),
                        DueDate = DateTime.UtcNow.AddDays(30 - loan.BorrowedDaysAgo),
                    });
                }
            }

            // Przeterminowane
            var overdueLoans = new[]
            {
                (BookId: 11, UserIdx: 7, BorrowedDaysAgo: 45),
                (BookId: 13, UserIdx: 0, BorrowedDaysAgo: 40),
            };

            foreach (var loan in overdueLoans)
            {
                if (loan.UserIdx < approvedReaders.Count)
                {
                    loans.Add(new Loan
                    {
                        BookId = loan.BookId,
                        UserId = approvedReaders[loan.UserIdx].Id,
                        Status = LoanStatus.Borrowed,
                        CreatedAt = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo - 1),
                        BorrowedAt = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo),
                        DueDate = DateTime.UtcNow.AddDays(-loan.BorrowedDaysAgo + 30),
                    });
                }
            }

            // Rezerwacje
            var onHoldLoans = new[]
            {
                (BookId: 36, UserIdx: 1),
                (BookId: 38, UserIdx: 3),
            };

            foreach (var loan in onHoldLoans)
            {
                if (loan.UserIdx < approvedReaders.Count)
                {
                    loans.Add(new Loan
                    {
                        BookId = loan.BookId,
                        UserId = approvedReaders[loan.UserIdx].Id,
                        Status = LoanStatus.OnHold,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                    });
                }
            }

            // Oczekiwanie w kolejce
            if (approvedReaders.Count > 6)
            {
                loans.Add(new Loan
                {
                    BookId = 34,
                    UserId = approvedReaders[5].Id,
                    Status = LoanStatus.Waiting,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                });

                loans.Add(new Loan
                {
                    BookId = 34,
                    UserId = approvedReaders[6].Id,
                    Status = LoanStatus.Waiting,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                });
            }

            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();

            // Aktualizacja StockCount
            var borrowedBookIds = loans
                .Where(l => l.Status == LoanStatus.Borrowed || l.Status == LoanStatus.OnHold)
                .Select(l => l.BookId)
                .ToList();

            foreach (var bookId in borrowedBookIds.Distinct())
            {
                var book = await context.Books.FindAsync(bookId);
                if (book != null)
                {
                    var borrowedCount = borrowedBookIds.Count(id => id == bookId);
                    book.StockCount = Math.Max(0, book.StockCount - borrowedCount);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}