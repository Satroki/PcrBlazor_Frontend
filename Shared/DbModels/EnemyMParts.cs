using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PcrBlazor.Shared
{
    public class EnemyMParts
    {
        [Key]
        public int EnemyId { get; set; }
        public string Name { get; set; }
        public int ChildEnemyParameter_1 { get; set; }
        public int ChildEnemyParameter_2 { get; set; }
        public int ChildEnemyParameter_3 { get; set; }
        public int ChildEnemyParameter_4 { get; set; }
        public int ChildEnemyParameter_5 { get; set; }
        [NotMapped]
        public List<EnemyParameter> ChildEnemyParameters { get; set; }
        [NotMapped]
        public int[] ChildEnemyParameterIds => new[] { ChildEnemyParameter_1, ChildEnemyParameter_2, ChildEnemyParameter_3, ChildEnemyParameter_4, ChildEnemyParameter_5 };
    }
}
