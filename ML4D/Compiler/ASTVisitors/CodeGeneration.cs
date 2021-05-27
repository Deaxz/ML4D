using System;
using System.Collections.Generic;
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
            string Includes = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Tensor.c");
            string CMainFunction = "\nint main() {\n";
            string programText = Includes + "\n" +  _FuncPrototypes + _GlobalVariables + 
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
                Emit("static " + node.Type + " " + node.ID + ";\n");
                GlobalScope = false;
                Emit(node.ID + " = ");
                base.Visit(node);
                Emit(";\n");
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
            if (SymbolTable.Retrieve(node.ID) is null)
            {
                Emit("Tensor* " + node.ID + " = ");
                PrintTensorDCL(node);
            }
            else
            {
                GlobalScope = true;
                Emit("static Tensor* " + node.ID + ";\n");
                GlobalScope = false;
                Emit(node.ID + " = ");
                PrintTensorDCL(node);
            }
        }

        private void PrintTensorDCL(TensorDCLNode node)
        {
            
            if (node.Init is TensorInitNode initNode)
            {
                Emit($"newTensor((double[]){{");

                List<Node> children = initNode.GetChildren();
                for(int i=0; i < children.Count - 1; i++)
                {
                    Visit(children[i]);
                    Emit(",");
                }
                Visit(children.Last());

                Emit($"}}, {node.Rows}, {node.Columns})");
            }
            else
                Visit(node.Init);
            Emit(";\n");
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
            Emit("} ");
            
            foreach (IfNode elseifNode in node.IfNodes.Skip(1))
            {
                Emit("else if (");
                Visit(elseifNode.Predicate);
                Emit(") {\n");
                Visit(elseifNode.Body);
                Emit("} ");
            }

            if (node.ElseBody is not null)
            {
                Emit("else {\n");
                Visit(node.ElseBody);
                Emit("}\n");
            }
            else
                Emit("\n");
        }

        public override void Visit(ForNode node)
        {
            Emit("for (");
            Emit(node.Init.Type + " " + node.Init.ID + " = ");
            Visit(node.Init.Init);  
            Emit("; ");
            Visit(node.Predicate);
            Emit("; ");
            Emit(node.Final.ID + " = ");
            Visit(node.Final.Right);
            Emit(") {\n");
            Visit(node.Body);
            Emit("}\n");
        }

        public override void Visit(GradientsNode node)
        {
            Emit("tensorBackwards(" + node.tensorID + ");\n");
            for (int i = 0; i < node.GradVariables.Count; i++)
                Emit("Tensor* " + node.GradVariables[i] + " = readGradients(" + node.GradTensors[i] + ");\n");
            Visit(node.Body);
            Emit("zeroGradients(" + node.tensorID + ");\n");
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
            if (node.Type == "tensor")
                TensorCodeGen(node);
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
                }
                else
                    Emit(" " + node.Symbol + " ");
                Visit(node.Right);
            }
        }
        
        private void PrintExpression(UnaryExpressionNode node)
        {
            if (node is UnaryMinusNode && node.Type is "tensor")
            {
                Emit("scalarmul(-1, ");
                Visit(node.Inner);
                Emit(")");
            }
            else
            {
                Emit(node is NotNode ? "!" : "-");
                Visit(node.Inner);                
            }
        }

        private void TensorCodeGen(InfixExpressionNode node)
        {
            switch (node)
            {
                // Left or right is a tensor
                case AdditionNode or SubtractionNode:
                    Emit(node is AdditionNode ? "tadd(" : "tsub(");
                    
                    if (node.Left.Type != "tensor")
                    {
                        Emit("convertToTensor(");
                        Visit(node.Left);
                        Emit($", {node.Rows}, {node.Columns})");
                    } 
                    else
                        Visit(node.Left);

                    Emit(", ");

                    if (node.Right.Type != "tensor")
                    {
                        Emit("convertToTensor(");
                        Visit(node.Right);
                        Emit($", {node.Rows}, {node.Columns})");
                    } 
                    else
                        Visit(node.Right);
                    Emit(")");
                    break;
                
                // Left or right is a tensor
                case MultiplicationNode when node.Left.Type != "tensor" || node.Right.Type != "tensor":
                    Emit("scalarmul(");
                    if (node.Left.Type != "tensor")
                    {
                        Visit(node.Left);
                        Emit(", ");
                        Visit(node.Right);
                        Emit(")");
                    }
                    else
                    {
                        Visit(node.Right);
                        Emit(", ");
                        Visit(node.Left);
                        Emit(")");
                    }
                    break;
                
                // Left and right are tensors
                case MultiplicationNode:
                    Emit("tmul(");
                    Visit(node.Left);
                    Emit(", ");
                    Visit(node.Right);
                    Emit(")");
                    break;
                case DivisionNode:
                    Emit("tdiv(");
                    Visit(node.Left);
                    Emit(", ");
                    Visit(node.Right);
                    Emit(")");
                    break;

                case PowerNode:
                    Emit("tpow(");
                    Visit(node.Left);
                    Emit(", ");
                    Visit(node.Right);
                    Emit(")");
                    break;
            }
        }
    }
}