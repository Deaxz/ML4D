using System.Collections.Generic;

namespace ML4D.Compiler
{
    // Value nodes
    public class IDNode : ExpressionNode
    {
        public string Name { get; set; }
        public IDNode(string name)
        {
            Name = name;
        }
    }
    public class DoubleNode : ExpressionNode
    {
        public double Value { get; set; }
        public DoubleNode(double value)
        {
            Value = value;
        }
    }
    public class IntNode : ExpressionNode
    {
        public int Value { get; set; }
        public IntNode(int value)
        {
            Value = value;
        }
    }
    public class BoolNode : ExpressionNode
    {
        public bool Value { get; set; }
        public BoolNode(bool value)
        {
            Value = value;
        }
    }
    public class VoidNode : ExpressionNode
    {
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