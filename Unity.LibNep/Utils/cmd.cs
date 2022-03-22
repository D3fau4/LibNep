using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNep.Utils
{
    public class cmd
    {
        public enum LogType
        {
            Error = ConsoleColor.Red,
            Normal = ConsoleColor.White,
            Warning = ConsoleColor.Yellow,
            Success = ConsoleColor.Green,
            Info = ConsoleColor.Magenta
        }

        public static void print(string message, LogType color = LogType.Normal)
        {
            Console.ForegroundColor = (ConsoleColor)color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void print(int message, LogType color = LogType.Normal)
        {
            Console.ForegroundColor = (ConsoleColor)color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
