using System;
using System.Collections.Generic;

namespace Assembler
{
    public class HackTranslator
    {
        private int variableAddress = 16;

        private static readonly Dictionary<string, string> jumpTable = new Dictionary<string, string>()
        {
            {"", "000"},
            {"JGT", "001"},
            {"JEQ", "010"},
            {"JGE", "011"},
            {"JLT", "100"},
            {"JNE", "101"},
            {"JLE", "110"},
            {"JMP", "111"}
        };

        private static readonly Dictionary<string, string> compTable = new Dictionary<string, string>()
        {
            {"0", "101010"},
            {"1", "111111"},
            {"-1", "111010"},
            {"D", "001100"},
            {"A", "110000"},
            {"!D", "001101"},
            {"!A", "110001"},
            {"-D", "001111"},
            {"-A", "110011"},
            {"D+1", "011111"},
            {"A+1", "110111"},
            {"D-1", "001110"},
            {"A-1", "110010"},
            {"D+A", "000010"},
            {"D-A", "010011"},
            {"A-D", "000111"},
            {"D&A", "000000"},
            {"D|A", "010101"},

            {"M", "110000"},
            {"!M", "110001"},
            {"-M", "110011"},
            {"M+1", "110111"},
            {"M-1", "110010"},
            {"D+M", "000010"},
            {"D-M", "010011"},
            {"M-D", "000111"},
            {"D&M", "000000"},
            {"D|M", "010101"}
        };

        /// Транслирует инструкции ассемблерного кода (без меток) в бинарное представление.
        public string[] TranslateAsmToHack(string[] instructions, Dictionary<string, int> symbolTable)
        {
            var result = new List<string>();
            int tempVarCounter = 0;

            for (int i = 0; i < instructions.Length; i++)
            {
                string instruction = instructions[i].Trim();

                if (string.IsNullOrWhiteSpace(instruction) || instruction.StartsWith("//"))
                    continue;

                // Убираем комментарии
                if (instruction.Contains("//"))
                    instruction = instruction.Substring(0, instruction.IndexOf("//")).Trim();

                if (instruction.StartsWith("@"))
                    result.Add(AInstructionToCode(instruction, symbolTable));
                else if (IsExtendedCInstruction(instruction))
                {
                    // Обрабатываем расширенные C-инструкции
                    var expandedInstructions = TranslateExtendedCInstruction(instruction, ref tempVarCounter);
                    foreach (var expandedInstruction in expandedInstructions)
                    {
                        if (expandedInstruction.StartsWith("@"))
                            result.Add(AInstructionToCode(expandedInstruction, symbolTable));
                        else if (!expandedInstruction.StartsWith("(")) // Пропускаем метки
                            result.Add(CInstructionToCode(expandedInstruction));
                    }
                }
                else
                    result.Add(CInstructionToCode(instruction));
            }

            return result.ToArray();
        }

        /// Транслирует одну A-инструкцию ассемблерного кода в бинарное представление
        public string AInstructionToCode(string aInstruction, Dictionary<string, int> symbolTable)
        {
            string addressText = aInstruction.Substring(1);
            int addressNumber;

            if (!int.TryParse(addressText, out addressNumber))
            {
                if (!symbolTable.ContainsKey(addressText))
                {
                    symbolTable[addressText] = variableAddress;
                    variableAddress++;
                }
                addressNumber = symbolTable[addressText];
            }

            // Проверяем диапазон адреса
            if (addressNumber < 0 || addressNumber > 32767)
                throw new Exception("Адрес вне диапазона");

            // Используем PadLeft(16, '0') вместо "0" + PadLeft(15, '0')
            string binary = Convert.ToString(addressNumber, 2).PadLeft(16, '0');

            return binary;
        }

