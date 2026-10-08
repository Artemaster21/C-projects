namespace MinPolindrom
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s = "abqwertydsfghjtfcgfh";
            Console.WriteLine(GetPolin(s));
        }

        public static string GetPolin(string s)
        {
            if (s.Length < 2) return "";

            var bestStart = -1;
            var bestLength = 0;

            for (var i = 0; i < s.Length; i++)
            {
                if (i + 1 < s.Length && s[i] == s[i + 1])
                {
                    if (IsBetter(i, 2, bestStart, bestLength, s))
                    {
                        bestStart = i;
                        bestLength = 2;
                    }
                }

                if (i + 2 < s.Length && s[i] == s[i + 2])
                {
                    if (IsBetter(i, 3, bestStart, bestLength, s))
                    {
                        bestStart = i;
                        bestLength = 3;
                    }
                }
            }

            return bestStart == -1 ? "нет" : s.Substring(bestStart, bestLength);
        }

        private static bool IsBetter(int newStart, int newLength, int oldStart, int oldLength, string s)
        {
            if (oldStart == -1) return true;

            if (newLength < oldLength) return true;
            if (newLength > oldLength) return false;


            for (int i = 0; i < newLength; i++)
            {
                if (s[newStart + i] < s[oldStart + i]) return true;
                if (s[newStart + i] > s[oldStart + i]) return false;
            }

            return false; 
        }
    }
}