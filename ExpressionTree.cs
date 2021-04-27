using System;
using System.Linq;

namespace code
{
    public class ExpressionTree : Node // Има всички качества на Node класа.
    {
        private ExpressionTree() {} // Не ни е нужно да правим обект от този клас, защото имаме метод за това.



        /// <summary>Създава дърво от дадения низ който представлява израза.</summary>
        public static ExpressionTree Build(string expression)
        {
            Node node = BuildNode(expression);

            ExpressionTree tree = new ExpressionTree()
            {
                Left = node.Left,
                Right = node.Right,
                Value = node.Value,
                Symbol = node.Symbol
            };

            return tree;
        }



        /// <summary>Рекурсивно създава дървото, като разделя всеки кратък израз на отделен Node.</summary>
        private static Node BuildNode(string expression)
        {
            Node newNode = new Node();

            // Проверяваме дали в израза има задължителните скоби с които ни показват началото и края на
            // отделните другите изрази в него.
            if ((expression.Contains('(') && expression.Contains(')')) && 
            (expression.Count(c => c == '(') == expression.Count(c => c == ')')))
            {

                // Проверяваме дали в израза има аритметични символи.
                // Ако има, това означава че трябва да го разделим на две части.
                if (expression.Any(x => ((new char[] {'+', '-', '*', '/', '^'}).Any(c => c == x))))
                {

                    // Цикъл чрез който проверяваме дали скобите са сложени правилно и ако са,
                    // да открива двата под-израза.
                    // op -> отварящи скоби, cp -> затварящи скоби
                    for (int i = 0, op = 0, cp = 0; i < expression.Length; i++)
                    {
                        if (expression[i] == '(')
                        {
                            op++;
                        }
                        else if (expression[i] == ')')
                        {
                            cp++;

                            // Ако имаме повече затварящи скоби отколкото отварящи.
                            if (cp > op) 
                            {
                                throw new Exception("Скобите са сложени неправилно!");
                            }
                            // Ако затварящите скоби са с една по-малко, това означава, че можем да
                            // разделим израза на два под-израза.
                            else if (cp == op - 1)
                            {
                                // Рекурсивно извикваме същия метод, но с новия под-израз за
                                // лявата и дясна част, като по средата е аритметичния символ.
                                newNode.Left = BuildNode(expression.Substring(1, i++));
                                newNode.Symbol = expression[i++];
                                newNode.Right = BuildNode(expression.Substring(i, expression.Length - i - 1));

                                // Прекратяваме цикъла, защото сме приключили с текущия израз.
                                break;
                            }
                        }
                    }
                }
                // Ако изразът не съдържа аритметични символи, товага е само число.
                else
                {
                    double num;

                    // Проверяваме дали всъщност е число.
                    try 
                    {
                        num = Convert.ToDouble(expression.Substring(1, expression.Length-2));
                    }
                    catch (FormatException)
                    {
                        throw new Exception($"Неуспешно преобразуване на {expression} в число!");
                    }

                    newNode.Value = num;
                }
            }
            // Ако изразът не съдържа задължителните скоби, тогава неможем да продължим.
            else throw new Exception("Скобите са сложени неправилно!");
             

            return newNode;
        }
    
    
        /// <summary>Изчислява израза в дървото.</summary>
        public double Evaluate() => Evaluate(this);


        // Рекурсивен метод с който в дълбочина пресмятаме всеки Node.
        private double Evaluate(Node node)
        {
            double result;

            // Проверяваме дали текущия израз има символ. 
            if (node.Symbol != default)
            {
                // Чрез рекурсия извикваме същия медот за лявата и дясната част на израза.
                double leftValue = Evaluate(node.Left);
                double rightValue = Evaluate(node.Right);

                // Изчисляваме текущия израз.
                result = Calculate(leftValue, rightValue, node.Symbol);
            }
            // Ако израза няма символ
            else
            {
                // Резултатът е само число.
                result = node.Value;
            }  
 
            return result;            
        }


        // Изчислява прост израз.
        private double Calculate(double n1, double n2, char symbol)
        {
            double result;

            // Проверяваме дали символът е валиден. Ако е, тогава изчисляваме.
            switch (symbol)
            {
                case '+':
                    result = n1 + n2;
                    break;
                case '-':
                    result = n1 - n2;
                    break;
                case '*':
                    result = n1 * n2;
                    break;
                case '/':
                    if (n2 != 0) result = n1 / n2;
                    else throw new DivideByZeroException($"Делене на нула при [{n1}/{n2}]!");
                    break;
                case '^':
                    result = Math.Pow(n1, n2);
                    break;
                // Ако символът е непознат, хвърляме грешка.
                default: throw new Exception($"Непознат аритметичен символ [{symbol}]!");
            }

            return result;
        }
    }

    public class Node
    {
        public double Value { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public char Symbol { get; set; }
    }
}