using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.JJ.Sandbox.Model
{
    public class Pricelist
    {
        private IEnumerable<string> Members()
        {
            yield return PricelistKey + " ";
            yield return Name;
        }

        public override string ToString()
        {
            return string.Join(" ", Members());
        }

        public Pricelist()
        {
            PricelistKey = 1;
            Name = "Webshop prijslijst";
            Created = new DateTime(2018, 3, 1, 12, 2, 13);

            Customer = new Customer()
            {
                Country = "Nederland",
                Name = "Jos the boss BV",
                CustomerKey = 1
            };

            PricelistRows = new List<PriceListRow>()
            {
                new PriceListRow()
                {
                    ArticleKey = 1,
                    ArticleName = "ROSA RED",
                    Price = 0.12M,
                    PriceListRowKey = 1,
                    Stems = 40
                },
                new PriceListRow()
                {
                    ArticleKey = 2,
                    ArticleName = "ROSA MAXIMA",
                    Price = 0.24M,
                    PriceListRowKey = 2,
                    Stems = 120
                }
                ,
                new PriceListRow()
                {
                    ArticleKey = 2,
                    ArticleName = "ROSA Purple",
                    Price = 0.24M,
                    PriceListRowKey = 2,
                    Stems = 5
                }
            };

        }

        public int PricelistKey { get; set; }
        public string Name { get; set; }

        public DateTime Created { get; set; }
        public List<PriceListRow> PricelistRows { get; set; }

        public Customer Customer { get; set; }
    }
}
