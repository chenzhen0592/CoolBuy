using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolBuy.Data;


namespace CoolBuy.Business
{
    public class Test
    {
        public string GetName()
        {
            return new TestData().GetName();
        }
    }
}
