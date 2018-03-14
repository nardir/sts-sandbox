using Axerrio.JJ.Sandbox.Model;
using LinqKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace Axerrio.JJ.Sandbox
{
    public enum AndOr
    {
        AND,
        OR
    }
    public class DynamicExpressionFilter<TContext>
    //where TContext is een Entiteit waar op gefilterd mag worden...
    {
        private IQueryable<TContext> _context;
        public DynamicExpressionFilter(IQueryable<TContext> context)
        {
            _context = context;
        }

        private LambdaExpression ParseFilterLamda(string expression, params object[] values)        
        {
            return DynamicExpressionParser.ParseLambda(typeof(TContext), typeof(bool), expression, values);
        }


        public void TestFilterProperty(string filterProperty, string filterOperator, params object[] filterValues)
        {
            var expression = filterOperator.Replace("@filterProperty", filterProperty); //Check nog:  substitution values           

            LambdaExpression e = ParseFilterLamda(expression, filterValues);            
            

            var results = _context.Where(e).ToDynamicList<TContext>();
            foreach(var result in results)
                Console.WriteLine($"{result.ToString()}");
        }


        

        public void TestFilterRows(AndOr AndOrOperator, params FilterCondition[] conditions)
        {                     
            
            List<LambdaExpression> expressions = new List<LambdaExpression>();
            List<TContext> results = null;

            foreach (FilterCondition fc in conditions)
            {
                var expression = fc.Operator.Replace("@filterProperty", fc.PropertyName); //Check nog:  substitution values         
                LambdaExpression e = ParseFilterLamda(expression, fc.FilterValues);
                
                expressions.Add(e);
            }

            if (expressions.Any())
            {
                if (expressions.Count == 1)
                    results = _context.Where(expressions.First()).ToDynamicList<TContext>();

                else
                {
                    var whereExpression = BuildOperandString(expressions.Count, AndOrOperator.ToString()); //"or"
                    results = _context.Where(whereExpression, expressions.ToArray()).ToDynamicList<TContext>();
                }
            }


            foreach (var result in results)
                Console.WriteLine($"{result.ToString()}");
        }

        private string BuildOperandString(int count, string operand)
        {
            
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                if(i!=0)
                    builder.Append($" {operand} ");
                builder.Append($"@{i}(it)");                
            }

            return builder.ToString();
            
        }

        public void Test2()
        {

            


        }


    }

    
}


