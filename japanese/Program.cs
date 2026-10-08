using System;
using System.Collections.Generic;
using System.Linq;

public class JapaneseCrosswordSolver
{
    private int width, height;
    private List<int>[] colClues, rowClues;
    private char[,] grid;

    public JapaneseCrosswordSolver(string columns, string rows)
    {
        ParseClues(columns, rows);
        InitializeGrid();
    }

    private void ParseClues(string columns, string rows)
    {
        var colParts = columns.Split(';');
        width = colParts.Length;
        colClues = new List<int>[width];

        for (int i = 0; i < width; i++)
        {
            colClues[i] = new List<int>();
            if (!string.IsNullOrEmpty(colParts[i]) && colParts[i] != "0")
                foreach (var num in colParts[i].Split(','))
                    if (int.TryParse(num, out int value) && value > 0)
                        colClues[i].Add(value);
        }

        var rowParts = rows.Split(';');
        height = rowParts.Length;
        rowClues = new List<int>[height];

        for (int i = 0; i < height; i++)
        {
            rowClues[i] = new List<int>();
            if (!string.IsNullOrEmpty(rowParts[i]) && rowParts[i] != "0")
                foreach (var num in rowParts[i].Split(','))
                    if (int.TryParse(num, out int value) && value > 0)
                        rowClues[i].Add(value);
        }
    }

    private void InitializeGrid()
    {
        grid = new char[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                grid[i, j] = '?';
    }

    public string Solve()
    {
        bool changed;
        do
        {
            changed = false;
            for (int i = 0; i < height; i++) if (ProcessLine(i, true)) changed = true;
            for (int i = 0; i < width; i++) if (ProcessLine(i, false)) changed = true;
        } while (changed && !IsSolved());

        if (!IsSolved() && width <= 15 && height <= 15) BacktrackSolve();

        EnsureEmptyLinesFilled();
        return FormatSolution();
    }

    private void EnsureEmptyLinesFilled()
    {
        for (int i = 0; i < height; i++)
            if (rowClues[i].Count == 0)
                for (int j = 0; j < width; j++)
                    if (grid[i, j] == '?') grid[i, j] = '.';

        for (int i = 0; i < width; i++)
            if (colClues[i].Count == 0)
                for (int j = 0; j < height; j++)
                    if (grid[j, i] == '?') grid[j, i] = '.';
    }

    private bool ProcessLine(int index, bool isRow)
    {
        char[] line = isRow ? GetRow(index) : GetColumn(index);
        List<int> clues = isRow ? rowClues[index] : colClues[index];

        if (clues.Count == 0)
        {
            bool changed = false;
            for (int i = 0; i < line.Length; i++)
                if (line[i] != '.') { line[i] = '.'; changed = true; }
            if (changed) UpdateLine(index, line, isRow);
            return changed;
        }

        var possibilities = GenerateLinePossibilities(line, clues);
        if (possibilities.Count == 0) return false;

        bool changedInLine = false;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != '?') continue;
            bool allFilled = possibilities.All(p => p[i] == '#');
            bool allEmpty = possibilities.All(p => p[i] == '.');

            if (allFilled) { line[i] = '#'; changedInLine = true; }
            else if (allEmpty) { line[i] = '.'; changedInLine = true; }
        }

