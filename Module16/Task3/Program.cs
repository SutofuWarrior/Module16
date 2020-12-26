using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task3
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
            long size = GetFolderSize(folder);
            Console.WriteLine($"Размер папки перед удалением: {size} байт");

            TimeToCheck = DateTime.Now.AddMinutes(-30);
            int deleted;
            (deleted, size) = CleanFolder(folder);

            Console.WriteLine($"Удалено файлов: {deleted} шт.");
            Console.WriteLine($"Размер удаленных файлов: {size} байт");

            size = GetFolderSize(folder);
            Console.WriteLine($"Размер папки после удаления: {size} байт");

            Console.ReadKey();
        }

        private static long GetFolderSize(DirectoryInfo folder)
        {
            long size = 0;
            GetFolderContent(folder, out IEnumerable<DirectoryInfo> folders, out IEnumerable<FileInfo> files);

            foreach (var file in files)
                size += file.Length;

            foreach (var dir in folders)
                size += GetFolderSize(dir);

            return size;
        }

        private static (int, long) CleanFolder(DirectoryInfo folder)
        {
            GetFolderContent(folder, out IEnumerable<DirectoryInfo> folders, out IEnumerable<FileInfo> files);

            int deleted = 0;
            long size = 0;

            foreach (var file in files)
            {
                //if (file.LastWriteTime < TimeToCheck)
                if (file.LastAccessTime < TimeToCheck)
                    try
                    {
                        file.Delete();

                        deleted++;
                        size += file.Length;

                        Console.WriteLine($"Удален файл {file.FullName}");
                    }
                    catch
                    {
                        Console.WriteLine($"Не удается удалить {file.FullName}");
                    }
            }

            int subDeleted;
            long subSize;

            foreach (var dir in folders)
            {
                (subDeleted, subSize) = CleanFolder(dir);

                deleted += subDeleted;
                size += subSize;
            }

            return (deleted, size);
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
