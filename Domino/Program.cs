using System.Security.Principal;

namespace Domino
{
    public class DominoPiece
    {
        int LeftValue;
        int RightValue;
        bool uses;

        public void Rotate() => (LeftValue, RightValue) 
                              = (RightValue, LeftValue);
    }

    public class DominoSet()
    {
        DomSet= GetDomSet();
        
    }
    public class BoardPlase()
    {


    }
    internal class Program
    {

        static void Main(string[] args)
        {

        }
    }
}
