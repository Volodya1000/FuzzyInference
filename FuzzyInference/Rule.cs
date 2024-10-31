///////////////////////////////////////////////
//Индивидуальная практическая работа 1 по дисциплине ЛОИС
//Выполнена студентом группы 221701 БГУИР Дичковским Владимиром Андреевичем
//Класс Rule хранит правило
//10.10.2024
//Использованные материалы:
//Голенков, В. В. Логические основы интеллектуальных систем.
//Практикум: учебное методическое пособие БГУИР, 2011.

namespace FuzzyInference
{
    public class Rule
    {
        public readonly Predicate Antecedent;  // Поля только для чтения
        public readonly Predicate Consequent;

        private readonly float[,] _implicationMatrix;
        public float[,] ImplicationMatrix => _implicationMatrix; // Публичное свойство только для чтения

        public Rule(Predicate antecedent, Predicate consequent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
            _implicationMatrix = GenerateImplicationMatrix(); // Устанавливаем значение один раз
        }

        private float[,] GenerateImplicationMatrix()
        {
            float[,] implicationMatrix = new float[Antecedent.Cardinality, Consequent.Cardinality];

            for (int row = 0; row < Antecedent.Cardinality; row++)
            {
                for (int column = 0; column < Consequent.Cardinality; column++)
                {
                    implicationMatrix[row, column] = (Antecedent.ValuesArray[row] == 1)
                        ? Consequent.ValuesArray[column]
                        : 1;
                }
            }

            return implicationMatrix;
        }

        public override string ToString()
        {
            return $"{Antecedent.Name}({Antecedent.VariableName})~>{Consequent.Name}({Consequent.VariableName})";
        }
    }

}
