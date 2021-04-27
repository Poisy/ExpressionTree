namespace code
{
    /// <summary>Представлява израз от три части {число-знак-число}.</summary>
    public class Expression : IExpression
    {
        /// <summary>Лявата част на израза.</summary>
        public IExpression Left { get; set; }


        /// <summary>Дясната част на израза.</summary>
        public IExpression Right { get; set; }


        /// <summary>Ascii кода на символа.</summary>
        public double Value { get; set; }
    }
}