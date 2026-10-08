using System.Collections.Generic;
using System.Linq;
using System;

namespace Assembler
{
    public class Preprocessor
    {   /// Преобразует нестандартные макро-инструкции в инструкции обычного языка ассемблера.
        public string[] PreprocessAsm(string[] instructions)
        {
            var asmCode = new List<string>();
            for (int i = 0; i < instructions.Length; i++)
            {
                var instr = instructions[i];
                try
                {
                    TranslateInstruction(instr, asmCode);
                }
                catch (Exception e)
                {
                    throw new FormatException($"Can't parse at line {i + 1}: {instr}", e);
                }
            }

            return asmCode.ToArray();
        }


        public void TranslateInstruction(string instruction, List<string> asmCode)
        {
            var queueInstructions = new Queue<string>();
            queueInstructions.Enqueue(instruction);

            // Проход по всем трансляторам инструкций
            foreach (var translator in translators)
            {
                var count = queueInstructions.Count;

                // Каждый транслятор получает по одной инструкций из очереди
                for (int i = 0; i < count; i++) // Транслятор возвращает список получившихся инструкций
                    foreach (var newInstruction in translator.TryTranstale(queueInstructions.Dequeue()))
                        queueInstructions.Enqueue(newInstruction);
            }
            asmCode.AddRange(queueInstructions);
        }


        private readonly List<IInstructionTranslator> translators = new()
        {
            new DRegisterJumpTranslator(),
            new SquareBracketsTranslator(),
        };
    }

    public interface IInstructionTranslator
    {
        string[] TryTranstale(string instruction);
    }

    public class DRegisterJumpTranslator : IInstructionTranslator
    {
        public string[] TryTranstale(string instruction)
        {
            if (instruction.Length >= 3)
            {
                var jump = instruction[..3];

                if (jump == "JMP")
                    return new string[] { "0;" + instruction };
                else if (jumpInstructions.Contains(jump))
                    return new string[] { "D;" + instruction };
            }

            return new string[] { instruction };
        }

        private static HashSet<string> jumpInstructions = new()
        {
            { "JGT" },
            { "JEQ" },
            { "JGE" },
            { "JLT" },
            { "JNE" },
            { "JLE" },
        };
    }

    public class SquareBracketsTranslator : IInstructionTranslator
    {
        public string[] TryTranstale(string instruction)
        {
            var leftBracket = instruction.IndexOf('[');
            var rightBracket = instruction.IndexOf(']');

            if (leftBracket == -1 || rightBracket == -1)
                return new string[] { instruction };

            string address = instruction.Substring(leftBracket + 1, rightBracket - leftBracket - 1);

            return new string[] {
                "@" + address,
                instruction.Replace($"[{address}]", "")
            };
        }
    }
}