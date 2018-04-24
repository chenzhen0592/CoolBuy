using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolBuy.Data
{
    public class DataAccessBase
    {
        protected IDatabaseConnect DataBase
        {
            get
            {
                return DataBus.GetDataBase();

            }
        }

    }
}
