using System;
using System.Linq;

namespace code
{
    /// <summary>Бинарно изразно дърво с което се представят аритметични изрази.</summary>
    public class ExpressionTree
    {
        /// <summary>Корена на дървото.</summary>
        public IExpression Root { get; set; }


        /// <summary>Създава дърво от дадения низ който представлява израза.</summary>
        public static ExpressionTree Build(string expression)
        {
            return new ExpressionTree()
            {
                Root = BuildExpression(expression)
            };
        }



        /// <summary>Рекурсивно създава дървото, като разделя всеки кратък израз на отделен Node.</summary>
        private static IExpression BuildExpression(string expression)
        {
            // Съдържа подразбиращата стойност която по-нататък ще се промени.
            IExpression result = default;

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
                                Expression newExpression = new Expression();

                                // Рекурсивно извикваме същия метод, но с новия под-израз за
                                // лявата и дясна част, като по средата е аритметичния символ.
                                newExpression.Left = BuildExpression(expression.Substring(1, i++));
                                // Символа е от тип Char, но ние можем да го запишем като Double като
                                // стойноста му ще е ascii кода.
                                newExpression.Value = expression[i++];
                                newExpression.Right = BuildExpression(expression.Substring(i, expression.Length - i - 1));

                                result = newExpression;

                                break;
                            }
                        }
                    }
                }
                // Ако изразът не съдържа аритметични символи, товага е само число.
                else
                {
                    Constant newConstant = new Constant();
                    double value;

                    // Проверяваме дали всъщност е число.
                    try 
                    {
                        value = Convert.ToDouble(expression.Substring(1, expression.Length-2));
                    }
                    catch (FormatException)
                    {
                        throw new Exception($"Неуспешно преобразуване на {expression} в число!");
                    }

                    newConstant.Value = value;
                    result = newConstant;
                }
            }
            // Ако изразът не съдържа задължителните скоби, тогава неможем да продължим.
            else throw new Exception("Скобите са сложени неправилно!");

            return result;
        }
    
    
        /// <summary>Връща низ който представлява израза на дървото.</summary>
        public string GetExpression() 
        {
            string result = GetExpression(Root);

            // Почти винаги ще имаме скоби които ще ограждат всичко, но на нас не ни трябват.
            if (result[0] == '(') result = result.Substring(1, result.Length - 2);

            return result;
        } 


        // Рекурсивен метод с който структурираме един израз от всички под-изрази.
        private string GetExpression(IExpression exp)
        {
            string result; // Резултатът който връщаме.
            Type expType = exp.GetType(); // Типът на текущия израз който е от интерфейсът IExpression.

            // Ако е от тип Expression, тоест е аритметичен символ с ляво и дясно дете.
            if (expType == typeof(Expression))
            {
                Expression expression = exp as Expression;

                // Рекурсивно извикваме същия медот за двете деца.
                string left = GetExpression(expression.Left);
                string right = GetExpression(expression.Right);

                // Преобразуваме ascii кода обратно в символ.
                char symbol = (char)expression.Value;

                // Структурираме резултатът.
                result = $"({left}{symbol}{right})";
            }
            // Ако е от тип Constant, тоест е число и няма деца.
            else if (expType == typeof(Constant))
            {
                Constant constant = exp as Constant;

                // Резултатът ни е самото число.
                result = constant.Value.ToString();
            }
            // Ако типът е непознат.
            else throw new TypeInitializationException(expType.FullName, new Exception("Непознат тип."));

            return result;
        }



        /// <summary>Изчислява израза в дървото.</summary>
        public double Evaluate() => Evaluate(this.Root);


        // Рекурсивен метод с който в дълбочина пресмятаме всеки Node.
        private double Evaluate(IExpression expression)
        {
            double result; // Резултатът който връщаме.
            Type expType = expression.GetType(); // Типът на текущия израз който е от интерфейсът IExpression.

            // Ако е от тип Expression, тоест е аритметичен символ с ляво и дясно дете.
            if (expType == typeof(Expression))
            {
                // Чрез рекурсия извикваме същия медот за лявата и дясната част на израза.
                var exp = expression as Expression;
                double leftValue = Evaluate(exp.Left);
                double rightValue = Evaluate(exp.Right);

                // Преобразуваме стойноста от ascii код към символ.
                char symbol = (char)exp.Value;

                // Изчисляваме текущия израз.
                result = Calculate(leftValue, rightValue, symbol);
            }
            // Ако е от тип Constant, тоест е число и няма деца.
            else if (expType == typeof(Constant))
            {
                // Резултатът е число тоест от класа Constant.
                result = expression.Value;
            }
            // Ако типът е непознат.
            else throw new TypeInitializationException(expType.FullName, new Exception("Непознат тип."));
 
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
}