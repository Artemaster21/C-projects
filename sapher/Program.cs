using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ConsoleApp
{
    public class SolverNotImplementedException : Exception
    {
        public SolverNotImplementedException() { }
        public SolverNotImplementedException(string message) : base(message) { }
        public SolverNotImplementedException(string message, Exception inner) : base(message, inner) { }
    }

    public static class Cypher
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789' ";
        private static readonly int AlphabetSize = Alphabet.Length;

        private static readonly Dictionary<char, int> AlphabetMap = Alphabet
            .Select((c, i) => new { Char = c, Index = i })
            .ToDictionary(x => x.Char, x => x.Index);

        private static List<string> commonWords;
        private static Dictionary<string, int> wordFrequency;
        private static Dictionary<string, int> bigramFrequency;

        public static string SolveCypher(string question)
        {
            if (commonWords == null)
                InitializeCommonWords();

            var content = question.Substring(1, question.Length - 2);
            var parsedText = content.Split(new[] { '#', '=' }, StringSplitOptions.RemoveEmptyEntries);
            var cypherType = parsedText[0];

            switch (cypherType)
            {
                case "Caesar's code":
                    return DecodeCaesar(parsedText[2], int.Parse(parsedText[1]));
                case "prime multiplicator":
                    {
                        var multiplicatorStr = parsedText[1].Split(' ')[0];
                        return DecodePrimeMultiplicator(parsedText[2], int.Parse(multiplicatorStr));
                    }
                case "Vigenere's code":
                    return DecodeVigenere(parsedText[2], parsedText[1]);
                case "reversed":
                    return DecodeReversed(parsedText[1]);
                case "a first longest word of the message":
                    var res = DecodeSubstitution(parsedText[2], parsedText[1]);
                    return res;
                default:
                    throw new SolverNotImplementedException($"Unknown cypher type: {cypherType}");
            }
        }

        private static void InitializeCommonWords()
        {
            try
            {
                var harryPotterText = File.ReadAllText("HarryPotterText.txt");
                var sentences = SentencesParserTask.ParseSentences(harryPotterText);
                wordFrequency = new Dictionary<string, int>();
                bigramFrequency = new Dictionary<string, int>();

                foreach (var sentence in sentences)
                {
                    foreach (var word in sentence)
                    {
                        if (!wordFrequency.ContainsKey(word))
                            wordFrequency[word] = 0;
                        wordFrequency[word]++;
                    }

                    for (var i = 0; i < sentence.Count - 1; i++)
                    {
                        var bigram = sentence[i] + " " + sentence[i + 1];
                        if (!bigramFrequency.ContainsKey(bigram))
                            bigramFrequency[bigram] = 0;
                        bigramFrequency[bigram]++;
                    }
                }

                commonWords = wordFrequency
                    .Where(kv => kv.Value >= 1)
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .ToList();
            }
            catch (FileNotFoundException)
            {
                commonWords = new List<string> { "the", "and", "to", "a", "of", "he", "was", "it", "in", "you", "his", "that", "had", "with", "her", "for", "as", "she", "but", "not", "at", "him", "on", "have", "up", "all", "out", "so", "we", "be", "what", "there", "this", "got", "like", "one", "if", "about", "into", "or", "them", "me", "more", "now", "know", "who", "over", "did", "back", "see", "very", "were", "get", "then", "come", "no", "right", "oh", "its", "been", "before", "down", "man", "too", "little", "can", "could", "way", "again", "old", "your", "how", "their", "much", "where", "after", "think", "here", "just", "why", "because", "any", "many", "well", "only", "time", "even", "is", "my", "would", "said", "when", "will", "they", "are", "i", "from", "by", "has", "an" };
                wordFrequency = commonWords.ToDictionary(w => w, w => 1);
                bigramFrequency = new Dictionary<string, int>();
            }
        }

        private static string DecodeSubstitution(string encrypted, string hint)
        {
            Console.WriteLine("1");
            foreach (var spaceChar in encrypted.Distinct())
            {
                var words = encrypted.Split(new[] { spaceChar }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length < 2) continue;
                Console.WriteLine("2");
                var maxLen = words.Max(w => w.Length);
                var firstLongest = words.First(w => w.Length == maxLen);
                
                if (firstLongest.Length != hint.Length) continue;
                Console.WriteLine("3");
                var initialMapping = new Dictionary<char, char>();
                var valid = true;

                for (var i = 0; i < hint.Length; i++)
                {
                    if (initialMapping.ContainsKey(firstLongest[i]))
                    {
                        if (initialMapping[firstLongest[i]] != hint[i])
                        {
                            valid = false;
                            Console.WriteLine("valid = false");
                            break;
                        }
                    }
                    else
                    {
                        if (initialMapping.ContainsValue(hint[i]))
                        {
                            valid = false;
                            break;
                        }

                        initialMapping[firstLongest[i]] = hint[i];
                    }
                }
                Console.WriteLine("!valid");
                if (!valid) continue;
                initialMapping[spaceChar] = ' ';

                var uniqueWords = words.Distinct().ToList();
                var solutions = new List<string>();

                SolveWithBacktracking(uniqueWords, initialMapping, encrypted, hint, solutions, 200);

                if (solutions.Count > 0)
                    return solutions.OrderByDescending(ScoreSolution).First();
                return solutions.OrderByDescending(ScoreSolution).First();
            }

            return string.Empty;
        }

        private static void SolveWithBacktracking(List<string> words, Dictionary<char, char> mapping,
            string encrypted, string hint, List<string> solutions, int maxSolutions)
        {
            if (solutions.Count >= maxSolutions) return;

            var unsolvedWords = words.Where(w => !w.All(mapping.ContainsKey)).ToList();
            if (unsolvedWords.Count == 0)
            {
                var result = DecryptText(encrypted, mapping);
                if (result.Contains(hint)) solutions.Add(result);
                return;
            }

            var targetWord = unsolvedWords
                .Select(w => new
                {
                    Word = w,
                    Candidates = commonWords
                        .Where(c => c.Length == w.Length && IsCompatible(w, c, mapping))
                        .OrderByDescending(c => wordFrequency.GetValueOrDefault(c, 0))
                        .Take(30)
                        .ToList()
                })
                .Where(x => x.Candidates.Count > 0)
                .OrderBy(x => x.Candidates.Count)
                .FirstOrDefault();

            if (targetWord == null) return;

            foreach (var candidate in targetWord.Candidates)
            {
                if (solutions.Count >= maxSolutions) return;
                var newMapping = new Dictionary<char, char>(mapping);
                TryMatch(targetWord.Word, candidate, newMapping);

                if (ValidateDecryptedWords(words, newMapping))
                    SolveWithBacktracking(words, newMapping, encrypted, hint, solutions, maxSolutions);
            }
        }

        private static double ScoreSolution(string solution)
        {
            var words = solution.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var score = words.Sum(word => Math.Log(1 + wordFrequency.GetValueOrDefault(word, 0)));

            for (var i = 0; i < words.Length - 1; i++)
            {
                var bigram = words[i] + " " + words[i + 1];
                score += Math.Log(1 + bigramFrequency.GetValueOrDefault(bigram, 0)) * 5;
            }

            return score;
        }

        private static bool ValidateDecryptedWords(List<string> words, Dictionary<char, char> mapping)
        {
            return words.Where(w => w.All(mapping.ContainsKey))
                .Select(word => new string(word.Select(c => mapping[c]).ToArray()))
                .All(decrypted => wordFrequency.ContainsKey(decrypted));
        }

        private static string DecryptText(string encrypted, Dictionary<char, char> mapping)
        {
            return new string(encrypted.Select(c => mapping.ContainsKey(c) ? mapping[c] : c).ToArray());
        }

        private static bool IsCompatible(string encrypted, string plain, Dictionary<char, char> mapping)
        {
            var reverseMap = new Dictionary<char, char>();
            foreach (var kv in mapping)
                reverseMap[kv.Value] = kv.Key;

            for (var i = 0; i < encrypted.Length; i++)
            {
                if (mapping.TryGetValue(encrypted[i], out var mapped) && mapped != plain[i])
                    return false;
                if (reverseMap.TryGetValue(plain[i], out var rev) && rev != encrypted[i])
                    return false;
            }

            return true;
        }

        private static void TryMatch(string encrypted, string plain, Dictionary<char, char> mapping)
        {
            if (!IsCompatible(encrypted, plain, mapping)) return;
            for (var i = 0; i < encrypted.Length; i++)
                mapping[encrypted[i]] = plain[i];
        }

        private static long SimpleModInverse(long a, long m)
        {
            a = (a % m + m) % m;
            for (var x = 1; x < m; x++)
            {
                if (a * x % m == 1)
                    return x;
            }

            throw new SolverNotImplementedException("Modular inverse does not exist");
        }

        private static int Mod(int x, int m) => (x % m + m) % m;

        private static string DecodeCaesar(string text, int shift)
        {
            var result = new StringBuilder(text.Length);
            foreach (var newIndex in text.Select(c => AlphabetMap[c]).Select(index => Mod(index - shift, AlphabetSize)))
            {
                result.Append(Alphabet[newIndex]);
            }

            return result.ToString();
        }

        private static string DecodePrimeMultiplicator(string text, int multiplicator)
        {
            long mod = AlphabetSize + 1;
            var inverseMult = SimpleModInverse(multiplicator, mod);
            var result = new StringBuilder(text.Length);
            foreach (var originalIndex in from c in text
                                          select AlphabetMap[c]
                     into encryptedIndex
                                          select (encryptedIndex + 1L) * inverseMult % mod
                     into temp
                                          select temp - 1)
            {
                result.Append(Alphabet[(int)originalIndex]);
            }

            return result.ToString();
        }

        private static string DecodeVigenere(string text, string key)
        {
            if (string.IsNullOrEmpty(key)) return text;
            var keyIndices = key.Select(c => AlphabetMap[c]).ToArray();
            var keyLen = key.Length;
            var result = new StringBuilder(text.Length);
            for (var i = 0; i < text.Length; i++)
            {
                var index = AlphabetMap[text[i]];
                var shift = keyIndices[i % keyLen];
                var newIndex = Mod(index - shift, AlphabetSize);
                result.Append(Alphabet[newIndex]);
            }

            return result.ToString();
        }

        private static string DecodeReversed(string text)
        {
            var charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    static class SentencesParserTask
    {
        private static readonly char[] SentenceSeparators = { '.', '!', '?', ';', ':', '(', ')' };

        public static List<List<string>> ParseSentences(string text)
        {
            var rawSentences = text.Split(SentenceSeparators, StringSplitOptions.RemoveEmptyEntries);
            return rawSentences.Select(ParseSentenceIntoWords).Where(words => words.Count > 0).ToList();
        }

        private static List<string> ParseSentenceIntoWords(string sentence)
        {
            var sentenceWords = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var character in sentence)
            {
                if (char.IsLetter(character) || character == '\'')
                    currentWord.Append(character.ToString().ToLower());
                else if (currentWord.Length > 0)
                {
                    sentenceWords.Add(currentWord.ToString());
                    currentWord.Clear();
                }
            }

            if (currentWord.Length > 0)
                sentenceWords.Add(currentWord.ToString());

            return sentenceWords;
        }
    }

    // Класс с точкой входа
    class Program
    {
        static void Main(string[] args)
        {
            // Пример использования:
            // Для шифров
            string cypherQuestion = Console.ReadLine();
            string result = Cypher.SolveCypher(cypherQuestion);
            Console.WriteLine($"Decoded message: {result}");

            Console.ReadLine();
        }
    }
}