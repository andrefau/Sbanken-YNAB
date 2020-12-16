using System;

namespace SbankenYnab
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Missing arguments.\nYou must supply two arguments when running this program.\nExample: dotnet run \"My account\" \"My budget\" ");
                return;
            }

            var client = new SbankenClient();
            client.init().Wait();

            Console.WriteLine("Hello World!");
            Console.WriteLine($"Args1 {args[0]}");
            Console.WriteLine($"Args2 {args[1]}");
        }
    }
}
