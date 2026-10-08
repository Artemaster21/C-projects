using System;
using System.Collections.Generic;
using System.Linq;

namespace GaussAlgorithm;

public class Solver
{
    public double[] Solve(double[][] matrix, double[] freeMembers)
    {
        // Создаем расширенную матрицу [A|b]
        var augmentedMatrix = matrix
            .Select((row, index) => row.Concat(new[] { freeMembers[index] }).ToArray())
            .ToArray();

        int rows = augmentedMatrix.Length;
        int cols = augmentedMatrix[0].Length - 1; // последний столбец - свободные члены

        // Копируем для хранения истории (для исключения)
        var originalMatrix = matrix.Select(r => r.ToArray()).ToArray();
        var originalFree = freeMembers.ToArray();

        // Прямой ход метода Гаусса
        var usedRows = new HashSet<int>();
        var rowForColumn = new Dictionary<int, int>();

        for (int col = 0; col < cols; col++)
        {
            // Ищем строку с ненулевым элементом в текущем столбце
            int pivotRow = -1;
            for (int row = 0; row < rows; row++)
            {
                if (!usedRows.Contains(row) && Math.Abs(augmentedMatrix[row][col]) > 1e-10)
                {
                    pivotRow = row;
                    break;
                }
            }

            if (pivotRow == -1)
                continue; // переменная свободная (может быть 0)

            // Помечаем строку как использованную
            usedRows.Add(pivotRow);
            rowForColumn[col] = pivotRow;

            // Нормализуем строку-основу (делим на ведущий элемент)
            double pivot = augmentedMatrix[pivotRow][col];
            for (int j = col; j <= cols; j++)
                augmentedMatrix[pivotRow][j] /= pivot;

            // Обнуляем текущий столбец во всех остальных строках
            for (int row = 0; row < rows; row++)
            {
                if (row == pivotRow) continue;

                double factor = augmentedMatrix[row][col];
                if (Math.Abs(factor) < 1e-10) continue;

                for (int j = col; j <= cols; j++)
                    augmentedMatrix[row][j] -= factor * augmentedMatrix[pivotRow][j];
            }
        }

        // Проверка на несовместность
        for (int row = 0; row < rows; row++)
        {
            bool allZeros = true;
            for (int j = 0; j < cols; j++)
            {
                if (Math.Abs(augmentedMatrix[row][j]) > 1e-10)
                {
                    allZeros = false;
                    break;
                }
            }

            if (allZeros && Math.Abs(augmentedMatrix[row][cols]) > 1e-10)
                throw new NoSolutionException(originalMatrix, originalFree, augmentedMatrix);
        }

        // Обратный ход - вычисляем значения переменных
        var solution = new double[cols];

        // Переменные, для которых нашли строки
        foreach (var pair in rowForColumn)
        {
            int col = pair.Key;
            int row = pair.Value;
            solution[col] = augmentedMatrix[row][cols];
        }

        // Остальные переменные уже 0 (инициализировано по умолчанию)
        return solution;
    }
}