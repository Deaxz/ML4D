using System;
using System.IO;
using Antlr4.Runtime;
using ML4D.Compiler;
using ML4D.Compiler.ASTVisitors;

namespace ML4D
{
    class Program
    {
        static void Main(string[] args)
        {
            // Automatic test
            // List<string> examples = new List<string>()
            // {

            //     //"double a = 2.0; double b = (a + 10) * 2**3;",
            //     //"int b = 0; bool a = 10 < 11 and 12 == 13 or 14 != 15 and 16 > b;", "bool b = 10 == 10;",
            //     "double a = 1.0; double b = 2.0; double f(int x) { return a * x + b; }; int x0 = 0; int x1 = 1; int x2 = 2; int x3 = 3; int x4 = 4; int y0 = 0; int y1 = 1; int y2 = 2; int y3 = 3; int y4 = 4; double step_size = 0.1; int i = 0; while (i < 100) { double y_hat0 = (f(x0) - y0)**2; double y_hat1 = (f(x1) - y1)**2; double y_hat2 = (f(x2) - y2)**2; double y_hat3 = (f(x3) - y3)**2; double y_hat4 = (f(x4) - y4)**2; double loss = (y_hat0 + y_hat1 + y_hat2 + y_hat3 + y_hat4)/5; a = a - (a * step_size); b = b - (b * step_size); i = i + 1; };",
            //     //"int i = 0; double d = i + 2.2;", "double d = 2+2;", "void f(int x) { return; };"

            // };
            //
            // foreach (string s in examples)
            // {
            //     var inputStream = new AntlrInputStream(new StringReader(s));
            //     var lexer = new ML4DLexer(inputStream);
            //     var tokenStream = new CommonTokenStream(lexer);
            //     var parser = new ML4DParser(tokenStream);
            //     
            //     try 
            //     {
            //         var cst = parser.lines();
            //         var ast = new ASTBuilder().VisitLines(cst);
            //         
            //         // Pretty print
            //         PrettyPrintVisitor prettyprint = new PrettyPrintVisitor();
            //         prettyprint.Visit(ast);
            //         Console.WriteLine("^ Is pretty print");
            //
            //         // Symbol Table and Type check
            //         SymbolTable symbolTable = new SymbolTable();
            //         var typesymbolVisitor = new TypeCheckSymbolTableVisitor(symbolTable);
            //         typesymbolVisitor.Visit(ast);
            //         symbolTable.Clear();
            //         
            //         Console.WriteLine("^ Symbol Table and Type check done");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine(ex.Message);
            //     }
            //     Console.WriteLine();
            // }

            try
            {
                if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
                    throw new Exception(
                        "No run arguments were found. Missing arguments: <source filename> <target filename>");

                string sourceFilename = args[0].Trim();
                string targetFileName = args[1].Trim();
                string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + sourceFilename + ".ML4D");

                // ANTLR Lexer, Paser, and AST
                var inputStream = new AntlrInputStream(new StringReader(text));
                var lexer = new ML4DLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new ML4DParser(tokenStream);
                var cst = parser.lines();
                var ast = new ASTBuilder().VisitLines(cst);

                // Symbol Table and Type check
                SymbolTable symbolTable = new SymbolTable();
                symbolTable.OpenScope(); // Global scope
                var typesymbolVisitor = new TypeCheckSymbolTableVisitor(symbolTable);
                typesymbolVisitor.Visit(ast);

                // Code generation
                symbolTable.OnlyGlobalScope();
                CodeGeneration codeGen = new CodeGeneration(symbolTable);
                codeGen.Visit(ast);
                codeGen.WriteToFile(targetFileName);
                SymbolTable.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
