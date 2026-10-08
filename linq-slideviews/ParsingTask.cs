using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class ParsingTask
{
    public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
    {
        return lines
            .Skip(1)
            .Select(ParseSlideLine)
            .Where(record => record != null)
            .ToDictionary(record => record.SlideId, record => record);
    }

    private static SlideRecord? ParseSlideLine(string line)
    {
        string[] parts = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], out int slideId))
            return null;

        if (!Enum.TryParse(parts[1], true, out SlideType slideType))
            return null;

        return new SlideRecord(slideId, slideType, parts[2]);
    }

    public static IEnumerable<VisitRecord> ParseVisitRecords(
        IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
    {
        return lines
            .Skip(1)
            .Select((line, index) => ParseVisitLine(line, index + 2, slides))
            .ToList();
    }

    private static VisitRecord ParseVisitLine(string line, int lineNumber, IDictionary<int, SlideRecord> slides)
    {
        string[] parts = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

        // Проверяем количество частей
        if (parts.Length != 4)
            throw new FormatException($"Wrong line [{line}]");

        // Парсим UserId
        if (!int.TryParse(parts[0], out int userId))
            throw new FormatException($"Wrong line [{line}]");

        // Парсим SlideId
        if (!int.TryParse(parts[1], out int slideId))
            throw new FormatException($"Wrong line [{line}]");

        // Проверяем существование слайда
        if (!slides.ContainsKey(slideId))
            throw new FormatException($"Wrong line [{line}]");

        // Парсим дату и время
        if (!DateTime.TryParse($"{parts[2]} {parts[3]}", out DateTime dateTime))
            throw new FormatException($"Wrong line [{line}]");

        return new VisitRecord(userId, slideId, dateTime, slides[slideId].SlideType);
    }
}