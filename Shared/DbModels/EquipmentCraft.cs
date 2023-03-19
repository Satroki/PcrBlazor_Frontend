using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace PcrBlazor.Shared
{
    public class EquipmentCraft : IEquipmentId
    {
        public static Dictionary<string, PropertyInfo> PropDict = typeof(EquipmentCraft).GetProperties().ToDictionary(p => p.Name);

        [Key]
        public int EquipmentId { get; set; }
        public int CraftedCost { get; set; }
        public int ConditionEquipmentId_1 { get; set; }
        public int ConsumeNum_1 { get; set; }
        public int ConditionEquipmentId_2 { get; set; }
        public int ConsumeNum_2 { get; set; }
        public int ConditionEquipmentId_3 { get; set; }
        public int ConsumeNum_3 { get; set; }
        public int ConditionEquipmentId_4 { get; set; }
        public int ConsumeNum_4 { get; set; }
        public int ConditionEquipmentId_5 { get; set; }
        public int ConsumeNum_5 { get; set; }
        public int ConditionEquipmentId_6 { get; set; }
        public int ConsumeNum_6 { get; set; }
        public int ConditionEquipmentId_7 { get; set; }
        public int ConsumeNum_7 { get; set; }
        public int ConditionEquipmentId_8 { get; set; }
        public int ConsumeNum_8 { get; set; }
        public int ConditionEquipmentId_9 { get; set; }
        public int ConsumeNum_9 { get; set; }
        public int ConditionEquipmentId_10 { get; set; }
        public int ConsumeNum_10 { get; set; }

        public bool CheckConsume(int eid)
        {
            for (int i = 1; i <= 10; i++)
            {
                var cid = (int)PropDict[$"ConditionEquipmentId_{i}"].GetValue(this);
                if (cid == 0)
                    break;
                if (cid == eid)
                    return true;
            }
            return false;
        }
    }
}
