using System;
using System.Collections.Generic;
using System.IO;

namespace TinyLanguage
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            if (args.Length == 2)
            {
                return RunFileProcessorMode(args[0], args[1]);
            }

            if (args.Length == 0)
            {
                if (Console.IsInputRedirected && Console.In.Peek() != -1)
                {
                    return RunPipedMode();
                }
                return RunDemoMode();
            }

            Console.Error.WriteLine("Usage: TinyLanguage [input.tlg output.txt]");
            return 1;
        }

        private static int RunFileProcessorMode(string inputPath, string outputPath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(inputPath))
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    IReadOnlyList<TinyLanguage.Lexer.Token> tokens = TinyLanguage.Lexer.Lexer.Tokenise(reader);
                    TinyLanguage.Lexer.ProgramNode ast = TinyLanguage.Lexer.Parser.Parse(tokens);
                    TinyLanguage.Interpreter.Interpreter interpreter = new TinyLanguage.Interpreter.Interpreter(writer, Console.In);
                    interpreter.Execute(ast);
                }
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }

        private static int RunPipedMode()
        {
            try
            {
                IReadOnlyList<TinyLanguage.Lexer.Token> tokens = TinyLanguage.Lexer.Lexer.Tokenise(Console.In);
                TinyLanguage.Lexer.ProgramNode ast = TinyLanguage.Lexer.Parser.Parse(tokens);
                TinyLanguage.Interpreter.Interpreter interpreter = new TinyLanguage.Interpreter.Interpreter(Console.Out, Console.In);
                interpreter.Execute(ast);
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }

        private static int RunDemoMode()
        {
            string demoDirectory = FindDemoFilesDirectory();
            string[] demoFiles = System.IO.Directory.GetFiles(demoDirectory, "*.tlg");
            System.Array.Sort(demoFiles);

            bool anyFailed = false;

            foreach (string demoFilePath in demoFiles)
            {
                string fileName = System.IO.Path.GetFileName(demoFilePath);
                Console.WriteLine("=== " + fileName + " ===");

                try
                {
                    using (StringWriter outputWriter = new StringWriter())
                    {
                        using (StreamReader reader = new StreamReader(demoFilePath))
                        {
                            IReadOnlyList<TinyLanguage.Lexer.Token> tokens = TinyLanguage.Lexer.Lexer.Tokenise(reader);
                            TinyLanguage.Lexer.ProgramNode ast = TinyLanguage.Lexer.Parser.Parse(tokens);
                            TinyLanguage.Interpreter.Interpreter interpreter = new TinyLanguage.Interpreter.Interpreter(outputWriter, Console.In);
                            interpreter.Execute(ast);
                        }
                        Console.Write(outputWriter.ToString());
                    }
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine(exception.Message);
                    Console.WriteLine("Demo " + fileName + " failed.");
                    anyFailed = true;
                }

                Console.WriteLine("---");
            }

            if (anyFailed)
            {
                return 1;
            }

            Console.WriteLine("All demos completed successfully.");
            return 0;
        }

        private static string FindDemoFilesDirectory()
        {
            string dir = System.AppContext.BaseDirectory;
            while (dir != null)
            {
                string candidate = System.IO.Path.Combine(dir, "TinyLanguage.DemoFiles");
                if (System.IO.Directory.Exists(candidate))
                    return candidate;

                string candidate2 = System.IO.Path.Combine(dir, "..", "..", "..", "TinyLanguage.DemoFiles");
                string fullCandidate2 = System.IO.Path.GetFullPath(candidate2);
                if (System.IO.Directory.Exists(fullCandidate2))
                    return fullCandidate2;

                dir = System.IO.Directory.GetParent(dir)?.FullName;
            }
            throw new Exception("Cannot find TinyLanguage.DemoFiles directory");
        }
    }
}
