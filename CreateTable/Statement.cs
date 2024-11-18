namespace CreateTable;

public class Statement
{
    public static string GetCreateTableStatement(string tableName, IEnumerable<string> headers, int[] sizes)
    {
        var headersList = headers.ToList();
        var result = new StringBuilder();
        result.Append($"CREATE TABLE {tableName} (\n");
        for (int i = 0; i < headersList.Count; i++)
        {
            var size = sizes[i] == 0 ? 1 : sizes[i];
            if (size > 2000)
            {
                result.Append($"{headersList[i]} TEXT");
            }
            else result.Append($"{headersList[i]} VARCHAR({size})");
            if (i < headersList.Count - 1)
            {
                result.Append(",\n");
            }
        }
        result.Append($"\n);");
        return result.ToString();
    }

    public static string ReplaceInvalidChars(string input)
    {
        var result = input;
        foreach (char character in Consts.REPLACEMENT_CHARACTERS)
        {
            if (result.Contains(character))
            {
                result = result.Replace(character, Consts.VALID_SEPARATOR);
            }
        }
        if (result.EndsWith(Consts.VALID_SEPARATOR))
        {
            result = result[..^1];
        }
        var doubleSeparator = Consts.VALID_SEPARATOR.ToString() + Consts.VALID_SEPARATOR;
        while (result.Contains(doubleSeparator))
        {
            result = result.Replace(doubleSeparator, "_");
        }
        if (result.StartsWith(Consts.VALID_SEPARATOR))
        {
            result = result[1..];
        }
        if (result.EndsWith(Consts.VALID_SEPARATOR))
        {
            result = result[..^1]; // == result.Substring(0,result.Length - 1);
        }
        return result;
    }

    public static string[] GetHeaders(string file, char delimiter, Encoding encoding)
    {
        if (!file.Contains(".csv"))
        {
            throw new Exception("Работаем только с .csv");
        }
        else if (file.Contains(".csv"))
        {
            return GetHeadersFromCsv(file, delimiter, encoding);
        }
        return [];
    }

    public static List<string> GetHeadersFromExcel(string file)
    {
        List<string> headers = [];
        using (var excel = new ExcelApp())
        {
            var success = excel.OpenDoc(file);
            if (!success)
            {
                return headers;
            }
            for (int column = 1; column <= excel.LastColumn; column++)
            {
                var header = excel.GetText(1, column);
                if (!string.IsNullOrEmpty(header))
                {
                    headers.Add(header);
                }
            }
        }
        return headers;
    }

    private static string[] GetHeadersFromCsv(string file, char delimiter, Encoding encoding)
    {
        string headersString = string.Empty;
        foreach (var line in File.ReadLines(file, encoding))
        {
            headersString = line;
            break;
        }

        var headers = ParseCsvString(headersString, delimiter);
        return headers;
    }

    public static List<string> PrepareHeaders(IEnumerable<string> originalHeaders)
    {
        var result = new List<string>();
        foreach (string header in originalHeaders)
        {
            var headersTranslitirated = Unidecoder.Unidecode(header);
            var headersWithoutInvalidChars = ReplaceInvalidChars(headersTranslitirated);

            headersWithoutInvalidChars = CutHeader(headersWithoutInvalidChars);

            if(!result.Contains(headersWithoutInvalidChars))
            {
                result.Add(headersWithoutInvalidChars);
            }
            else
            {
                var uniqHeader = MakeHeaderUniq(headersWithoutInvalidChars);
                result.Add(uniqHeader);
            }
        }
        return result;
    }

    public static string CutHeader(string header)
    {
        if(header.Length > Consts.HEADER_MAX_LENGTH)
        {
            return header[..(Consts.HEADER_MAX_LENGTH + 1)];
        }
        return header;
    }

    public static string MakeHeaderUniq(string header)
    {
        return CutHeader(Consts.ADDED_SYMBOL_FOR_UNIQ + header);
    }

    public static int[] GetColumnSizes(string file, char delimiter, Encoding encoding, int columnsCount)
    {
        var sizes = new int[columnsCount];

        var lineIndex = 0;
        foreach (var line in File.ReadLines(file, encoding))
        {
            if (lineIndex == 0)
            {
                lineIndex++;
                continue;
            }

            var columns = ParseCsvString(line, delimiter);
            for (int i = 0; i < sizes.Length; i++)
            {
                if (i < columns.Length)
                {
                    if (sizes[i] < columns[i].Length)
                    {
                        sizes[i] = columns[i].Length;
                    }
                }
            }

            lineIndex++;
        }
        return sizes;
    }

    public static void ConvertFileEncoding(string inputFilePath, string outputFilePath, Encoding fromEncoding, Encoding toEncoding)
    {
        try
        {
            string content = File.ReadAllText(inputFilePath, fromEncoding);
            File.WriteAllText(outputFilePath, content, toEncoding);
        }
        catch (Exception ex)
        {
            throw new Exception($"Произошла ошибка: {ex.Message}");
        }
    }

    public static string[] ParseCsvString(string input, char separator)
    {
        List<string> parts = [];
        bool insideQuotes = false;
        int startIndex = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (input[i] == separator && !insideQuotes)
            {
                var line = input[startIndex..i]; // == input.Substring(startIndex, i - startIndex) ???
                if (line.StartsWith('\"') && line.EndsWith('\"'))
                    line = line[1..^1]; // == line.Substring(1, line.Lenght - 2)
                parts.Add(line);
                startIndex = i + 1;
            }
        }
        parts.Add(input[startIndex..]); // == inputs.Substring(startIndex)
        return [.. parts]; // == parts.ToArray()
    }
}
