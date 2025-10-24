using System;

namespace TextEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new TextEditorContext();

            
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ШАБЛОНУ STRATEGY ===\n");

            string text = "Мій код:\n";
            var result = context.ExecuteWithStrategy("macro", text, text.Length, 0, "class");

            Console.WriteLine("1. Макрос 'class':");
            Console.WriteLine($"Результат: {result.Success}");
            Console.WriteLine($"Текст:\n{result.ModifiedText}");
            Console.WriteLine();

  
            var snippetResult = context.ExecuteWithStrategy("snippet", "Властивість: ", 12, 0, "prop|type=int|name=Age");
            Console.WriteLine("2. Сніппет 'prop':");
            Console.WriteLine($"Результат: {snippetResult.Success}");
            Console.WriteLine($"Текст: {snippetResult.ModifiedText}");

            Console.ReadKey();
        }
    }
}