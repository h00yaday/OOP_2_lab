namespace Lab3Json.Services;

/// <summary>Дані автора для форми «Про програму».</summary>
public static class AuthorInfo
{
    public const string FullName = "Черненко Іван Олегович";
    public const string Course = "2";
    public const string Group = "K-26";
    public const string Year = "2026";

    public const string Description =
        "Лабораторна робота №3. Диспетчер JSON-файлів.\n" +
        "Предметна область: «Кафедральна бібліотека» (видання та їх читачі).\n" +
        "Програма відкриває JSON-файл, показує список видань у таблиці, дозволяє додавати, " +
        "редагувати й видаляти записи через окрему форму, шукати за допомогою LINQ " +
        "та зберігати дані назад у JSON (серіалізація / десеріалізація System.Text.Json).";
}
