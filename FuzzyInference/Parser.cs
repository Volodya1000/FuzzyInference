using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FuzzyInference
{
    public static class Parser
    {
        static KnowledgeBase knowledgeBase;

       private static List<Predicate> predicates = new List<Predicate>();

        // Перечисление для состояний парсера
        private enum ParserState
        {
            ExpectOpenBracket,
            ParseVarName,
            ParseVarNumber,
            ParseFirstNumber,
            Drop,
            ParseNumber,
            ExpectComma,
            ExpectCloseBracket
        }

        public static (List<Predicate>, List<Rule>) ParseFile(string fileName)
        {
            List<Rule> rules = new List<Rule>();

            string[] lines = File.ReadAllLines(fileName);
            bool parsingRules = false;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    parsingRules = true;
                    continue;
                }
                if (!parsingRules)
                {
                    Predicate predicate = ParsePredicate(line);
                    predicates.Add(predicate);
                }
                else
                {
                    Rule rule = ParseRule(line);
                    if (rule!=null)
                        rules.Add(rule);
                }
            }

            return (predicates, rules);
        }

        private static Predicate ParsePredicate(string line)
        {
            List<float> elements = new List<float>();
            line = line.Replace(" ", "");
            int i = 0;
            int n = line.Length;

            void Error(string expected, char found)
            {
                throw new ArgumentException($"Ошибка: ожидалось {expected}, но найден '{found}' на позиции {i}");
            }

            string PredicateName = "";
            if (i < n && char.IsUpper(line[i]))
            {
                PredicateName += line[i];
                i++;

                // Читаем все последующие цифры в имени предиката
                while (i < n && char.IsDigit(line[i]))
                {
                    PredicateName += line[i];
                    i++;
                }

                if (predicates.Any(p => p.Name == PredicateName))
                {
                    throw new ArgumentException($"Ошибка: Предикат с именем {PredicateName} уже существует");
                }
            }
            else
            {
                Error("Имя предиката (одна заглавная буква и цифры)", i < n ? line[i] : '\0');
            }

            // Проверяем, что следующий символ '='
            if (i < n && line[i] == '=')
            {
                i++;
            }
            else
            {
                Error("символ '='", line[i]);
            }

            // Ожидаем начало элемента: '{'
            if (i < n && line[i] == '{')
            {
                i++;
            }
            else
            {
                Error("символ '{'", line[i]);
            }

            ParserState state = ParserState.ExpectOpenBracket; // Начальное состояние парсера

            char letter = '0';
            int previousNumber = 0;
            string varNumber = "";
            string varName = "";
            string numStr = "";

            while (i < n - 1)
            {
                char currentChar = line[i];
                switch (state)
                {
                    case ParserState.ExpectOpenBracket:
                        if (currentChar == '<')
                        {
                            state = ParserState.ParseVarName;
                        }
                        else
                        {
                            Error("символ '<'", currentChar);
                        }
                        break;

                    case ParserState.ParseVarName:
                        if (char.IsLower(currentChar))
                        {
                            if (letter == '0')
                            {
                                letter = currentChar;
                            }
                            else if (currentChar != letter)
                            {
                                Error($"латинская буква {letter}", currentChar);
                            }
                            varName = currentChar.ToString();
                            state = ParserState.ParseVarNumber;
                        }
                        else
                        {
                            Error("строчная латинская буква", currentChar);
                        }
                        break;

                    case ParserState.ParseVarNumber:
                        if (char.IsDigit(currentChar))
                        {
                            varNumber += currentChar;
                        }
                        else if (currentChar == ',')
                        {
                            if (varNumber == "")
                            {
                                throw new ArgumentException($"Ошибка: Имя переменной не содержит цифр на позиции {i}");
                            }
                            else if (int.Parse(varNumber) != previousNumber + 1)
                            {
                                throw new ArgumentException($"Ошибка: Номера переменных не идут по порядку на позиции {i}");
                            }
                            else
                            {
                                previousNumber = int.Parse(varNumber);
                                varName += varNumber;
                                varNumber = "";
                                state = ParserState.ParseFirstNumber;
                            }
                        }
                        else
                        {
                            Error("цифра или запятая", currentChar);
                        }
                        break;

                    case ParserState.ParseFirstNumber:
                        if (char.IsDigit(currentChar))
                        {
                            numStr += currentChar;
                            state = ParserState.Drop;
                        }
                        else
                        {
                            Error("цифра (0 или 1)", currentChar);
                        }
                        break;

                    case ParserState.Drop:
                        if (currentChar == '.')
                        {
                            numStr += currentChar;
                            state = ParserState.ParseNumber;
                        }
                        else
                        {
                            Error("символ '.'", currentChar);
                        }
                        break;

                    case ParserState.ParseNumber:
                        if (char.IsDigit(currentChar))
                        {
                            numStr += currentChar;
                        }
                        else if (currentChar == '>')
                        {
                            if (numStr != "")
                            {
                                try
                                {
                                    float degree = float.Parse(numStr, CultureInfo.InvariantCulture);
                                    elements.Add(degree);
                                    varName = "";
                                    numStr = "";
                                    state = ParserState.ExpectComma;
                                }
                                catch (FormatException)
                                {
                                    throw new ArgumentException($"Ошибка: невозможно преобразовать '{numStr}' в число на позиции {i}");
                                }
                            }
                            else
                            {
                                Error("числовое значение", currentChar);
                            }
                        }
                        break;

                    case ParserState.ExpectCloseBracket:
                        if (currentChar == '>')
                        {
                            state = ParserState.ExpectComma;
                        }
                        else
                        {
                            Error("символ '>'", currentChar);
                        }
                        break;

                    case ParserState.ExpectComma:
                        if (currentChar == ',')
                        {
                            state = ParserState.ExpectOpenBracket;
                        }
                        else
                        {
                            Error("символ ','", currentChar);
                        }
                        break;
                }
                i++;
            }

            // Проверяем, что строка заканчивается на '}'
            if (line[i] != '}')
            {
                Error("символ '}'", line[i]);
            }

            return new Predicate(PredicateName, letter, elements.ToArray()); // Возвращаем элементы для использования, если это необходимо
        }


        private static Rule ParseRule(string line)
        {
            line = line.Replace(" ", "");
            int i = 0, n = line.Length;
            Predicate Antecedent, Consequent;

            Predicate ParsePredicate(ref int index)
            {
                string predicateName = "";

                if (index < n && char.IsUpper(line[index]))
                {
                    predicateName += line[index++];
                    while (index < n && char.IsDigit(line[index]))
                    {
                        predicateName += line[index++];
                    }

                    Predicate result = predicates.Find(p => p.Name == predicateName);
                    if (result == null)
                        throw new ArgumentException($"Ошибка: Предикат с именем {predicateName} не описан");

                    // Проверка структуры (VariableName)
                    Expect('(', ref index);
                    Expect(result.VariableName, ref index);
                    Expect(')', ref index);

                    return result;
                }
                throw new ArgumentException("Ошибка: ожидалось имя предиката");
            }

            void Expect(char expected, ref int index)
            {
                if (index >= n || line[index] != expected)
                    throw new ArgumentException($"Ошибка: ожидался символ '{expected}'");
                index++;
            }

            // Парсинг антецедента
            Antecedent = ParsePredicate(ref i);
            Expect('~', ref i);
            Expect('>', ref i);

            // Парсинг консеквента
            Consequent = ParsePredicate(ref i);

            if (Antecedent.Cardinality == Consequent.Cardinality) return null;
            return new Rule(Antecedent, Consequent);
        }

    }
}
