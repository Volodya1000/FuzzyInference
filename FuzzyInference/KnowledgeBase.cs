namespace FuzzyInference
{
    public class KnowledgeBase
    {
        // Коллекции для хранения исходных предикатов и выведенных предикатов
        public List<Predicate> InitialPredicatesList { get; }
        public List<Predicate> InferredPredicatesList { get; }
        public List<Rule> RulesList { get; }

        // Конструктор инициализирует списки
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
        

       
        public override string ToString()
        {  
            var inferredPredicatesStr = string.Join("\n", InferredPredicatesList.Select(p => p.ToString()));
            return $"Inferred Predicates:\n{inferredPredicatesStr}";
        }
    }

}
