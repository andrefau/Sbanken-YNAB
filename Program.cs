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

            Console.WriteLine("*** EXIT ***");
        }
    }
}
