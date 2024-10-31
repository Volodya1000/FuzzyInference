///////////////////////////////////////////////
//Индивидуальная практическая работа 1 по дисциплине ЛОИС
//Выполнена студентом группы 221701 БГУИР Дичковским Владимиром Андреевичем
//Класс Inference содержит алгоритм для прямого нечеткого логического вывода
//10.10.2024
//Использованные материалы:
//Голенков, В. В. Логические основы интеллектуальных систем.
//Практикум: учебное методическое пособие БГУИР, 2011.

namespace FuzzyInference
{
    public static class Inference
    {
        public static KnowledgeBase knowledgeBase;

        private static float[] GetMaxInColumns(float[,] dractricMatrix)
        {
            int numRows = dractricMatrix.GetLength(0);  
            int numColumns = dractricMatrix.GetLength(1);  
            float[] maxValues = new float[numColumns];

            for (int column = 0; column < numColumns; column++)
            {
                float max = dractricMatrix[0, column]; 

                for (int row = 1; row < numRows; row++)
                    if (dractricMatrix[row, column] > max)
                        max = dractricMatrix[row, column]; 
                maxValues[column] = max;  
            }

            return maxValues;
        }


        public static void ProcessInference(List<Predicate>InputPredicates)
        {
            float DrasticMultiplication(float x1, float x2)
            {
                if (x1 == 1) return x2;
                else if(x2 == 1) return x1;
                else return 0;
            }
            List<Predicate> PredicatesInferedOnThisStep = new();
            foreach (Rule rule in knowledgeBase.RulesList)
            {
                if (rule.Antecedent.Cardinality == rule.Consequent.Cardinality)
                    continue;
                    //Находим все предикаты с мощностью равной мощности посылки
                 foreach (Predicate predicate in InputPredicates)
                 {
                    if (knowledgeBase.InferredPredicatesList.Count >= 50)
                        return;
                    if (predicate.Cardinality == rule.Antecedent.Cardinality&& predicate.VariableName== rule.Antecedent.VariableName)
                    {
                        float[,] DractricMatrix = new float[rule.Antecedent.Cardinality, rule.Consequent.Cardinality];
                        for (int row = 0; row < DractricMatrix.GetLength(0); row++)
                            for (int column = 0; column < DractricMatrix.GetLength(1); column++)
                                DractricMatrix[row, column] = DrasticMultiplication(predicate.ValuesArray[row], rule.ImplicationMatrix[row, column]);
                        float[] NewPredicateValues = GetMaxInColumns(DractricMatrix);
                        var uniquePredicates = knowledgeBase.InferredPredicatesList
    .Distinct(new PredicateComparer()) // Используем компаратор
    .ToList(); // Конвертируем в список для получения количества

                        //// Теперь используем количество уникальных предикатов для создания нового имени
                        string NewPredicateName = "I" + uniquePredicates.Count.ToString();

                        //string NewPredicateName = "I"+knowledgeBase.InferredPredicatesList.Count.ToString();
                        InferredPredicate NewPredicate=new(NewPredicateName, rule.Consequent.VariableName, NewPredicateValues, rule, predicate);
                        var existingPredicate = knowledgeBase.InferredPredicatesList
                         .FirstOrDefault(p => p.AreEqual(NewPredicate));
                        if (existingPredicate != null)
                        {
                            NewPredicateName = existingPredicate.Name;
                            NewPredicate = new(NewPredicateName, rule.Consequent.VariableName, NewPredicateValues, rule, predicate);
                            knowledgeBase.AddInferredPredicate(NewPredicate);
                        }
                        else
                        {
                            PredicatesInferedOnThisStep.Add(NewPredicate);
                            knowledgeBase.AddInferredPredicate(NewPredicate);
                        }
                       
                    }
                }
            }
            if (PredicatesInferedOnThisStep.Count != 0)
                ProcessInference(PredicatesInferedOnThisStep);
        }
    }
}
