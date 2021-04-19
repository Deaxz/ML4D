using System;
using System.Globalization;
using Antlr4.Runtime.Tree;

namespace ML4D.Compiler
{
	public class ASTBuilder : dinoBaseVisitor<Node>
	{
		public override LinesNode VisitLines(dinoParser.LinesContext context)
		{
			LinesNode linesNode = new LinesNode();
			Node node;
			
			foreach (IParseTree child in context.children)
			{
				node = Visit(child);
				if (node is not null) // Necessary because ';' returns null. 
					linesNode.lines.Add(node);
			}
			return linesNode;
		}
		
		// Declarations
		public override Node VisitVarDecl(dinoParser.VarDeclContext context) // TODO add void probably.
		{
			VariableDCLNode varDeclNode;

			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token. 
			{
				case dinoLexer.INT:
					varDeclNode = new VariableDCLNode("int", context.id.Text);
					break;
				case dinoLexer.DOUBLE:
					varDeclNode = new VariableDCLNode("double", context.id.Text);
					break;
				case dinoLexer.BOOL:
					varDeclNode = new VariableDCLNode("bool", context.id.Text);
					break;
				default:
					throw new NotSupportedException($"The variable {context.id.Text}, was declared with an illegal type.");
			}
			if (context.right is not null) // Declaration with initialisation
				varDeclNode.Init = (ExpressionNode)  Visit(context.right);
			
			return varDeclNode;
		}

		public override Node VisitFuncDecl(dinoParser.FuncDeclContext context)
		{
			FunctionDCLNode functionDclNode;
			
			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token. 
			{
				case dinoLexer.INT:
					functionDclNode = new FunctionDCLNode("int", context.id.Text);
					break;
				case dinoLexer.DOUBLE:
					functionDclNode = new FunctionDCLNode("double", context.id.Text);
					break;
				case dinoLexer.BOOL:
					functionDclNode = new FunctionDCLNode("bool", context.id.Text);
					break;
				case dinoLexer.VOID:
					functionDclNode = new FunctionDCLNode("void", context.id.Text);
					break;
				default:
					throw new NotSupportedException();
			}
			
			if (context._argtype.Count != context._argid.Count) // TODO overvej slet, det er jo egentligt defineret i grammar
				throw new ArgumentException("ArgID and ArgType do not contain the same amount of elements.");

			for (int i = 0; i < context._argid.Count; i++)
			{
				FunctionArgumentNode argumentNode = new FunctionArgumentNode(context._argtype[i].type.Text, context._argid[i].Text);
				functionDclNode.Arguments.Add(argumentNode);
			}
			functionDclNode.Body = VisitLines(context.body);
			
			return functionDclNode;
		}

		// Statements
		public override Node VisitAssignStmt(dinoParser.AssignStmtContext context)
		{
			AssignNode assignNode = new AssignNode(context.id.Text, (ExpressionNode) Visit(context.right));
			return assignNode;
		}

		public override Node VisitWhileStmt(dinoParser.WhileStmtContext context)
		{
			WhileNode whileNode = new WhileNode((ExpressionNode) Visit(context.predicate), (LinesNode) Visit(context.body));
			return whileNode;
		}

		public override Node VisitBackwardStmt(dinoParser.BackwardStmtContext context) // TODO slet
		{
			BackwardNode backwardNode = new BackwardNode(context.id.Text);
			return backwardNode;
		}

		public override Node VisitReturnStmt(dinoParser.ReturnStmtContext context) // TODO check efter null inner node
		{
			ReturnNode returnNode;
			if (context.inner is not null)
				returnNode = new ReturnNode((ExpressionNode) Visit(context.inner));
			else
				returnNode = new ReturnNode();
			return returnNode;
		}

