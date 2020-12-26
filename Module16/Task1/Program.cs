using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task1
{
    class Program
    {
        private static DateTime TimeToCheck;

        static void Main()
        {
            Console.WriteLine("Folder to clean:");
            string folderPath = Console.ReadLine();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Путь некорректен или не существует");
                Console.ReadKey();
                return;
            }

            var folder = new DirectoryInfo(folderPath);
            ShowFolderFiles(folder);

            TimeToCheck = DateTime.Now.AddMinutes(-30);
            Console.ForegroundColor = ConsoleColor.Red;
            CleanFolder(folder);
            Console.ForegroundColor = ConsoleColor.Yellow;

            ShowFolderFiles(folder);

            Console.ReadKey();
        }

        private static void ShowFolderFiles(DirectoryInfo folder, int offset = 0)
        {
            var strOffset = new string('\t', offset);

            GetFolderContent(folder, out IEnumerable<DirectoryInfo> folders, out IEnumerable<FileInfo> files);

            foreach (var dir in folders)
            {
                Console.WriteLine($"{strOffset}{dir.Name}");
                ShowFolderFiles(dir, offset + 1);
            }

            foreach (var file in files)
                Console.WriteLine($"{strOffset}{file.Name} [{file.LastAccessTime}]");
        }

        private static void CleanFolder(DirectoryInfo folder)
        {
            GetFolderContent(folder, out IEnumerable<DirectoryInfo> folders, out IEnumerable<FileInfo> files);

            foreach (var file in files)
            {
                if (file.LastAccessTime < TimeToCheck)
                    try
                    {
                        file.Delete();
                        Console.WriteLine($"Удален файл {file.FullName}");
                    }
                    catch
                    {
                        Console.WriteLine($"Не удается удалить {file.FullName}");
                    }
            }

            foreach (var dir in folders)
                CleanFolder(dir);
        }

        private static void GetFolderContent(in DirectoryInfo folder, out IEnumerable<DirectoryInfo> folders, out IEnumerable<FileInfo> files)
        {
            folders = null;
            files = null;

            try
            {
                folders = folder.GetDirectories();
                files = folder.GetFiles();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа по пути");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine($"Слишком длинный путь");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Непредвиденная ошибка: {e}");
            }
            finally
            {
                folders ??= Enumerable.Empty<DirectoryInfo>();
                files ??= Enumerable.Empty<FileInfo>();
            }
        }
    }
}
