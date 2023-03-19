using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class UnitAttackPattern
    {
        [Key]
        public int PatternId { get; set; }
        public int UnitId { get; set; }
        public int LoopStart { get; set; }
        public int LoopEnd { get; set; }
        public int AtkPattern_1 { get; set; }
        public int AtkPattern_2 { get; set; }
        public int AtkPattern_3 { get; set; }
        public int AtkPattern_4 { get; set; }
        public int AtkPattern_5 { get; set; }
        public int AtkPattern_6 { get; set; }
        public int AtkPattern_7 { get; set; }
        public int AtkPattern_8 { get; set; }
        public int AtkPattern_9 { get; set; }
        public int AtkPattern_10 { get; set; }
        public int AtkPattern_11 { get; set; }
        public int AtkPattern_12 { get; set; }
        public int AtkPattern_13 { get; set; }
        public int AtkPattern_14 { get; set; }
        public int AtkPattern_15 { get; set; }
        public int AtkPattern_16 { get; set; }
        public int AtkPattern_17 { get; set; }
        public int AtkPattern_18 { get; set; }
        public int AtkPattern_19 { get; set; }
        public int AtkPattern_20 { get; set; }
    }

    public class AttackPattern
    {
        public int[] Start { get; set; }
        public int[] Loop { get; set; }
        public AttackPattern()
        {

        }
        public AttackPattern(UnitAttackPattern pattern)
        {
            var props = typeof(UnitAttackPattern).GetProperties().ToDictionary(p => p.Name);

            Start = Enumerable.Range(1, pattern.LoopStart - 1)
                .Where(i => i != 14)
                .Select(i => (int)props[$"AtkPattern_{i}"].GetValue(pattern))
                .Where(a => a > 0)
                .ToArray();
            Loop = Enumerable.Range(pattern.LoopStart, pattern.LoopEnd - pattern.LoopStart + 1)
                .Where(i => i != 14)
                .Select(i => (int)props[$"AtkPattern_{i}"].GetValue(pattern))
                .Where(a => a > 0)
                .ToArray();
        }
    }
}
