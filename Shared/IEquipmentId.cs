using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PcrBlazor.Shared
{
    public interface IEquipmentId
    {
        int EquipmentId { get; set; }
    }


    public class EquipmentIdEqualityComparer : IEqualityComparer<IEquipmentId>
    {
        public bool Equals(IEquipmentId x, IEquipmentId y)
        {
            return x.EquipmentId == y.EquipmentId;
        }

        public int GetHashCode([DisallowNull] IEquipmentId obj)
        {
            return obj.EquipmentId.GetHashCode();
        }
    }
}