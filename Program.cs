namespace code
{
    class Program
    {
        static void Main(string[] args)
        {
            double x = 9;
            double y = 4;
            string expression = $"((((9)+({x}))*(5))/((({x})-({y}))^(3)))";

            ExpressionTree tree = ExpressionTree.Build(expression);
            System.Console.WriteLine(tree.Evaluate());
        }
    }
}
