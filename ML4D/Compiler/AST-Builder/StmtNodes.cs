namespace ML4D.Compiler
{
    public class AssignNode : Node
    {
        //public string Type { get; set; } //TODO ikke brugt pt, men jeg tænker nødvendigt for type checking.
        public string ID { get; set; }
        public ExpressionNode Right { get; set; }
        
        public AssignNode(string id, ExpressionNode right)
        {
            ID = id;
            Right = right;
        }
    }
    
    public class WhileNode : Node
    {
        public ExpressionNode Predicate { get; set; }
        public LinesNode Body { get; set; }
        
        public WhileNode(ExpressionNode predicate, LinesNode body)
        {
            Predicate = predicate;
            Body = body;
        }
    }
    
    public class BackwardNode : Node
    {
        public string ID { get; set; }

        public BackwardNode(string id)
        {
            ID = id;
        }
    }
    
    public class ReturnNode : UnaryExpressionNode
    {
        public ReturnNode(ExpressionNode inner)
        {
            Inner = inner;
        }
    }
}