namespace ML4D.Compiler
{
    public class AssignNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public ExpressionNode Right { get; set; }
    }
    
    public class WhileNode : Node
    {
        public ExpressionNode Predicate { get; set; }
        public LinesNode Body { get; set; }
    }
    
    public class BackwardNode : UnaryExpressionNode
    {
    }
    
    public class ReturnNode : UnaryExpressionNode
    {
        
    }
}