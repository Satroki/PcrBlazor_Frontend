using System;
using System.Collections.Generic;
using System.Text;

namespace PcrBlazor.Shared
{
    public class UserEquipStock : IUserId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public List<EquipmentStock> Stocks { get; set; }
    }

    public class EquipmentStock
    {
        public int EId { get; set; }
        public int Stock { get; set; }
        public int Ex { get; set; }
        public EquipmentStock()
        {

        }
        public EquipmentStock(int eid, int stock, int exStock = 0)
        {
            EId = eid;
            Stock = stock;
            Ex = exStock;
        }
    }
}
