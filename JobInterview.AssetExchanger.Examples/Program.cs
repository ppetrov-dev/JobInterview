using System;

namespace JobInterview.AssetExchanger.Examples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var bootstrapper = new Bootstrapper();

            bootstrapper.Run();

            //if (bootstrapper.Container == null)
            //    Console.WriteLine("Hello World!");
        }
    }
}