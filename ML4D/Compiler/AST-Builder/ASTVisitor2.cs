namespace ML4D.Compiler
{
    public abstract class ASTVisitor2
    {
        public void Visit(Node node)
        {
            Visit((dynamic)node);
        }
        public void VisitChildren(Node node)
        {
            foreach (Node child in node.GetChildren())
            {
                Visit(child);
            }
        }

        // Start Node
        public virtual void Visit(LinesNode node) {VisitChildren(node);}
        
        //-----Declaration-----
        public virtual void Visit(VariableDCLNode node) {VisitChildren(node);}
        public virtual void Visit(FunctionDCLNode node) {VisitChildren(node);}

        //-----Statement-----
        public virtual void Visit(AssignNode node) {VisitChildren(node);}
        public virtual void Visit(WhileNode node) {VisitChildren(node);}
        public virtual void Visit(BackwardNode node) {VisitChildren(node);}
        public virtual void Visit(ReturnNode node) {VisitChildren(node);}
        public virtual void Visit(FunctionExprNode node) {VisitChildren(node);}
        
        //-----Expression-----
        
        // Value
        public virtual void Visit(IDNode node) {VisitChildren(node);}
        public virtual void Visit(DoubleNode node) {VisitChildren(node);}
        public virtual void Visit(IntNode node) {VisitChildren(node);}
        public virtual void Visit(BoolNode node) {VisitChildren(node);}
        
        // Arithmetic
        public virtual void Visit(AdditionNode node) {VisitChildren(node);}
        public virtual void Visit(SubtractionNode node) {VisitChildren(node);}
        public virtual void Visit(MultiplicationNode node) {VisitChildren(node);}
        public virtual void Visit(DivisionNode node) {VisitChildren(node);}
        public virtual void Visit(PowerNode node) {VisitChildren(node);}
        
        // Boolean
        public virtual void Visit(AndNode node) {VisitChildren(node);}
        public virtual void Visit(OrNode node) {VisitChildren(node);}
        public virtual void Visit(NotNode node) {VisitChildren(node);}

        // Equality
        public virtual void Visit(EqualNode node) {VisitChildren(node);}
        public virtual void Visit(NotEqualNode node) {VisitChildren(node);}

        // Relational
        public virtual void Visit(GreaterThanNode node) {VisitChildren(node);}
        public virtual void Visit(GreaterEqualThanNode node) {VisitChildren(node);}
        public virtual void Visit(LessThanNode node) {VisitChildren(node);}
        public virtual void Visit(LessEqualThanNode node) {VisitChildren(node);}
    }
}