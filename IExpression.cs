namespace code
{
    /// <summary>Позволява множество класове да споделят <see cref="IExpression.Value"/>.</summary>
    public interface IExpression
    {
        /// <summary>Съдържа стойноста на израза.</summary>
        double Value { get; set; }
    }
}