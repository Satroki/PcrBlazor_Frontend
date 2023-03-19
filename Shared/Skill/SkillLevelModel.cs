namespace PcrBlazor.Shared
{
    public class SkillLevelModel
    {
        public int UnionBurst { get; set; }
        public int Main1 { get; set; }
        public int Main2 { get; set; }
        public int Main3 { get; set; }
        public int Main4 { get; set; }
        public int Main5 { get; set; }
        public int Main6 { get; set; }
        public int Main7 { get; set; }
        public int Main8 { get; set; }
        public int Main9 { get; set; }
        public int Main10 { get; set; }

        public SkillLevelModel()
        {

        }
        public SkillLevelModel(EnemyParameter ep)
        {
            UnionBurst = ep.UnionBurstLevel;
            Main1 = ep.MainSkillLv_1;
            Main2 = ep.MainSkillLv_2;
            Main3 = ep.MainSkillLv_3;
            Main4 = ep.MainSkillLv_4;
            Main5 = ep.MainSkillLv_5;
            Main6 = ep.MainSkillLv_6;
            Main7 = ep.MainSkillLv_7;
            Main8 = ep.MainSkillLv_8;
            Main9 = ep.MainSkillLv_9;
            Main10 = ep.MainSkillLv_10;
        }
    }
}
