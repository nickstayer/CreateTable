using CreateTable;

Console.WriteLine(Consts.DESCRIPRION + Environment.NewLine);

while (true)
{
    Console.WriteLine($"Укажите путь к файлу:");
    var file = Console.ReadLine()?.Replace("\"", "");
    if (!File.Exists(file))
    {
        Console.WriteLine($"Файл не существует {file}");
        continue;
    }

    if (!file.Contains(".csv"))
    {
        Console.WriteLine($"Работаем только с .csv");
        continue;
    }

    try
    {
        Console.WriteLine($"Нажмите 1, чтобы выбрать кодировку windows-1251. По умолчанию UTF8:");
        var encodingUTF8 = Consts.Utf8;
        var enconding1251 = Consts.Windows1251;
        Encoding encoding = Console.ReadLine() == "1" ? Consts.Windows1251() : Consts.Utf8();

        Console.WriteLine($"Нажмите 1, чтобы выбрать рездлителем: ';' . По умолчанию: ',' :");
        var delimiter = Console.ReadLine() == "1" ? ';' : ',';

        var headers = Statement.GetHeaders(file, delimiter, encoding);

        if (headers.Length == 0)
        {
            Console.WriteLine($"Не удалось получить заголовки из файла {file}");
            continue;
        }

        Console.WriteLine("Вычисляю размеры полей.");

        var sizes = Statement.GetColumnSizes(file, delimiter, encoding, headers.Length);
        var translitiratedHeaders = Statement.PrepareHeaders(headers);
        var statement = Statement.GetCreateTableStatement(Consts.TABLENAME_DEFAULT, translitiratedHeaders, sizes);
        Console.WriteLine(Environment.NewLine + statement + Environment.NewLine);
        Console.WriteLine("Работа программы завершена." + Environment.NewLine);
    }
    catch (Exception ex)
    {

        Console.WriteLine($"Возникло исключение: {ex}" + Environment.NewLine);
        continue;
    }
}