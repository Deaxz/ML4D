using ML4D.Compiler.Nodes;

namespace ML4D.Compiler
{
    public abstract class ASTVisitor
    {
        public void Visit(Node node)
        {
            Visit((dynamic)node);
        }
        public void VisitChildren(Node node)
        {
            foreach (Node child in node.GetChildren())
                Visit(child);
        }

        // Start Node
        public virtual void Visit(LinesNode node) {VisitChildren(node);}
        
        //-----Declaration-----
        public virtual void Visit(VariableDCLNode node) {VisitChildren(node);}
        public virtual void Visit(FunctionDCLNode node) {VisitChildren(node);}
        public virtual void Visit(FunctionArgumentNode node) {VisitChildren(node);}
        
        //-----Statement-----
        public virtual void Visit(AssignNode node) {VisitChildren(node);}
        public virtual void Visit(WhileNode node) {VisitChildren(node);}
        public virtual void Visit(BackwardNode node) {} // TODO slet
        public virtual void Visit(ReturnNode node) {VisitChildren(node);}
        public virtual void Visit(FunctionStmtNode node) {VisitChildren(node);}
        
        //-----Expression-----
        public virtual void Visit(FunctionExprNode node) {VisitChildren(node);}
        
        // Value
        public virtual void Visit(IDNode node) {}
        public virtual void Visit(DoubleNode node) {}
        public virtual void Visit(IntNode node) {}
        public virtual void Visit(BoolNode node) {}
        
        // Arithmetic
        public virtual void Visit(InfixExpressionNode node) {VisitChildren(node);}
        public virtual void Visit(UnaryExpressionNode node) {VisitChildren(node);}
    }
}