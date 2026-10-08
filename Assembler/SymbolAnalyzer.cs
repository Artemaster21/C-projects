using System.Collections.Generic;

namespace Assembler
{
    public class SymbolAnalyzer
	{   /// Создает таблицу предопределенных символов
        private Dictionary<string, int> CreatePredefinedSymbols()
        {
            Dictionary<string, int> symbols = new Dictionary<string, int>();
            
            // Добавляем специальные адреса
            symbols["SCREEN"] = 16384;
            symbols["KBD"] = 24576;
            
            // Добавляем системные указатели
            symbols["SP"] = 0;
            symbols["LCL"] = 1;
            symbols["ARG"] = 2;
            symbols["THIS"] = 3;
            symbols["THAT"] = 4;

            for (var i = 0; i <= 15; i++) // Добавляем регистры R0-R15
            {
                var registerName = "R" + i;
                symbols[registerName] = i;
            }
            
            return symbols;
        }
                
		public Dictionary<string, int> CreateSymbolsTable(string[] instructionsWithLabels,
            out string[] instructionsWithoutLabels)
		{   
            // Шаг 1: Создаем таблицу с предопределенными символами
            Dictionary<string, int> symbolTable = CreatePredefinedSymbols(); 
            
            List<string> instructionsList = new List<string>(); // Шаг 2: Создаем список для инструкций без меток

            for (var i = 0; i < instructionsWithLabels.Length; i++) // Шаг 3: Обрабатываем каждую строку
            {
                var currentLine = instructionsWithLabels[i];
                
                if (IsLabel(currentLine)) // Шаг 4: Проверяем, является ли строка меткой
                {
                    var labelName = GetLabelName(currentLine); // Шаг 5: Извлекаем имя метки
                    symbolTable[labelName] = instructionsList.Count;// Шаг 6: Добавляем метку в таблицу символов
                }
                else
                    instructionsList.Add(currentLine);  // Шаг 7: Это инструкция, добавляем ее в список
            }
            
            instructionsWithoutLabels = instructionsList.ToArray(); // Шаг 8: Преобразуем список в массив
            return symbolTable; // Шаг 9: Возвращаем таблицу символов
		}


        private bool IsLabel(string line) /// Проверяет, является ли строка меткой
        {
            var startsWithBracket = line.StartsWith("(");
            var endsWithBracket = line.EndsWith(")");
            
            return startsWithBracket && endsWithBracket;
        }
        
        /// Извлекает имя метки из строки
        private string GetLabelName(string labelLine)
        {
            var withoutFirstChar = labelLine.Substring(1);
            var labelName = withoutFirstChar.Substring(0, withoutFirstChar.Length - 1);
            
            return labelName;
        }
    }
}