        /// Транслирует одну C-инструкцию ассемблерного кода в бинарное представление
        public string CInstructionToCode(string cInstruction)
        {
            // Ищем точку с запятой для jump части
            string jumpPart = "";
            string beforeSemicolon = cInstruction;

            int semicolonIndex = cInstruction.IndexOf(';');
            if (semicolonIndex != -1)
            {
                jumpPart = cInstruction.Substring(semicolonIndex + 1);
                beforeSemicolon = cInstruction.Substring(0, semicolonIndex);
            }

            // Используем словарь для jump битов
            string jumpBits = "000";
            if (jumpTable.ContainsKey(jumpPart))
                jumpBits = jumpTable[jumpPart];

            // Ищем знак равно для dest части
            string destPart = "";
            string compPart = beforeSemicolon;

            int equalsIndex = beforeSemicolon.IndexOf('=');
            if (equalsIndex != -1)
            {
                destPart = beforeSemicolon.Substring(0, equalsIndex);
                compPart = beforeSemicolon.Substring(equalsIndex + 1);
            }

            // Получаем dest биты
            bool hasA = destPart.Contains("A");
            bool hasD = destPart.Contains("D");
            bool hasM = destPart.Contains("M");

            string destBits = (hasA ? "1" : "0") + (hasD ? "1" : "0") + (hasM ? "1" : "0");

            // Получаем comp биты
            bool usesM = compPart.Contains("M");
            string aBit = usesM ? "1" : "0";

            if (!compTable.ContainsKey(compPart))
                throw new Exception("Неизвестная операция: " + compPart);

            string cBits = compTable[compPart];
            string compBits = aBit + cBits;

            return "111" + compBits + destBits + jumpBits;
        }

