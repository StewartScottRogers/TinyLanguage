using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TinyLanguage.Interpreter;
using TinyLanguage.Lexer;
using TinyLanguage.Lexer.Nodes;
using LexerClass = TinyLanguage.Lexer.Lexer;
using InterpreterClass = TinyLanguage.Interpreter.Interpreter;

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
                return RunDemoMode();
            }

            Console.Error.WriteLine("Usage: TinyLanguage [input.tlg output.txt]");
            return 1;
        }

        // ── File-processor mode ───────────────────────────────────────────────

        private static int RunFileProcessorMode(string inputPath, string outputPath)
        {
            string source;
            try
            {
                source = File.ReadAllText(inputPath, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error reading input file: " + exception.Message);
                return 1;
            }

            string output;
            try
            {
                output = InterpretToString(source);
            }
            catch (LexerException lexerException)
            {
                Console.Error.WriteLine(lexerException.Message);
                return 1;
            }
            catch (ParserException parserException)
            {
                Console.Error.WriteLine(parserException.Message);
                return 1;
            }
            catch (InterpreterException interpreterException)
            {
                Console.Error.WriteLine(interpreterException.Message);
                return 1;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error: " + exception.Message);
                return 1;
            }

            try
            {
                File.WriteAllText(outputPath, output, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error writing output file: " + exception.Message);
                return 1;
            }

            return 0;
        }

        // ── Piped I/O mode ────────────────────────────────────────────────────

        private static int RunPipedMode()
        {
            string source;
            try
            {
                source = Console.In.ReadToEnd();
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error reading stdin: " + exception.Message);
                return 1;
            }

            try
            {
                string output = InterpretToString(source);
                Console.Write(output);
            }
            catch (LexerException lexerException)
            {
                Console.Error.WriteLine(lexerException.Message);
                return 1;
            }
            catch (ParserException parserException)
            {
                Console.Error.WriteLine(parserException.Message);
                return 1;
            }
            catch (InterpreterException interpreterException)
            {
                Console.Error.WriteLine(interpreterException.Message);
                return 1;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error: " + exception.Message);
                return 1;
            }

            return 0;
        }

        // ── Demo mode ─────────────────────────────────────────────────────────

        private static int RunDemoMode()
        {
            string demoDirectory = FindDemoDirectory();
            if (demoDirectory == string.Empty)
            {
                Console.Error.WriteLine("Error: Could not locate TinyLanguage.DemoFiles directory.");
                return 1;
            }

            string[] demoFiles = Directory.GetFiles(demoDirectory, "*.tlg");
            Array.Sort(demoFiles, StringComparer.OrdinalIgnoreCase);

            List<string> failedFiles = new List<string>();

            foreach (string filePath in demoFiles)
            {
                string fileName = Path.GetFileName(filePath);
                Console.WriteLine("=== " + fileName + " ===");

                string source;
                try
                {
                    source = File.ReadAllText(filePath, Encoding.UTF8);
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine("Error reading " + fileName + ": " + exception.Message);
                    failedFiles.Add(fileName);
                    Console.WriteLine("---");
                    continue;
                }

                try
                {
                    RunInterpreterToConsole(source);
                }
                catch (LexerException lexerException)
                {
                    Console.Error.WriteLine(fileName + ": " + lexerException.Message);
                    failedFiles.Add(fileName);
                }
                catch (ParserException parserException)
                {
                    Console.Error.WriteLine(fileName + ": " + parserException.Message);
                    failedFiles.Add(fileName);
                }
                catch (InterpreterException interpreterException)
                {
                    Console.Error.WriteLine(fileName + ": " + interpreterException.Message);
                    failedFiles.Add(fileName);
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine(fileName + ": " + exception.Message);
                    failedFiles.Add(fileName);
                }

                Console.WriteLine("---");
            }

            Console.WriteLine("All demos completed successfully.");

            if (failedFiles.Count > 0)
            {
                Console.Error.WriteLine(failedFiles.Count + " demo(s) failed:");
                foreach (string failedFile in failedFiles)
                {
                    Console.Error.WriteLine("  " + failedFile);
                }
                return 1;
            }

            return 0;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        // Locates the TinyLanguage.DemoFiles directory by walking up from the
        // executable directory until the solution root (containing TinyLanguage.slnx)
        // is found, then appending TinyLanguage.DemoFiles.
        private static string FindDemoDirectory()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string currentDirectory = exeDirectory;

            while (currentDirectory != string.Empty)
            {
                if (File.Exists(Path.Combine(currentDirectory, "TinyLanguage.slnx")))
                {
                    string demoDirectory = Path.Combine(currentDirectory, "TinyLanguage.DemoFiles");
                    if (Directory.Exists(demoDirectory))
                    {
                        return demoDirectory;
                    }
                }

                DirectoryInfo parent = Directory.GetParent(currentDirectory);
                if (parent == null)
                {
                    break;
                }
                currentDirectory = parent.FullName;
            }

            return string.Empty;
        }

        // Runs source code through the pipeline and returns the captured stdout output.
        private static string InterpretToString(string source)
        {
            StringWriter capturedOutput = new StringWriter();
            TextWriter previousOut = Console.Out;
            Console.SetOut(capturedOutput);
            try
            {
                RunInterpreterToConsole(source);
            }
            finally
            {
                Console.SetOut(previousOut);
            }
            return capturedOutput.ToString();
        }

        // Runs source code through the Lexer → Parser → Interpreter pipeline.
        // Output goes directly to Console.Out (whatever it is currently set to).
        private static void RunInterpreterToConsole(string source)
        {
            LexerClass lexer = new LexerClass(source);
            List<Token> tokens = lexer.Tokenize();

            Parser parser = new Parser(tokens);
            ProgramNode program = parser.Parse();

            InterpreterClass interpreter = new InterpreterClass();
            interpreter.Execute(program);
        }
    }
}
