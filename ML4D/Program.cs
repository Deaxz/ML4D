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
