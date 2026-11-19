using Lab3Json.Models;

namespace Lab3Json.Services;

/// <summary>
/// Пошук по списку видань за допомогою LINQ.
/// Критерії: автор, вид видання, діапазон років, читач, ключове слово + сортування.
/// Кожен критерій додається до запиту лише тоді, коли його задано.
/// </summary>
public static class BookSearchService
{
    public static List<Book> Search(IEnumerable<Book> books, SearchCriteria criteria)
    {
        IEnumerable<Book> query = books;

        // 1. За автором (частина прізвища, без урахування регістру)
        if (!string.IsNullOrWhiteSpace(criteria.Author))
        {
            query = query.Where(b => b.Authors.Any(a => Contains(a, criteria.Author)));
        }

        // 2. За видом видання (значення з файлу)
        if (!string.IsNullOrWhiteSpace(criteria.Type))
        {
            query = query.Where(b => b.Type == criteria.Type);
        }

        // 3. За діапазоном років
        if (criteria.YearFrom is int from)
        {
            query = query.Where(b => b.Year >= from);
        }

        if (criteria.YearTo is int to)
        {
            query = query.Where(b => b.Year <= to);
        }

        // 4. За читачем — пошук у вкладеному списку
        if (!string.IsNullOrWhiteSpace(criteria.Reader))
        {
            query = from b in query
                    where b.Readers.Any(r => Contains(r.FullName, criteria.Reader))
                    select b;
        }

        // 5. Ключове слово в назві або анотації
        if (!string.IsNullOrWhiteSpace(criteria.Keyword))
        {
            query = query.Where(b => Contains(b.Title, criteria.Keyword) || Contains(b.Annotation, criteria.Keyword));
        }

        query = criteria.Sort switch
        {
            SortOrder.ByTitle => query.OrderBy(b => b.Title),
            SortOrder.ByYearDescending => query.OrderByDescending(b => b.Year).ThenBy(b => b.Title),
            SortOrder.ByReadersDescending => query.OrderByDescending(b => b.Readers.Count).ThenBy(b => b.Title),
            _ => query.OrderBy(b => b.Id)
        };

        return query.ToList();
    }

    private static bool Contains(string text, string part) =>
        text.Contains(part.Trim(), StringComparison.CurrentCultureIgnoreCase);
}
