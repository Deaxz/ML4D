using System.Collections.Generic;

namespace ML4D.Compiler
{
    // Value nodes
    public class IDNode : ExpressionNode
    {
        public string Name { get; set; } // TODO overvej at skifte til ID, så det er konsistent med andre nodes.
        public IDNode(string name)
        {
            Name = name;
        }

        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        {
            return new List<Node>();
        }
    }
    public class DoubleNode : ExpressionNode
    {
        public double Value { get; set; }
        public DoubleNode(double value)
        {
            Value = value;
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        {
            return new List<Node>();
        }
    }
    public class IntNode : ExpressionNode
    {
        public int Value { get; set; }
        public IntNode(int value)
        {
            Value = value;
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        {
            return new List<Node>();
        }
    }
    public class BoolNode : ExpressionNode
    {
        public bool Value { get; set; }
        public BoolNode(bool value)
        {
            Value = value;
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        {
            return new List<Node>();
        }
    }
    public class VoidNode : ExpressionNode
    {
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        {
            return new List<Node>();
        }
    }
    
    // Function node - Used for both funcExpr and funcStmt
    public class FunctionExprNode : ExpressionNode 
    {
        public string ID { get; set; }
        public List<ExpressionNode> Arguments = new List<ExpressionNode>();

        public FunctionExprNode(string id)
        {
            ID = id;
        }

        public override List<Node> GetChildren() // Expects list of Node, but can't cast list. TODO consider changing list to nodes, and casting to Node in AST builder
        {
            List<Node> nodes = new List<Node>();
            foreach (ExpressionNode node in Arguments)
            {
                nodes.Add(node);
            }
            return nodes;
        }
    }
    
    // Arithmetic nodes
    public class AdditionNode : InfixExpressionNode
    {
    }
    public class SubtractionNode : InfixExpressionNode
    {
    }
    public class MultiplicationNode : InfixExpressionNode
    {
    }
    public class DivisionNode : InfixExpressionNode
    {
    }
    public class PowerNode : InfixExpressionNode
    {
    }

    // Boolean nodes
    public class AndNode : InfixExpressionNode
    {
    }
    public class OrNode : InfixExpressionNode
    {
    }
    public class NotNode : UnaryExpressionNode
    {
    }
    
    // Equality nodes
    public class EqualNode : InfixExpressionNode
    {
    }
    public class NotEqualNode : InfixExpressionNode
    {
    }
    
    // Relational nodes 
    public class LessThanNode : InfixExpressionNode
    {
    }
    public class LessEqualThanNode : InfixExpressionNode
    {
    }
    public class GreaterThanNode : InfixExpressionNode
    {
    }
    public class GreaterEqualThanNode : InfixExpressionNode
    {
    }
}