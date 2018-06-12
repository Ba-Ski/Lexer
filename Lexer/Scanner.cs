using System;
using System.Collections.Generic;
using System.IO;
using Lexer.Entity;

namespace Lexer
{
    public class Scanner
    {
        private readonly HashSet<string> _keyWords =
            new HashSet<string>(new[] {"Var", "End", "Begin", "Boolean", "Decimal", "If", "Else"});

        private readonly HashSet<char> _unaryOperations = new HashSet<char>(new[] {'!'});

        private readonly HashSet<string> _binaryOperations =
            new HashSet<string>(new[] {"&", "^", "-", "+", "*", "/", ">", "<", "=="});

        private readonly HashSet<char> _separators = new HashSet<char>(new[] {',', ':', ';', '(', ')'});
        private readonly HashSet<char> _endToken = new HashSet<char>(new[] {'.'});
        private readonly HashSet<char> _emptySymbols = new HashSet<char>(new[] {' ', '\t', '\n', '\r'});


        public Scanner()
        {
        }

        public void Run(string path)
        {
            var tokenizer = new Tokenizer.Builder()
                .WithKeyWords(_keyWords)
                .WithUnaryOperations(_unaryOperations)
                .WithBinaryOperations(_binaryOperations)
                .WithSeparators(_separators)
                .WithEndTokens(_endToken)
                .WithEmptySymbols(_emptySymbols)
                .Build();

            var result = tokenizer.tokenize(ReadFromFile(path));
            Print(result);
            Console.ReadLine();
        }

        private static string ReadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path can't be null or empty");

            try
            {
                var fullPath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\Source\", path);
                using (var reader = new StreamReader(fullPath))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                throw;
            }
        }

        private static void Print(Result result)
        {
            var ids = result.Ids;
            var tokens = result.Tokens;
            Console.WriteLine("Tokens.");
            Console.WriteLine("<type, value> (line, row)");
            Console.WriteLine("-------------------------");
            foreach (var token in tokens)
            {
                Console.WriteLine($"<{token.Type}, {token.Value}> ({token.Position.Line}, {token.Position.Row})");
            }


            PrintIdsTable(ids);
        }

        private static void PrintIdsTable(IList<string> ids)
        {
            Console.WriteLine("\nIDs table.");
            Console.WriteLine("---------");
            for (var i = 0; i < ids.Count; i++)
            {
                Console.WriteLine(i + " " + ids[i]);
            }
        }
    }
}