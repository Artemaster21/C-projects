namespace operation
{
    internal class Program
    {
        static void Main()
        {
            var array = new int[5001];

            var n=int.Parse(Console.ReadLine());
            
            for (var i=0; i<n; i++)
            {
                var a = int.Parse(Console.ReadLine());
                array[a]++;
            }
            
            var list1 = new List<int>();

            for (var a=0; a<5001; a++) 
            {
                for (var b=0; b<array[a];b++)
                    list1.Add(a); 
            }           

            Console.ReadLine();

            var k = int.Parse(Console.ReadLine());

            for (var i = 0; i < k; i++)
            {
                var p = int.Parse(Console.ReadLine());
                Console.WriteLine(list1[p-1]);
            }
        }
    }
}
