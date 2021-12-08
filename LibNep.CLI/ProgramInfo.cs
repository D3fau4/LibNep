using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNep.CLI
{
    public static class ProgramInfo
    {
        public static int MAJOR = 1;
        public static int MENOR = 0;
        

        public static string GetVersion()
        {
            return MAJOR + "." + MENOR;
        }
    }
}