        /// Транслирует расширенные C-инструкции с арифметикой (сложение, вычитание, умножение, деление)
        /// в стандартные команды Hack ассемблера.
        public string[] TranslateExtendedCInstruction(string cInstruction, ref int tempVarCounter)
        {
            // Формат: var = num1 op num2 или var = num op var2 и т.д.
            // Поддерживаемые операции: +, -, *, /

            var resultInstructions = new List<string>();

            // Убираем пробелы и разбиваем на части
            cInstruction = cInstruction.Replace(" ", "");
            var parts = cInstruction.Split('=');

            if (parts.Length != 2)
                throw new Exception($"Неверный формат инструкции: {cInstruction}");

            string destVar = parts[0];
            string expression = parts[1];

            // Определяем операцию
            char[] operators = { '+', '-', '*', '/' };
            char op = ' ';
            int opIndex = -1;

            foreach (var oper in operators)
            {
                opIndex = expression.IndexOf(oper);
                if (opIndex > 0 && opIndex < expression.Length - 1)
                {
                    op = oper;
                    break;
                }
            }

            if (op == ' ')
                throw new Exception($"Не найдена операция в выражении: {expression}");

            string leftOperand = expression.Substring(0, opIndex);
            string rightOperand = expression.Substring(opIndex + 1);

            // Проверяем, являются ли операнды числами
            bool leftIsNum = int.TryParse(leftOperand, out int leftNum);
            bool rightIsNum = int.TryParse(rightOperand, out int rightNum);

            // Генерируем код в зависимости от операции
            switch (op)
            {
                case '+':
                    // Сложение
                    if (leftIsNum && rightIsNum)
                    {   // Оба числа - просто вычисляем и присваиваем
                        int sum = leftNum + rightNum;
                        resultInstructions.Add($"@{sum}");
                        resultInstructions.Add("D=A");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    else if (leftIsNum)
                    {   // Число + переменная
                        resultInstructions.Add($"@{leftNum}");
                        resultInstructions.Add("D=A");
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=D+M");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    else if (rightIsNum)
                    {   // Переменная + число
                        resultInstructions.Add($"@{leftOperand}");
                        resultInstructions.Add("D=M");
                        resultInstructions.Add($"@{rightNum}");
                        resultInstructions.Add("D=D+A");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    else
                    {   // Переменная + переменная
                        resultInstructions.Add($"@{leftOperand}");
                        resultInstructions.Add("D=M");
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=D+M");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    break;

                case '-':
                    // Вычитание
                    if (leftIsNum && rightIsNum)
                    {   // Оба числа
                        int diff = leftNum - rightNum;
                        resultInstructions.Add($"@{diff}");
                        resultInstructions.Add("D=A");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    else if (leftIsNum)
                    {   // Число - переменная
                        resultInstructions.Add($"@{leftNum}");
                        resultInstructions.Add("D=A");
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=D-M");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    else if (rightIsNum)
                    {   // Переменная - число
                        resultInstructions.Add($"@{leftOperand}");
                        resultInstructions.Add("D=M");
                        resultInstructions.Add($"@{rightNum}");
                        resultInstructions.Add("D=D-A");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    else
                    {   // Переменная - переменная
                        resultInstructions.Add($"@{leftOperand}");
                        resultInstructions.Add("D=M");
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=D-M");
                        resultInstructions.Add($"@{destVar}");
                        resultInstructions.Add("M=D");
                    }
                    break;

                case '*':
                    // Умножение - генерируем цикл умножения
                    string multResult = $"TEMP_{tempVarCounter++}";
                    string multCounter = $"TEMP_{tempVarCounter++}";

                    // Инициализация результата
                    resultInstructions.Add($"@{multResult}");
                    resultInstructions.Add("M=0");

                    // Загружаем левый операнд в D
                    if (leftIsNum)
                    {
                        resultInstructions.Add($"@{leftNum}");
                        resultInstructions.Add("D=A");
                    }
                    else
                    {
                        resultInstructions.Add($"@{leftOperand}");
                        resultInstructions.Add("D=M");
                    }

                    // Сохраняем в счетчик
                    resultInstructions.Add($"@{multCounter}");
                    resultInstructions.Add("M=D");

                    // Проверяем на 0
                    resultInstructions.Add($"@MULT_END_{tempVarCounter}");
                    resultInstructions.Add("D;JEQ");

                    // Метка начала цикла
                    resultInstructions.Add($"(MULT_LOOP_{tempVarCounter})");

                    // Добавляем правый операнд к результату
                    if (rightIsNum)
                    {
                        resultInstructions.Add($"@{rightNum}");
                        resultInstructions.Add("D=A");
                    }
                    else
                    {
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=M");
                    }

                    resultInstructions.Add($"@{multResult}");
                    resultInstructions.Add("M=D+M");

                    // Уменьшаем счетчик
                    resultInstructions.Add($"@{multCounter}");
                    resultInstructions.Add("M=M-1");
                    resultInstructions.Add("D=M");

                    // Проверяем условие продолжения цикла
                    resultInstructions.Add($"@MULT_LOOP_{tempVarCounter}");
                    resultInstructions.Add("D;JGT");

                    // Метка конца умножения
                    resultInstructions.Add($"(MULT_END_{tempVarCounter})");

                    // Копируем результат в destVar
                    resultInstructions.Add($"@{multResult}");
                    resultInstructions.Add("D=M");
                    resultInstructions.Add($"@{destVar}");
                    resultInstructions.Add("M=D");
                    break;

                case '/':
                    // Деление - генерируем цикл деления
                    string divQuotient = $"TEMP_{tempVarCounter++}";  // Частное
                    string divRemainder = $"TEMP_{tempVarCounter++}"; // Остаток
                    string divTemp = $"TEMP_{tempVarCounter++}";     // Временная переменная

                    // Инициализация частного
                    resultInstructions.Add($"@{divQuotient}");
                    resultInstructions.Add("M=0");

                    // Проверка деления на 0
                    if (rightIsNum)
                    {
                        if (rightNum == 0)
                            throw new Exception("Деление на 0");
                        resultInstructions.Add($"@{rightNum}");
                        resultInstructions.Add("D=A");
                    }
                    else
                    {
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=M");
                    }

                    resultInstructions.Add($"@DIV_END_{tempVarCounter}");
                    resultInstructions.Add("D;JEQ");

                    // Проверка делимого на 0
                    if (leftIsNum)
                    {
                        if (leftNum == 0)
                        {
                            resultInstructions.Add($"@{destVar}");
                            resultInstructions.Add("M=0");
                            resultInstructions.Add($"@DIV_END_FINAL_{tempVarCounter}");
                            resultInstructions.Add("0;JMP");
                        }
                        resultInstructions.Add($"@{leftNum}");
                        resultInstructions.Add("D=A");
                    }
                    else
                    {
                        resultInstructions.Add($"@{leftOperand}");
                        resultInstructions.Add("D=M");
                        resultInstructions.Add($"@DIV_END_{tempVarCounter}");
                        resultInstructions.Add("D;JEQ");
                    }

                    // Сохраняем делимое во временную переменную (остаток)
                    resultInstructions.Add($"@{divRemainder}");
                    resultInstructions.Add("M=D");

                    // Метка начала цикла деления
                    resultInstructions.Add($"(DIV_LOOP_{tempVarCounter})");

                    // Загружаем текущий остаток
                    resultInstructions.Add($"@{divRemainder}");
                    resultInstructions.Add("D=M");

                    // Вычитаем делитель
                    if (rightIsNum)
                    {
                        resultInstructions.Add($"@{rightNum}");
                        resultInstructions.Add("D=D-A");
                    }
                    else
                    {
                        resultInstructions.Add($"@{rightOperand}");
                        resultInstructions.Add("D=D-M");
                    }

                    // Если результат отрицательный - конец цикла
                    resultInstructions.Add($"@DIV_END_LOOP_{tempVarCounter}");
                    resultInstructions.Add("D;JLT");

                    // Сохраняем новый остаток
                    resultInstructions.Add($"@{divRemainder}");
                    resultInstructions.Add("M=D");

                    // Увеличиваем частное на 1
                    resultInstructions.Add($"@{divQuotient}");
                    resultInstructions.Add("M=M+1");

                    // Повторяем цикл
                    resultInstructions.Add($"@DIV_LOOP_{tempVarCounter}");
                    resultInstructions.Add("0;JMP");

                    // Метка конца цикла деления
                    resultInstructions.Add($"(DIV_END_LOOP_{tempVarCounter})");

                    // Копируем частное в destVar
                    resultInstructions.Add($"@{divQuotient}");
                    resultInstructions.Add("D=M");
                    resultInstructions.Add($"@{destVar}");
                    resultInstructions.Add("M=D");

                    // Остаток можно сохранить в другую переменную, если нужно
                    // resultInstructions.Add($"@{divRemainder}");
                    // resultInstructions.Add("D=M");
                    // resultInstructions.Add($"@{destVar}_REM");
                    // resultInstructions.Add("M=D");

                    // Метка конца деления
                    resultInstructions.Add($"(DIV_END_{tempVarCounter})");
                    resultInstructions.Add($"(DIV_END_FINAL_{tempVarCounter})");
                    break;

                default:
                    throw new Exception($"Неподдерживаемая операция: {op}");
            }

            return resultInstructions.ToArray();
        }

        /// Проверяет, является ли инструкция расширенной C-инструкцией
        public bool IsExtendedCInstruction(string instruction)
        {
            instruction = instruction.Replace(" ", "");

            // Проверяем формат var=expression
            if (!instruction.Contains("="))
                return false;

            var parts = instruction.Split('=');
            if (parts.Length != 2)
                return false;

            string expression = parts[1];

            // Проверяем наличие операций
            char[] operators = { '+', '-', '*', '/' };
            foreach (var op in operators)
            {
                int opIndex = expression.IndexOf(op);
                if (opIndex > 0 && opIndex < expression.Length - 1)
                    return opIndex > 0 && opIndex < expression.Length - 1;
            }

            return false;
        }
    }
}