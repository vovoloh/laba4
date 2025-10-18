using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace laba4
{
    internal class Program
    {
        interface IStudentInfo
        {
            void ShowInfo();
        }

        interface IGrade
        {
            double Average(); 
            bool HasExcellent();  
        }
        class Student : IStudentInfo, IGrade
        {
            public string Institute;
            public int Course;
            public string Group;
            public string Name;
            public List<int> Grades;

            public Student(string name, List<int> grades, string group, int course, string institute)
            {
                Name = name;
                Grades = grades;
                Group = group;
                Course = course;
                Institute = institute;
            }
            public bool HasTwo()
            {
                int countTwo = Grades.Count(grade => grade == 2);
                return countTwo >= 1;
            }
            public void ShowInfo()
            {
                Console.WriteLine($"{Name} | Институт: {Institute} | Группа: {Group} | Курс: {Course} | Оценки: {string.Join(",", Grades)}");
            }

            public double Average()
            {
                if (Grades.Count == 0)
                    return 0;

                int sum = 0;
                foreach (int grade in Grades)
                {
                    sum += grade;
                }

                double avg = (double)sum / Grades.Count;
                return avg;
            }

            public bool HasExcellent()
            {
                int countTwo = Grades.Count(grade => grade == 5);
                return countTwo >= 3;
            }
        }

        class Group
        {
            public string Name;
            public List<Student> Students = new List<Student>();
        }

        class Institute
        {
            public string Name;
            public List<Group> Groups = new List<Group>();
        }

        static List<Institute> institutes = new List<Institute>();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("МЕНЮ");
                Console.WriteLine("1. Добавить институт");
                Console.WriteLine("2. Добавить группу");
                Console.WriteLine("3. Добавить студента");
                Console.WriteLine("4. Показать всех студентов");
                Console.WriteLine("5. Сохранить в файл");
                Console.WriteLine("6. Загрузить из файла");
                Console.WriteLine("7. Найти курс с наибольшим количеством исключенных студентов");
                Console.WriteLine("8. Проверить работу интерфейсов");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                string choice = Console.ReadLine();
                Console.WriteLine('\n');
                if (choice == "1") AddInstitute();
                else if (choice == "2") AddGroup();
                else if (choice == "3") AddStudent();
                else if (choice == "4") ShowAllStudents();
                else if (choice == "5") SaveToFile();
                else if (choice == "6") LoadFromFile();
                else if (choice == "7") FindCourseWithMostExpelled();
                else if (choice == "8") DemonstrationInterfaces();
                else if (choice == "0") break;
            }
        }
        static void AddInstitute()
        {
            Console.Write("Введите название института: ");
            string name = Console.ReadLine();
            institutes.Add(new Institute { Name = name });
            Console.WriteLine("Институт добавлен");
        }

        static void AddGroup()
        {
            if (institutes.Count == 0)
            {
                Console.WriteLine("Нет институтов");
                return;
            }
            Console.WriteLine("Институты:");
            for (int i = 0; i < institutes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {institutes[i].Name}");
            }
            Console.Write("Выберите номер института: ");
            int instIndex = int.Parse(Console.ReadLine()) - 1;
            Console.Write("Введите название группы: ");
            string name = Console.ReadLine();
            institutes[instIndex].Groups.Add(new Group { Name = name });
            Console.WriteLine("Группа добавлена");
        }

        static void AddStudent()
        {
            if (institutes.Count == 0)
            {
                Console.WriteLine("Нет институтов");
                return;
            }
            Console.WriteLine("Институты:");
            for (int i = 0; i < institutes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {institutes[i].Name}");
            }
            Console.Write("Выберите номер института: ");
            int instIndex = int.Parse(Console.ReadLine()) - 1;
            if (institutes[instIndex].Groups.Count == 0)
            {
                Console.WriteLine("Нет групп");
                return;
            }
            Console.WriteLine("Группы:");
            for (int i = 0; i < institutes[instIndex].Groups.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {institutes[instIndex].Groups[i].Name}");
            }
            Console.Write("Выберите номер группы: ");
            int grpIndex = int.Parse(Console.ReadLine()) - 1;
            Console.Write("Введите фамилию студента: ");
            string lastName = Console.ReadLine();
            Console.Write("Введите оценки через запятую: ");
            string[] gradesInput = Console.ReadLine().Split(',');
            List<int> grades = new List<int>();
            foreach (string grade in gradesInput)
            {
                grades.Add(int.Parse(grade));
            }
            Console.Write("Введите курс: ");
            int course = int.Parse(Console.ReadLine());
            Student newStudent = new Student(lastName, grades,
                institutes[instIndex].Groups[grpIndex].Name, course, institutes[instIndex].Name);
            institutes[instIndex].Groups[grpIndex].Students.Add(newStudent);
            Console.WriteLine("Студент добавлен");
        }

        static void ShowAllStudents()
        {
            bool studentsFound = false;
            foreach (var institute in institutes)
            {
                foreach (var group in institute.Groups)
                {
                    if (group.Students.Count > 0)
                    {
                        studentsFound = true;
                        Console.WriteLine($"\nИнститут: {institute.Name}, Группа: {group.Name}");
                        foreach (Student student in group.Students)
                        {
                            student.ShowInfo();
                        }
                    }
                }
            }

            if (!studentsFound)
            {
                Console.WriteLine("Студентов нет");
            }
        }

        static void SaveToFile()
        {
            Console.Write("Директория файла: ");
            string filename = Console.ReadLine();
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (var institute in institutes)
                {
                    foreach (var group in institute.Groups)
                    {
                        foreach (Student student in group.Students)
                        {
                            writer.WriteLine($"{student.Name};{string.Join(",", student.Grades)};{student.Group};{student.Course};{student.Institute}");
                        }
                    }
                }
            }
            Console.WriteLine("Данные сохранены");
        }

        static void LoadFromFile()
        {
            Console.Write("Директория файла: ");
            string filename = Console.ReadLine();

            institutes.Clear();
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    string[] gradesInput = parts[1].Split(',');
                    List<int> grades = (from string grade in gradesInput
                                        select int.Parse(grade)).ToList();
                    Student student = new Student(parts[0], grades, parts[2], int.Parse(parts[3]), parts[4]);
                    bool instituteExists = false;

                    foreach (var institute in institutes)
                    {
                        if (institute.Name == student.Institute)
                        {
                            bool groupExists = false;
                            foreach (var group in institute.Groups)
                            {
                                if (group.Name == student.Group)
                                {
                                    group.Students.Add(student);
                                    groupExists = true;
                                    break;
                                }
                            }
                            if (!groupExists)
                            {
                                Group newGroup = new Group { Name = student.Group };
                                newGroup.Students.Add(student);
                                institute.Groups.Add(newGroup);
                            }
                            instituteExists = true;
                            break;
                        }
                    }

                    if (!instituteExists)
                    {
                        Institute newInstitute = new Institute { Name = student.Institute };
                        Group newGroup = new Group { Name = student.Group };
                        newGroup.Students.Add(student);
                        newInstitute.Groups.Add(newGroup);
                        institutes.Add(newInstitute);
                    }
                }
            }
            Console.WriteLine("Данные загружены");
        }

        static void FindCourseWithMostExpelled()
        {
            Dictionary<int, int> expelled = new Dictionary<int, int>();

            foreach (var institute in institutes)
            {
                foreach (var group in institute.Groups)
                {
                    foreach (Student student in group.Students)
                    {
                        if (student.HasTwo())
                        {
                            if (expelled.ContainsKey(student.Course))
                            {
                                expelled[student.Course]++;
                            }
                            else
                            {
                                expelled.Add(student.Course, 1);
                            }
                        }
                    }
                }
            }

            if (expelled.Count == 0)
            {
                Console.WriteLine("Нет студентов с двойками");
                return;
            }

            var maxCourse = expelled.OrderByDescending(x => x.Value).First();
            Console.WriteLine($"Курс {maxCourse.Key}: {maxCourse.Value} исключенных студентов");
        }

        static void DemonstrationInterfaces()
        {
            if (!institutes.Any() || !institutes.SelectMany(i => i.Groups).Any(g => g.Students.Any()))
            {
                Console.WriteLine("Нет студентов для проверки");
                return;
            }

            var student = institutes.First().Groups.First().Students.First();

            Console.WriteLine(" Проверка интерфейсов ");
            student.ShowInfo();
            Console.WriteLine($"Средний балл: {student.Average()}");
            Console.WriteLine(student.HasExcellent() ? "<Больше трех пятерок" : "Меньше трех пятерок");
        }
    }
}
