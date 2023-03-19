using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class ResistData
    {
        [Key]
        public int ResistStatusId { get; set; }
        public int? Ailment_1 { get; set; }
        public int? Ailment_2 { get; set; }
        public int? Ailment_3 { get; set; }
        public int? Ailment_4 { get; set; }
        public int? Ailment_5 { get; set; }
        public int? Ailment_6 { get; set; }
        public int? Ailment_7 { get; set; }
        public int? Ailment_8 { get; set; }
        public int? Ailment_9 { get; set; }
        public int? Ailment_10 { get; set; }
        public int? Ailment_11 { get; set; }
        public int? Ailment_12 { get; set; }
        public int? Ailment_13 { get; set; }
        public int? Ailment_14 { get; set; }
        public int? Ailment_15 { get; set; }
        public int? Ailment_16 { get; set; }
        public int? Ailment_17 { get; set; }
        public int? Ailment_18 { get; set; }
        public int? Ailment_19 { get; set; }
        public int? Ailment_20 { get; set; }
        public int? Ailment_21 { get; set; }
        public int? Ailment_22 { get; set; }
        public int? Ailment_23 { get; set; }
        public int? Ailment_24 { get; set; }
        public int? Ailment_25 { get; set; }
        public int? Ailment_26 { get; set; }
        public int? Ailment_27 { get; set; }
        public int? Ailment_28 { get; set; }
        public int? Ailment_29 { get; set; }
        public int? Ailment_30 { get; set; }
        public int? Ailment_31 { get; set; }
        public int? Ailment_32 { get; set; }
        public int? Ailment_33 { get; set; }
        public int? Ailment_34 { get; set; }
        public int? Ailment_35 { get; set; }
        public int? Ailment_36 { get; set; }
        public int? Ailment_37 { get; set; }
        public int? Ailment_38 { get; set; }
        public int? Ailment_39 { get; set; }
        public int? Ailment_40 { get; set; }
        public int? Ailment_41 { get; set; }
        public int? Ailment_42 { get; set; }
        public int? Ailment_43 { get; set; }
        public int? Ailment_44 { get; set; }
        public int? Ailment_45 { get; set; }
        public int? Ailment_46 { get; set; }
        public int? Ailment_47 { get; set; }
        public int? Ailment_48 { get; set; }
        public int? Ailment_49 { get; set; }
        public int? Ailment_50 { get; set; }

        public IEnumerable<(int, int)> GetAilmentResist()
        {
            var type = typeof(ResistData);
            for (int i = 1; i <= 50; i++)
            {
                var v = type.GetProperty($"Ailment_{i}").GetValue(this);
                if (v is int r)
                    yield return (i, r);
                else
                    break;
            }
        }
    }

    public class AilmentData
    {
        [Key]
        [JsonIgnore]
        public int AilmentId { get; set; }
        public string AilmentName { get; set; }
        [NotMapped]
        public int Resist { get; set; }

        public AilmentData()
        {

        }
    }
}
