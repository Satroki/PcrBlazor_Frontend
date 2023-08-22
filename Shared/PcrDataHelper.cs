using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PcrBlazor.Shared
{
    public static class PcrDataHelper
    {
        #region 技能
        public static UnitSkillModel CreateUnitSkillModel(UnitSkillData sd, List<SkillData> skills, List<SkillAction> actions, SkillLevelModel lvs = null, bool withNo = false)
        {
            var skillDict = skills.ToDictionary(s => s.SkillId);
            var actionDict = actions.ToDictionary(a => a.ActionId);
            lvs ??= new SkillLevelModel();
            var model = new UnitSkillModel()
            {
                UnionBurst = CreateSkillModel(sd.UnionBurst, lvs.UnionBurst),
                UnionBurstEvo = CreateSkillModel(sd.UnionBurstEvolution),
                Main1 = CreateSkillModel(sd.MainSkill_1, lvs.Main1),
                Main1Evo = CreateSkillModel(sd.MainSkillEvolution_1),
                Main2 = CreateSkillModel(sd.MainSkill_2, lvs.Main2),
                Main2Evo = CreateSkillModel(sd.MainSkillEvolution_2),
                Ex = CreateSkillModel(sd.ExSkill_1),
                ExEvo = CreateSkillModel(sd.ExSkillEvolution_1),
                Sp1 = CreateSkillModel(sd.SpSkill_1),
                Sp1Evo = CreateSkillModel(sd.SpSkillEvolution_1),
                Sp2 = CreateSkillModel(sd.SpSkill_2),
                Sp2Evo = CreateSkillModel(sd.SpSkillEvolution_2),
                Sp3 = CreateSkillModel(sd.SpSkill_3),
                Sp4 = CreateSkillModel(sd.SpSkill_4),
                Sp5 = CreateSkillModel(sd.SpSkill_5),
                Main3 = CreateSkillModel(sd.MainSkill_3, lvs.Main3),
                Main4 = CreateSkillModel(sd.MainSkill_4, lvs.Main4),
                Main5 = CreateSkillModel(sd.MainSkill_5, lvs.Main5),
                Main6 = CreateSkillModel(sd.MainSkill_6, lvs.Main6),
                Main7 = CreateSkillModel(sd.MainSkill_7, lvs.Main7),
                Main8 = CreateSkillModel(sd.MainSkill_8, lvs.Main8),
                Main9 = CreateSkillModel(sd.MainSkill_9, lvs.Main9),
                Main10 = CreateSkillModel(sd.MainSkill_10, lvs.Main10),
            };
            return model;

            SkillModel CreateSkillModel(int sid, int lv = 0)
            {
                if (!skillDict.TryGetValue(sid, out var data))
                    return null;
                var aids = data.GetActionIds().ToList();
                var sm = new SkillModel
                {
                    SkillId = data.SkillId,
                    Name = data.Name,
                    Description = data.Description,
                    CastTime = data.SkillCastTime,
                    UbCoolTime = data.BossUbCoolTime,
                    Icon = data.IconType,
                    ActionIds = aids,
                    SkillLevel = lv
                };
                sm.Actions = aids.Select(i => CreateSkillActionModel(i.actionId, i.dependActionId, sm))
                    .Where(i => i != null).ToList();
                return sm;
            }

            SkillActionModel CreateSkillActionModel(int aid, int daid, SkillModel sm)
            {
                if (!actionDict.TryGetValue(aid, out var a))
                    return null;
                actionDict.TryGetValue(daid, out var da);
                var am = new SkillActionModel()
                {
                    ActionId = a.ActionId,
                    Description = a.Description,
                    Type = a.ActionType
                };
                var sap = SkillActionParameter.Create(a, da, sm, withNo);
                am.Description = sap?.GetDescription();
                am.RelatedUnit = sap?.RelatedUnit;
                return am;
            }
        }
        static void AppendDespLine(this SkillActionModel am, string value)
        {
            am.Description = am.Description.AppendNewLine(value);
        }

        #endregion

        #region 装备
        public static List<SimpleEquipmentData> CalcBoxLineDiff(IEnumerable<(UserBoxLine ol, UserBoxLine nl)> lineGroups, List<EquipmentData> datas, List<UnitPromotion> promotions, int? mr, int? ms, bool ma)
        {
            var list = new List<int>();
            foreach (var (line1, line2) in lineGroups)
            {
                if (line2.Promotion == line1.Promotion)
                {
                    var up = promotions.Find(p => p.UnitId == line1.UnitId && p.PromotionLevel == line1.Promotion)?.CheckLimit(mr, ms, ma, line2);
                    if (up == null)
                        continue;
                    var slots = up.EquipSlots;
                    var us1 = line1.Slots;
                    var us2 = line2.Slots;
                    for (int j = 0; j < 6; j++)
                    {
                        if (slots[j] != UserBoxLine.EmptyEquipId)
                        {
                            if (us2[j] && !us1[j])
                                list.Add(slots[j]);
                        }
                    }
                }
                else
                {
                    for (int i = line1.Promotion; i <= line2.Promotion; i++)
                    {
                        var up = promotions.Find(p => p.UnitId == line1.UnitId && p.PromotionLevel == i)?.CheckLimit(mr, ms, ma, line2);
                        if (up == null)
                            continue;
                        var slots = up.EquipSlots;
                        if (i == line2.Promotion)
                        {
                            var us = line2.Slots;
                            for (int j = 0; j < 6; j++)
                            {
                                if (us[j] && slots[j] != UserBoxLine.EmptyEquipId)
                                    list.Add(slots[j]);
                            }
                        }
                        else if (i == line1.Promotion)
                        {
                            var us = line1.Slots;
                            for (int j = 0; j < 6; j++)
                            {
                                if (!us[j] && slots[j] != UserBoxLine.EmptyEquipId)
                                    list.Add(slots[j]);
                            }
                        }
                        else
                        {
                            list.AddRange(slots.Where(s => s != UserBoxLine.EmptyEquipId));
                        }
                    }
                }
            }
            var diff = ConvertIdToData(datas, list);
            return diff.ToList();
        }

        public static List<SimpleEquipmentData> CalcBoxEquipmentsGap(IEnumerable<UserBoxLine> lines, List<EquipmentData> datas, List<UnitPromotion> promotions, int? mr, int? ms, bool ma, out List<UnitEquipmentGap> unitGaps)
        {
            var list = new List<int>();
            unitGaps = new List<UnitEquipmentGap>();
            foreach (var line in lines)
            {
                var ulist = new List<int>();
                for (int i = line.Promotion; i <= line.TargetPromotion; i++)
                {
                    var up = promotions.Find(p => p.UnitId == line.UnitId && p.PromotionLevel == i)?.CheckLimit(mr, ms, ma, line);
                    if (up == null)
                        continue;
                    var slots = up.EquipSlots;
                    var checkTs = line.EnableTargetSolts && i == line.TargetPromotion;
                    var ts = line.TargetSolts;
                    if (i == line.Promotion)
                    {
                        var us = line.Slots;
                        for (int j = 0; j < 6; j++)
                        {
                            if (!us[j] && slots[j] != UserBoxLine.EmptyEquipId && (!checkTs || ts.IsBitSet(j + 1)))
                            {
                                ulist.Add(slots[j]);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (slots[j] != UserBoxLine.EmptyEquipId && (!checkTs || ts.IsBitSet(j + 1)))
                            {
                                ulist.Add(slots[j]);
                            }
                        }
                    }
                }
                if (ulist.Any())
                {
                    unitGaps.Add(new UnitEquipmentGap
                    {
                        UnitId = line.UnitId,
                        Icon = line.Icon,
                        Priority = line.Priority,
                        Gap = ConvertIdToData(datas, ulist).ToList()
                    });
                    list.AddRange(ulist);
                }
            }
            var gap = ConvertIdToData(datas, list).ToList();
            return gap.OrderByDescending(s => s.PromotionLevel).ToList();
        }

        private static IEnumerable<SimpleEquipmentData> ConvertIdToData(List<EquipmentData> datas, List<int> ids)
        {
            foreach (var g in ids.GroupBy(id => id))
            {
                var equip = datas.Find(e => e.EquipmentId == g.Key);
                if (equip != null)
                {
                    var sp = new SimpleEquipmentData(equip, g.Count());
                    sp.ConsumedEquips = null;
                    yield return sp;
                }
            }
        }

        public static List<SimpleEquipmentData> CalcEquipmentGapConsume(IEnumerable<SimpleEquipmentData> gap, List<EquipmentData> datas,
            List<EquipmentCraft> crafts)
        {
            var dict = CalcEquipmentsConsume(gap, datas, crafts);
            var spList = new List<SimpleEquipmentData>();
            foreach (var (k, v) in dict)
            {
                var eq = datas.Find(e => e.EquipmentId == k);
                var sp = new SimpleEquipmentData(eq, v);
                sp.ConsumedEquips = null;
                spList.Add(sp);
            }
            return spList.OrderByDescending(s => s.PromotionLevel).ThenByDescending(s => s.EquipmentId).ToList();
        }

        public static Dictionary<int, int> CalcEquipmentsConsume(IEnumerable<SimpleEquipmentData> gap, List<EquipmentData> datas,
            List<EquipmentCraft> crafts)
        {
            var dict = new Dictionary<int, int>();
            foreach (var g in gap)
            {
                var equip = datas.Find(e => e.EquipmentId == g.EquipmentId);
                if (equip != null)
                {
                    var sp = CalcEquipmentCrafts(equip, g.ConsumeNum, datas, crafts);
                    AddToDict(sp, sp.ConsumeNum);
                }
            }
            return dict;

            void AddToDict(SimpleEquipmentData sp, int baseNum)
            {
                if (dict.ContainsKey(sp.EquipmentId))
                    dict[sp.EquipmentId] += baseNum;
                else
                    dict.Add(sp.EquipmentId, baseNum);
                if (sp.ConsumedEquips.IsNullOrEmpty())
                    return;
                foreach (var ssp in sp.ConsumedEquips)
                {
                    var num = ssp.ConsumeNum * baseNum;
                    AddToDict(ssp, num);
                }
            }
        }

        public static SimpleEquipmentData CalcEquipmentCrafts(EquipmentData equipment, int consumeNum, List<EquipmentData> datas, List<EquipmentCraft> crafts)
        {
            var sp = new SimpleEquipmentData(equipment, consumeNum);
            if (equipment.CraftFlg && equipment.ConsumedEquips.IsNullOrEmpty())
            {
                var craft = crafts.Find(e => e.EquipmentId == equipment.EquipmentId);
                if (craft == null)
                    return sp;
                sp.ConsumedEquips = equipment.ConsumedEquips = new List<SimpleEquipmentData>();
                for (int i = 1; i < 11; i++)
                {
                    var eid = (int)EquipmentCraft.PropDict[$"ConditionEquipmentId_{i}"].GetValue(craft);
                    if (eid == 0)
                        break;
                    var num = (int)EquipmentCraft.PropDict[$"ConsumeNum_{i}"].GetValue(craft);

                    var sube = datas.Find(e => e.EquipmentId == eid);
                    if (sube != null)
                    {
                        sp.ConsumedEquips.Add(CalcEquipmentCrafts(sube, num, datas, crafts));
                    }
                }
            }
            return sp;
        }

        public static SimpleEquipmentData CalcEquipmentConsumeBy(EquipmentData equipment, List<EquipmentData> datas, List<EquipmentCraft> crafts, Dictionary<int, int> equipUnitDict)
        {
            var sp = new SimpleEquipmentData(equipment, 1);
            equipUnitDict.TryGetValue(equipment.EquipmentId, out var units);
            sp.ConsumeNum = units;
            if (sp.ConsumeByEquips == null)
            {
                sp.ConsumeByEquips = equipment.ConsumeByEquips = new List<SimpleEquipmentData>();
                var ces = crafts.Where(c => c.CheckConsume(sp.EquipmentId));
                foreach (var ce in ces)
                {
                    var pe = datas.Find(e => e.EquipmentId == ce.EquipmentId);
                    if (pe != null)
                    {
                        sp.ConsumeByEquips.Add(CalcEquipmentConsumeBy(pe, datas, crafts, equipUnitDict));
                    }
                }
            }
            return sp;
        }

        public static Dictionary<int, int> CalcEquipmentDemand(List<EquipmentData> equipments, List<EquipmentCraft> crafts, List<UnitPromotion> promotions)
        {
            var ids = promotions.SelectMany(p => p.EquipSlots).Where(e => e != UnitPromotion.DefaultSlot).ToList();
            var gaps = ConvertIdToData(equipments, ids);
            return CalcEquipmentsConsume(gaps, equipments, crafts);
        }

        public static Dictionary<int, int> CalcEquipmentDemand(List<UserBoxLine> box, List<EquipmentData> equipments, List<EquipmentCraft> crafts, List<UnitPromotion> promotions, UnitsTraceSetting traceSetting)
        {
            if (box.IsNullOrEmpty() && traceSetting?.UnitTraces?.Any(t => t.TraceType != UnitTraceType.Max) != true)
                return CalcEquipmentDemand(equipments, crafts, promotions);
            box ??= new();

            var ignoreUnit = traceSetting?.UnitTraces?.Where(t => t.TraceType == UnitTraceType.Ignore).Select(s => s.UnitId).ToHashSet() ?? new();
            var limitUnit = traceSetting?.UnitTraces?.Where(t => t.TraceType == UnitTraceType.Rank).ToDictionary(s => s.UnitId, s => s.Rank) ?? new();

            var boxDict = box.ToDictionary(l => l.UnitId);

            List<int> ids = new();
            foreach (var p in promotions)
            {
                if (ignoreUnit.Contains(p.UnitId))
                    continue;
                if (limitUnit.TryGetValue(p.UnitId, out var limit) && p.PromotionLevel > limit)
                    continue;
                if (boxDict.TryGetValue(p.UnitId, out var line))
                {
                    if (p.PromotionLevel > line.Promotion)
                        ids.AddRange(p.EquipSlots);
                    else if (p.PromotionLevel == line.Promotion)
                    {
                        var slots = p.EquipSlots;
                        var bs = line.Slots;
                        for (int i = 0; i < 6; i++)
                        {
                            if (!bs[i])
                                ids.Add(slots[i]);
                        }
                    }
                }
                else
                {
                    ids.AddRange(p.EquipSlots);
                }
            }
            ids = ids.Where(e => e != UnitPromotion.DefaultSlot).ToList();
            var gaps = ConvertIdToData(equipments, ids);
            return CalcEquipmentsConsume(gaps, equipments, crafts);
        }

        public static List<EquipmentStatus> CalcEquipmentStatuses(List<EquipmentData> equipmentsCn, List<EquipmentData> equipmentsJp, List<EquipmentEnhanceRate> enhanceRates)
        {
            var eqs = equipmentsJp.Where(e => e.EquipmentId < 13_0000)
                .Select(e => new EquipmentStatus
                {
                    JpName = e.EquipmentName,
                    EquipmentId = e.EquipmentId,
                    EquipmentUid = e.EquipmentId % 1_0000,
                    Data = e
                }).ToList();
            var cq = from eq in eqs
                     join cn in equipmentsCn.Where(e => e.EquipmentId < 13_0000)
                     on eq.EquipmentId equals cn.EquipmentId
                     select (eq, cn);
            foreach (var (eq, cn) in cq)
            {
                eq.CnName = cn.EquipmentName;
                if (!eq.Data.StatusEquals(cn))
                    eq.Data.EquipmentDataCn = cn;
            }
            var rq = from eq in eqs
                     join er in enhanceRates
                     on eq.EquipmentId equals er.EquipmentId
                     select (eq, er);
            foreach (var (eq, er) in rq)
            {
                eq.EnhanceRate = er;
            }

            var emptyRate = new EquipmentEnhanceRate();
            var statusProperties = typeof(IUnitStatus).GetProperties().ToDictionary(p => p.Name);
            var props = statusProperties.Keys.OrderBy(s => UnitStatus.Names[s]).ToList();
            foreach (var eq in eqs)
            {
                if (eq.EquipmentId < 11_0000)
                {
                    eq.Status = CalcStatus(eq, false);
                    eq.StatusCn = CalcStatus(eq, true);
                }
                eq.EnhanceRate = null;
                eq.Data = null;
            }
            return eqs;

            string CalcStatus(EquipmentStatus eq, bool cn)
            {
                var dict = new Dictionary<string, int>();
                var enhanceLevel = eq.Data.PromotionLevel.GetEquipmentEnhanceLevel();
                var enhanceRate = eq.EnhanceRate ?? emptyRate;
                var data = eq.Data;
                if (cn)
                {
                    if (enhanceRate.EquipmentEnhanceRateCn == null && data.EquipmentDataCn == null)
                        return null;
                    enhanceRate = enhanceRate.EquipmentEnhanceRateCn ?? enhanceRate;
                    data = data.EquipmentDataCn ?? data;
                }

                foreach (var p in props)
                {
                    var value = (double)statusProperties[p].GetValue(data);
                    value += Math.Ceiling(enhanceLevel * (double)statusProperties[p].GetValue(enhanceRate));
                    if (value > 0)
                        dict.Add(p, (int)value);
                }
                return string.Join("\n", props.Where(k => dict.ContainsKey(k)).Select(k => $"{UnitStatus.Names[k]}：{dict[k]}"));
            }
        }


        public const string Craft = @"■所需碎片数量从「5」下调至「3」的装备
忍者鞋、女巫发箍、蛋白石耳环、祖母绿耳环、绿松石耳环、龙纹剑、花蕾法杖、狮鹫羽饰
■所需碎片数量从「20」下调至「10」的装备
天使剑、妖蝶刀、紫龙斧、愤怒法杖、隐身战裙、大魔导师长袍、月下法衣、巫师鞋
■所需碎片数量从「20」下调至「15」的装备
太阳剑、公主短剑、狮子王的护臂、天使弓、女王之矛、生命法杖、太阳法杖、元素之心、新月的悲叹、替身手镯
■所需碎片数量从「30」下调至「20」的装备
冰之大剑、鹰神之煌剑、暗之刃、阿尔忒弥斯之弓、战女王的烈斧、司法神的锡杖、太阳护身符、龙之泪、月之手镯
■所需碎片数量从「30」下调至「25」的装备
混沌之刃、苍雷拳、精灵树之弓、盖亚之矛、水精灵之斧、大天使法杖、焰帝戒指、大贤者宝珠、圣信徒吊坠、猫神吊坠
■所需碎片数量从「35」下调至「30」的装备
闪电之刃、百华白樱刀、翡翠匕首、冰霜矛、火精灵法杖、异界法杖、千禧耳环、海神耳饰、精灵王护石、流祸苍刃、深红爪、深渊之弓、神判圣斧、创世法杖、赫怒龙吊坠、圣兽的祈祷、人鱼公主的灵泪、提尔纳诺短剑、红天弓、护天的圣枪、斗神勇腕、睿智手镯、英盾王的手镯
■所需设计图数量从「5」下调至「3」的装备
狮子王的守护
■所需设计图数量从「20」下调至「10」的装备
珍贵匕首、火焰护臂、雷光弓、绯龙矛、月华法杖、天使护甲、女王盾、圣骑士护胫甲、巫师头巾、羽翼头盔
■所需设计图数量从「20」下调至「15」的装备
光辉剑、地狱火之斧、秘银铠甲、绯红铠甲、圣人长袍、隐士服、天使鞋、法王头巾、暴雪之爪、兴风者、凤凰之杖、十字军盔甲、紫罗兰盔甲、苍天之袍、守护者之盾、死灵法师之靴、福音之冠、凤凰刀、独角兽匕首、大恶魔法杖、猩红铠甲、弑龙铠甲、翠绿灵衣、暗黑礼裙、妖精鞋、宝瓶鞋
■所需设计图数量从「25」下调至「20」的装备
月形拳套、白银大翼弓、死灵斧、幻影铠甲、苍辉之铠、纯洁巫女服、神盾战衣、星咏圆盾、忘哭之冠、天开圣剑、熔岩短剑、风神矛、神花圣杖、九天之铠、圣樱之铠、女皇长袍、少女服、地狱胫甲、白祈圣冠、罪过的大剑、妖刀血鸦、悲叹河之爪、盖亚之斧、亚特兰蒂斯之杖、雷霆之杖、愤怒女王的礼裙、煌金王铠、魔导王长袍、鬼焰斗衣、血红宝石高跟鞋、皇家守卫帽";

        public static List<EquipmentCraft> GetEquipmentCrafts(List<EquipmentData> eds, List<EquipmentCraft> ecs)
        {
            var edDict = eds.ToDictionary(e => e.EquipmentName);
            var ecDict = ecs.ToDictionary(e => e.EquipmentId);
            var fromC = 0;
            var toC = 0;
            foreach (var line in Craft.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.StartsWith("■"))
                {
                    var matches = Regex.Matches(line, @"「\d{1,2}」");
                    fromC = matches[0].Value[1..^1].AsInt();
                    toC = matches[1].Value[1..^1].AsInt();
                    continue;
                }
                foreach (var ename in line.Split('、', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()))
                {
                    if (edDict.TryGetValue(ename, out var ed) && ecDict.TryGetValue(ed.EquipmentId, out var ec))
                    {
                        if (ec.ConsumeNum_1 == toC)
                        {
                            ec.ConsumeNum_1 = fromC;
                        }
                    }
                }
            }
            return ecs;
        }
        #endregion

        #region 角色
        public static string ExportBoxData(IEnumerable<UserFavorite> favs, List<string> fields)
        {
            fields ??= new List<string>();
            var sb = new StringBuilder();
            sb.Append("用户,角色");
            foreach (var f in fields)
                sb.Append(",").Append(f.ExportFieldName());
            sb.AppendLine();
            foreach (var f in favs)
            {
                foreach (var line in f.Box)
                {
                    sb.Append($"{f.Note},{line.UnitName}");
                    foreach (var field in fields)
                    {
                        switch (field)
                        {
                            case nameof(UserBoxLine.Level):
                                sb.Append(',').Append(line.Level);
                                break;
                            case nameof(UserBoxLine.Rarity):
                                sb.Append(',').Append(line.Rarity);
                                break;
                            case nameof(UserBoxLine.Promotion):
                                sb.Append(',').Append(line.Promotion).Append('-').Append(line.Slots.Count(s => s));
                                break;
                            case nameof(UserBoxLine.LoveLevel):
                                sb.Append(',').Append(line.LoveLevel);
                                break;
                            case nameof(UserBoxLine.UniqueEquipRank):
                                sb.Append(',').Append(line.UniqueEquipRank);
                                break;
                        }
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 公会战
        public static Dictionary<string, double> GetClanBattleScores(ClanBattleInfo last)
        {
            var dict = new Dictionary<string, double>();
            for (int i = 0; i < last.Phases.Count; i++)
            {
                var p = last.Phases[i];
                for (int j = 0; j < p.Bosses.Count; j++)
                {
                    dict.Add($"{(char)(65 + i)}{j + 1}", p.Bosses[j].ScoreCoefficient);
                }
            }
            return dict;
        }
        #endregion

        public static double GetValue(this IUnitStatus s, string key) => key switch
        {
            nameof(IUnitStatus.Accuracy) => s.Accuracy,
            nameof(IUnitStatus.Atk) => s.Atk,
            nameof(IUnitStatus.Def) => s.Def,
            nameof(IUnitStatus.Dodge) => s.Dodge,
            nameof(IUnitStatus.EnergyRecoveryRate) => s.EnergyRecoveryRate,
            nameof(IUnitStatus.EnergyReduceRate) => s.EnergyReduceRate,
            nameof(IUnitStatus.Hp) => s.Hp,
            nameof(IUnitStatus.HpRecoveryRate) => s.HpRecoveryRate,
            nameof(IUnitStatus.LifeSteal) => s.LifeSteal,
            nameof(IUnitStatus.MagicCritical) => s.MagicCritical,
            nameof(IUnitStatus.MagicDef) => s.MagicDef,
            nameof(IUnitStatus.MagicPenetrate) => s.MagicPenetrate,
            nameof(IUnitStatus.MagicStr) => s.MagicStr,
            nameof(IUnitStatus.PhysicalCritical) => s.PhysicalCritical,
            nameof(IUnitStatus.PhysicalPenetrate) => s.PhysicalPenetrate,
            nameof(IUnitStatus.WaveEnergyRecovery) => s.WaveEnergyRecovery,
            nameof(IUnitStatus.WaveHpRecovery) => s.WaveHpRecovery,
            _ => 0
        };
    }

    #region 角色属性
    public class UnitStatusCalculator
    {
        private readonly List<string> statusKeys = typeof(IUnitStatus).GetProperties().Select(p => p.Name).ToList();
        private readonly Dictionary<string, PropertyInfo> unitRarityProperties
            = typeof(UnitRarity).GetProperties().ToDictionary(p => p.Name);
        private readonly Dictionary<string, PropertyInfo> CharaStoryStatusProperties
            = typeof(CharaStoryStatus).GetProperties().ToDictionary(p => p.Name);

        private readonly List<EquipmentEnhanceRate> equipmentEnhanceRates;
        private readonly List<EquipmentData> equipmentDatas;
        private readonly bool isCn;

        public UnitStatusCalculator(List<EquipmentEnhanceRate> eers, List<EquipmentData> eds, string server)
        {
            equipmentEnhanceRates = eers;
            equipmentDatas = eds;
            isCn = server == "cn";
        }

        public Dictionary<string, double> CalcEquipmentStatus(EquipmentData eq, int enhanceLevel)
        {
            var status = new Dictionary<string, double>();
            var enhanceRate = equipmentEnhanceRates.Find(e => e.EquipmentId == eq.EquipmentId) ?? new EquipmentEnhanceRate();
            if (isCn)
                enhanceRate = enhanceRate.EquipmentEnhanceRateCn ?? enhanceRate;
            foreach (var p in statusKeys)
            {
                var value = eq.GetValue(p);
                value += Math.Ceiling(enhanceLevel * enhanceRate.GetValue(p));
                status.Add(p, value);
            }
            return status;
        }

        public Dictionary<string, double> GetStatusDict(IUnitStatus s)
        {
            var status = new Dictionary<string, double>();
            foreach (var p in statusKeys)
            {
                var value = s.GetValue(p);
                status.Add(p, value);
            }
            return status;
        }

        public Dictionary<string, double> CalcUniqueEquipmentStatus(UniqueEquipmentData eq, List<UniqueEquipEnhanceRate> enhanceRates, int enhanceLevel)
        {
            var status = new Dictionary<string, double>();
            if (enhanceRates.Count == 1)
            {
                var rate = enhanceRates[0];
                foreach (var p in statusKeys)
                {
                    var value = eq.GetValue(p) + Math.Ceiling(enhanceLevel * rate.GetValue(p));
                    status.Add(p, value);
                }
            }
            else if (enhanceRates.Count == 2)
            {
                var r1 = enhanceRates[0];
                var r2 = enhanceRates[1];
                var e1 = Math.Min(enhanceLevel, 260) - 1;
                var e2 = Math.Max(enhanceLevel, 260) - 260;
                foreach (var p in statusKeys)
                {
                    var value = eq.GetValue(p) + Math.Ceiling(e1 * r1.GetValue(p) + e2 * r2.GetValue(p));
                    status.Add(p, value);
                }
            }
            return status;
        }

        public UnitStatus CalcUnitStatus(UserBoxLine line, UnitSourceData sd, IEnumerable<UserBoxLine> allLine, int? mr, int? ms, bool ma, bool withRB = true)
        {
            var us = new UnitStatus();
            var uid = line.UnitId / 100;
            try
            {
                var status = new Dictionary<string, double>();
                var unitRarity = sd.UnitRarities.Find(u => u.Rarity == line.Rarity);
                if (unitRarity == null && line.Rarity == 6)
                {
                    unitRarity = sd.UnitRarities.Find(u => u.Rarity == 5);
                }
                var promotionStatus = sd.UnitPromotionStatuses.Find(p => p.PromotionLevel == line.Promotion);
                if (isCn && promotionStatus?.UnitPromotionStatusCn != null)
                    promotionStatus = promotionStatus.UnitPromotionStatusCn;
                foreach (var p in statusKeys)
                {
                    var value = 0d;
                    if (promotionStatus != null)
                        value += promotionStatus.GetValue(p);
                    if (unitRarity != null)
                    {
                        value += unitRarity.GetValue(p);
                        var growth = (double)unitRarityProperties[p + "Growth"].GetValue(unitRarity);
                        value += growth * (line.Level + line.Promotion);
                    }
                    value = Math.Round(value, MidpointRounding.AwayFromZero);
                    status.Add(p, value);
                }

                var up = sd.UnitPromotions.Find(p => p.PromotionLevel == line.Promotion)?.CheckLimit(mr, ms, ma, line);
                if (up != null)
                {
                    var equipSlots = up.EquipSlots;
                    var slots = line.Slots;
                    for (var i = 0; i < equipSlots.Length; i++)
                    {
                        if (!slots[i])
                            continue;
                        var equipId = equipSlots[i];
                        if (equipId == UserBoxLine.EmptyEquipId)
                            continue;
                        var eq = equipmentDatas.Find(e => e.EquipmentId == equipId);
                        if (eq == null)
                            continue;
                        var enhanceLevel = eq.PromotionLevel.GetEquipmentEnhanceLevel();
                        var enhanceRate = equipmentEnhanceRates.Find(e => e.EquipmentId == equipId) ?? new EquipmentEnhanceRate();
                        if (isCn)
                            enhanceRate = enhanceRate.EquipmentEnhanceRateCn ?? enhanceRate;

                        foreach (var p in statusKeys)
                        {
                            var value = eq.GetValue(p);
                            value += Math.Ceiling(enhanceLevel * enhanceRate.GetValue(p));
                            status[p] = status[p] + value;
                        }
                    }
                }

                var cids = sd.CharaStoryStatuses.Select(c => c.CharaId_1).Distinct();
                foreach (var cid in cids)
                {
                    var cl = cid == uid ? line : allLine?.FirstOrDefault(l => l.UnitId / 100 == cid);
                    if (cl == null)
                        continue;
                    var stories = sd.CharaStoryStatuses.Where(s => s.StoryId / 1000 == cid && s.StoryDetail.LoveLevel <= cl.LoveLevel);
                    foreach (var story in stories)
                    {
                        var cs = isCn ? story.StatusCn ?? story : story;

                        for (int i = 1; i < 6; i++)
                        {
                            var sp = GetStatusTypeProp((int)CharaStoryStatusProperties[$"StatusType_{i}"].GetValue(cs));
                            if (sp is null)
                                break;
                            var svalue = (int)CharaStoryStatusProperties[$"StatusRate_{i}"].GetValue(cs);
                            status[sp] = status[sp] + svalue;
                        }
                    }
                }

                if (line.UniqueEquipRank > 0)
                {
                    if (sd.UniqueEquipmentData != null)
                    {
                        var dict = CalcUniqueEquipmentStatus(sd.UniqueEquipmentData, sd.UniqueEquipmentEnhanceRates, line.UniqueEquipRank);
                        foreach (var p in statusKeys)
                        {
                            status[p] = status[p] + dict.GetValueOrDefault(p);
                        }
                    }
                }

                if (line.UniqueEquip2Rank >= 0)
                {
                    if (sd.UniqueEquipment2Data != null)
                    {
                        var dict = CalcUniqueEquipmentStatus(sd.UniqueEquipment2Data, sd.UniqueEquipment2EnhanceRates, line.UniqueEquip2Rank.Value);
                        foreach (var p in statusKeys)
                        {
                            status[p] = status[p] + dict.GetValueOrDefault(p);
                        }
                    }
                }

                if (withRB)
                {
                    var rbs = isCn ? sd.PromotionBonusCn : sd.PromotionBonus;
                    var pb = rbs?.Find(b => b.PromotionLevel == line.Promotion);
                    if (pb != null)
                    {
                        foreach (var p in statusKeys)
                        {
                            var value = pb.GetValue(p);
                            status[p] = status[p] + value;
                        }
                    }
                }
                us.StatusDict = status;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return us;
        }

        private string GetStatusTypeProp(int type)
        {
            return type switch
            {
                1 => nameof(UnitStatus.Hp),
                2 => nameof(UnitStatus.Atk),
                3 => nameof(UnitStatus.Def),
                4 => nameof(UnitStatus.MagicStr),
                5 => nameof(UnitStatus.MagicDef),
                6 => nameof(UnitStatus.PhysicalCritical),
                7 => nameof(UnitStatus.MagicCritical),
                8 => nameof(UnitStatus.Dodge),
                9 => nameof(UnitStatus.LifeSteal),
                10 => nameof(UnitStatus.WaveHpRecovery),
                11 => nameof(UnitStatus.WaveEnergyRecovery),
                12 => nameof(UnitStatus.PhysicalPenetrate),
                13 => nameof(UnitStatus.MagicPenetrate),
                14 => nameof(UnitStatus.EnergyRecoveryRate),
                15 => nameof(UnitStatus.HpRecoveryRate),
                16 => nameof(UnitStatus.EnergyReduceRate),
                17 => nameof(UnitStatus.Accuracy),
                _ => null
            };
        }
    }
    #endregion

    #region 规划
    public class PcrSolveHelper
    {
        private readonly Func<JsSolveModel, Task<JsonElement>> solveTask;
        private readonly List<EquipmentData> eds;
        private readonly List<AreaInfo> areas;
        private readonly Dictionary<int, int> questStDict;
        private List<EquipmentSource> equipmentSources;
        private Dictionary<int, int> demandsDict;
        private bool includeExtraDrops;
        public string Server { get; }

        public PcrSolveHelper(string server, List<EquipmentData> eds, List<AreaInfo> areas, Func<JsSolveModel, Task<JsonElement>> solveTask)
        {
            Server = server;
            this.eds = eds;
            this.areas = areas;
            this.solveTask = solveTask;
            equipmentSources = CreateEquipmentSources(eds, areas, includeExtraDrops);
            questStDict = areas.SelectMany(a => a.Quests).Where(q => q.Section > 0).ToDictionary(q => q.QuestId, q => q.Stamina);
        }

        public void UpdateIncludeExtraDrops(bool b)
        {
            if (includeExtraDrops != b)
            {
                includeExtraDrops = b;
                equipmentSources = CreateEquipmentSources(eds, areas, includeExtraDrops);
            }
        }

        public void UpdateDemands(Dictionary<int, int> demandsDict)
        {
            this.demandsDict = demandsDict;
        }

        public async Task<List<SolveResult>> GetJsSolveResult(SolveRequest r)
        {
            if (r.Normal == 0 && r.Hard == 0)
                return null;
            if (r.Requirements.IsNullOrEmpty() || r.Requirements.All(v => v == 0))
                return null;
            if (r.AreaLimit > 0 && r.Forecast > r.AreaLimit)
            {
                r = r.JsonCopy();
                var edict = r.EquipmentIds.Zip(r.Requirements).ToDictionary(t => t.First, t => t.Second);
                var tempr = await Solve(r, r.Forecast);
                foreach (var tr in tempr)
                {
                    var qlev = tr.QuestId / 1000 % 100;
                    if (qlev > r.AreaLimit)
                    {
                        foreach (var (eid, num) in tr.EquipmentIds.Zip(tr.Expected))
                        {
                            if (edict.ContainsKey(eid))
                                edict[eid] -= num;
                        }
                    }
                }
                var kvs = edict.Where(kv => kv.Value > 0).ToList();
                r.EquipmentIds = kvs.Select(kv => kv.Key).ToArray();
                r.Requirements = kvs.Select(kv => kv.Value).ToArray();
            }
            return await Solve(r);
        }

        private async Task<List<SolveResult>> Solve(SolveRequest r, int? forecastLimit = null)
        {
            var ess = equipmentSources;
            var areaLimit = forecastLimit ?? r.AreaLimit;
            ess = ess.Where(e => r.EquipmentIds.Contains(e.EquipmentId)).ToList();
            var qids = ess.SelectMany(e => e.QuestIds).Where(id => id < 13_000_000);
            if (r.Normal == 0)
                qids = qids.Where(id => id > 12_000_000);
            if (r.Hard == 0)
                qids = qids.Where(id => id < 12_000_000);
            if (areaLimit > 0)
                qids = qids.Where(id => id / 1000 % 100 <= areaLimit);
            var questIds = qids.Distinct().ToArray();
            Array.Sort(questIds);
            Array.Reverse(questIds);

            var model = GetJsSolveModel(ess, questIds, r);
            var times = await DoLpSolveAsync(model, questIds);
            if (times == null)
                return null;

            #region 副产品最大化
            var questEquipDict = new Dictionary<int, int[]>();
            for (int i = 0; i < questIds.Length; i++)
            {
                if (times[i] == 0)
                    continue;
                var qid = questIds[i];
                var eqids = ess.Where(es => es.QuestIds.Contains(qid)).Select(e => e.EquipmentId).ToArray();
                questEquipDict.Add(qid, eqids);
            }

            var q1 = questEquipDict.Where(kv => kv.Value.Length == 1).ToList();
            if (q1.Any())
            {
                var dict = demandsDict;
                var ais = areas;
                foreach (var (qid, eids) in q1)
                {
                    var eid = eids[0];
                    var es = ess.First(e => e.EquipmentId == eid);
                    var odd = es.Odds[Array.IndexOf(es.QuestIds, qid)];
                    var st = questStDict[qid];
                    var rqids = es.QuestIds
                        .Where((id, i) => es.Odds[i] == odd && questStDict.ContainsKey(id) && questStDict[id] == st)
                        .Intersect(questIds).ToList();
                    if (rqids.Count > 1)
                    {
                        var maxScore = 0d;
                        QuestInfo qi = null;
                        foreach (var qrid in rqids)
                        {
                            var quest = ais.First(a => a.AreaId == (qrid / 1000)).Quests.First(q => q.QuestId == qrid);
                            var score = quest.Efficiency;
                            if (score > maxScore)
                            {
                                maxScore = score.Value;
                                qi = quest;
                            }
                        }
                        if (qi.QuestId != qid)
                        {
                            var ii = Array.IndexOf(questIds, qid);
                            var time = times[ii];
                            times[ii] = 0;
                            times[Array.IndexOf(questIds, qi.QuestId)] += time;
                        }
                    }
                }
            }
            #endregion

            var results = new List<SolveResult>();
            for (int i = 0; i < questIds.Length; i++)
            {
                var qid = questIds[i];
                var ts = times[i];
                if (ts == 0)
                    continue;
                var sr = new SolveResult()
                {
                    QuestId = qid,
                    QuestName = GetQuestAbbr(qid),
                    Times = (int)Math.Ceiling(ts),
                    EquipmentIds = new List<int>(),
                    Expected = new List<int>()
                };
                sr.Recommend = sr.Times;
                sr.Stamina = sr.Times * questStDict[qid];
                results.Add(sr);
            }
            foreach (var es in ess)
            {
                var rq = r.Requirements[Array.IndexOf(r.EquipmentIds, es.EquipmentId)];
                foreach (var sr in results)
                {
                    var i = Array.IndexOf(es.QuestIds, sr.QuestId);
                    if (i >= 0)
                    {
                        sr.EquipmentIds.Add(es.EquipmentId);
                        var odds = r.GetOdds(sr.QuestId, es.Odds[i]) / 100d;
                        var num = (int)Math.Floor(sr.Times * odds);
                        sr.Effective += (int)es.Odds[i];
                        sr.AllRequirements += rq;
                        sr.Recommend = Math.Min(sr.Recommend, (int)Math.Ceiling(rq / odds));
                        sr.Expected.Add(num);
                    }
                }
            }
            results.ForEach(r => r.SortEquipment());
            return results.OrderByDescending(r => r.Stamina).ToList();
        }
        private JsSolveModel GetJsSolveModel(List<EquipmentSource> ess, int[] questIds, SolveRequest r)
        {
            var m = new JsSolveModel();
            foreach (var es in ess)
            {
                var rq = r.Requirements[Array.IndexOf(r.EquipmentIds, es.EquipmentId)];
                var qids = es.QuestIds.Intersect(questIds).ToArray();
                if (qids.IsNullOrEmpty())
                {
                    continue;
                }
                m.Constraints.Add(es.EquipmentId, new(rq * 100));
                for (var i = 0; i < qids.Length; i++)
                {
                    var qid = qids[i];
                    if (!m.Variables.ContainsKey(qid))
                        m.Variables.Add(qid, new() { [1] = questStDict[qid], [qid] = 1 });
                    var odds = es.Odds[Array.IndexOf(es.QuestIds, qid)];
                    m.Variables[qid].Add(es.EquipmentId, r.GetOdds(qid, odds));
                }
            }

            if (r.Hard > 0 && r.HardTimes > 0)
            {
                foreach (var qid in questIds.Where(q => q > 12_000_000))
                {
                    m.Constraints.Add(qid, new(null, r.HardTimes));
                }
            }

            return m;
        }

        private async Task<double[]> DoLpSolveAsync(JsSolveModel m, int[] questIds)
        {
            var json = await solveTask(m);
            if (json.TryGetProperty("error", out _))
            {
                return null;
            }
            var result = new double[questIds.Length];
            for (int i = 0; i < questIds.Length; i++)
            {
                int qid = questIds[i];
                if (json.TryGetProperty(qid.ToString(), out var je))
                    result[i] = je.GetDouble();
            }
            return result;
        }

        public static List<EquipmentSource> CreateEquipmentSources(List<EquipmentData> eds, List<AreaInfo> areas, bool withExtra = false)
        {
            var esDict = eds.Where(e => !e.CraftFlg)
                .Select(e => new EquipmentSource(e.EquipmentId)).ToDictionary(e => e.EquipmentId);
            foreach (var a in areas)
            {
                var exDrops = Enumerable.Empty<QuestDropItem>();
                if (withExtra && a.Quests[0].Section == 0)
                {
                    exDrops = a.Quests[0].DropItems;
                }

                foreach (var q in a.Quests)
                {
                    if (q.Section == 0)
                        continue;
                    var drops = withExtra ? q.DropItems.Concat(exDrops) : q.DropItems;
                    foreach (var d in drops)
                    {
                        if (esDict.TryGetValue(d.Id, out var es))
                        {
                            es.AddSource(q.QuestId, d.DoubleOdds ?? d.Odds);
                        }
                    }
                }
            }
            var ess = esDict.Values.OrderBy(v => v.EquipmentId).ToList();
            foreach (var es in ess)
            {
                es.FinishAdd();
            }
            return ess;
        }

        public static string GetQuestAbbr(int qid)
        {
            var a = qid / 1000 % 100;
            var n = qid < 12_000_000;
            var q = qid % 100;
            return $"{(n ? "N" : "H")} {a}-{q}";
        }

        public class JsSolveModel
        {
            public string Optimize { get; set; } = "1";
            public string OpType { get; set; } = "min";
            public Dictionary<int, SolveConstraint> Constraints { get; set; } = new();
            public Dictionary<int, Dictionary<int, double>> Variables { get; set; } = new();
        }

        public class SolveConstraint
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Min { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Max { get; set; }

            public SolveConstraint(int? min, int? max = null)
            {
                Min = min;
                Max = max;
            }
        }
    }

    #endregion
}
