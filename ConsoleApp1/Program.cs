using System;

namespace ConsoleApp1
{
    class Program
    {
       
        static void Main(string[] args)
        {
            ImageClass img = new ImageClass();
            img.Brighten(10, false, 1);
            Console.WriteLine("You are done");
        }
    }
}
