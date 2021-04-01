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
            // Automatic test - TODO flere strings så vi presser sproget og finder flere bugs.
            List<string> examples = new List<string>()
            {
                "double d = 2+2;", "int i = 2+2;", "d = i + 2.2;", "double a = -1.0; double b; b = 2.0; bool c = true; bool d = not c; double linearFunction(int x) { return a * x + b; }; int x0 = 0; int x1 = 1; int x2 = 2; int x3 = 3; int x4 = 4; int y0 = 0; int y1 = 1; int y2 = 2; int y3 = 3; int y4 = 4; double step_size = 0.1; int i = 0; while (i < 100) { double y_hat0 = (f(x0) - y0)**2; double y_hat1 = (f(x1) - y1)**2; double y_hat2 = (f(x2) - y2)**2; double y_hat3 = (f(x3) - y3)**2; double y_hat4 = (f(x4) - y4)**2; double loss = (y_hat0 + y_hat1 + y_hat2 + y_hat3 + y_hat4)/5; loss<-; a = a - (a * step_size); b = b - (b * step_size); zero(a); zero(b); i = i + 1; };",
                "bool d = d < a and x >= v or 10 == p and 10 != q;", "func(d < a and x >= v or 10 == p and 10 != q);"
            };
            
            foreach (string s in examples)
            {
                var inputStream = new AntlrInputStream(new StringReader(s));
                var lexer = new dinoLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new dinoParser(tokenStream);
                
                try // TODO Kunne nok være nice med nogle custom exceptions til ASTBuilder, så man nemmere kan debug fordi man ved hvor de bliver thrown.
                {
                    var cst = parser.lines();
                    var ast = new ASTBuilder().VisitLines(cst);
                    var prettyprint = new PrettyPrintVisitor().Visit(ast);
                    
                    Console.WriteLine("^ Is pretty print");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            
                Console.WriteLine();
            }
            
             // Command line version
             // while (true)
             // {
             //     Console.Write("> ");
             //     var exprText = Console.ReadLine();
             //
             //     if (string.IsNullOrWhiteSpace(exprText))
             //         break;
             //
             //     try // TODO Kunne nok være nice med nogle custom exceptions til ASTBuilder, så man nemmere kan debug fordi man ved hvor de bliver thrown.
             //     {
             //         var inputStream = new AntlrInputStream(new StringReader(exprText));
             //         var lexer = new dinoLexer(inputStream);
             //         var tokenStream = new CommonTokenStream(lexer);
             //         var parser = new dinoParser(tokenStream);
             //
             //         var cst = parser.lines();
             //         var ast = new ASTBuilder().VisitLines(cst);
             //         var prettyprint = new PrettyPrintVisitor().Visit(ast);
             //
             //         Console.WriteLine("^ Is pretty print");
             //     }
             //     catch (NullReferenceException ex)
             //     {
             //         Console.WriteLine("fejler 1" + ex.Message);
             //     }
             //     catch (Exception ex)
             //     {
             //         Console.WriteLine("fejler 2" + ex.Message);
             //     }
             //     
             //     Console.WriteLine();
             // }
        }
    }
}
