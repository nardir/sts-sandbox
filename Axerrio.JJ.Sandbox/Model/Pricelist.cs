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
                    Stems = 40,
                    Party = new Party()
                    {
                        PartyKey = 2,
                        Color = "RED"
                    }
                },
                new PriceListRow()
                {
                    ArticleKey = 2,
                    ArticleName = "ROSA MAXIMA",
                    Price = 0.24M,
                    PriceListRowKey = 2,
                    Stems = 120,
                    Party = new Party()
                    {
                        PartyKey = 2,
                        Color = "PURPLE"
                    }
                }
                ,
                new PriceListRow()
                {
                    ArticleKey = 3,
                    ArticleName = "ROSA Purple",
                    Price = 0.24M,
                    PriceListRowKey = 3,
                    Stems = 5,
                    Party = new Party()
                    {
                        PartyKey = 2,
                        Color = "BLUE"
                    }
                },
                new PriceListRow()
                {
                    ArticleKey = 3,
                    ArticleName = "ROSA Purple",
                    Price = 1.24M,
                    PriceListRowKey = 4,
                    Stems = 150,
                    Party = new Party()
                    {
                        PartyKey = 1,
                        Color = "RED"
                    }
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
