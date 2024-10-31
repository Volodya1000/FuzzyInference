/////////////////////////////////
//Индивидуальная практическая работа 1 по дисциплине ЛОИС
//Выполнена студентом группы 221701 БГУИР Дичковским Владимиром Андреевичем
//класс KnowlegeBase содержит список начальных предикатов, список правил и список выведенных предикатов.
//В этом классе есть метод, который возвращает строку для записи в файл и строку для вывода в консоль выведенных предикатов
//10.10.2024
//Использованные материалы:
//Голенков, В. В. Логические основы интеллектуальных систем.
//Практикум: учебное методическое пособие БГУИР, 2011.
namespace FuzzyInference
{
    public class KnowledgeBase
    {
        public List<Predicate> InitialPredicatesList { get; }
        public List<Predicate> InferredPredicatesList { get; }
        public List<Rule> RulesList { get; }
        public KnowledgeBase(List<Predicate>  initialPredicatesList, List<Rule> rulesList)
        {
            InitialPredicatesList = initialPredicatesList;
            InferredPredicatesList = new List<Predicate>();
            RulesList = rulesList;
        }
       
        public void AddInferredPredicate(Predicate predicate)
        {
            InferredPredicatesList.Add(predicate);
        }
       
        public string GetInferedPredicstesString()
        {  
            var inferredPredicatesStr = string.Join("\n", InferredPredicatesList.Select(p => p.ToString()));
            return $"Inferred Predicates:\n{inferredPredicatesStr}";
        }

        public override string ToString()
        {
            var initialPredicatesStr = string.Join("\n", InitialPredicatesList.Select(p => p.ToString()));
            var rulesStr = string.Join("\n", RulesList.Select(r => r.ToString()));
            return $"Initial Predicates:\n{initialPredicatesStr}\n\nRules:\n{rulesStr}\n\n{GetInferedPredicstesString()}";
        }
    }

}
