using System;
using System.Globalization;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace ML4D.Compiler
{
	public class ASTBuilder : dinoBaseVisitor<Node>
	{
		
		public override LinesNode VisitLines(dinoParser.LinesContext context)
		{
			LinesNode linesNode = new LinesNode();

			try
			{
				foreach (IParseTree child in context.children)
				{
					Node? g = Visit(child);
					
					if (g is not null)
						linesNode.lines.Add(g);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
			return linesNode;
		}
		
		// Declarations
		public override Node VisitDeclVar(dinoParser.DeclVarContext context) // TODO add void probably.
		{
			VariableDCLNode varDeclNode;

			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token. 
			{
				case dinoLexer.INT:
					varDeclNode = new VariableDCLNode("int", context.id.Text);
					if (context.right is not null) 
						varDeclNode.Init = (ExpressionNode)  Visit(context.right);
					break;
				case dinoLexer.DOUBLE:
					varDeclNode = new VariableDCLNode("double", context.id.Text);
					if (context.right is not null) 
						varDeclNode.Init = (ExpressionNode)  Visit(context.right);
					break;
				case dinoLexer.BOOL:
					varDeclNode = new VariableDCLNode("bool", context.id.Text);
					if (context.right is not null) 
						varDeclNode.Init = (ExpressionNode)  Visit(context.right);
					break;
				default:
					throw new NotSupportedException();
			}
			return varDeclNode;
		}
		
		// Statements
		public override Node VisitAssignStmt(dinoParser.AssignStmtContext context)
		{
			return base.VisitAssignStmt(context);
		}

		public override Node VisitWhileStmt(dinoParser.WhileStmtContext context)
		{
			return base.VisitWhileStmt(context);
		}

		public override Node VisitBackwardStmt(dinoParser.BackwardStmtContext context)
		{
			return base.VisitBackwardStmt(context);
		}

		public override Node VisitReturnStmt(dinoParser.ReturnStmtContext context)
		{
			return base.VisitReturnStmt(context);
		}


		// Expressions
		public override ExpressionNode VisitInfixBoolExpr(dinoParser.InfixBoolExprContext context)
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

		public override ExpressionNode VisitInfixExpr(dinoParser.InfixExprContext context)
		{
			InfixExpressionNode node;
			
			switch (context.op.Type)
			{
				// Arithmetic
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
				
				// Relational
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
				
				// Equality
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

		public override ExpressionNode VisitUnaryExpr(dinoParser.UnaryExprContext context)
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
		
		// Types
		public override ExpressionNode VisitTypeExpr(dinoParser.TypeExprContext context)
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
		
		
		
		
	}
}


