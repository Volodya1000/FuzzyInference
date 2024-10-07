namespace FuzzyInference
{
    public class InferredPredicate : Predicate
    {
        public Rule InferenceRule { get; }// Правило, из которого был выведен предикат
        public Predicate SourcePredicate { get; }  // Исходный предикат, который использовался для вывода

        private ParserState ParserState
        {
            get => default(Parser.ParserState);
            set
            {
            }
        }

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

/*
 План работы программы
Получить через консоль имя файла базы знаний(расширение файла .kb)
Парсер читает этот текстовый файл и на его основе наполняет базу знаний класс KnowlegeBase
К сформированной базе знаний применяется Вывод из отдельного класса Inference. Резульат работы вывода просто изменяет KnowlegeBase
Происходит вывод в консоль результата с помощью KnowlegeBase.GetStringReport()
Отчёт сохраняется в новый файл с именем исходного файла с добавлением слова inference
 */
