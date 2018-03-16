using Axerrio.JJ.Sandbox.Model;

using System;
using System.Collections.Generic;
using System.Configuration;
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
            var useIdentityServer = bool.Parse(ConfigurationManager.AppSettings["UseIdentityServer"]??"false");

            Test1();
            Test2();
        }

        public static void Test2()
        {
            Pricelist pl = new Pricelist();
            var plrContext = pl.PricelistRows.AsQueryable();

            FilterCondition fc1 = new FilterCondition("ArticleName", "@filterProperty.Contains(@0)", "RED");
            FilterCondition fc2 = new FilterCondition("ArticleName", "@filterProperty.Contains(@0)", "PURPLE");
            FilterCondition fc3 = new FilterCondition("ArticleName", "@filterProperty.Contains(@0)", "BLUE");
            FilterCondition fc4 = new FilterCondition("Stems", "@filterProperty > @0", 10);
            FilterCondition fc5 = new FilterCondition("Stems", "@filterProperty > @0", 40);
            FilterCondition fc6 = new FilterCondition("Party.Color", "@filterProperty = @0", "RED");

            var expParser = new DynamicExpressionFilter<PriceListRow>(null);

            //FilterConditions
            var expName1 = expParser.ParseCondition(fc1);
            var expStems1 = expParser.ParseCondition(fc4);

            var expName2 = expParser.ParseCondition(fc2);
            var expStems2 = expParser.ParseCondition(fc5);

            var expColor = expParser.ParseCondition(fc6);

            //FilterRows (AND van meerdere Conditions)
            var expFilterRow1 = expParser.ParseExpressions(AndOr.AND, expName1, expStems1);
            var expFilterRow2 = expParser.ParseExpressions(AndOr.AND, expName2, expStems2);

            //Filter (OR van meerdere rows)
            var expFilter = expParser.ParseExpressions(AndOr.OR, expFilterRow1, expFilterRow2, expColor);


            var results = plrContext.Where(expFilter).ToDynamicList<PriceListRow>();
        }

        public static void Test1()
        {

            Pricelist pl = new Pricelist();
            var pricelistRowsContext = pl.PricelistRows.AsQueryable();



            var expCondition1 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "ArticleName.Contains(@0)", "RED");
            var expCondition2 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "ArticleName == @0", "ROSA Purple");
            var expCondition3 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "Stems > @0", 4);
            var expCondition4 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "Stems > @0", 40);

            var expFilterRow1 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "@0(it) AND @1(it)", expCondition1, expCondition3);
            var expFilterRow2 = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "@0(it) AND @1(it)", expCondition2, expCondition4);

            var expFilter = DynamicExpressionParser.ParseLambda(typeof(PriceListRow), typeof(bool), "@0(it) OR @1(it)", expFilterRow1, expFilterRow2);

            var results1 = pricelistRowsContext.Where(expFilter).ToDynamicList<PriceListRow>();
            var results2 = pricelistRowsContext.Where(expFilterRow1).ToDynamicList<PriceListRow>();

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

            //Console.ReadKey();

           
        }
    }
}
