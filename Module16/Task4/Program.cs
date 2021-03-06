﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Task4
{
    class Program
    {
        static void Main()
        {
            string sourcePath = Path.Combine(AppContext.BaseDirectory, "Students2.dat");
            CreateMySource(sourcePath);

            if (!File.Exists(sourcePath))
            {
                Console.WriteLine("Не найден файл Students.dat");
                Console.ReadKey();
                return;
            }

            var students = GetStudentsFromSource(sourcePath);
            var groups = GroupStudents(students);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string folderPath = Path.Combine(desktopPath, "Students");

            ExportGroups(groups, folderPath);

            Console.ReadKey();
        }

        private static IEnumerable<Student> GetStudentsFromSource(string sourcePath)
        {
            using var file = File.OpenRead(sourcePath);
            var formatter = new BinaryFormatter();
            //formatter.Binder = new PreMergeToMergedDeserializationBinder();

            try
            {
                return (Student[])formatter.Deserialize(file);
                //{"Unable to find assembly 'FinalTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'."}
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось считать БД из файла. Ошибка: {e}");
                return Enumerable.Empty<Student>();
            }
        }

        private static IDictionary<string, IList<Student>> GroupStudents(IEnumerable<Student> students)
        {
            var result = new Dictionary<string, IList<Student>>();

            foreach (var student in students)
            {
                if (!result.ContainsKey(student.Group))
                    result.Add(student.Group, new List<Student>());

                result[student.Group].Add(student);
            }

            return result;
        }

        private static void ExportGroups(IDictionary<string, IList<Student>> groups, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                try
                {
                    Directory.CreateDirectory(folderPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при создании папки: {e}");
                    return;
                }

            foreach (var group in groups)
            {
                string groupFilePath = Path.Combine(folderPath, $"{group.Key}.txt");

                try
                {
                    //IEnumerable<string> exportLines = group.Value.Select(s => s.ToString());
                    //File.WriteAllLines(groupFilePath, exportLines);
                    
                    using var file = new FileStream(groupFilePath, FileMode.CreateNew, FileAccess.Write);
                    using var writer = new StreamWriter(file);

                    foreach (var student in group.Value)
                        writer.WriteLine($"{student.Name}, {student.DateOfBirth}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при записи в файл {groupFilePath}: {e}");
                    break;
                }
            }
        }

        private static void CreateMySource(string sourcePath)
        {
            var students = new Student[]
            {
                new Student { Name = "Name1", Group = "Group1", DateOfBirth = new DateTime(2000, 1, 1) },
                new Student { Name = "Name2", Group = "Group2", DateOfBirth = new DateTime(2001, 2, 3) },
                new Student { Name = "Name3", Group = "Group1", DateOfBirth = new DateTime(2002, 3, 5) },
                new Student { Name = "Name4", Group = "Group3", DateOfBirth = new DateTime(2003, 4, 7) },
                new Student { Name = "Name5", Group = "Group2", DateOfBirth = new DateTime(2004, 5, 9) }
            };

            File.Delete(sourcePath);

            using var file = new FileStream(sourcePath, FileMode.CreateNew, FileAccess.Write);
            var formatter = new BinaryFormatter();

            formatter.Serialize(file, students);
        }
    }

    [Serializable]
    public class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Student()
        {
        }

        public Student(string name, string group, DateTime date)
        {
            Name = name;
            Group = group;
            DateOfBirth = date;
        }
    }

    //public sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
    //{
    //    public override Type BindToType(string assemblyName, string typeName)
    //    {
    //        // For each assemblyName/typeName that you want to deserialize to
    //        // a different type, set typeToDeserialize to the desired type.
    //        string exeAssembly = Assembly.GetExecutingAssembly().FullName;
    //        return Type.GetType($"{typeName}, {exeAssembly}");
    //    }
    //}
}
