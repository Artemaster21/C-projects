namespace practic10._02._26
{
    //public class ListStack
    //{
    //    List<int> Stack = new List<int>();
    //    List<int> maxStack = new List<int>();
    //    public void Push(int value)
    //    {
    //        Stack.Add(value);

    //        if (maxStack.Count == 0 )
    //            maxStack.Add(value);
    //        else 
    //            if (maxStack[^1] <= value)
    //                maxStack.Add(value);
    //    }
    //    public int Pop()
    //    {
    //        if (Stack.Count == 0) throw new InvalidOperationException();
    //        var result = Stack[Stack.Count - 1];
    //        Stack.RemoveAt(Stack.Count - 1);
    //        if (result == maxStack[^1])
    //            maxStack.Remove(result);
    //        return result;
    //    }
    //}

    public class Superhero 
    {
        private static readonly Lazy<Superhero> superman = new Lazy<Superhero>(() => new Superhero("Superman"));
        private static readonly Lazy<Superhero> batman = new Lazy<Superhero>(() => new Superhero("Batman"));

        public string Name { get; }
        private Superhero(string name)
        {
            Name = name;
        }

        public static Superhero Superman
        {
            get 
            {
                return superman.Value;    
            }
        }

        public static Superhero Batman
        {
            get
            {
                return batman.Value;
            }
        }
    }
}
