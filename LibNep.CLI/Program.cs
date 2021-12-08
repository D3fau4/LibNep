using System;
using LibNep.FileFormats;
using LibNep.Utils;

namespace LibNep.CLI
{
    class Program
    {
        public static void Printinfo()
        {
            cmd.print("LibNep.CLI.exe [FILE FORMAT] [OPTIONS] [FILE]");
            cmd.print("SSA --upscale [SCALE]");
        }

        static void Main(string[] args)
        {
            cmd.print("Welcomen to LibNep.CLI - Version: " +  ProgramInfo.GetVersion(), cmd.LogType.Info);
            switch (args[0])
            {
                case "TID":
                    TID tid = new TID(args[1]);
                    cmd.print("Name: " + tid.Name);
                    cmd.print("Version: 0x" + tid.version.ToString("x"));
                    cmd.print("Width: " + tid.Width);
                    cmd.print("Height: " + tid.Height);
                    cmd.print("Data Lenght: " + tid.DataLength);
                    break;
                case "CL3":
                    CL3 cL3 = new CL3(args[1]);
                    break;
                case "SSA":
                    if (args[1] != null)
                    {
                        if (args[1].Contains("--upscale"))
                        {
                            if (int.TryParse(args[2], out int result))
                            {
                                SSA ssa = new SSA(args[3]);
                                ssa.Upscale(result);
                            }
                        } else
                        {
                            Printinfo();
                        }
                    }
                    else
                    {
                        cmd.print("No option found", cmd.LogType.Error);
                        Printinfo();
                    }
                    
                    break;
            }
        }
    }
}
