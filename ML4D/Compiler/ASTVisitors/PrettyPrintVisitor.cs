using System;
using System.Linq;

namespace ML4D.Compiler.ASTVisitors
{
    public class PrettyPrintVisitor : ASTVisitor<string>
    {
        // Indentation method, used to dynamically indent the nodes.
        public string Inden()
        {
            return string.Concat(Enumerable.Repeat(" ", i));
        }
        public int i = 0;
        
        // Main structure nodes
        public override string Visit(LinesNode node)
        {
            foreach (Node n in node.lines)
            {
                //Console.WriteLine(inden + Visit(n));
                Visit(n);
                i = 0;
            }

            string result = "prettyprint successful";
            return result;
        }
        public override string Visit(VariableDCLNode node)
        {
            if (node.Init is not null)
            {
                Console.WriteLine(Inden() + node.Type + " " + node.ID);
                i += 2;
                Console.WriteLine(Inden() + Visit(node.Init));
                i -= 2;
            }
            else
                Console.WriteLine(node.Type + " " + node.ID);

            string result = "\n";            
            return result;
        }

        // Value
        public override string Visit(IDNode node)
        {
            string result = "" + node.Name;            
            return result;
        }
        public override string Visit(VoidNode node)
        {
            string result = ""; // TODO Evt. print void, så det er tydeligt.            
            return result;
        }
        public override string Visit(DoubleNode node)
        {
            string result = "" + node.Value;            
            return result;
        }
        public override string Visit(IntNode node)
        {
            string result = "" + node.Value;            
            return result;     
        }
        public override string Visit(BoolNode node)
        {
            string result = "" + node.Value;            
            return result;    
        }
        
        // Arithmetic
        public override string Visit(AdditionNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + '+');
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(SubtractionNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + '-');
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;       
        }
        public override string Visit(MultiplicationNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + '*');
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(DivisionNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + '/');
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(PowerNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + "**");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        
        // Boolean
        public override string Visit(AndNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + "and");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        
        public override string Visit(OrNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + "or");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        
        public override string Visit(NotNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + "not");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Inner));
            i -= 2;
            
            string result = "";            
            return result;
        }
        
        // Equality
        public override string Visit(EqualNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + "==");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(NotEqualNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + "!=");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        
        // Relational
        public override string Visit(GreaterThanNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + '>');
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(GreaterEqualThanNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + ">=");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(LessThanNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + '<');
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
        public override string Visit(LessEqualThanNode node)
        {
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Left));
            i -= 2;
            Console.WriteLine(Inden() + "<=");
            i += 2;
            Console.WriteLine(Inden() + Visit(node.Right));
            
            string result = "";            
            return result;
        }
    }
}