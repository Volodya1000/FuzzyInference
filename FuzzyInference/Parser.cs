///////////////////////////////////////////////
//Индивидуальная практическая работа 1 по дисциплине ЛОИС
//Выполнена студентом группы 221701 БГУИР Дичковским Владимиром Андреевичем
//содержит парсер исходного файла.
//В парсере есть функция, которая последовательно обрабатывает каждую строку.
//В нём есть функция для анализа предиката и анализ правила
//10.10.2024
//Использованные материалы:
//Голенков, В. В. Логические основы интеллектуальных систем.
//Практикум: учебное методическое пособие БГУИР, 2011.

using System.Globalization;

namespace FuzzyInference
{
    public static class Parser
    {
        private static List<Predicate> predicates;
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
            predicates = new List<Predicate>();

            string[] lines = File.ReadAllLines(fileName);
            bool parsingRules = false;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    parsingRules = true;
                else if (!parsingRules)
                    predicates.Add(ParsePredicate(line));
                else
                {
                    Rule rule = ParseRule(line);
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

            string PredicateName = "";
            if (i < n && char.IsUpper(line[i]))
            {
                PredicateName += line[i];
                i++;

                // Читаем все последующие цифры в имени предиката
                while (i < n && char.IsDigit(line[i]))
                    PredicateName += line[i++];

                if (predicates.Any(p => p.Name == PredicateName))
                    throw new ArgumentException($"Ошибка: Предикат с именем {PredicateName} уже существует");
            }
            else
                ThrowParsingError(nameof(ParsePredicate), line, i, "Имя предиката (одна заглавная буква и цифры)", line[i]);

            // Используем метод Expect для проверки символа '='
            Expect('=', ref i, line, nameof(ParsePredicate));

            // Используем метод Expect для проверки символа '{'
            Expect('{', ref i, line, nameof(ParsePredicate));

            ParserState state = ParserState.ExpectOpenBracket;
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
                        Expect('<',ref i, line, nameof(ParsePredicate));
                        i--;
                        state = ParserState.ParseVarName;
                        break;

                    case ParserState.ParseVarName:
                        if (char.IsLower(currentChar))
                        {
                            if (letter == '0')
                                letter = currentChar;
                            else if (currentChar != letter)
                                ThrowParsingError(nameof(ParsePredicate), line, i, $"латинская буква {letter}", currentChar);
                            varName = currentChar.ToString();
                            state = ParserState.ParseVarNumber;
                        }
                        else
                            ThrowParsingError(nameof(ParsePredicate), line, i, "строчная латинская буква", currentChar);
                        break;

                    case ParserState.ParseVarNumber:
                        if (char.IsDigit(currentChar))
                            varNumber += currentChar;
                        else if (currentChar == ',')
                        {
                            if (varNumber == "")
                                throw new ArgumentException($"Ошибка: Имя переменной не содержит цифр на позиции {i}");
                            else if (int.Parse(varNumber) != previousNumber + 1)
                                throw new ArgumentException($"Ошибка: Номера переменных не идут по порядку на позиции {i}");
                            else
                            {
                                previousNumber = int.Parse(varNumber);
                                varName += varNumber;
                                varNumber = "";
                                state = ParserState.ParseFirstNumber;
                            }
                        }
                        else
                            ThrowParsingError(nameof(ParsePredicate), line, i, "цифра или запятая", currentChar);
                        break;

                    case ParserState.ParseFirstNumber:
                        if (char.IsDigit(currentChar))
                        {
                            numStr += currentChar;
                            state = ParserState.Drop;
                        }
                        else
                            ThrowParsingError(nameof(ParsePredicate), line, i, "цифра (0 или 1)", currentChar);
                        break;

                    case ParserState.Drop:
                        Expect('.', ref i, line, nameof(ParsePredicate));
                        numStr += ".";
                        i--;
                        state = ParserState.ParseNumber;
                        break;

                    case ParserState.ParseNumber:
                        if (char.IsDigit(currentChar))
                            numStr += currentChar;
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
                                    ThrowParsingError(nameof(ParsePredicate), line, i, "число", currentChar);
                                }
                            }
                            else
                                ThrowParsingError(nameof(ParsePredicate), line, i, "числовое значение", currentChar);
                        }
                        break;

                    case ParserState.ExpectComma:
                        Expect(',', ref i, line, nameof(ParsePredicate));
                        i--;
                        state = ParserState.ExpectOpenBracket;
                        break;
                }
                i++;
            }

            Expect('}', ref i, line, nameof(ParsePredicate));

            return new Predicate(PredicateName, letter, elements.ToArray());
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
                        predicateName += line[index++];
                    Predicate result = predicates.Find(p => p.Name == predicateName);
                    if (result == null)
                        throw new ArgumentException($"Предикат с именем {predicateName} не описан");
                    Expect('(', ref index, line, nameof(ParseRule));
                    Expect(result.VariableName, ref index, line, nameof(ParseRule));
                    Expect(')', ref index, line, nameof(ParseRule));

                    return result;
                }
                throw new ArgumentException("Ошибка: ожидалось имя предиката");
            }
            
            Antecedent = ParsePredicate(ref i);
            Expect('~', ref i, line, nameof(ParseRule));
            Expect('>', ref i, line, nameof(ParseRule));
            Consequent = ParsePredicate(ref i);
            return new Rule(Antecedent, Consequent);
        }

      
        private static void Expect(char expected, ref int index, string line, string methodName)
        {
            if (index >= line.Length || line[index] != expected)
                ThrowParsingError(methodName, line, index, $"символ '{expected}'", line[index]);
            index++;
        }
        private static void ThrowParsingError(string methodName, string line, int index, string expected, char found)
        {
            throw new ArgumentException($"Ошибка в {methodName}: ожидалось {expected}, но найден '{found}' на позиции {index} в строке '{line}'");
        }
    }
}
