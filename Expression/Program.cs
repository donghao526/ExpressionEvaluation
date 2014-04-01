using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Expression
{
    class Program
    {
        static void Main(string[] args)
        {
            //string expression = "(2/3+6*sin(2)-5)*3";
            string expression = Console.ReadLine();
            ExpressionEval exprEval = new ExpressionEval();
            exprEval.parse(expression);
            double result = exprEval.calculate();
            Console.WriteLine(result);
        }
    }
}
