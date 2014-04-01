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
        
        private ArrayList expression;

        private int HIGH = 1;
        private int EQUAL = 0;
        private int LOW = -1;

        public ExpressionEval()
        {
            expression = new ArrayList();
        }

        public void parse(String expression)
        {
            string pattern = @"((?<=^|,|\[|\()([+|-])(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+)))|(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+))|\+|-|\*|\/|\[[\s\S]*\]|sin|cos|tan|arcsin|arcos|arctan|sinh|cosh|tanh|log|ln|pow|exp|fact|mod|sqrt|cuberoot|yroot|\^|\(|\)|avg|sum|var|varp|stdev|stdevp";       
            MatchCollection collection = Regex.Matches(expression, pattern);

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
                this.expression.Add(S2.Pop());
            this.expression.Reverse();           
        }

        public bool isNumber(String element)
        {
            string pattern = @"([+|-])?(((\d+)(\.\d*)+)|((\.)(\d)+)|(\d+))";
            if(Regex.IsMatch(element,pattern))
                return true;
            else
                return false;
        }

        public double calculate()
        {
            ArrayList expression = new ArrayList();
            expression = this.expression;

            for (int i = 0; i < expression.Count; i++)
            {
                Object obj = this.expression[i];
                if(!this.isNumber(obj.ToString()))
                {
                    int operationNumber = this.getOperatorNumber(obj.ToString());
                    // 运算符需要两个操作数
                    if (operationNumber == 2)
                    {
                        double operator1 = double.Parse(expression[i - 2].ToString());
                        double operator2 = double.Parse(expression[i - 1].ToString());
                        double result = this.operationWithDoubleParameters(operator1, operator2, obj.ToString());
                        int backup = i;
                        expression.RemoveAt(backup);
                        expression.Insert(backup, result.ToString());
                        expression.RemoveAt(backup - 1);
                        expression.RemoveAt(backup - 2);
                        i = backup - 2;
                    }
                    // 运算符需要一个操作数
                    else
                    {
                        double operator1 = double.Parse(expression[i - 1].ToString());
                        double result = this.operationWithSingleParameter(operator1,obj.ToString());
                        int backup = i;
                        expression.RemoveAt(backup);
                        expression.Insert(backup, result.ToString());
                        expression.RemoveAt(backup-1);
                        i = backup - 1;
                    }
                }
            }
            return double.Parse(expression[0].ToString());
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
                case "/": return 2;
                default: return 1;
            }
        }

        /**
         * 判断操作数是否为数组
         */
        public bool isArray(string element)
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
        public int priority(string operation1, string operation2)
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
        public int operationLevel(String operation)
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
        public double operationWithSingleParameter(double singleOperator, string operation)
        { 
            switch(operation)
            {
                case "sin": return Math.Sin(singleOperator);
                case "cos": return Math.Cos(singleOperator);
                case "tan": return Math.Tan(singleOperator);
                case "arcsin": return Math.Asin(singleOperator);
                case "arcos": return Math.Acos(singleOperator);
                case "artan": return Math.Atan(singleOperator);
                case "sinh": return Math.Sinh(singleOperator);
                case "cosh": return Math.Cosh(singleOperator);
                case "tanh": return Math.Tanh(singleOperator);
                case "ln": return Math.Log(singleOperator);
                default: return 0;
            }
        }

        /**
         * 进行单目运算符的操作
         */
        public double operationWithDoubleParameters(double operator1, double operator2, string operation)
        {
            switch(operation)
            {
                case "+": return operator1 + operator2;
                case "-": return operator1 - operator2;
                case "*": return operator1 * operator2;
                case "/": return operator1 / operator2;
                case "mod": return operator1 % operator2;
                case "^": return 0;
                default: return 0;
            }
        }

    }
}
