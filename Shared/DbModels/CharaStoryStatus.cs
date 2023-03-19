using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class CharaStoryStatus
    {
        /// <summary>
        /// 4位+3位
        /// </summary>
        [Key]
        public int StoryId { get; set; }
        [JsonIgnore]
        public string UnlockStoryName { get; set; }
        public int StatusType_1 { get; set; }
        public int StatusRate_1 { get; set; }
        public int StatusType_2 { get; set; }
        public int StatusRate_2 { get; set; }
        public int StatusType_3 { get; set; }
        public int StatusRate_3 { get; set; }
        public int StatusType_4 { get; set; }
        public int StatusRate_4 { get; set; }
        public int StatusType_5 { get; set; }
        public int StatusRate_5 { get; set; }
        public int CharaId_1 { get; set; }
        public int CharaId_2 { get; set; }
        public int CharaId_3 { get; set; }
        public int CharaId_4 { get; set; }
        public int CharaId_5 { get; set; }
        public int CharaId_6 { get; set; }
        public int CharaId_7 { get; set; }
        public int CharaId_8 { get; set; }
        public int CharaId_9 { get; set; }
        public int CharaId_10 { get; set; }

        [ForeignKey(nameof(StoryId))]
        public StoryDetail StoryDetail { get; set; }

        [NotMapped]
        public CharaStoryStatus StatusCn { get; set; }

        public void SetStatusCn(CharaStoryStatus cn)
        {
            var isEq = StoryId == cn.StoryId
                && StatusType_1 == cn.StatusType_1 && StatusRate_1 == cn.StatusRate_1
                && StatusType_2 == cn.StatusType_2 && StatusRate_2 == cn.StatusRate_2
                && StatusType_3 == cn.StatusType_3 && StatusRate_3 == cn.StatusRate_3
                && StatusType_4 == cn.StatusType_4 && StatusRate_4 == cn.StatusRate_4
                && StatusType_5 == cn.StatusType_5 && StatusRate_5 == cn.StatusRate_5;
            if (!isEq)
                StatusCn = cn;
        }
    }
}
