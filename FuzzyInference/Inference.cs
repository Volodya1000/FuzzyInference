using System.Runtime.CompilerServices;

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
;           foreach (Rule rule in knowledgeBase.RulesList)
            {
                //Находим все предикаты с мощностью равной мощности посылки
                foreach (Predicate predicate in knowledgeBase.InitialPredicatesList)
                {
                    if (predicate.Cardinality == rule.Antecedent.Cardinality)
                    {
                        float[,] DractricMatrix = new float[rule.Antecedent.Cardinality, rule.Consequent.Cardinality];
                        for (int row = 0; row < rule.Antecedent.Cardinality; row++)
                            for (int column = 0; column < rule.Antecedent.Cardinality; column++)
                                DractricMatrix[row, column] = DrasticMultiplication(predicate.ValuesArray[row], rule.ImplicationMatrix[row, column]);
                        float[] NewPredicateValues = GetMaxInColumns(DractricMatrix);
                        string NewPredicateName = "I"+knowledgeBase.InferredPredicatesList.Count.ToString();
                        InferredPredicate NewPredicate=new(NewPredicateName, predicate.VariableName, NewPredicateValues, rule, predicate);
                        if (knowledgeBase.InferredPredicatesList.All(p => !p.AreEqual(NewPredicate)))
                        {
                            PredicatesInferedOnThisStep.Add(NewPredicate);
                            knowledgeBase.AddInferredPredicate(NewPredicate);
                        }
                    }
                }
            }
            if (PredicatesInferedOnThisStep.Count != 0&& knowledgeBase.InferredPredicatesList.Count<100)
                ProcessInference(PredicatesInferedOnThisStep);
        }
    }
}