        if (changedInLine) UpdateLine(index, line, isRow);
        return changedInLine;
    }

    private List<char[]> GenerateLinePossibilities(char[] line, List<int> clues)
    {
        var possibilities = new List<char[]>();

        if (clues.Count == 0)
        {
            if (line.Any(c => c == '#')) return possibilities;
            possibilities.Add(Enumerable.Repeat('.', line.Length).ToArray());
            return possibilities;
        }

        GenerateLineRecursive(line, clues, 0, 0, new char[line.Length], possibilities);
        return possibilities;
    }

    private void GenerateLineRecursive(char[] line, List<int> clues, int clueIndex, int position, char[] current, List<char[]> possibilities)
    {
        if (position == 0) Array.Fill(current, '.');

        if (clueIndex == clues.Count)
        {
            for (int i = position; i < line.Length; i++)
                if (line[i] == '#') return;

            if (line.Where((c, i) => c != '?' && c != current[i]).Any()) return;
            possibilities.Add((char[])current.Clone());
            return;
        }

        int blockSize = clues[clueIndex];
        int minSpace = blockSize + clues.Skip(clueIndex + 1).Sum() + (clues.Count - clueIndex - 1);

        for (int start = position; start <= line.Length - minSpace; start++)
        {
            if (!CanPlaceBlock(line, start, blockSize)) continue;
            if (start > 0 && line[start - 1] == '#') continue;
            if (clueIndex < clues.Count - 1 && (start + blockSize >= line.Length || line[start + blockSize] == '#')) continue;

            PlaceBlock(current, start, blockSize);
            GenerateLineRecursive(line, clues, clueIndex + 1, start + blockSize + 1, current, possibilities);
            PlaceBlock(current, start, blockSize, '.');

            if (line[start] == '#') break;
        }
    }

    private bool CanPlaceBlock(char[] line, int start, int size)
    {
        for (int i = 0; i < size; i++)
            if (start + i >= line.Length || line[start + i] == '.') return false;
        return true;
    }

    private void PlaceBlock(char[] current, int start, int size, char fill = '#')
    {
        for (int i = 0; i < size; i++)
            if (start + i < current.Length) current[start + i] = fill;
    }

    private char[] GetRow(int row)
    {
        char[] result = new char[width];
        for (int col = 0; col < width; col++) result[col] = grid[row, col];
        return result;
    }

    private char[] GetColumn(int col)
    {
        char[] result = new char[height];
        for (int row = 0; row < height; row++) result[row] = grid[row, col];
        return result;
    }

    private void UpdateLine(int index, char[] line, bool isRow)
    {
        if (isRow)
            for (int col = 0; col < width; col++) grid[index, col] = line[col];
        else
            for (int row = 0; row < height; row++) grid[row, index] = line[row];
    }

    private bool IsSolved()
    {
        return grid.Cast<char>().All(c => c != '?');
    }

    private bool BacktrackSolve() => Backtrack(0, 0);

    private bool Backtrack(int row, int col)
    {
        if (row == height) return IsValidSolution();

        int nextRow = col == width - 1 ? row + 1 : row;
        int nextCol = col == width - 1 ? 0 : col + 1;

        if (grid[row, col] != '?') return Backtrack(nextRow, nextCol);

        foreach (var cell in new[] { '.', '#' })
        {
            grid[row, col] = cell;
            if (IsPartialValid(row, col) && Backtrack(nextRow, nextCol)) return true;
        }

        grid[row, col] = '?';
        return false;
    }

    private bool IsPartialValid(int lastRow, int lastCol)
    {
        for (int r = 0; r <= lastRow; r++)
            if (!IsLineValid(GetRow(r), rowClues[r], r == lastRow)) return false;

        for (int c = 0; c <= lastCol; c++)
            if (!IsLineValid(GetColumn(c), colClues[c], true)) return false;

        return true;
    }

    private bool IsLineValid(char[] line, List<int> clues, bool isPartial)
    {
        if (clues.Count == 0) return line.All(c => c != '#');

        var groups = new List<int>();
        int current = 0;
        bool inBlock = false;

        foreach (var c in line)
        {
            if (c == '#') { current++; inBlock = true; }
            else if (c == '.' && inBlock) { groups.Add(current); current = 0; inBlock = false; }
            else if (c == '?' && isPartial && inBlock) break;
        }

        if (inBlock) groups.Add(current);

        if (!isPartial) return groups.SequenceEqual(clues);

        if (groups.Count > clues.Count) return false;
        for (int i = 0; i < groups.Count; i++)
            if ((i == groups.Count - 1 && inBlock) ? groups[i] > clues[i] : groups[i] != clues[i]) return false;

        return true;
    }

    private bool IsValidSolution()
    {
        for (int i = 0; i < height; i++)
            if (!IsLineValid(GetRow(i), rowClues[i], false)) return false;

        for (int j = 0; j < width; j++)
            if (!IsLineValid(GetColumn(j), colClues[j], false)) return false;

        return true;
    }

    private string FormatSolution()
    {
        var result = new List<string>();
        for (int i = 0; i < height; i++)
        {
            var row = new char[width];
            for (int j = 0; j < width; j++)
                row[j] = grid[i, j] == '?' ? (rowClues[i].Count == 0 ? '.' : grid[i, j]) : grid[i, j];
            result.Add(new string(row));
        }
        return string.Join(";", result);
    }

    public static string SolveCrossword(string columns, string rows)
    {
        return new JapaneseCrosswordSolver(columns, rows).Solve();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Японский кроссворд - решатель");
        Console.WriteLine("Введите данные в формате: columns:2,1;2;;1,1;4;rows:1;1,1;1,1;1,2;2,1;");
        Console.WriteLine("Или введите 'exit' для выхода");

        while (true)
        {
            Console.Write("\nВвод: ");
            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input)) continue;
            if (input.ToLower() == "exit") break;

            try
            {
                var (columns, rows) = ParseInput(input);
                string solution = JapaneseCrosswordSolver.SolveCrossword(columns, rows);

                Console.WriteLine("Решение");
                Console.WriteLine(solution);
                Console.WriteLine("\nГрафическое представление:");
                //foreach (string line in solution.Split(';'))
                //    Console.WriteLine(line.Replace('#', '1').Replace('.', '0'));
                foreach (string line in solution.Split(';'))
                    Console.WriteLine(line.Replace('#', '█').Replace('.', '·'));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    static (string columns, string rows) ParseInput(string input)
    {
        string cleanInput = input.Replace(" ", "").ToLower();
        int rowsIndex = cleanInput.IndexOf("rows:");
        if (rowsIndex == -1) throw new ArgumentException("Не найден разделитель 'rows:'");

        string columnsPart = cleanInput.Substring("columns:".Length, rowsIndex - "columns:".Length).Replace(";;", ";0;").TrimEnd(';');
        string rowsPart = cleanInput.Substring(rowsIndex + "rows:".Length).Replace(";;", ";0;").TrimEnd(';');
        
        columnsPart = columnsPart.Replace(";;", ";0;");
        rowsPart = rowsPart.Replace(";;", ";0;");
        
        if (string.IsNullOrEmpty(columnsPart) || string.IsNullOrEmpty(rowsPart))
            throw new ArgumentException("Недостаточно данных");

        return (columnsPart, rowsPart);
    }
}