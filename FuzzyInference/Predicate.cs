///////////////////////////////////////////////
//Индивидуальная практическая работа 1 по дисциплине ЛОИС
//Выполнена студентом группы 221701 БГУИР Дичковским Владимиром Андреевичем
//Класс Predicate содержит предикат
//10.10.2024
//Использованные материалы:
//Голенков, В. В. Логические основы интеллектуальных систем.
//Практикум: учебное методическое пособие БГУИР, 2011.

using System.Globalization;
using System.Text;

namespace FuzzyInference
{
    public class Predicate 
    {
        public Predicate(string name, char variableName, float[] valuesArray)
        {
            Name = name;
            VariableName = variableName;
            ValuesArray = valuesArray;
        }

        public string Name { get; }
        public char VariableName { get; }
        public float[] ValuesArray { get; }


        public int Cardinality => ValuesArray.Length;

        public bool AreEqual(Predicate other)
        {
            if (VariableName != other.VariableName)
                return false;

            if (ValuesArray.Length != other.ValuesArray.Length)
                return false;

            for (int i = 0; i < ValuesArray.Length; i++)
            {
                if (ValuesArray[i] != other.ValuesArray[i])
                    return false;
            }

            return true;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Name} = {{");

            for (int i = 0; i < ValuesArray.Length; i++)
            {
                sb.Append($"<{VariableName}{i + 1}, {ValuesArray[i].ToString("F5", CultureInfo.InvariantCulture)}>");
                if (i < ValuesArray.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            int hash = VariableName.GetHashCode();
            foreach (var value in ValuesArray)
            {
                hash = hash * 31 + value.GetHashCode(); 
            }
            return hash;
        }
    }

    public class PredicateComparer : IEqualityComparer<Predicate>
    {
        public bool Equals(Predicate x, Predicate y)
        {
            if (x == null || y == null)
                return false;

            return x.AreEqual(y);
        }

        public int GetHashCode(Predicate obj)
        {
            return obj.GetHashCode();
        }
    }

}


