using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task2
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Найти размер папки:");
            string folderPath = Console.ReadLine();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Путь некорректен или не существует");
                Console.ReadKey();
                return;
            }

            var folder = new DirectoryInfo(folderPath);
            long size = GetFolderSize(folder);
            Console.WriteLine($"Общий размер папки {size} байт");

            Console.ReadKey();
        }

        private static long GetFolderSize(DirectoryInfo folder)
        {
            IEnumerable<DirectoryInfo> folders = null;
            IEnumerable<FileInfo> files = null;
            long size = 0;

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

            foreach (var file in files)
                size += file.Length;

            foreach (var dir in folders)
                size += GetFolderSize(dir);

            return size;
        }
    }
}
