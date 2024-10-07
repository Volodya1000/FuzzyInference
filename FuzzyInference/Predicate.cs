
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
            return $"{Name} = {{{string.Join(", ", ValuesArray.Select((v, i) => $"<{VariableName}{i + 1}, {v:0.0}>"))}}}";
        }
    }
}
