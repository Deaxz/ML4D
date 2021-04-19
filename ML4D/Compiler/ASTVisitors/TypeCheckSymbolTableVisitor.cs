using System;
using ML4D.Compiler.Exceptions;

namespace ML4D.Compiler.ASTVisitors
{
    public class TypeCheckSymbolTableVisitor : ASTVisitor
    {
        private SymbolTable SymTable { get; }

        public TypeCheckSymbolTableVisitor(SymbolTable symTable)
        {
            SymTable = symTable;
        }

        // Declaration
        public override void Visit(VariableDCLNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                SymTable.Insert(node.ID, node.Type);
            else
                throw new VariableAlreadyDeclaredException(node, $"The variable \"{node.ID}\" could not be declared, as it has already been declared in the current or parent scope.");
            
            base.Visit(node);
            if (node.Type != node.Init.Type)
                throw new VariableInitialisationException(node, "Failed to initialise in declaration. The expression has an incorrect type.");
        }
        
        public override void Visit(FunctionDCLNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                SymTable.Insert(node.ID, node.Type);
            else
                throw new FunctionAlreadyDeclaredException(node, $"The function \"{node.ID}\" has already been declared in the current or parent scope.");
            
            SymTable.OpenScope();
            base.Visit(node);
            // Typechecking
            SymTable.CloseScope();
        }

        public override void Visit(FunctionArgumentNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                SymTable.Insert(node.ID, node.Type);
            else
                throw new VariableAlreadyDeclaredException($"The variable \"{node.ID}\" has already been declared. And would hide the variable in the parent scope if declared inside the function.");
        }
        
        // Statement      
        public override void Visit(AssignNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                throw new VariableNotDeclaredException($"The variable \"{node.ID}\" cannot be assigned, as it has not been declared.");
            
            base.Visit(node);

            node.Type = SymTable.Retrieve(node.ID).Type;
            if (node.Type != node.Right.Type)
                throw new VariableAssignmentException(node, "Failed to assign variable. The expression has an incorrect type.");
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
            if (SymTable.Retrieve(node.ID) is null)
                throw new FunctionNotDeclaredException(node, $"The function \"{node.ID}\" cannot be called, as it is not declared");

            base.Visit(node);
            // Typechecking functionexpr should have correct return type for call site, but also correct parameter type and parameter number.
        }
        
        // Expression
        public override void Visit(IDNode node)
        {
            Symbol variable = SymTable.Retrieve(node.ID);
            if (variable is null)
                throw new VariableNotDeclaredException(node, $"The variable \"{node.ID}\" cannot be used, as it has not been declared.");

            node.Type = variable.Type;
        }
        
        
        
        
        public override void Visit(InfixExpressionNode node)
        {
            switch (node)
            {
                // Arithmetic
                case AdditionNode:
                case SubtractionNode:
                case MultiplicationNode:
                case DivisionNode:
                case PowerNode:
                    
                    VisitChildren(node);
                    
                    // TODO fix, den håndtere ikke mixed som ikke giver mening. 
                    if (node.Left.Type == "double" && node.Right.Type == "double")
                        node.Type = "double";
                    else if (node.Left.Type == "int" && node.Right.Type == "int")
                        node.Type = "int";
                    else
                    {
                        
                    }
                        
                    break;
                    
                // Relational
                case LessThanNode:
                case LessEqualThanNode:
                case GreaterThanNode:
                case GreaterEqualThanNode:
                
                // Equality
                case EqualNode:
                case NotEqualNode:

                    // Typechecking for value operators
                    VisitChildren(node);

                    if (node.Left.Type == "double" || node.Left.Type == "int" && node.Right.Type == "double" ||
                        node.Right.Type == "int")
                    {
                        node.Type = "bool";
                    }
                    else
                    {
                        //throw new InvalidValueOperatorException(node, "Expected operands to be int/double");
                    }
                    break;
                    
                // Boolean
                case AndNode:
                case OrNode:

                    // Typechecking for boolean operators
                    VisitChildren(node);
                    if (node.Left.Type == "bool" && node.Right.Type == "bool")
                    {
                        node.Type = "bool";
                    }
                    else
                    {
                        //throw new InvalidValueOperatorException(node, "Expected operands to be bool");
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        
        public override void Visit(UnaryExpressionNode node)
        {
            switch (node)
            {
                case NotNode:
                    // Typechecking for not operator
                    
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}