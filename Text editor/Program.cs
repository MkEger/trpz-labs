using System;

namespace TextEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new TextEditorContext();

            
            Console.WriteLine("=== ���������ֲ� ������� STRATEGY ===\n");

            string text = "̳� ���:\n";
            var result = context.ExecuteWithStrategy("macro", text, text.Length, 0, "class");

            Console.WriteLine("1. ������ 'class':");
            Console.WriteLine($"���������: {result.Success}");
            Console.WriteLine($"�����:\n{result.ModifiedText}");
            Console.WriteLine();

  
            var snippetResult = context.ExecuteWithStrategy("snippet", "����������: ", 12, 0, "prop|type=int|name=Age");
            Console.WriteLine("2. ������ 'prop':");
            Console.WriteLine($"���������: {snippetResult.Success}");
            Console.WriteLine($"�����: {snippetResult.ModifiedText}");

            Console.ReadKey();
        }
    }
}