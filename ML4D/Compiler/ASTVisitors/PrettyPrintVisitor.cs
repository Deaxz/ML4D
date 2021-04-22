using System;
using System.Linq;

namespace ML4D.Compiler.ASTVisitors
{
    public class PrettyPrintVisitor : ASTVisitor
    {
        private string Inden()
        {
            return string.Concat(Enumerable.Repeat(" ", i));
        }
        private int i = 0;
        
        public void VisitChildren(Node node, string inden)
        {
            foreach (Node child in node.GetChildren())
            {
                Console.Write(inden);
                Visit(child);
            }
        }

        public override void Visit(LinesNode node)
        {
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(VariableDCLNode node)
        {
            Console.WriteLine(node.Type + " " + node.ID);
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(FunctionDCLNode node)
        {
            Console.WriteLine(node.Type + " " + node.ID);
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(FunctionArgumentNode node)
        {
            i += 2;
            Console.WriteLine(node.Type + " " + node.ID);
            i -= 2;
        }


        public override void Visit(AssignNode node)
        {
            Console.WriteLine(node.ID);
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(WhileNode node)
        {
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(BackwardNode node) // TODO slet
        {
            Console.WriteLine(node.ID + "<-");
        }

        public override void Visit(ReturnNode node) // TODO if you want to denote return statement
        {
            base.Visit(node);
        }

        public override void Visit(FunctionExprNode node)
        {
            Console.WriteLine(node.ID);
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(IDNode node)
        {
            Console.WriteLine(node.ID);
        }

        public override void Visit(DoubleNode node)
        {
            Console.WriteLine(node.Value);
        }

        public override void Visit(IntNode node)
        {
            Console.WriteLine(node.Value);
        }

        public override void Visit(BoolNode node)
        {
            Console.WriteLine(node.Value);
        }

        public override void Visit(InfixExpressionNode node)
        {
            switch (node)
            {
                // Arithmetic
                case AdditionNode:
                    Console.WriteLine("+");
                    break;
                case SubtractionNode:
                    Console.WriteLine("-");
                    break;
                case MultiplicationNode:
                    Console.WriteLine("*");
                    break;
                case DivisionNode:
                    Console.WriteLine("/");
                    break;
                case PowerNode:
                    Console.WriteLine("**");
                    break;
                
                // Equality
                case EqualNode:
                    Console.WriteLine("==");
                    break;
                case NotEqualNode:
                    Console.WriteLine("!=");
                    break;
                
                // Boolean
                case AndNode:
                    Console.WriteLine("and");
                    break;
                case OrNode:
                    Console.WriteLine("or");
                    break;
                
                // Relational
                case LessThanNode:
                    Console.WriteLine("<");
                    break;
                case LessEqualThanNode:
                    Console.WriteLine("<=");
                    break;
                case GreaterThanNode:
                    Console.WriteLine(">");
                    break;
                case GreaterEqualThanNode:
                    Console.WriteLine(">=");
                    break;
                default:
                    throw new NotSupportedException();
            }
            
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }

        public override void Visit(UnaryExpressionNode node)
        {
            switch (node)
            {
                case NotNode:
                    Console.WriteLine("not");
                    break;
                default:
                    throw new NotSupportedException();
            }
            
            i += 2;
            VisitChildren(node, Inden());
            i -= 2;
        }
    }
}