using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocket_bot;

public partial class Bot
{
    public Rocket GetNextMove(Rocket rocket)
    {
        // Создаём список задач для параллельного поиска
        var tasks = new List<Task<(Turn Turn, double Score)>>();

        // Определяем количество итераций на один поток
        int iterationsPerThread = iterationsCount / threadsCount;

        // Запускаем потоки
        for (int i = 0; i < threadsCount; i++)
        {
            // Создаём отдельный Random для каждого потока
            // Используем новый Random с разными seed, чтобы избежать одинаковых последовательностей
            int threadSeed = random.Next();
            var threadRandom = new Random(threadSeed);

            // Запускаем задачу в потоке
            var task = Task.Run(() => SearchBestMove(rocket, threadRandom, iterationsPerThread));
            tasks.Add(task);
        }

        // Дожидаемся завершения всех потоков
        Task.WhenAll(tasks).Wait();

        // Выбираем лучший результат из всех потоков
        var bestTurn = Turn.None;
        var bestScore = double.NegativeInfinity;

        foreach (var task in tasks)
        {
            var (turn, score) = task.Result;
            if (score > bestScore)
            {
                bestScore = score;
                bestTurn = turn;
            }
        }

        // Возвращаем ракету с лучшим найденным ходом
        return rocket.Move(bestTurn, level);
    }

    public List<Task<(Turn Turn, double Score)>> CreateTasks(Rocket rocket)
    {
        // Этот метод больше не используется в новой реализации,
        // но оставляем для совместимости с тестами
        var tasks = new List<Task<(Turn Turn, double Score)>>();

        int iterationsPerThread = iterationsCount / threadsCount;

        for (int i = 0; i < threadsCount; i++)
        {
            int threadSeed = random.Next();
            var threadRandom = new Random(threadSeed);
            var task = Task.Run(() => SearchBestMove(rocket, threadRandom, iterationsPerThread));
            tasks.Add(task);
        }

        return tasks;
    }
}