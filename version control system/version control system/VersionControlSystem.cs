using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace version_control_system
{
    static class VersionControlSystem
    {
        public const string databasePath = @"Tracked directories\";
        public const string folderPathsPath = @"Folder paths\";
        public static TrackedDirectory activeDirectory;

        static List<TrackedDirectory> directories = new List<TrackedDirectory>();

        public static bool ActiveDirectoryExist()
        {
            if (activeDirectory == null)
            {
                Console.WriteLine("Сначала выберете активную дирректорию!");
                return (false);
            }
            return (true);
        }

        public static void Read()
        {
            DirectoryInfo dir = new DirectoryInfo(folderPathsPath);
            foreach (FileInfo fileInDir in dir.GetFiles())
            {
                StreamReader path_reader = new StreamReader(fileInDir.FullName, Encoding.Default);
                string dirFullName = path_reader.ReadToEnd().Replace("\0", "");
                if (!Directory.Exists(dirFullName))
                {
                    Console.WriteLine("!!! Директория " + dirFullName + " была удалена или перемещена");
                    continue;
                }
                directories.Add(new TrackedDirectory(dirFullName));
                path_reader.Close();
            }
        }

        public static void Save()
        {
            foreach (TrackedDirectory dirInDirectories in directories)
            {
                dirInDirectories.Save();
            }
            Environment.Exit(0);
        }

        public static void ListBranch()
        {
            Console.WriteLine("Отслеживаемые папки: ");
            int x = 0;
            foreach (TrackedDirectory dirInDirectories in directories)
            {
                x++;
                Console.WriteLine(x + ". " + dirInDirectories.fullPath);
            }
        }

        public static void Apply(string fullPath)
        {
            foreach (TrackedDirectory dirInDirectories in directories)
            {
                if (dirInDirectories.fullPath == fullPath)
                {
                    dirInDirectories.Apply();
                    Console.WriteLine("Изменения в папке " + fullPath + " сохранены");
                    return;
                }
            }
            Console.WriteLine("Папки  " + fullPath + " не существует");
            return;
        }

        public static void Checkout(string fullPath)
        {
            foreach (TrackedDirectory dirInDirectories in directories)
            {
                if (dirInDirectories.fullPath == fullPath)
                {
                    activeDirectory = dirInDirectories;
                    Console.WriteLine("Директория " + fullPath + " выбрана");
                    return;
                }
            }
            Console.WriteLine("Папки " + fullPath + " не существует");
            return;
        }

        public static void Checkout(int dirNumder)
        {
            if (dirNumder < 1 || dirNumder > directories.Count)
            {
                Console.WriteLine("Неверно задан номер директории");
                return;
            }
            activeDirectory = directories[dirNumder -1];
            Console.WriteLine("Директория " + activeDirectory.fullPath + " выбрана");
            return;
        }

        public static void Init(string fullPath)
        {
            if (!Directory.Exists(fullPath))
            {
                Console.WriteLine("Введённой вами директории не существует!");
                return;
            }

            DirectoryInfo item = new DirectoryInfo(fullPath);

            if (File.Exists(databasePath + item.Name + ".txt") || File.Exists(folderPathsPath + item.Name + ".txt"))
            {
                Console.WriteLine("Введённая вами директория уже отслеживается");
                return;
            }

            File.Create(folderPathsPath + item.Name + ".txt").Close();
            File.WriteAllText(folderPathsPath + item.Name + ".txt", fullPath, Encoding.Default);

            File.Create(databasePath + item.Name + ".txt").Close();
            TrackedDirectory newDir = new TrackedDirectory(fullPath);
            foreach (TrackedFile dataSet in newDir.files)
            {
                dataSet.mark = Mark.Inited;
            }
            activeDirectory = newDir;
            directories.Add(newDir);
            activeDirectory.Save();
            Console.WriteLine("Директория добавлена под контроль");
        }

        static public void Help()
        {
            Style.CommandColor();
            Console.WriteLine("Список команд:");
            Style.SingleCommandColor();
            Console.WriteLine("\nОдиночные команды:");
            Style.CommandColor();
            Console.Write("help");
            Style.SingleCommandColor();
            Console.Write(" – выводит на экран все возможные комманды");
            Style.CommandColor();
            Console.Write("\nlist branch");
            Style.SingleCommandColor();
            Console.Write(" – показать все отслеживаемые папки");
            Style.CommandColor();
            Console.Write("\nstatus");
            Style.SingleCommandColor();
            Console.Write(" - отображение статуса отслеживаемых файлов последней проинициализированной папки (какие файлы отслеживаются, краткая информация по ним) Note: ");
            Style.FileChangedColor();
            Console.Write("красным выделяется измененный файл, ");
            Style.FileNotChangedColor();
            Console.Write("зеленым, соответственно, нет");
            Style.CommandColor();
            Console.Write("\napply");
            Style.SingleCommandColor();
            Console.Write(" – сохранить все изменения в отслеживаемой папке ");
            Style.CommandColor();
            Console.Write("\nexit");
            Style.SingleCommandColor();
            Console.Write(" – сохранить все изменения во всех папках и закрыть программу");
            Style.CommandWithOperatorColor();
            Console.WriteLine("\n\nКоманды с операторами:");
            Console.Write("Все команды с операторами вводятся так: ");
            Style.CommandColor();
            Console.Write("команда пробел требуеммые для данной команды данные");
            Console.Write("\ninit путь к папке");
            Style.CommandWithOperatorColor();
            Console.Write(" - инициализация СКВ для папки с файлами");
            Style.CommandColor();
            Console.Write("\nadd имя файла");
            Style.CommandWithOperatorColor();
            Console.Write(" - добавить файл под версионный контроль");
            Style.CommandColor();
            Console.Write("\nremove имя файла");
            Style.CommandWithOperatorColor();
            Console.Write(" - удалить файл из-под версионного контроля");
            Style.CommandColor();
            Console.Write("\napply путь к папке");
            Style.CommandWithOperatorColor();
            Console.Write(" - сохранить все изменения в ведённой папке папке ");
            Style.CommandColor();
            Console.Write("\ncheckout путь к папке");
            Style.CommandWithOperatorColor();
            Console.Write(" - перейти к указанной отслеживаемой директории");
            Style.CommandColor();
            Console.Write("\ncheckout# номер папки");
            Style.CommandWithOperatorColor();
            Console.Write(" (чтобы узнать номер используйте команду ");
            Style.CommandColor();
            Console.Write("list branch");
            Style.CommandWithOperatorColor();
            Console.Write(")");            
            Console.Write(" - перейти к указанной отслеживаемой директории");
            Style.Default();
            Console.WriteLine();
        }

        public static void ExecuteCommand(string userInput)
        {
            switch (userInput)
            {
                case "help":
                    Help();
                    return;
                case "status":
                    if (ActiveDirectoryExist())
                    {
                        activeDirectory.Update();
                        activeDirectory.Status();
                    }
                    else
                    {
                        return;
                    }
                    return;
                case "apply":
                    if (ActiveDirectoryExist())
                    {
                        activeDirectory.Apply();
                        Console.WriteLine("Изменения в активной дирректории: " + activeDirectory.path + " сохранены");
                    }
                    else
                    {
                        return;
                    }
                    return;
                case "exit":
                    Save();
                    break;
                default:
                    break;
            }

            int spaceIndex = userInput.IndexOf(" ");

            if (spaceIndex == -1)
            {
                Console.WriteLine("Введённая вами команда не корректна");
                return;
            }

            string commandType = userInput.Substring(0, spaceIndex);
            string commandData = userInput.Substring(spaceIndex + 1, userInput.Length - spaceIndex - 1);

            if (commandType == String.Empty || commandData == String.Empty)
            {
                Console.WriteLine("Введённая вами команда не корректна");
                return;
            }

            switch (commandType)
            {
                case "init":
                    Init(commandData);
                    break;
                case "add":
                    if (ActiveDirectoryExist())
                    {
                        activeDirectory.Update();
                        TrackedFile result = activeDirectory.FindAndReturn(commandData);
                        if (result == null)
                        {
                            Console.WriteLine("Файла " + commandData + " не существует!");
                            return;
                        }
                        if (result.mark == Mark.Added || result.mark == Mark.Inited)
                        {
                            Console.WriteLine("Файл " + commandData + " уже находится под контролем");
                            return;
                        }
                        if (result.mark == Mark.Deleted)
                        {
                            Console.WriteLine("Файл " + commandData + " был удалён!");
                            return;
                        }
                        result.mark = Mark.Added;
                        Console.WriteLine("Файл " + commandData + " добавлен под контроль");
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "remove":
                    if (ActiveDirectoryExist())
                    {
                        activeDirectory.Update();
                        TrackedFile result = activeDirectory.FindAndReturn(commandData);
                        if (result == null)
                        {
                            Console.WriteLine("Файла " + commandData + " не существует!");
                            return;
                        }
                        if (result.mark == Mark.Removed || result.mark == Mark.New)
                        {
                            Console.WriteLine("Файл " + commandData + " не находится под контролем");
                            return;
                        }
                        if (result.mark == Mark.Deleted)
                        {
                            Console.WriteLine("Файл " + commandData + " был удалён!");
                            return;
                        }
                        result.mark = Mark.Removed;
                        Console.WriteLine("Файл " + commandData + " удалён из под контроля");
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "apply":
                    if (directories.Count == 0)
                    {
                        Console.WriteLine("Список отслеживаемых папок пуст");
                    }
                    else
                    {
                        Apply(commandData);
                    }
                    break;
                case "list":
                    if (directories.Count == 0)
                    {
                        Console.WriteLine("Список отслеживаемых папок пуст");
                    }
                    else
                    {
                        if (commandData == "branch")
                        {
                            ListBranch();
                        }
                        else
                        {
                            Console.WriteLine("Введённая вами команда не корректна");
                        }
                    }
                    break;
                case "checkout":
                    if (directories.Count == 0)
                    {
                        Console.WriteLine("Список отслеживаемых папок пуст");
                    }
                    else
                    {
                        Checkout(commandData);
                    }
                    break;
                case "checkout#":
                    if (directories.Count == 0)
                    {
                        Console.WriteLine("Список отслеживаемых папок пуст");
                    }
                    else
                    {
                        try
                        {
                            Checkout(Int32.Parse(commandData));
                        }
                        catch
                        {
                            Console.WriteLine("Введённое вами число не корректно");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Введённой вами команды не существует");
                    break;
            }
        }
    }
}