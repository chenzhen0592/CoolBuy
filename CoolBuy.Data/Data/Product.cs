using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace CoolBuy.Data.Data
{
    public class Product:DataAccessBase
    {
        public int GetProduct(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@Id", id);
            return DataBase.ExecuteSingle<int>("GetProduct", param);

        }
    }
}
