namespace code
{
    /// <summary>Израз който е просто число.</summary>
    public class Constant : IExpression
    {
        /// <summary>Стойноста на числото.</summary>
        public double Value { get; set; }
    }
}