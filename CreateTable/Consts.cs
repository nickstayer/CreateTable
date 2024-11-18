namespace CreateTable;

public class Consts
{
    public static char[] REPLACEMENT_CHARACTERS = [ ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '`', '{', '|', '}', '~' ];
    public const char VALID_SEPARATOR = '_';
    public const string TABLENAME_DEFAULT = "my_table";
    public const int HEADER_MAX_LENGTH = 60;
    public const string ADDED_SYMBOL_FOR_UNIQ = "another_";
    public const string DESCRIPRION = "Программа принимает файл в формате *.csv. "
        + $"Возвращает запрос на создание таблицы в базе данных.";

    static Consts()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static Encoding Windows1251()
    {
        return Encoding.GetEncoding("windows-1251");
    }

    public static Encoding Utf8()
    {
        return Encoding.GetEncoding("utf-8");
    }
}
