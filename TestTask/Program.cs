using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // ----------------------------------------------------
            // Коллекция с простыми типами

            ComplexCollection<Int32, String, Int32> col = new ComplexCollection<Int32, String, Int32>();

            col.Add(5, "Имя 1", 2);
            col.Add(5, "Имя 2", 3);
            col.Add(6, "Имя 3", 1);
            col.Add(4, "Имя 1", 5);
            col.Add(6, "Имя 1", 5);
            col.Add(5, "Имя 4", 5);
            col.Remove(4, "Имя 1");

            Console.WriteLine();
            Console.WriteLine("Исходная коллекция с простыми типами (число элементов " + col.Count + "):");
            Console.WriteLine();
            Console.WriteLine(col.ToString());

            var valueByKey = col.GetValue(6,"Имя 3");
            Console.WriteLine("Значение по составному ключу (6, Имя 3) = " + valueByKey);

            var idToSearch = 5;
            Console.WriteLine();
            Console.WriteLine("Результаты поиска по id = " + idToSearch + ":");
            Console.WriteLine();
            IList<Int32> valuesById = col.GetValuesById(idToSearch);
            foreach (var value in valuesById) 
            {
                Console.WriteLine("[" + idToSearch.ToString() + ", ..] " + value.ToString());
            }

            var nameToSearch = "Имя 1";
            Console.WriteLine();
            Console.WriteLine("Результаты поиска по name = " + nameToSearch + ":");
            Console.WriteLine();
            IList<Int32> valuesByName = col.GetValuesByName(nameToSearch);
            foreach (var value in valuesByName)
            {
                Console.WriteLine("[.. , " + nameToSearch.ToString() + "] " + value.ToString());
            }

            // ----------------------------------------------------
            // Коллекция с использованием UserType

            ComplexCollection<UserType, String, Int32> col2 = new ComplexCollection<UserType, String, Int32>();

            col2.Add(new UserType(2, 3), "Имя 10", 23);
            col2.Add(new UserType(2, 3), "Имя 12", 25);
            col2.Add(new UserType(4, 5), "Имя 12", 34);
            col2.Add(new UserType(5, 3), "Имя 18", 35);
            col2.Add(new UserType(9, 9), "Имя 12", 35);
            col2.Add(new UserType(4, 5), "Имя 14", 36);
            col2.Add(new UserType(2, 3), "Имя 18", 37);
            col2.Remove(new UserType(9, 9),"Имя 12");

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Исходная коллекция с UserType (число элементов " + col2.Count + "):");
            Console.WriteLine();
            Console.WriteLine(col2.ToString());

            var valueByKey2 = col2.GetValue(new UserType(2, 3), "Имя 12");
            Console.WriteLine("Значение по составному ключу (UserType(2, 3), Имя 12) = " + valueByKey2);

            var idToSearch2 = new UserType(2, 3);
            Console.WriteLine();
            Console.WriteLine("Результаты поиска по id = " + idToSearch2 + ":");
            Console.WriteLine();
            IList<Int32> valuesById2 = col2.GetValuesById(idToSearch2);
            foreach (var value2 in valuesById2)
            {
                Console.WriteLine("[" + idToSearch2.ToString() + ", ..] " + value2.ToString());
            }

            var nameToSearch2 = "Имя 12";
            Console.WriteLine();
            Console.WriteLine("Результаты поиска по name = " + nameToSearch2 + ":");
            Console.WriteLine();
            IList<Int32> valuesByName2 = col2.GetValuesByName(nameToSearch2);
            foreach (var value2 in valuesByName2)
            {
                Console.WriteLine("[.. , " + nameToSearch2.ToString() + "] " + value2.ToString());
            }

            // ----------------------------------------------------
            // Коллекция с использованием UserType

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода..");
            Console.ReadKey();
        }
    }
}
