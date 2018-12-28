using System;

namespace version_control_system
{
    static class Style
    {
        public static void FileNotChangedColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        public static void FileChangedColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }

        public static void CommandColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }

        public static void SingleCommandColor()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }

        public static void CommandWithOperatorColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }

        public static void InputColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }

        public static void Default()
        {
            Console.ResetColor();
        }
    }
}
