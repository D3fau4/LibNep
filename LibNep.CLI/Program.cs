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
                    Console.WriteLine("Version: 0x" + tid.version.ToString("x"));
                    Console.WriteLine("Width: " + tid.Width);
                    Console.WriteLine("Height: " + tid.Height);
                    Console.WriteLine("Data Lenght: " + tid.DataLength);
                    break;
                case "CL3":
                    CL3 cL3 = new CL3(args[1]);
                    break;
                case "SSA":
                    SSA ssa = new SSA(args[1]);
                    ssa.Upscale(2);
                    break;
            }
        }
    }
}
