namespace PcrBlazor.Shared
{
    public class ActionValue
    {
        public SkillActionValue Initial;
        public SkillActionValue PerLevel;
        public string StatusKey;

        public ActionValue(SkillActionValue initial, SkillActionValue perLevel, string statusKey)
        {
            Initial = initial;
            PerLevel = perLevel;
            StatusKey = statusKey;
        }
    }
}
