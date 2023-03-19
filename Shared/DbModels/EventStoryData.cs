using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PcrBlazor.Shared
{
    public class EventStoryData
    {
        [Key]
        public int StoryGroupId { get; set; }
        public int StoryType { get; set; }
        public int Value { get; set; }
        public string Title { get; set; }
        [ForeignKey(nameof(Value))]
        public HatsuneSchedule HatsuneSchedule { get; set; }
    }
}
