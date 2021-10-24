using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Laba3

{
    class Student : IEquatable<Student>, IComparable<Student> {
        public string Name { set; get; } // Имя студента
        public string Surname { set; get; } // Фамилия студента
        public string Patronymic { set; get; } // Отчество
        public string Sex { set; get; } // Пол
        public DateTime DateOfBirth { set; get; } // Дата рождения
        public string Group { set; get; } // Группа
        public string Flow { set; get; } //поток
        public int Course { set; get; } // Курс
        public Dictionary<String, int> Points { set; get; } // Баллы по предметам егэ
        public int sumPoints { get; }

        private string[] maleNames = new string[8] { "Пётр", "Сергей", "Никита", "Александр", "Павел", "Георгий", "Виктор", "Карен" };
        private string[] femaleNames = new string[4] {"Анастасия", "Елена", "Валентина", "Владислава" };
        private string[] surnames = new string[] { "Ткаченко", "Примерко", "Субботченко", "Пониделко", "Солнечно" };
        private string[] malePatronymics = new string[8] { "Петрович", "Сергеевич", "Никитич", "Александрович", "Павлович", "Георгиевич", "Викторович", "Каренович" };
        private string[] femalePatronymics = new string[8] { "Петровна", "Сергеевна", "Никитична", "Александровна", "Павловна", "Георгиевна", "Викторовна", "Кареновна" };
        private string[] groupStarts = new string[3] {"БИСТ", "БПИ", "БИВТ"};
       
        // Конструктор случайного студента
        public Student(int startYear)
        {
            Random rand = new Random();
            this.Surname = surnames[rand.Next(0, surnames.Length - 1)];
            if (rand.Next(1, 2) == 1)
            {
                this.Name = maleNames[rand.Next(0, maleNames.Length - 1)];
                this.Patronymic = malePatronymics[rand.Next(0, malePatronymics.Length - 1)];
                this.Sex = "Мужской пол";
            }
            else
            {
                this.Name = femaleNames[rand.Next(0, femaleNames.Length - 1)];
                this.Patronymic = femalePatronymics[rand.Next(0, femalePatronymics.Length - 1)];
                this.Sex = "Женский пол";
            }

            DateTime DateOfBirth = new DateTime(startYear - 18, 1, 1);
            DateOfBirth = DateOfBirth.AddDays(-rand.Next(1095));

            this.DateOfBirth = DateOfBirth;
            this.Flow = groupStarts[rand.Next(3)];
            this.Group = this.Flow + "-" + startYear.ToString().Substring(2);
            this.Course = 1;

            int mathPoints = rand.Next(100);
            int informaticsPoints = rand.Next(100);
            int russianPoints = rand.Next(100);

            this.Points = new Dictionary<string, int>();
            this.Points.Add("math", mathPoints);
            this.Points.Add("informatics", informaticsPoints);
            this.Points.Add("russian", russianPoints);

            this.sumPoints = mathPoints + informaticsPoints + russianPoints;
        }

        public override string ToString() {
            return Surname + " " + Name + " " + Patronymic + "\n" + Sex + "\n" + DateOfBirth + "\n" + Group + "\n" + Course + " курс";
        }

        public bool Equals(Student otherStudent) {
            if (otherStudent == null) return false;
            return (this.Name.Equals(otherStudent.Name) && this.Surname.Equals(otherStudent.Surname) && this.Patronymic.Equals(otherStudent.Patronymic)
                && this.Sex.Equals(otherStudent.Sex) && this.DateOfBirth.Equals(otherStudent.DateOfBirth) && this.Course.Equals(otherStudent.Course)
                && this.Group.Equals(otherStudent.Group) && this.sumPoints.Equals(otherStudent.sumPoints));

        }

        // Метод сравнения по умолчанию - сравнивает студентов по сумме баллов

        public int CompareTo(Student compareStudent)
        {
            if (compareStudent == null)
                return 1;
            else
                return sumPoints.CompareTo(compareStudent.sumPoints);
        }
 }

    class Program
    {
        //Метод проводит обработку списка студентов за один год
        static void MakeYear(List<Student> students, int n, int year)
        {
            //Перевод на след. курс или отчисление
            for(int i = students.Count()-1; i > -1; i--){
                if (students[i].Course < 4)
                    students[i].Course++;
                else{
                    students.RemoveAt(i);
                }
            }

            for(int i = 0; i < n; i++)
            {
                students.Add(new Student(year));
            }
        }

        //Метод позволяет получить словарь типа Фамилия-Студенты с такой фамилией по списку студентов
        static Dictionary<string, List<Student>> GetDictSurnameStudents (List<Student> students)
        {
            Dictionary<string, List<Student>> dictSurnameStudents = new Dictionary<string, List<Student>>();
            for (int i = 0; i < students.Count(); i++){
                if (dictSurnameStudents.ContainsKey(students[i].Surname))
                    dictSurnameStudents[students[i].Surname].Add(students[i]);
                else
                    dictSurnameStudents.Add(students[i].Surname, new List<Student>());
            }
            return dictSurnameStudents;
        }

        //Метод позволяет получить словарь типа Поток-Студенты с такой фамилией по списку студентов
        static Dictionary<string, List<Student>> GetDictFlowStudents(List<Student> students)
        {
            Dictionary<string, List<Student>> dictFlowStudents = new Dictionary<string, List<Student>>();
            for (int i = 0; i < students.Count(); i++)
            {
                if (dictFlowStudents.ContainsKey(students[i].Flow))
                    dictFlowStudents[students[i].Flow].Add(students[i]);
                else
                    dictFlowStudents.Add(students[i].Flow, new List<Student>());
            }
            return dictFlowStudents;
        }

        //Метод позволяет получить список студентов с 
        static List<Student> GetNamesakesBPI(List<Student> students)
        {
            //Найти студентов потока «БПИ», для которых в потоках «БИВТ» или «БИСТ» найдутся студенты с такой же фамилией.
            List<Student> partList = new List<Student>();

            Dictionary<string, List<Student>> dictSurnameStudents = GetDictSurnameStudents(students);
            foreach (KeyValuePair<string, List<Student>> entry in dictSurnameStudents)
            {
                Dictionary<string, List<Student>> dictFlowStudents = GetDictFlowStudents(entry.Value);
                if (dictFlowStudents.ContainsKey("БПИ") && (dictFlowStudents.ContainsKey("БИВТ") || dictFlowStudents.ContainsKey("БИСТ")))
                    partList.AddRange(dictFlowStudents["БПИ"]);
            }
            return partList;
        }

        static void MakeTaskLaba3(){
            List<Student> students = new List<Student>();
            int year = 2020;
            int n = 15;
            //Проматываем 5 лет
            for (int i = 0; i < 5; i++)
            {
                MakeYear(students, n, year);
                year++;
            }

            Console.WriteLine("-----------------------------------------------------------------\n" +
                "Пролетело 5 лет... Список студентов:\n");

            for (int i = 0; i < students.Count(); i++)
                Console.WriteLine(students[i] + "\n");

            List<Student> namesakesBPI = GetNamesakesBPI(students);

            Console.WriteLine("-----------------------------------------------------------------\n" +
                "Список студентов потока БПИ с однофамильцами на других потоках:\n");
            for (int i = 0; i < namesakesBPI.Count(); i++)
                 Console.WriteLine(namesakesBPI[i] + "\n");
        }

        static void Main(string[] args)
        {
            MakeTaskLaba3();
        }
    }
}
