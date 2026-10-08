using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembler.Tests
{
    [TestFixture]
    public class HackTranslatorTests
    {
        [TestCase("M=A", "1110110000001000")]
        [TestCase("D=M", "1111110000010000")]
        [TestCase("A=D", "1110001100100000")]
        [TestCase("AMD=D", "1110001100111000")]
        [TestCase("D;JMP", "1110001100000111")]
        [TestCase("D;JLT", "1110001100000100")]
        [TestCase("D;JGE", "1110001100000011")]
        [TestCase("D;JEQ", "1110001100000010")]
        [TestCase("D;JNE", "1110001100000101")]
        [TestCase("D=0", "1110101010010000")]
        [TestCase("D=1", "1110111111010000")]
        [TestCase("D=-1", "1110111010010000")]
        [TestCase("D=D+M", "1111000010010000")]
        [TestCase("D=D-A", "1110010011010000")]
        [TestCase("D=D&M", "1111000000010000")]
        [TestCase("D=D|A", "1110010101010000")]
        [TestCase("AMD=0;JLE", "1110101010111110")]
        [TestCase("D=D+1;JGT", "1110011111010001")]
        [TestCase("0;JMP", "1110101010000111")]
        public void CInstruction(string text, string expectedCode)
        {
            var translator = new HackTranslator();
            var code = translator.CInstructionToCode(text);
            Assert.AreEqual(expectedCode, code);
        }

        [TestCase("@0", "0000000000000000")]
        [TestCase("@1", "0000000000000001")]
        [TestCase("@2", "0000000000000010")]
        [TestCase("@32767", "0111111111111111")]
        [TestCase("@B", "0000000000000001")]
        [TestCase("@abc", "0000000000101010")]
        public void AInstruction(string text, string expectedCode)
        {
            var translator = new HackTranslator();
            var symbolTable = new Dictionary<string, int>
            {
                {"abc", 42},
                {"B", 1},
            };
            var code = translator.AInstructionToCode(text, symbolTable);
            Assert.AreEqual(expectedCode, code);
        }

        [Test]
        public void AInstructionWithVariables()
        {
            var translator = new HackTranslator();
            var symbolTable = new Dictionary<string, int>();
            Assert.AreEqual("0000000000010000", translator.AInstructionToCode("@x", symbolTable));
            Assert.AreEqual("0000000000010001", translator.AInstructionToCode("@y", symbolTable));
            Assert.AreEqual("0000000000010000", translator.AInstructionToCode("@x", symbolTable));
            Assert.AreEqual("0000000000010010", translator.AInstructionToCode("@ZZZ", symbolTable));
            Assert.AreEqual("0000000000010001", translator.AInstructionToCode("@y", symbolTable));
            Assert.AreEqual("0000000000010010", translator.AInstructionToCode("@ZZZ", symbolTable));
        }

        // Тесты для расширенной арифметики
        [TestCase("c=9+4")]
        [TestCase("result=6-4")]
        [TestCase("sum=a+b")]
        [TestCase("diff=x-y")]
        [TestCase("prod=m*n")]
        [TestCase("quot=p/q")]
        public void IsExtendedCInstruction_ValidCases_ReturnsTrue(string instruction)
        {
            var translator = new HackTranslator();
            Assert.IsTrue(translator.IsExtendedCInstruction(instruction));
        }

        [TestCase("D=M")]
        [TestCase("@123")]
        [TestCase("(LOOP)")]
        [TestCase("M=M+1")]
        [TestCase("0;JMP")]
        public void IsExtendedCInstruction_InvalidCases_ReturnsFalse(string instruction)
        {
            var translator = new HackTranslator();
            Assert.IsFalse(translator.IsExtendedCInstruction(instruction));
        }

        [Test]
        public void TranslateExtendedCInstruction_AdditionWithNumbers()
        {
            var translator = new HackTranslator();
            int tempVarCounter = 0;

            var result = translator.TranslateExtendedCInstruction("c=9+4", ref tempVarCounter);

            Assert.AreEqual(6, result.Length);
            Assert.AreEqual("@13", result[0]);
            Assert.AreEqual("D=A", result[1]);
            Assert.AreEqual("@c", result[2]);
            Assert.AreEqual("M=D", result[3]);
        }

        [Test]
        public void TranslateExtendedCInstruction_SubtractionWithNumbers()
        {
            var translator = new HackTranslator();
            int tempVarCounter = 0;

            var result = translator.TranslateExtendedCInstruction("diff=10-3", ref tempVarCounter);

            Assert.AreEqual(6, result.Length);
            Assert.AreEqual("@7", result[0]);
            Assert.AreEqual("D=A", result[1]);
            Assert.AreEqual("@diff", result[2]);
            Assert.AreEqual("M=D", result[3]);
        }

        [Test]
        public void TranslateExtendedCInstruction_VariableAddition()
        {
            var translator = new HackTranslator();
            int tempVarCounter = 0;

            var result = translator.TranslateExtendedCInstruction("sum=x+y", ref tempVarCounter);

            Assert.AreEqual(6, result.Length);
            Assert.AreEqual("@x", result[0]);
            Assert.AreEqual("D=M", result[1]);
            Assert.AreEqual("@y", result[2]);
            Assert.AreEqual("D=D+M", result[3]);
            Assert.AreEqual("@sum", result[4]);
            Assert.AreEqual("M=D", result[5]);
        }

        [Test]
        public void TranslateExtendedCInstruction_Multiplication()
        {
            var translator = new HackTranslator();
            int tempVarCounter = 0;

            var result = translator.TranslateExtendedCInstruction("prod=3*4", ref tempVarCounter);

            // Умножение генерирует больше команд из-за цикла
            Assert.IsTrue(result.Length > 10);
            // Проверяем наличие команд цикла
            Assert.IsTrue(result.Any(cmd => cmd.Contains("MULT_LOOP")));
            Assert.IsTrue(result.Any(cmd => cmd.Contains("MULT_END")));
        }

        [Test]
        public void TranslateExtendedCInstruction_Division()
        {
            var translator = new HackTranslator();
            int tempVarCounter = 0;

            var result = translator.TranslateExtendedCInstruction("quot=12/3", ref tempVarCounter);

            // Деление также генерирует много команд из-за цикла
            Assert.IsTrue(result.Length > 10);
            // Проверяем наличие команд цикла деления
            Assert.IsTrue(result.Any(cmd => cmd.Contains("DIV_LOOP")));
            Assert.IsTrue(result.Any(cmd => cmd.Contains("DIV_END")));
        }

        [Test]
        public void TranslateExtendedCInstruction_DivisionByZero_ThrowsException()
        {
            var translator = new HackTranslator();
            int tempVarCounter = 0;

            Assert.Throws<Exception>(() =>
                translator.TranslateExtendedCInstruction("quot=5/0", ref tempVarCounter));
        }

        [Test]
        public void TranslateAsmToHack_WithExtendedArithmetic()
        {
            var translator = new HackTranslator();
            var symbolTable = new Dictionary<string, int>();

            string[] instructions = {
                "@x",
                "M=5",
                "@y",
                "M=3",
                "sum=x+y",
                "prod=x*y",
                "quot=x/y"
            };

            var result = translator.TranslateAsmToHack(instructions, symbolTable);

            // Проверяем что результат содержит корректное количество команд
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);

            // Проверяем что все строки имеют длину 16 (бинарный формат)
            foreach (var line in result)
            {
                Assert.AreEqual(16, line.Length);
                Assert.IsTrue(line.All(c => c == '0' || c == '1'));
            }
        }

        [Test]
        public void TranslateComplexArithmeticProgram()
        {
            // Создаем тестовую программу с различными арифметическими операциями
            string[] instructions = {
                "// Простая арифметика",
                "@a",
                "M=10",
                "@b",
                "M=4",
                "// Сложение",
                "sum=a+b",
                "// Вычитание",
                "diff=a-b",
                "// Умножение",
                "prod=a*b",
                "// Деление",
                "quot=a/b",
                "// Конец программы",
                "@END",
                "0;JMP"
            };

            var translator = new HackTranslator();
            var symbolTable = new Dictionary<string, int>();

            var result = translator.TranslateAsmToHack(instructions, symbolTable);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 20); // Должно быть много команд из-за циклов умножения/деления

            // Проверяем что все команды валидны
            foreach (var line in result)
            {
                Assert.AreEqual(16, line.Length);
                Assert.IsTrue(line.All(c => c == '0' || c == '1'));
            }
        }

        [TestCase("Add.asm")]
        [TestCase("Max.asm")]
        [TestCase("Rect.asm")]
        [TestCase("Pong.asm")]
        public void TranslateSampleProgram(string filename)
        {
            var asmFile = Path.Combine("Samples", filename);
            if (!File.Exists(asmFile))
            {
                Assert.Ignore($"Файл {asmFile} не найден, пропускаем тест");
                return;
            }

            var asmCode = File.ReadAllLines(asmFile);
            var actualHack = Program.TranslateAsmToHack(asmCode);
            var hackFile = Path.ChangeExtension(asmFile, ".hack");
            var expectedHack = File.ReadAllLines(hackFile);
            Assert.AreEqual(expectedHack, actualHack);
        }

        [TestCase("AdditionShort.asm", "AdditionLong.asm")]
        [TestCase("SubtractionShort.asm", "SubtractionLong.asm")]
        [TestCase("MultiplicationShort.asm", "MultiplicationLong.asm")]
        [TestCase("DivisionShort.asm", "DivisionLong.asm")]
        [TestCase("MixedShort.asm", "MixedLong.asm")]
        public void TranslateExtendedArithmetic(string shortFilename, string longFilename)
        {
            // Создаем тестовые файлы если их нет
            CreateTestFiles();

            // Читаем файл с сокращенным синтаксисом
            var shortAsmFile = Path.Combine("TestPrograms", shortFilename);
            if (!File.Exists(shortAsmFile))
            {
                Assert.Ignore($"Файл {shortAsmFile} не найден, пропускаем тест");
                return;
            }

            var shortAsmCode = File.ReadAllLines(shortAsmFile);

            // Транслируем через наш расширенный ассемблер
            var translator = new HackTranslator();
            var symbolTable = new Dictionary<string, int>();
            var actualHack = translator.TranslateAsmToHack(shortAsmCode, symbolTable);

            // Читаем файл с обычным синтаксисом
            var longAsmFile = Path.Combine("TestPrograms", longFilename);
            if (!File.Exists(longAsmFile))
            {
                Assert.Ignore($"Файл {longAsmFile} не найден, пропускаем тест");
                return;
            }

            var longAsmCode = File.ReadAllLines(longAsmFile);

            // Транслируем через обычный ассемблер (ваша текущая реализация)
            var expectedHack = Program.TranslateAsmToHack(longAsmCode);

            // Сравниваем результаты
            if (expectedHack.Length != actualHack.Length)
            {
                Console.WriteLine($"\n=== Debug info for {shortFilename} ===");
                Console.WriteLine($"Expected {expectedHack.Length} lines, got {actualHack.Length} lines");
                Console.WriteLine("\nExpected code:");
                for (int i = 0; i < Math.Min(expectedHack.Length, 20); i++)
                {
                    Console.WriteLine($"{i:D3}: {expectedHack[i]}");
                }
                Console.WriteLine("\nActual code:");
                for (int i = 0; i < Math.Min(actualHack.Length, 20); i++)
                {
                    Console.WriteLine($"{i:D3}: {actualHack[i]}");
                }
            }

            Assert.AreEqual(expectedHack.Length, actualHack.Length,
                $"Разное количество команд для {shortFilename}");

            bool allMatch = true;
            for (int i = 0; i < expectedHack.Length; i++)
            {
                if (expectedHack[i] != actualHack[i])
                {
                    allMatch = false;
                    Console.WriteLine($"\nРасхождение в строке {i + 1}:");
                    Console.WriteLine($"Ожидалось: {expectedHack[i]}");
                    Console.WriteLine($"Получено:  {actualHack[i]}");

                    // Выводим контекст
                    int start = Math.Max(0, i - 2);
                    int end = Math.Min(expectedHack.Length - 1, i + 2);
                    Console.WriteLine("Контекст:");
                    for (int j = start; j <= end; j++)
                    {
                        string marker = (j == i) ? ">>> " : "    ";
                        Console.WriteLine($"{marker}{j + 1:D3}: {expectedHack[j]}");
                    }
                    break;
                }
            }

            Assert.IsTrue(allMatch,
                $"Найдены расхождения для {shortFilename}. Проверьте вывод выше.");
        }

        [SetUp]
        public void Setup()
        {
            // Создаем директорию для тестовых файлов если её нет
            if (!Directory.Exists("TestPrograms"))
            {
                Directory.CreateDirectory("TestPrograms");
            }

            // Создаем директорию для образцов если её нет
            if (!Directory.Exists("Samples"))
            {
                Directory.CreateDirectory("Samples");
            }
        }

        private void CreateTestFiles()
        {
            // Создаем тестовые файлы для арифметики

            // Addition
            File.WriteAllText("TestPrograms/AdditionShort.asm", @"// Test addition with constants
@result1
M=5+3

// Test addition with variables
@x
M=10
@y
M=4
@sum
M=x+y

// Test addition with mixed operands
@mixed
M=x+2
@mixed2
M=3+y

// Test chained addition
@temp
M=x
@chained
M=temp+y

(END)
@END
0;JMP");

            File.WriteAllText("TestPrograms/AdditionLong.asm", @"// Test addition with constants
@8
D=A
@result1
M=D

// Test addition with variables
@10
D=A
@x
M=D
@4
D=A
@y
M=D
@x
D=M
@y
D=D+M
@sum
M=D

// Test addition with mixed operands
@x
D=M
@2
D=D+A
@mixed
M=D
@3
D=A
@y
D=D+M
@mixed2
M=D

// Test chained addition
@x
D=M
@temp
M=D
@temp
D=M
@y
D=D+M
@chained
M=D

(END)
@END
0;JMP");

            // Subtraction
            File.WriteAllText("TestPrograms/SubtractionShort.asm", @"// Test subtraction with constants
@diff1
M=10-3

// Test subtraction with variables
@a
M=15
@b
M=5
@result
M=a-b

// Test edge cases
@zero
M=5-5
@negative
M=3-7

// Test with immediate values
@immediate
M=a-3
@immediate2
M=10-b

(END)
@END
0;JMP");

            File.WriteAllText("TestPrograms/SubtractionLong.asm", @"// Test subtraction with constants
@7
D=A
@diff1
M=D

// Test subtraction with variables
@15
D=A
@a
M=D
@5
D=A
@b
M=D
@a
D=M
@b
D=D-M
@result
M=D

// Test edge cases
@0
D=A
@zero
M=D
@65532
D=A
@negative
M=D

// Test with immediate values
@a
D=M
@3
D=D-A
@immediate
M=D
@10
D=A
@b
D=D-M
@immediate2
M=D

(END)
@END
0;JMP");

            // Multiplication
            File.WriteAllText("TestPrograms/MultiplicationShort.asm", @"// Test multiplication with constants
@prod1
M=3*4
@prod2
M=0*5
@prod3
M=1*10

// Test multiplication with variables
@m
M=5
@n
M=6
@result
M=m*n

// Test multiplication edge cases
@zero1
M=0*m
@zero2
M=n*0
@same
M=m*m

(END)
@END
0;JMP");

            File.WriteAllText("TestPrograms/MultiplicationLong.asm", @"// Test multiplication with constants
// Multiplication loop for 3*4
@TEMP_0
M=0
@3
D=A
@TEMP_1
M=D
@MULT_END_1
D;JEQ
(MULT_LOOP_1)
@4
D=A
@TEMP_0
M=D+M
@TEMP_1
M=M-1
D=M
@MULT_LOOP_1
D;JGT
(MULT_END_1)
@TEMP_0
D=M
@prod1
M=D

// 0*5 = 0
@0
D=A
@prod2
M=D

// 1*10 = 10
@10
D=A
@prod3
M=D

// Test multiplication with variables
@5
D=A
@m
M=D
@6
D=A
@n
M=D

// Multiplication loop for m*n
@TEMP_2
M=0
@m
D=M
@TEMP_3
M=D
@MULT_END_2
D;JEQ
(MULT_LOOP_2)
@n
D=M
@TEMP_2
M=D+M
@TEMP_3
M=M-1
D=M
@MULT_LOOP_2
D;JGT
(MULT_END_2)
@TEMP_2
D=M
@result
M=D

// Test multiplication edge cases
@0
D=A
@zero1
M=D
@0
D=A
@zero2
M=D

// m*m = 5*5
@TEMP_4
M=0
@m
D=M
@TEMP_5
M=D
@MULT_END_3
D;JEQ
(MULT_LOOP_3)
@m
D=M
@TEMP_4
M=D+M
@TEMP_5
M=M-1
D=M
@MULT_LOOP_3
D;JGT
(MULT_END_3)
@TEMP_4
D=M
@same
M=D

(END)
@END
0;JMP");

            // Division
            File.WriteAllText("TestPrograms/DivisionShort.asm", @"// Test division with constants
@quot1
M=12/3
@quot2
M=10/3
@quot3
M=1/1

// Test division with variables
@p
M=20
@q
M=4
@div_result
M=p/q

// Test edge cases
@self
M=p/p
@one
M=p/1

(END)
@END
0;JMP");

            File.WriteAllText("TestPrograms/DivisionLong.asm", @"// Test division with constants
// Division loop for 12/3
@TEMP_0
M=0
@3
D=A
@DIV_END_1
D;JEQ
@12
D=A
@DIV_END_1
D;JEQ
@TEMP_1
M=D
(DIV_LOOP_1)
@TEMP_1
D=M
@3
D=D-A
@DIV_END_LOOP_1
D;JLT
@TEMP_1
M=D
@TEMP_0
M=M+1
@DIV_LOOP_1
0;JMP
(DIV_END_LOOP_1)
@TEMP_0
D=M
@quot1
M=D
(DIV_END_1)

// Division loop for 10/3
@TEMP_2
M=0
@3
D=A
@DIV_END_2
D;JEQ
@10
D=A
@DIV_END_2
D;JEQ
@TEMP_3
M=D
(DIV_LOOP_2)
@TEMP_3
D=M
@3
D=D-A
@DIV_END_LOOP_2
D;JLT
@TEMP_3
M=D
@TEMP_2
M=M+1
@DIV_LOOP_2
0;JMP
(DIV_END_LOOP_2)
@TEMP_2
D=M
@quot2
M=D
(DIV_END_2)

// 1/1 = 1
@1
D=A
@quot3
M=D

// Test division with variables
@20
D=A
@p
M=D
@4
D=A
@q
M=D

// Division loop for p/q
@TEMP_4
M=0
@q
D=M
@DIV_END_3
D;JEQ
@p
D=M
@DIV_END_3
D;JEQ
@TEMP_5
M=D
(DIV_LOOP_3)
@TEMP_5
D=M
@q
D=D-M
@DIV_END_LOOP_3
D;JLT
@TEMP_5
M=D
@TEMP_4
M=M+1
@DIV_LOOP_3
0;JMP
(DIV_END_LOOP_3)
@TEMP_4
D=M
@div_result
M=D
(DIV_END_3)

// Test edge cases
@1
D=A
@self
M=D
@20
D=A
@one
M=D

(END)
@END
0;JMP");

            // Mixed operations
            File.WriteAllText("TestPrograms/MixedShort.asm", @"// Test mixed arithmetic operations
@x
M=15
@y
M=3

// Combined operations
@result1
M=x+y*2
@result2
M=(x+y)/2

// Sequential operations
@temp
M=x*y
@final
M=temp+5

// Multiple operations
@a
M=2+2
@b
M=a*3
@c
M=b/2

(END)
@END
0;JMP");

            File.WriteAllText("TestPrograms/MixedLong.asm", @"// Test mixed arithmetic operations
@15
D=A
@x
M=D
@3
D=A
@y
M=D

// Note: The original short syntax doesn't support operator precedence
// so we'll implement x+y*2 as (x+y)*2
@x
D=M
@y
D=D+M
@2
D=D+A
@result1
M=D

// For (x+y)/2
@x
D=M
@y
D=D+M
// Division loop
@TEMP_0
M=0
@2
D=A
@DIV_END_0
D;JEQ
@DIV_END_0
D;JEQ
@TEMP_1
M=D
(DIV_LOOP_0)
@TEMP_1
D=M
@2
D=D-A
@DIV_END_LOOP_0
D;JLT
@TEMP_1
M=D
@TEMP_0
M=M+1
@DIV_LOOP_0
0;JMP
(DIV_END_LOOP_0)
@TEMP_0
D=M
@result2
M=D
(DIV_END_0)

// Sequential operations: temp = x*y
// Multiplication loop
@TEMP_2
M=0
@x
D=M
@TEMP_3
M=D
@MULT_END_0
D;JEQ
(MULT_LOOP_0)
@y
D=M
@TEMP_2
M=D+M
@TEMP_3
M=M-1
D=M
@MULT_LOOP_0
D;JGT
(MULT_END_0)
@TEMP_2
D=M
@temp
M=D

// final = temp+5
@temp
D=M
@5
D=D+A
@final
M=D

// Multiple operations
@4
D=A
@a
M=D

// b = a*3
@TEMP_4
M=0
@a
D=M
@TEMP_5
M=D
@MULT_END_1
D;JEQ
(MULT_LOOP_1)
@3
D=A
@TEMP_4
M=D+M
@TEMP_5
M=M-1
D=M
@MULT_LOOP_1
D;JGT
(MULT_END_1)
@TEMP_4
D=M
@b
M=D

// c = b/2
// Division loop
@TEMP_6
M=0
@2
D=A
@DIV_END_1
D;JEQ
@b
D=M
@DIV_END_1
D;JEQ
@TEMP_7
M=D
(DIV_LOOP_1)
@TEMP_7
D=M
@2
D=D-A
@DIV_END_LOOP_1
D;JLT
@TEMP_7
M=D
@TEMP_6
M=M+1
@DIV_LOOP_1
0;JMP
(DIV_END_LOOP_1)
@TEMP_6
D=M
@c
M=D
(DIV_END_1)

(END)
@END
0;JMP");
        }
    }
}