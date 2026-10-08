namespace Assembler
{
    public class Parser
    {
        /// Удаляет все комментарии и пустые строки из программы. Удаляет все пробелы из команд.
        /// <param name="asmLines">Строки ассемблерного кода</param>
        /// <returns>Только значащие строки строки ассемблерного кода без комментариев и лишних пробелов</returns>
        public string[] RemoveWhitespacesAndComments(string[] asmLines)
        {
            List<string> cleanedCode = new List<string>(); // Создаем список для результата

            for (var i = 0; i < asmLines.Length; i++) // Проходим по всем строкам
            {
                var commentStart = asmLines[i].IndexOf("//"); // Ищем комментарий
                if (commentStart != -1) // Если нашли комментарий, обрезаем строку
                    asmLines[i] = asmLines[i].Substring(0, commentStart);

                asmLines[i] = asmLines[i].Replace(" ", "");  // Удаляем пробелы

                if (asmLines[i].Length > 0) // Если строка не пустая, добавляем в результат
                    cleanedCode.Add(asmLines[i]);
            }
            return cleanedCode.ToArray();
        }
    }
}