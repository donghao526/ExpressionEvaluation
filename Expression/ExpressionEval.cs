using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace Expression
{
    class ExpressionEval
    {
        
        //private ArrayList expression;
        private int HIGH = 1;
        private int EQUAL = 0;
        private int LOW = -1;

        public ExpressionEval()
        {
            
        }

        private ArrayList parse(String expression)
        {
            string pattern = @"((?<=^|,|\[|\()([+|-])(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+)))|(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+))|\+|-|\*|\/|\[[\s\S]*\]|sin|cos|tan|arcsin|arcos|arctan|sinh|cosh|tanh|log|ln|pow|exp|fact|mod|sqrt|cuberoot|yroot|\^|\(|\)|avg|sum|var|varp|stdev|stdevp";       
            MatchCollection collection = Regex.Matches(expression, pattern);
            ArrayList postfixExpression = new ArrayList();

            Stack<string> S1 = new Stack<string>();
            Stack<string> S2 = new Stack<string>();
            string value;

            S1.Push("#");

            // 中缀表达式转后缀过程
            foreach(Match element in collection)
            {
                value = element.Value;
                if (this.isNumber(element.Value))
                    S2.Push(value);
                else
                {
                    if (value == "(")
                        S1.Push("(");
                    else if (value == ")")
                    {
                        while (S1.Peek() != "(")
                            S2.Push(S1.Pop());
                        S1.Pop();
                    }
                    else
                    {
                        if (priority(value, S1.Peek()) == this.HIGH)
                            S1.Push(value);
                        else
                        {
                            while(priority(S1.Peek(), value) != this.LOW)
                                S2.Push(S1.Pop());
                            S1.Push(value);
                        }
                    }

                }
            }
            while (S1.Peek() != "#")
                S2.Push(S1.Pop());
            
            while (S2.Count != 0)
                postfixExpression.Add(S2.Pop());
            postfixExpression.Reverse();
            return postfixExpression;
        }
        private bool isStatisticsFunction(string operation)
        {
            string pattern = @"avg|sum|var|varp|stdev|stdevp";
            if (Regex.IsMatch(operation, pattern))
                return true;
            else
                return false;
        }

        private bool isNumber(String element)
        {
            string pattern = @"(^([+|-])?(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+))$)|(^\[[\s\S]*\]$)";
            if(Regex.IsMatch(element,pattern))
                return true;
            else
                return false;
        }

        public double calculate(string expression)
        {
            
            ArrayList postfixEpression = new ArrayList();
            postfixEpression = this.parse(expression);

            for (int i = 0; i < postfixEpression.Count; i++)
            {
                Object obj = postfixEpression[i];
                
                // 如果该元素为运算符
                if(!this.isNumber(obj.ToString()))
                {
                    int operationNumber = this.getOperatorNumber(obj.ToString());
                    // 运算符需要两个操作数
                    if (operationNumber == 2)
                    {
                        double operator1 = double.Parse(postfixEpression[i - 2].ToString());
                        double operator2 = double.Parse(postfixEpression[i - 1].ToString());
                        double result = this.operationWithDoubleParameters(operator1, operator2, obj.ToString());
                        int backup = i;
                        postfixEpression.RemoveAt(backup);
                        postfixEpression.Insert(backup, result.ToString());
                        postfixEpression.RemoveAt(backup - 1);
                        postfixEpression.RemoveAt(backup - 2);
                        i = backup - 2;
                    }
                    // 运算符需要一个操作数
                    else
                    {
                        if (!isStatisticsFunction(obj.ToString()))
                        {
                            double operator1 = double.Parse(postfixEpression[i - 1].ToString());
                            double result = this.operationWithSingleParameter(operator1, obj.ToString());
                            int backup = i;
                            postfixEpression.RemoveAt(backup);
                            postfixEpression.Insert(backup, result.ToString());
                            postfixEpression.RemoveAt(backup - 1);
                            i = backup - 1;
                        }
                        else
                        {
                            string operator1 = postfixEpression[i - 1].ToString();
                            double result = this.evaluateStatisticFunction(operator1, obj.ToString());
                            int backup = i;
                            postfixEpression.RemoveAt(backup);
                            postfixEpression.Insert(backup, result.ToString());
                            postfixEpression.RemoveAt(backup - 1);
                            i = backup - 1;
                        }
                    }
                }
                else
                {
                    if (this.isArray(obj.ToString()))
                        postfixEpression[i] = this.handleArray(obj.ToString());
                }
            }
            return double.Parse(postfixEpression[0].ToString());
        }

        private double evaluateStatisticFunction(string operator1, string operation)
        {
            operator1 = operator1.Substring(1, operator1.Length - 2);
            string[] collection = operator1.Split(',');
            double result = 0;
            switch (operation)
            { 
                case "avg": return this.sumOfArray(collection) / collection.Length;
                case "sum": return this.sumOfArray(collection);
                case "varp": return this.squaresOfElements(collection, this.sumOfArray(collection) / collection.Length) / collection.Length;
                case "var": return this.squaresOfElements(collection, this.sumOfArray(collection) / collection.Length) / collection.Length - 1;
                case "stdev": return Math.Sqrt(this.squaresOfElements(collection, this.sumOfArray(collection) / collection.Length) / collection.Length - 1);
                case "stdevp": return Math.Sqrt(this.squaresOfElements(collection, this.sumOfArray(collection) / collection.Length) / collection.Length);
                default: return result;
            }
        }

        private double squaresOfElements(string[] array, double avgValue)
        {
            double result = 0;
            for(int i = 0; i < array.Length; i++)
            {
                double value = double.Parse(array[i]);
                result += (value - avgValue) * (value - avgValue);
            }
            return result;
        }

        private double sumOfArray(string[] array)
        {
            double result = 0;
            for (int i = 0; i < array.Length; i++)
            {
                result += double.Parse(array[i]);
            }
            return result;
        }

        private string handleArray(string arrayExpression)
        {
            arrayExpression = arrayExpression.Substring(1, arrayExpression.Length - 2);
            string pattern = @"([+|-])?(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+))|[^,]([\s\S]*\([\s\S]+,[\s\S]+\))|[^,]([\s\S]*\[[\s\S]+,[\s\S]+\])";
            MatchCollection collection = Regex.Matches(arrayExpression, pattern);

            string str = "[";
            foreach (Match match in collection)
            {
                if (!this.isNumber(match.Value))
                    str += this.calculate(match.Value).ToString()+",";
                else
                    str += match.Value+","; 
            }
 
            str = str.Substring(0, str.Length - 1);
            str += "]";
            return str;
        }

        /**
         * 获取与该运算符对应的操作数数目
         */
        private int getOperatorNumber(string operation)
        {
            switch(operation)
            {
                case "+":
                case "-":
                case "*":
                case "/": 
                case "mod":
                case "log":
                case "pow":
                case "yroot":
                case "^":return 2;
                default: return 1;
            }
        }

        /**
         * 判断操作数是否为数组
         */
        private bool isArray(string element)
        {
            string pattern = @"\[[\s\S]*\]";
            if (Regex.IsMatch(element, pattern))
                return true;
            else
                return false;
        }

        /**
         * 判定两个操作符的优先级 
         */
        private int priority(string operation1, string operation2)
        {
            int level1 = this.operationLevel(operation1);
            int level2 = this.operationLevel(operation2);
            if (level1 > level2) return this.HIGH;
            else if (level1 == level2) return this.EQUAL;
            else return this.LOW;
        }

        /**
         * 获取运算符的优先级别
         */
        private int operationLevel(String operation)
        {
            switch (operation)
            {
                case "(": return -1;
                case "#": return 0;
                case "+":
                case "-": return 1;
                case "*": 
                case "/": return 2;
                default: return 3;
            }
        }

        /**
         * 进行双目运算符的运算
         */
        private double operationWithSingleParameter(double singleOperator, string operation)
        { 
            switch(operation.ToLower())
            {
                case "sin": return Math.Sin(singleOperator);
                case "cos": return Math.Cos(singleOperator);
                case "tan": return Math.Tan(singleOperator);
                case "arcsin": return Math.Asin(singleOperator);
                case "arccos": return Math.Acos(singleOperator);
                case "arctan": return Math.Atan(singleOperator);
                case "sinh": return Math.Sinh(singleOperator);
                case "cosh": return Math.Cosh(singleOperator);
                case "tanh": return Math.Tanh(singleOperator);
                case "ln": return Math.Log(singleOperator);
                case "sqrt": return Math.Sqrt(singleOperator);
                case "exp": return Math.Exp(singleOperator);
                case "cuberoot": return Math.Pow(singleOperator,1/3);
                case "fact":  int result = 1; 
                              int num = Convert.ToInt32(singleOperator);
                              for (int i = 1; i <= num; i++)
                                  result *= i;
                              return result;
                default: return 0;
            }
        }

        /**
         * 进行单目运算符的操作
         */
        private double operationWithDoubleParameters(double operator1, double operator2, string operation)
        {
            switch(operation.ToLower())
            {
                case "+": return operator1 + operator2;
                case "-": return operator1 - operator2;
                case "*": return operator1 * operator2;
                case "/": return operator1 / operator2;
                case "mod": return operator1 % operator2;
                case "log": return Math.Log(operator1,operator2);
                case "^": return Math.Pow(operator1, operator2);
                case "pow": return Math.Pow(operator1, operator2);
                case "yroot": return Math.Pow(operator1, 1/operator2);
                default: return 0;
            }
        }

    }
}
