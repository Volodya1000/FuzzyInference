///////////////////////////////////////////////
//Индивидуальная практическая работа 1 по дисциплине ЛОИС
//Выполнена студентом группы 221701 БГУИР Дичковским Владимиром Андреевичем
//Класс InferredPredicate содержит содержит класс для хранения выведенного предиката.
//Данный касс унаследован от Predicate.
//В нём дополнительно хранится правило по которому он был выведен и предикат который использовался для вывода.
//10.10.2024
//Использованные материалы:
//Голенков, В. В. Логические основы интеллектуальных систем.
//Практикум: учебное методическое пособие БГУИР, 2011.

namespace FuzzyInference
{
    public class InferredPredicate : Predicate
    {
        public Rule InferenceRule { get; }// Правило, из которого был выведен предикат
        public Predicate SourcePredicate { get; }  // Исходный предикат, который использовался для вывода

        public InferredPredicate(string name, char variableName, float[] valuesArray, Rule inferenceRule, Predicate sourcePredicate)
            : base(name, variableName, valuesArray)
        {
            InferenceRule = inferenceRule;
            SourcePredicate = sourcePredicate;
        }
        public override string ToString()
        {
            return $"{SourcePredicate.Name}/~\\({InferenceRule.ToString()})= {base.ToString()}";
        }
    }
}
