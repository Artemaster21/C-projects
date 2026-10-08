using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class StatisticsTask
{
    public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
    {
        // Шаг 1: Группируем по пользователям и сортируем по времени
        var times = visits
            .GroupBy(v => v.UserId)
            .SelectMany(group => GetSlideTimesForUser(group, slideType))
            .Where(time => time >= 1 && time <= 120) // Фильтр 1 минута - 2 часа
            .ToList();

        // Шаг 2: Если нет данных, возвращаем 0 (или можно выбросить исключение)
        if (times.Count == 0)
            return 0;

        // Шаг 3: Вычисляем медиану
        return times.Median();
    }

    private static IEnumerable<double> GetSlideTimesForUser(
        IEnumerable<VisitRecord> userVisits, SlideType slideType)
    {
        // Сортируем посещения пользователя по времени
        var sortedVisits = userVisits
            .OrderBy(v => v.DateTime)
            .ToList();

        // Используем Bigrams для получения пар (текущий, следующий)
        return sortedVisits
            .Bigrams()
            .Where(pair => pair.First.SlideType == slideType) // Только если первый слайд нужного типа
            .Select(pair => (pair.Second.DateTime - pair.First.DateTime).TotalMinutes);
    }
}