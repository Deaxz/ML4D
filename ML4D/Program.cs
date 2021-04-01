using System;
using System.Collections.Generic;
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
            // List<string> examples = new List<string>()
            // {
            //     "double d = 2+2;", "int i = 2+2;", "d = i + 2.2;"
            // };
            //
            // foreach (string s in examples)
            // {
            //     var inputStream = new AntlrInputStream(new StringReader(s));
            //     var lexer = new dinoLexer(inputStream);
            //     var tokenStream = new CommonTokenStream(lexer);
            //     var parser = new dinoParser(tokenStream);
            //     
            //     try
            //     {
            //         var cst = parser.lines();
            //         var ast = new ASTBuilder().VisitLines(cst);
            //         var prettyprint = new PrettyPrintVisitor().Visit(ast);
            //         
            //         Console.WriteLine("^ Is pretty print");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine(ex.Message);
            //     }
            //
            //     Console.WriteLine();
            // }
            
             // Command line version
             while (true)
             {
                 Console.Write("> ");
                 var exprText = Console.ReadLine();

                 if (string.IsNullOrWhiteSpace(exprText))
                     break;

                 try
                 {
                     var inputStream = new AntlrInputStream(new StringReader(exprText));
                     var lexer = new dinoLexer(inputStream);
                     var tokenStream = new CommonTokenStream(lexer);
                     var parser = new dinoParser(tokenStream);

                     var cst = parser.lines();
                     var ast = new ASTBuilder().VisitLines(cst);
                     var prettyprint = new PrettyPrintVisitor().Visit(ast);

                     Console.WriteLine("^ Is pretty print");
                 }
                 catch (NullReferenceException ex)
                 {
                     Console.WriteLine("fejler 1" + ex.Message);
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine("fejler 2" + ex.Message);
                 }
                 
                 Console.WriteLine();
             }
        }
    }
}
