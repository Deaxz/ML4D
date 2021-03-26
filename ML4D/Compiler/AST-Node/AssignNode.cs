internal class AssignNode
{
    public string ID { get; set; } // Retrieved from symbol table, if non-existent, symbol table issues error.
    public string type { get; set;}

    public ExprNode exprNode { get; set; }
}