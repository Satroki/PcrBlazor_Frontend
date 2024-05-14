using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class SkillAction
    {
        [Key]
        public int ActionId { get; set; }
        public int ClassId { get; set; }
        public int ActionType { get; set; }
        public int ActionDetail_1 { get; set; }
        public int ActionDetail_2 { get; set; }
        public int ActionDetail_3 { get; set; }
        public SkillActionValue ActionValue_1 { get; set; }
        public SkillActionValue ActionValue_2 { get; set; }
        public SkillActionValue ActionValue_3 { get; set; }
        public SkillActionValue ActionValue_4 { get; set; }
        public SkillActionValue ActionValue_5 { get; set; }
        public SkillActionValue ActionValue_6 { get; set; }
        public SkillActionValue ActionValue_7 { get; set; }
        public int TargetAssignment { get; set; }
        public int TargetArea { get; set; }
        public int TargetRange { get; set; }
        public int TargetType { get; set; }
        public int TargetNumber { get; set; }
        public int TargetCount { get; set; }
        public string Description { get; set; }
        public string LevelUpDisp { get; set; }
    }

    public struct SkillActionValue
    {
        public double Value;
        public string Name;
        public SkillActionValue(double v, string n)
        {
            Value = v;
            Name = n;
        }

        public static implicit operator double(SkillActionValue value)
        {
            return value.Value;
        }

        public static explicit operator int(SkillActionValue value)
        {
            return (int)value.Value;
        }

        public static SkillActionValue operator *(SkillActionValue value, double m)
        {
            return new SkillActionValue(value.Value * m, value.Name);
        }

        public SkillActionValue Multiply(double d)
        {
            return new(Value * d, Name);
        }

        public void Deconstruct(out double value, out string name)
        {
            value = Value;
            name = Name;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(bool withName)
        {
            return withName ? $"{Name} {Value}" : Value.ToString();
        }
    }
}
