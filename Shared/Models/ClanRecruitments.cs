using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class ClanRecruitment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public int Rank { get; set; }
        public string UserName { get; set; }
        public string ClanName { get; set; }
        public int Number { get; set; }
        public string Brief { get; set; }
        public string Target { get; set; }
        public string Requirement { get; set; }
        public string Contact { get; set; }
        public bool Auto { get; set; }
        public bool Arrange { get; set; }
        public bool Report { get; set; }
        public bool Bot { get; set; }
        public string Link { get; set; }
        [NotMapped]
        public bool Own { get; set; }
    }
}
