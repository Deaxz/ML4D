﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler.ASTVisitors
{
    public class CodeGeneration : ASTVisitor
    {
        private StringBuilder _FuncPrototypes = new StringBuilder();
        private StringBuilder _MainText = new StringBuilder();
        private StringBuilder _FuncDCLs = new StringBuilder();
        private StringBuilder _GlobalVariables = new StringBuilder();
        private SymbolTable SymbolTable { get; set; }
        private bool InsideFunc { get; set; }
        private bool GlobalScope { get; set; }

        public CodeGeneration(SymbolTable symbolTable)
        {
            SymbolTable = symbolTable;
        }
        
        public void WriteToFile(string fileName)
        {
            string CIncludes = "#include <stdio.h>\n#include <stdbool.h>\n#include <math.h>\n\n";
            string CMainFunction = "\nint main() {\n";
            string programText = CIncludes + _FuncPrototypes + _GlobalVariables + 
                                 CMainFunction + _MainText + "return 1;\n}\n\n" + _FuncDCLs;
            
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + fileName + ".c", programText);
        }
        
        private void Emit(string text)
        {
            if (GlobalScope)
                _GlobalVariables.Append(text);
            else if (InsideFunc)
                _FuncDCLs.Append(text);
            else 
                _MainText.Append(text);
        }
        
        // --- Declarations ---
        public override void Visit(VariableDCLNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
            {
                Emit(node.Type + " " + node.ID + " = ");
                base.Visit(node);
                Emit(";\n");
            }
            else
            {
                GlobalScope = true;
                Emit("static " + node.Type + " " + node.ID + " = ");
                base.Visit(node);
                Emit(";\n");
                GlobalScope = false;
            }
        }

        public override void Visit(FunctionDCLNode node)
        {
            InsideFunc = true;
            _FuncPrototypes.Append(node.Type + " " + node.ID + "(");
            Emit(node.Type + " " + node.ID + "(");
            
            foreach (FunctionArgumentNode argumentNode in node.Arguments)
            {
                if (argumentNode != node.Arguments[^1])
                {
                    _FuncPrototypes.Append(argumentNode.Type + " " + argumentNode.ID + ", ");
                    Emit(argumentNode.Type + " " + argumentNode.ID + ", ");
                }
                else
                {
                    _FuncPrototypes.Append(argumentNode.Type + " " + argumentNode.ID);
                    Emit(argumentNode.Type + " " + argumentNode.ID);
                }
            }
            _FuncPrototypes.Append(");\n");
            Emit(") {\n");
            
            Visit(node.Body);
            Emit("}\n");
            InsideFunc = false;
        }

        public override void Visit(TensorDCLNode node)
        {
            Emit("Tensor* " + node.ID + " = ");

            if(node.Init is TensorInitNode initNode){
                Emit($"newTensor(*(double[][{node.Columns}]){{{{");
                int i=1;
                foreach(ExpressionNode exprNode in initNode.GetChildren()){
                    Visit(exprNode);
                                      
                    if(i % node.Columns == 0){
                        Emit("}");
                        if(i != node.Rows * node.Columns){
                            Emit(",{");
                        }
                    }else
                    {
                        Emit(",");
                    }
                    i++;
                }
                Emit($"}}, {node.Rows}, {node.Columns});\n");
            }else{
                //might be wrong
                Visit(node.Init);
            }
        }

        public override void Visit(TensorInitNode node){
            Emit("newTensor(*(double[][");
            
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
            Emit("}\n");
        }

        public override void Visit(ReturnNode node)
        {
            if (node.Inner is null)
                Emit("return;\n");
            
            Emit("return ");
            Visit(node.Inner);
            Emit(";\n");
        }

        public override void Visit(FunctionNode node)
        {
            Emit(node.ID + "(");
            foreach (Node argumentNode in node.Arguments)
            {
                Visit(argumentNode);
                
                if (argumentNode != node.Arguments[^1])
                    Emit(", ");
            }
            Emit(node is FunctionStmtNode ? ");\n" : ")");
        }

        public override void Visit(IfElseChainNode node)
        {
            Emit("if (");
            Visit(node.IfNodes[0].Predicate);
            Emit(") {\n");
            Visit(node.IfNodes[0].Body);
            Emit("}\n"); // TODO Tjek om C tillader newline her!!!!
            
            foreach (IfNode elseifNode in node.IfNodes.Skip(1)) // TODO tjek om .Skip(1) gør det jeg forventer (skip første if)
            {
                Emit("else if (");
                Visit(elseifNode.Predicate);
                Emit(") {\n");
                Visit(elseifNode.Body);
                Emit("}\n"); // TODO Tjek om C tillader newline her!!!!
            }

            if (node.ElseBody is not null)
            {
                Emit("else {\n");
                Visit(node.ElseBody);
                Emit("}\n");
            }
        }

        public override void Visit(ForNode node)
        {
            Emit("for (");
            Emit(node.Init.Type + " " + node.Init.ID + " = ");
            base.Visit(node.Init);
            Emit("; ");
            Visit(node.Predicate);
            Emit("; ");
            Emit(node.Final.ID + " = ");
            base.Visit(node.Final);
            Emit(") {\n");
            Visit(node.Body);
            Emit("}\n");
        }

        public override void Visit(GradientsNode node)
        {
            // Det her bliver det spændende.
            base.Visit(node);
        }

        // --- Values ---
        public override void Visit(IDNode node)
        {
            Emit(node.ID + "");
        }

        public override void Visit(DoubleNode node)
        {
            Emit(node.Value.ToString(CultureInfo.InvariantCulture));
        }

        public override void Visit(IntNode node)
        {
            Emit(node.Value.ToString());
        }

        public override void Visit(BoolNode node)
        {
            Emit(node.Value.ToString().ToLower());
        }


        // --- Arithmetic ---
        public override void Visit(InfixExpressionNode node)
        {
            if (node.Parenthesized)
                Emit("(");
            PrintExpression(node);
            if (node.Parenthesized)
                Emit(")");
        }

        public override void Visit(UnaryExpressionNode node)
        {
            if (node.Parenthesized)
                Emit("(");
            PrintExpression(node);
            if (node.Parenthesized)
                Emit(")");
        }
        
        private void PrintExpression(InfixExpressionNode node)
        {
            if (node.Type.Equals("tensor"))
            {
                TensorCodeGen(node);
            }
            else if (node is PowerNode)
            {
                Emit("pow(");
                Visit(node.Left);
                Emit(", ");
                Visit(node.Right);
                Emit(")");
            }
            else
            {
                Visit(node.Left);
                if (node is AndNode || node is OrNode)
                {
                    string symbol = node is AndNode ? "&&" : "||";
                    Emit(" " + symbol + " ");
                } else
                    Emit(" " + node.Symbol + " ");
                Visit(node.Right);
            }
        }
        
        private void PrintExpression(UnaryExpressionNode node)
        {
            if (node is NotNode)
                Emit("!");
            Visit(node.Inner);
        }

        private void TensorCodeGen(InfixExpressionNode node)
        {
            if (node is AdditionNode)
            {
                Emit("tadd(");
                Visit(node.Left);
                Emit(",");
                Visit(node.Right);
                Emit(")");
            }
            else if (node is MultiplicationNode)
            {

                //At least left or right is of type tensor
                if (node.Left.Type != "tensor" || node.Right.Type != "tensor")
                {
                    Emit("scalarmul(");
                    if (node.Left.Type != "tensor")
                    {
                        Visit(node.Left);
                        Emit(",");
                        Visit(node.Right);
                        Emit(")");
                    }
                    else
                    {
                        Visit(node.Right);
                        Emit(",");
                        Visit(node.Left);
                        Emit(")");
                    }
                }
                else
                {
                    //left and right are tensors
                    Emit("tmul(");
                    Visit(node.Left);
                    Emit(",");
                    Visit(node.Right);
                    Emit(")");
                }
            }           

        }
    }
}