using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class UserFavorite : IUserId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string ShareId { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Server { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string Display => $"[{(Server == "cn" ? "中" : "日")}] {Note}";
        [NotMapped]
        [JsonIgnore]
        public List<UserBoxLine> Box { get; set; }
    }
}
