
namespace op28_10
{
    internal class Program
    {
        static void Main()
        {
            //var a = Console.ReadLine().Split();

            //var n = a.Count();
            //var s1 = new List<int>(n);
            //s1.AddRange(a.Select(x => int.Parse(x)));

            //var b = Console.ReadLine().Split().ToArray(); во втором полугодии

            //var N = b.Count();
            //var s2 = new List<int>(N);
            //s2.AddRange(b.Select(x => int.Parse(x)));

            var sN = Console.ReadLine().Split();
            var sn = Console.ReadLine().Split();

            var n = new int[100];
            var N= new int[100];
            for (var i = 0; i < sn.Length; i++) 
            {
                n[int.Parse(sn[i])-1]++;
                N[int.Parse(sN[i]) - 1]++;
            }

            Console.WriteLine(n.SequenceEqual(N));

            for (var i = sn.Length; i < sN.Length; i++)
            {
                N[int.Parse(sN[i - sn.Length]) - 1]--;
                N[int.Parse(sN[i]) - 1]++;
                Console.WriteLine(N.SequenceEqual(n));
            }
            //for (var i = 0; i < n; i++)
            //    s[s1[i]] += 1;
            //for (var i = 0;i < N; i++)
            //    sn[s2[i]] += 1;
            //for (var i = n; i < N; i++)
            //{
            //    sn[s2[i - n] - 1]--;
            //    sn[s2[i] - 1]++;
            //    for (var j = 0; j < N; j++)
            //    {
            //        if (s[j] != sn[j])
            //            Console.WriteLine((i - n + 1).ToString(), i);
            //    }
            //    Console.WriteLine((i - n + 1).ToString(), i);
            //}
        }
    }
}
