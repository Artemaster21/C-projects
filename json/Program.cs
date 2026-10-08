using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;

class Program
{
    static long E(string s)
    {
        if (s.StartsWith("math|")) s = s.Substring(5);
        s = s.Replace(" ", "");
        var v = new Stack<long>();
        var o = new Stack<char>();

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (char.IsDigit(c))
            {
                long n = 0;
                while (i < s.Length && char.IsDigit(s[i]))
                    n = n * 10 + (s[i++] - '0');
                i--;
                v.Push(n);
            }
            else if (c == '(') o.Push(c);
            else if (c == ')')
            {
                while (o.Count > 0 && o.Peek() != '(')
                    v.Push(O(o.Pop(), v.Pop(), v.Pop()));
                if (o.Count > 0) o.Pop();
            }
            else if ("+-*/%".Contains(c))
            {
                while (o.Count > 0 && P(c, o.Peek()))
                    v.Push(O(o.Pop(), v.Pop(), v.Pop()));
                o.Push(c);
            }
        }
        while (o.Count > 0) v.Push(O(o.Pop(), v.Pop(), v.Pop()));
        return v.Pop();
    }

    static bool P(char a, char b) => !(b == '(' || b == ')' ||
        ("*/%".Contains(a) && "+-".Contains(b)));

    static long O(char op, long b, long a) => op switch
    {
        '+' => a + b,
        '-' => a - b,
        '*' => a * b,
        '/' => b == 0 ? 0 : a / b,
        '%' => b == 0 ? 0 : a % b,
        _ => 0
    };

    static long D(string s)
    {
        s = s.Substring(12);
        var r = s.Split(new[] { " \\\\ " }, StringSplitOptions.RemoveEmptyEntries);
        int n = r.Length;
        var m = new long[n, n];
        for (int i = 0; i < n; i++)
        {
            var e = r[i].Split(new[] { " & " }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < n; j++) m[i, j] = long.Parse(e[j]);
        }
        if (n == 2) return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
        if (n == 3) return m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) -
                          m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                          m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
        long d = 0;
        for (int j = 0; j < 4; j++)
        {
            var mi = new long[3, 3];
            int mr = 0;
            for (int rw = 1; rw < 4; rw++)
            {
                int mc = 0;
                for (int c = 0; c < 4; c++)
                    if (c != j) mi[mr, mc++] = m[rw, c];
                mr++;
            }
            long md = mi[0, 0] * (mi[1, 1] * mi[2, 2] - mi[1, 2] * mi[2, 1]) -
                      mi[0, 1] * (mi[1, 0] * mi[2, 2] - mi[1, 2] * mi[2, 0]) +
                      mi[0, 2] * (mi[1, 0] * mi[2, 1] - mi[1, 1] * mi[2, 0]);
            d += (j % 2 == 0 ? 1 : -1) * m[0, j] * md;
        }
        return d;
    }

    static long W(string w) => w.ToLower() switch
    {
        "zero" => 0, "one" => 1, "two" => 2,"three" => 3, "four" => 4, "five" => 5, "six" => 6, "seven" => 7, "eight" => 8, "nine" => 9, "ten" => 10,
        "eleven" => 11, "twelve" => 12, "thirteen" => 13, "fourteen" => 14, "fifteen" => 15, "sixteen" => 16, "seventeen" => 17, "eighteen" => 18,
        "nineteen" => 19, "twenty" => 20,
        _ => throw new Exception($"Unknown: {w}")
    };

    static long C(string s)
    {
        s = s.Substring(7);
        var p = s.Split('#');
        string info = p[1];
        string abc = p[2].Substring(p[2].IndexOf('=') + 1);
        string enc = p[3];

        if (info.Contains("Caesar"))
        {
            int shift = int.Parse(info.Substring(info.IndexOf('=') + 1));
            var sb = new StringBuilder();
            foreach (char c in enc)
            {
                int idx = abc.IndexOf(c);
                if (idx >= 0)
                {
                    int ni = (idx - shift) % abc.Length;
                    if (ni < 0) ni += abc.Length;
                    sb.Append(abc[ni]);
                }
                else sb.Append(c);
            }
            return W(sb.ToString());
        }
        else // Vigenere
        {
            string key = info.Substring(info.IndexOf('=') + 1);
            var sb = new StringBuilder();
            var ki = 0;
            foreach (char c in enc)
            {
                var ci = abc.IndexOf(c);
                if (ci >= 0)
                {
                    int ki2 = abc.IndexOf(key[ki % key.Length]);
                    int ni = (ci - ki2) % abc.Length;
                    if (ni < 0) ni += abc.Length;
                    sb.Append(abc[ni]);
                    ki++;
                }
                else sb.Append(c);
            }
            return W(sb.ToString());
        }
    }

    static long S(JsonElement e)
    {
        long t = 0;
        switch (e.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var p in e.EnumerateObject()) t += S(p.Value); break;
            case JsonValueKind.Array:
                foreach (var i in e.EnumerateArray()) t += S(i); break;
            case JsonValueKind.String:
                string v = e.GetString();
                if (v.StartsWith("math|")) t += E(v.Substring(5));
                else if (v.StartsWith("determinant|")) t += D(v);
                else if (v.StartsWith("string-number|")) t += W(v.Substring(14));
                else if (v.StartsWith("cypher|")) t += C(v);
                else if (long.TryParse(v, out long n)) t += n;
                break;
            case JsonValueKind.Number:
                t += e.GetInt64(); break;
        }
        return t;
    }

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Enter JSON (empty line to finish, 'exit' to quit):");
            var sb = new StringBuilder();

            while (true)
            {
                string l = Console.ReadLine();
                if (l == null || l.Trim().ToLower() == "exit") return;
                if (string.IsNullOrEmpty(l)) break;
                sb.AppendLine(l);
            }

            string j = sb.ToString().Trim();
            if (string.IsNullOrWhiteSpace(j)) continue;

            try
            {
                var doc = JsonDocument.Parse(j);
                Console.WriteLine($"Result: {S(doc.RootElement)}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }
    }
}