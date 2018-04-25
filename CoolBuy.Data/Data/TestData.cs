using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace CoolBuy.Data
{
    public class TestData:DataAccessBase
    {
        public string GetName()
        {
            DynamicParameters param = new DynamicParameters();
            
            return DataBase.ExecuteSingle<string>("select name from test", param);

        }
    }
}
