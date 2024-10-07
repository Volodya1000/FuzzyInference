namespace FuzzyInference
{
    class Program
    {
        static void Main()
        {
            bool repeat = true;

            while (repeat)
            {
                string kbFileName = GetFileNameFromConsole();
                var (predicates, rules) = Parser.ParseFile(kbFileName);
                KnowledgeBase knowledgeBase = new KnowledgeBase(predicates, rules);

                Inference.knowledgeBase = knowledgeBase;
                Inference.ProcessInference(knowledgeBase.InitialPredicatesList);

                // Вывод результата в консоль
                Console.WriteLine(knowledgeBase);

                // Сохранение результата
                string fullResultFilePath = GetResultFilePath(kbFileName);
                SaveToFile(fullResultFilePath, knowledgeBase.ToString());

                // Запрос на повторение процесса
                repeat = AskToRepeat();
            }

            Console.WriteLine("Программа завершена.");
        }

        // Метод для получения корректного имени файла через консоль
        static string GetFileNameFromConsole()
        {
            string fullPath;
            string fileName;

            while (true)
            {
                Console.WriteLine("Введите имя файла базы знаний (с расширением .kb):");
                fileName = Console.ReadLine();
                fullPath = Path.Combine(GetInputDirectory(), fileName);

                if (IsValidFileName(fileName, fullPath))
                    break;
            }
            return fullPath;
        }

        // Метод для проверки валидности имени файла
        static bool IsValidFileName(string fileName, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Имя файла не может быть пустым.");
                return false;
            }
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("Файл не найден. Убедитесь, что файл существует.");
                return false;
            }
            if (Path.GetExtension(fileName).ToLower() != ".kb")
            {
                Console.WriteLine("Неверное расширение файла. Требуется .kb.");
                return false;
            }
            return true;
        }

        // Метод для получения директории ввода
        static string GetInputDirectory() =>
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "Input"));

        // Метод для получения пути для сохранения результата
        static string GetResultFilePath(string kbFileName)
        {
            string resultFileName = Path.GetFileNameWithoutExtension(kbFileName) + "_inference.kb";
            return Path.Combine(GetOutputDirectory(), resultFileName);
        }

        // Метод для получения директории вывода
        static string GetOutputDirectory() =>
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "Output"));

        // Метод для сохранения содержимого в файл
        static void SaveToFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
            Console.WriteLine($"Отчёт сохранён в файл: {Path.GetFileName(filePath)}");
        }

        // Метод для запроса повторного выполнения программы
        static bool AskToRepeat()
        {
            Console.WriteLine("Хотите ввести другой файл? (y/n)");
            string input = Console.ReadLine()?.Trim().ToLower();
            return input == "y" || input == "да";
        }
    }
}