#nullable enable
using System;
using System.Globalization;
using Antlr4.Runtime.Tree;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler
{
	public class ASTBuilder : ML4DBaseVisitor<Node>
	{
		public override LinesNode VisitLines(ML4DParser.LinesContext context)
		{
			LinesNode linesNode = new LinesNode();

			foreach (IParseTree child in context.children)
			{
				Node? node = Visit(child);
				if (node is not null) // Necessary because ';' returns null. 
					linesNode.lines.Add(node);
			}
			return linesNode;
		}
		
		// Declarations
		public override Node VisitVarDecl(ML4DParser.VarDeclContext context)
		{
			VariableDCLNode varDeclNode;

			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token. 
			{
				case ML4DLexer.INT:
					varDeclNode = new VariableDCLNode("int", context.id.Text, (ExpressionNode) Visit(context.right));
					break;
				case ML4DLexer.DOUBLE:
					varDeclNode = new VariableDCLNode("double", context.id.Text, (ExpressionNode) Visit(context.right));
					break;
				case ML4DLexer.BOOL:
					varDeclNode = new VariableDCLNode("bool", context.id.Text, (ExpressionNode) Visit(context.right));
					break;
				default:
					throw new NotSupportedException(
						$"The variable {context.id.Text}, was declared with an illegal type.");
			}
			return varDeclNode;
		}

		public override Node VisitFuncDecl(ML4DParser.FuncDeclContext context)
		{
			FunctionDCLNode functionDclNode;
			
			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token. 
			{
				case ML4DLexer.INT:
					functionDclNode = new FunctionDCLNode("int", context.id.Text);
					break;
				case ML4DLexer.DOUBLE:
					functionDclNode = new FunctionDCLNode("double", context.id.Text);
					break;
				case ML4DLexer.BOOL:
					functionDclNode = new FunctionDCLNode("bool", context.id.Text);
					break;
				case ML4DLexer.VOID:
					functionDclNode = new FunctionDCLNode("void", context.id.Text);
					break;
				default:
					throw new NotSupportedException(
						$"The function {context.id.Text}, was declared with an illegal type.");
			}
			
			for (int i = 0; i < context._argid.Count; i++)
				functionDclNode.Arguments.Add(new FunctionArgumentNode(
					context._argtype[i].type.Text, context._argid[i].Text));
			
			functionDclNode.Body = VisitLines(context.body);
			return functionDclNode;
		}

		// Statements
		public override Node VisitAssignStmt(ML4DParser.AssignStmtContext context)
		{
			AssignNode assignNode = new AssignNode(context.id.Text, (ExpressionNode) Visit(context.right));
			return assignNode;
		}

		public override Node VisitWhileStmt(ML4DParser.WhileStmtContext context)
		{
			WhileNode whileNode = new WhileNode(
				(ExpressionNode) Visit(context.predicate), (LinesNode) Visit(context.body));
			return whileNode;
		}

		public override Node VisitBackwardStmt(ML4DParser.BackwardStmtContext context) // TODO slet
		{
			BackwardNode backwardNode = new BackwardNode(context.id.Text);
			return backwardNode;
		}

		public override Node VisitReturnStmt(ML4DParser.ReturnStmtContext context)
		{
			ReturnNode returnNode;
			if (context.inner is not null)
				returnNode = new ReturnNode((ExpressionNode) Visit(context.inner));
			else
				returnNode = new ReturnNode();
			return returnNode;
		}

		public override Node VisitFuncStmt(ML4DParser.FuncStmtContext context)
		{
			FunctionExprNode functionExprNode = new(context.id.Text);

			foreach (ML4DParser.Bool_exprContext argument in context._argexpr)
				functionExprNode.Arguments.Add(Visit(argument));
			
			return functionExprNode;
		}
		
		// Expressions
		public override Node VisitInfixRelationalExpr(ML4DParser.InfixRelationalExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				case ML4DLexer.LTHAN:
					node = new LessThanNode();
					break;
				case ML4DLexer.GTHAN:
					node = new GreaterThanNode();
					break;
				case ML4DLexer.LETHAN:
					node = new LessEqualThanNode();
					break;
				case ML4DLexer.GETHAN:
					node = new GreaterEqualThanNode();
					break;
				case ML4DLexer.EQUALS:
					node = new EqualNode();
					break;
				case ML4DLexer.NOTEQUALS:
					node = new NotEqualNode();
					break;
				default:
					throw new NotSupportedException();
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}
		
		public override Node VisitInfixBoolExpr(ML4DParser.InfixBoolExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				case ML4DLexer.AND:
					node = new AndNode();
					break;
				case ML4DLexer.OR:
					node = new OrNode();
					break;
				default:
					throw new NotSupportedException();
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitParensExpr(ML4DParser.ParensExprContext context)
		{
			ExpressionNode node = (ExpressionNode) Visit(context.inner);
			node.Parenthesized = true;
			return node;
		}
		
		public override Node VisitInfixValueExpr(ML4DParser.InfixValueExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				case ML4DLexer.PLUS:
					node = new AdditionNode();
					break;
				case ML4DLexer.MINUS:
					node = new SubtractionNode();
					break;
				case ML4DLexer.MUL:
					node = new MultiplicationNode();
					break;
				case ML4DLexer.DIV:
					node = new DivisionNode();
					break;
				case ML4DLexer.POW:
					node = new PowerNode();
					break;
				default:
					throw new NotSupportedException(); // TODO overvej i cleanup at slette alle notsupportedexceptions, da de er umulige at nå. Men måske er de fine ift. udvidelser
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitUnaryExpr(ML4DParser.UnaryExprContext context)
		{
			UnaryExpressionNode node;
			
			switch (context.op.Type)
			{
				case ML4DLexer.NOT:
					node = new NotNode();
					break;
				default:
					throw new NotSupportedException();
			}
			node.Inner = (ExpressionNode) Visit(context.inner);
			return node;
		}

		public override Node VisitFuncExpr(ML4DParser.FuncExprContext context) 
		{
			FunctionExprNode functionExprNode = new FunctionExprNode(context.id.Text);
			
			foreach (ML4DParser.Bool_exprContext argument in context._argexpr)
				functionExprNode.Arguments.Add((ExpressionNode) Visit(argument));
			
			return functionExprNode;
		}

		// Types
		public override Node VisitTypeExpr(ML4DParser.TypeExprContext context)
		{
			ExpressionNode node;
			
			switch (context.value.Type)
			{
				case ML4DLexer.INUM:
					node = new IntNode(int.Parse(context.value.Text));
					break;
				case ML4DLexer.FNUM:
					node = new DoubleNode(double.Parse(context.value.Text, CultureInfo.InvariantCulture));
					break;
				case ML4DLexer.BOOLVAL:
					node = new BoolNode(bool.Parse(context.value.Text));
					break;
				case ML4DLexer.ID:
					node = new IDNode(context.value.Text);
					break;
				default:
					throw new NotSupportedException();
			}
			return node;
		}
	}
}