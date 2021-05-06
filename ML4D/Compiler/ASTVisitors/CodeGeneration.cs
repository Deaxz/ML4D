using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler.ASTVisitors
{
    public class CodeGeneration : ASTVisitor
    {
        //private string FileName { get; set; }
        private string ProgramText { get; set; }
        
        public CodeGeneration(string fileName)
        {
            //FileName = fileName;
        }

        public void WriteToFile(string fileName)
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + fileName + ".c", ProgramText);
        }
        
        // TODO test method, remember to remove.
        public void WriteToConsole(string fileName)
        {
            Console.WriteLine(ProgramText);
        }
        
        private void Emit(string text)
        {
            ProgramText += text;
        }
        
        // --- Declarations ---
        public override void Visit(VariableDCLNode node)
        {
            ProgramText += node.Type + " " + node.ID;
            base.Visit(node);
            Emit(";\n");
        }

        public override void Visit(FunctionDCLNode node)
        {
            Emit(node.Type + node.ID + "(");
            foreach (FunctionArgumentNode argumentNode in node.Arguments)
            {
                if (argumentNode != node.Arguments[^1])
                    Emit(argumentNode.Type + " " + argumentNode.ID + ", ");
                else 
                    Emit(argumentNode.Type + " " + argumentNode.ID);
            }
            Emit(") {\n");
            Visit(node.Body);
            Emit("\n}\n");
        }

        
        // --- Statements ---
        public override void Visit(AssignNode node)
        {
            Emit(node.ID + " = ");
            base.Visit(node);
            Emit(";\n");
        }

        public override void Visit(WhileNode node)
        {
            Emit("while (");
            Visit(node.Predicate);
            Emit(") {\n");
            Visit(node.Body);
            Emit("\n}\n");
        }

        public override void Visit(ReturnNode node)
        {
            if (node.Inner is null)
                Emit("return;");
            
            Emit("return ");
            Visit(node.Inner);
            Emit(";\n");
        }

        public override void Visit(FunctionExprNode node)
        {
            Emit(node.ID + "(");
            foreach (Node argumentNode in node.Arguments)
            {
                Visit(argumentNode);
                
                if (argumentNode != node.Arguments[^1])
                    Emit(", ");
            }
            Emit(")");
            // TODO ret stort problem, ved ikke om ";" eller no. Pga. funcStmt og funcExpr er same
        }

        
        // --- Values ---
        public override void Visit(IDNode node)
        {
            Emit(node.ID + "");
        }

        public override void Visit(DoubleNode node)
        {
            Emit(node.Value + "");
        }

        public override void Visit(IntNode node)
        {
            Emit(node.Value + "");
        }

        public override void Visit(BoolNode node)
        {
            Emit(node.Value ? "1" : "0");
        }


        // --- Arithmetic ---
        public override void Visit(InfixExpressionNode node)
        {
            if (node.Parenthesized)
                Emit("(");
            Visit(node.Left);
            // TODO operator symbol, Emit(" " + node.Operator + " ");
            Visit(node.Right);
            if (node.Parenthesized)
                Emit(")");
        }

        public override void Visit(UnaryExpressionNode node)
        {
            if (node.Parenthesized)
                Emit("(");
            // TODO operator symbol, Emit(" " + node.Operator + " ");
            Visit(node.Inner);
            if (node.Parenthesized)
                Emit(")");
        }
    }
}