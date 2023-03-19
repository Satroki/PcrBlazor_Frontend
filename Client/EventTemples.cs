using Microsoft.Extensions.Logging;
using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PcrBlazor.Client
{
    public class EventTemple
    {
        public static Lazy<Dictionary<string, EventTemple>> Temples { get; set; } = new(() =>
        {
            var list = new List<EventTemple> {
                new("「普通关卡」掉落2倍", EventType.Festival, "5:00", "4:59", 4),
                new("「困难关卡」掉落2倍", EventType.Festival, "5:00", "4:59", 4),
                new("「高难关卡」掉落2倍", EventType.Festival, "5:00", "4:59", 3),
                new("「圣迹调查&神殿调查」掉落2倍", EventType.Festival, "5:00", "4:59", 3),
                new("「地下城」玛那2倍", EventType.Festival, "5:00", "4:59", 10),
                new("「探索」掉落2倍", EventType.Festival, "5:00", "4:59", 10),
                new("「团队战」", EventType.ClanBattle, "5:00", "23:59", 4),
                new("「露娜塔」", EventType.Special, "12:00", "23:59", 5),
                new("「」 PICK UP扭蛋", EventType.Gacha, "14:00", "10:59", 9),
                new("「」 公主庆典扭蛋", EventType.Gacha, "14:00", "17:59", 3),
                new("附奖扭蛋", EventType.Gacha, "14:00", "10:59", 4),
                new("大师币1.5倍", EventType.Festival, "5:00", "4:59", 4),
                new("「每日任务报酬」2倍", EventType.Festival, "5:00", "4:59", 4),
                new("「每日任务」体力2倍", EventType.Festival, "5:00", "4:59", 4),
                new("更新维护", EventType.Other, "11:00", "14:00", 0),
                new("「免费10连」", EventType.Gacha, "5:00", "4:59", 4),
            };
            return list.ToDictionary(e => e.Name, e => e);
        });


        public string Name { get; set; }
        public EventType Type { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int LastDay { get; set; }

        public EventTemple()
        {

        }

        public EventTemple(string name, EventType type, string startTime, string endTime, int lastDay)
        {
            Name = name;
            Type = type;
            StartTime = startTime;
            EndTime = endTime;
            LastDay = lastDay;
        }

        public void UpdateEvent(EventCalendar e)
        {
            e.Title = Name;
            e.Type = Type;
            e.LocalStartTime = e.LocalStartTime.Date + TimeSpan.Parse(StartTime);
            e.LocalEndTime = e.LocalStartTime.Date.AddDays(LastDay) + TimeSpan.Parse(EndTime);
        }
    }
}
