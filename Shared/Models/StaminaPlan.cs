using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class StaminaPlan
    {
        public int UserId { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public int Table { get; set; }
        public int Dungeon { get; set; }
        public int Base { get; set; } = 250;
        public int Daily { get; set; } = 200;
        public int Buy { get; set; } = 0;
        public int AreaLimit { get; set; }
        public List<QuestPlan> QuestPlans { get; set; }

        public StaminaPlan()
        {

        }

        public StaminaPlan(string server)
        {
            Server = server;
            switch (server)
            {
                case "cn":
                    Table = 46;
                    Dungeon = 3;
                    AreaLimit = 13;
                    break;
                case "jp":
                    Table = 58;
                    Dungeon = 6;
                    AreaLimit = 37;
                    break;
            }
        }

        public int GetTotalStamina()
        {
            return Base + Table * 4 + Dungeon * 48 + Daily + Buy * 120;
        }

        public int GetTotalGem(string s)
        {
            return Enumerable.Range(1, Buy).Sum(i => GetStPrice(i));
        }

        private static int GetStPrice(int times)
        {
            if (times <= 3)
                return 40;
            else if (times <= 6)
                return 60;
            else if (times <= 14)
                return 100;
            else if (times <= 22)
                return 150;
            else if (times <= 30)
                return 180;
            else if (times <= 35)
                return 200;
            else
                return 250;
        }
    }

    public class QuestPlan
    {
        public int QuestId { get; set; }
        [JsonIgnore]
        public int QuestLevel { get; set; }
        [JsonIgnore]
        public int QuestSection { get; set; }
        [JsonIgnore]
        public string QuestName { get; set; }
        [JsonIgnore]
        public int Stamina { get; set; }
        [JsonIgnore]
        public int ItemId { get; set; }
        [JsonIgnore]
        public int ItemOdds { get; set; }
        [JsonIgnore]
        public string ItemName { get; set; }
        public bool Enabled { get; set; }
        public bool Reset { get; set; }
        public int ResetTimes { get; set; }
        [JsonIgnore]
        public int UnitUid { get; set; }
        [JsonIgnore]
        public string Type { get; set; }

        public QuestPlan()
        {

        }

        public QuestPlan(QuestInfo qi, string type)
        {
            QuestId = qi.QuestId;
            Stamina = qi.Stamina;
            QuestLevel = qi.QuestLevel;
            QuestSection = qi.Section;
            QuestName = $"{qi.QuestLevel}-{qi.Section}";
            var item = qi.DropItems.FirstOrDefault(d => d.IsItem && d.Id > 30000 && d.Id < 40000);
            if (item != null)
            {
                ItemId = item.Id;
                ItemOdds = item.Odds;
                ItemName = item.Name;
                UnitUid = ItemId - 30000;
            }
            Type = type;
        }

        public static IEnumerable<QuestPlan> GetEventQuests()
        {
            for (int i = 0; i < 5; i++)
            {
                var qp = new QuestPlan
                {
                    QuestId = i + 1,
                    QuestName = $"1-{i + 1}",
                    ItemName = (i % 2) switch
                    {
                        0 => "角色1",
                        _ => "角色2",
                    },
                    ItemId = 31000,
                    Stamina = 16 + (i / 2) * 2,
                    ItemOdds = 28 + (i / 2),
                    Type = "活动"
                };
                yield return qp;
            }
        }

        public static IEnumerable<QuestPlan> GetEventQuests(List<ShioriEvent> shioriEvents)
        {
            foreach (var si in shioriEvents)
            {
                var i = 0;
                si.Plans1 = new();
                si.Plans2 = new();
                foreach (var qp in GetEventQuests())
                {
                    qp.Type = si.Title;
                    qp.QuestSection = i + 1;
                    qp.QuestId = si.Id * 100 + i;
                    qp.ItemId = (i % 2) switch
                    {
                        0 => si.Unit1,
                        _ => si.Unit2,
                    };
                    qp.UnitUid = qp.ItemId - 30000;
                    if (i % 2 == 0)
                        si.Plans1.Add(qp);
                    else
                        si.Plans2.Add(qp);
                    i++;
                    yield return qp;
                }
            }
        }
    }
}
