using Axerrio.JJ.Sandbox.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Axerrio.JJ.Sandbox
{
    //https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions

    class Program
    {
        static void Main(string[] args)
        {

            Pricelist pl = new Pricelist();
            var pricelistRowsContext = pl.PricelistRows.AsQueryable();



            var exp1 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "ArticleName.Contains(@0)", "RED");
            var exp2 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "ArticleName == @0", "ROSA Purple");
            var exp3 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "Stems > @0", 4);

            IQueryable<PriceListRow> query1 = pricelistRowsContext.Where("@0(it) AND @1(it)", exp1, exp3);
            IQueryable<PriceListRow> query2 = pricelistRowsContext.Where("( @0(it) OR @1(it) ) and @2(it)", exp1, exp2, exp3);


            var results1 = query1.ToDynamicList<PriceListRow>();
            var results2 = query2.ToDynamicList<PriceListRow>();

            //Operator Metadata opslaan: Expression 
            //bv:--> '@propertyName.Contains(@0)'
            //   --> '@propertyName > @0'

            var dePLR = new DynamicExpressionFilter<PriceListRow>(pricelistRowsContext);

            var propertyName = "ArticleName";            
            var values = new object[] { "ROSA MAXIMA" };

            Console.WriteLine("TestFilterProperty");

            var operatorSyntax = "@filterProperty == @0";
            dePLR.TestFilterProperty(propertyName, operatorSyntax, values.ToArray());

           // Console.ReadKey();
            Console.WriteLine("Test Related Entity");

            var dePL = new DynamicExpressionFilter<Pricelist>(new List<Pricelist>() { pl }.AsQueryable());
            propertyName = "Customer.Name";

            values = new object[] { "Jos the boss BV" };
            operatorSyntax = "@filterProperty == @0";
            dePL.TestFilterProperty(propertyName, operatorSyntax, values.ToArray());

            //Console.ReadKey();
            propertyName = "Customer.Name";
            values = new object[] { "Jos" };
            operatorSyntax = "@filterProperty.Contains(@0)";
            dePL.TestFilterProperty(propertyName, operatorSyntax, values.ToArray());

            Console.WriteLine("Test OR ");

            FilterCondition f1 = new FilterCondition("ArticleName", "@filterProperty.Contains(@0)", "RED");
            FilterCondition f2 = new FilterCondition("ArticleName", "@filterProperty.Contains(@0)", "PURPLE");
            FilterCondition f3 = new FilterCondition("ArticleName", "@filterProperty.Contains(@0)", "BLUE");

            dePLR.TestFilterRows(AndOr.OR, f1, f2, f3);

            Console.ReadKey();

           
        }
    }
}
