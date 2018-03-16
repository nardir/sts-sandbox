using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.JJ.Sandbox.Model
{
    public class PriceListRow
    {
        private IEnumerable<string> Members()
        {
            yield return PriceListRowKey + "";
            yield return ArticleKey + "";
            yield return ArticleName;
        }
        public override string ToString()
        {
            return string.Join(" ", Members());
        }

        public int PriceListRowKey { get; set; }
        public int ArticleKey { get; set; }
        public string ArticleName { get; set; }
        public decimal Price { get; set; }
        public int Stems { get; set; }
        public Party Party { get; set; }

        
    }
}
