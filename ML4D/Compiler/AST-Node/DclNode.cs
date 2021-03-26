internal class DCLNode
{
    public string ID { get; set; }
    public string type { get; set;}

    public ExprNode init; // if init is null then it is declared without initialisation.
}