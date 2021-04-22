using System;
using ML4D.Compiler.Exceptions;

namespace ML4D.Compiler.ASTVisitors
{
    public class SymbolTableVisitor : ASTVisitor
    {
        private SymbolTable SymTable { get; set; }

        public SymbolTableVisitor(SymbolTable symTable)
        {
            SymTable = symTable;
        }

        // Declaration
        public override void Visit(VariableDCLNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                SymTable.Insert(node.ID, node.Type);
            else
                throw new VariableAlreadyDeclaredException(node, $"The variable \"{node.ID}\" could not be declared, as it has already been declared in the current or parent scope.");
            
            base.Visit(node);
        }
        
        public override void Visit(FunctionDCLNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                SymTable.Insert(node.ID, node.Type);
            else
                throw new FunctionAlreadyDeclaredException(node, $"The function \"{node.ID}\" has already been declared in the current or parent scope.");
            
            SymTable.OpenScope();
            base.Visit(node);
            SymTable.CloseScope();
        }

        public override void Visit(FunctionArgumentNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                SymTable.Insert(node.ID, node.Type);
            else
                throw new VariableAlreadyDeclaredException($"The variable \"{node.ID}\" has already been declared. And would hide the variable in the parent scope if declared inside the function.");
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

        // TODO Should check if it makes sense to return. I.e. it is inside a scope different from the global scope. 
        public override void Visit(ReturnNode node)
        {
            base.Visit(node);
        }


        public override void Visit(FunctionExprNode node)
        {
            if (!SymTable.Retrieve(node.ID))
                throw new FunctionNotDeclaredException(node, $"The function \"{node.ID}\" cannot be called, as it is not declared");

            base.Visit(node);
        }
        
        // Expression
        public override void Visit(IDNode node)
        {
            if (SymTable.Retrieve(node.Name))
                return;
            else 
                throw new VariableNotDeclaredException(node, $"The variable \"{node.Name}\" cannot be used, as it has not been declared.");
        }
    }
}