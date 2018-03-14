using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.JJ.Sandbox.Model
{
    public class Customer
    {
        private IEnumerable<string> Members()
        {
            yield return CustomerKey + "";
            yield return Name;
        }
        public override string ToString()
        {
            return string.Join(" ", Members());
        }
        public int CustomerKey { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
