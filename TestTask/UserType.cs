using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public class UserType
    {
        public Int32 FirstProperty { get; set; }
        public Int32 SecondProperty { get; set; }

        // ----------------------------------------------------
        // Конструктор

        public UserType(Int32 firstProperty, Int32 secondProperty)
        {
            FirstProperty = firstProperty;
            SecondProperty = secondProperty;
        }

        // ----------------------------------------------------
        // Получение хеш-кода объекта

        public override Int32 GetHashCode()
        {
            return FirstProperty ^ SecondProperty;
        }

        // ----------------------------------------------------
        // Сравнение объекта - используем сравнение по значению

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;

            UserType ut = (UserType)obj;
            return FirstProperty == ut.FirstProperty && SecondProperty == ut.SecondProperty;
        }

        // ----------------------------------------------------
        // Строковое представление объекта

        public override string ToString()
        {
            return String.Format("UserType ({0}, {1})", FirstProperty, SecondProperty);
        }
    }
}
