using System;
using ML4D.Compiler.Exceptions;

namespace ML4D.Compiler.ASTVisitors
{
    public class SymbolTableVisitor2 : ASTVisitor2
    {
        private SymbolTable SymTable { get; set; }

        public SymbolTableVisitor2(SymbolTable symTable)
        {
            SymTable = symTable;
        }

        // Declaration
        public override void Visit(VariableDCLNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                SymTable.Insert(node.ID, node.Type);
            else
                throw new VariableAlreadyDeclaredException(node, "The variable has already been declared.");
            
            base.Visit(node);
        }
        
        public override void Visit(FunctionDCLNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                SymTable.Insert(node.ID, node.Type);
            else
                throw new FunctionAlreadyDeclaredException(node, "The variable has already been declared.");
            
            base.Visit(node);
        }

        // Statement      
        public override void Visit(AssignNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                throw new VariableNotDeclaredException($"The variable \"{node.ID}\" cannot be assigned, as it has not been declared.");
            
            base.Visit(node);
        }
        
        public override void Visit(WhileNode node)
        {
            SymTable.OpenScope();
            
            base.Visit(node);
            
            SymTable.CloseScope();
        }
        
        public override void Visit(FunctionExprNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                throw new FunctionNotDeclaredException(node, "The function cannot be called, as it is not declared");

            base.Visit(node);
        }
        
        // Expression
        public override void Visit(IDNode node)
        {
            if (SymTable.Retrieve(node.Name))
                return;
            
            throw new VariableNotDeclaredException(node, "The variable has not been declared before use.");
        }
    }
}