		public override Node VisitFuncStmt(dinoParser.FuncStmtContext context)
		{
			FunctionExprNode functionExprNode = new FunctionExprNode(context.id.Text);

			foreach (dinoParser.Bool_exprContext argument in context._argexpr)
				functionExprNode.Arguments.Add((ExpressionNode) Visit(argument));
			
			return functionExprNode;
		}
		
		// Expressions
		public override Node VisitInfixRelationalExpr(dinoParser.InfixRelationalExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				case dinoLexer.LTHAN:
					node = new LessThanNode();
					break;
				case dinoLexer.GTHAN:
					node = new GreaterThanNode();
					break;
				case dinoLexer.LETHAN:
					node = new LessEqualThanNode();
					break;
				case dinoLexer.GETHAN:
					node = new GreaterEqualThanNode();
					break;
				case dinoLexer.EQUALS:
					node = new EqualNode();
					break;
				case dinoLexer.NOTEQUALS:
					node = new NotEqualNode();
					break;
				default:
					throw new NotSupportedException();
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}


		public override Node VisitInfixBoolExpr(dinoParser.InfixBoolExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				case dinoLexer.AND:
					node = new AndNode();
					break;
				case dinoLexer.OR:
					node = new OrNode();
					break;
				default:
					throw new NotSupportedException();
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitInfixExpr(dinoParser.InfixExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				case dinoLexer.PLUS:
					node = new AdditionNode();
					break;
				case dinoLexer.MINUS:
					node = new SubtractionNode();
					break;
				case dinoLexer.MUL:
					node = new MultiplicationNode();
					break;
				case dinoLexer.DIV:
					node = new DivisionNode();
					break;
				case dinoLexer.POW:
					node = new PowerNode();
					break;
				default:
					throw new NotSupportedException(); // TODO overvej i cleanup at slette alle notsupportedexceptions, da de er umulige at nå. Men måske er de fine ift. udvidelser
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitUnaryExpr(dinoParser.UnaryExprContext context)
		{
			UnaryExpressionNode node;
			
			switch (context.op.Type)
			{
				case dinoLexer.NOT:
					node = new NotNode();
					break;
				default:
					throw new NotSupportedException();
			}
			node.Inner = (ExpressionNode) Visit(context.inner);
			return node;
		}

		public override Node VisitFuncExpr(dinoParser.FuncExprContext context) // TODO over at lave liste af Node, så .GetChildren() bliver simpler. 
		{
			FunctionExprNode functionExprNode = new FunctionExprNode(context.id.Text);

			foreach (dinoParser.Bool_exprContext argument in context._argexpr)
			{
				functionExprNode.Arguments.Add((ExpressionNode) Visit(argument));
			}
			return functionExprNode;
		}

		// Types
		public override Node VisitTypeExpr(dinoParser.TypeExprContext context)
		{
			ExpressionNode node;
			
			switch (context.value.Type)
			{
				case dinoLexer.INUM:
					node = new IntNode(int.Parse(context.value.Text));
					break;
				case dinoLexer.FNUM:
					node = new DoubleNode(double.Parse(context.value.Text, CultureInfo.InvariantCulture));
					break;
				case dinoLexer.BOOLVAL:
					node = new BoolNode(bool.Parse(context.value.Text));
					break;
				case dinoLexer.ID:
					node = new IDNode(context.value.Text);
					break;
				case dinoLexer.VOID: // TODO Vær opmærksom, men jeg forventer det er noget vi skal fange i type checking.
					node = new VoidNode();
					break;
				default:
					throw new NotSupportedException();
			}
			return node;
		}

		// Fixes error - "The call is ambiguous between the following methods or properties: 'ML4D.Compiler.ASTVisitor<string>.Visit(ML4D.Compiler.LessThanNode)' and 'ML4D.Compiler.ASTVisitor<string>.Visit(ML4D.Compiler.LessEqualThanNode)'"
		// Not sure why tho, but keep it.
		public override Node VisitParensExpr(dinoParser.ParensExprContext context)
		{
			return Visit(context.bool_expr());
		}
	}
}