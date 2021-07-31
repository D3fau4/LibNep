using System;
using LibNep.FileFormats;

namespace LibNep.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcomen to LibNep");
            switch (args[0])
            {
                case "TID":
                    TID tid = new TID(args[1]);
                    Console.WriteLine("Name: " + tid.Name);
                    Console.WriteLine("Version: " + tid.version);
                    Console.WriteLine("Width: " + tid.Width);
                    Console.WriteLine("Height: " + tid.Height);
                    break;
            }
        }
    }
}
