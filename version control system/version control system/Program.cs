using System;

namespace version_control_system
{
    class Program
    {
        static void Main(string[] args)
        {
            VersionControlSystem.Read();
            while (true)
            {
                Console.Write("\nДля просмотра списка всех комманд введите \"");
                Style.CommandColor();
                Console.Write("help");
                Style.Default();
                Console.Write("\"");
                Style.InputColor();
                Console.Write("\nВедите команду: ");
                Style.CommandColor();
                string input = Console.ReadLine();
                Style.Default();
                Console.WriteLine();
                input = input.TrimStart(' ');
                input = input.TrimEnd(' ');
                VersionControlSystem.ExecuteCommand(input);
            }
        }
    }
}
