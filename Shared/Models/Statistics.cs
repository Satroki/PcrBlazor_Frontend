using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PcrBlazor.Shared
{
    public class Statistics
    {
        [Display(Name = "总用户")]
        public int UserCnt { get; set; }
        [Display(Name = "新用户")]
        public int NewUserCnt { get; set; }
        [Display(Name = "昨日新用户")]
        public int YestNewUserCnt { get; set; }
        [Display(Name = "活跃用户")]
        public int ActiveUserCnt { get; set; }
        [Display(Name = "昨日活跃用户")]
        public int YestActiveUserCnt { get; set; }
        [Display(Name = "Box条数")]
        public int BoxLineCnt { get; set; }
        [Display(Name = "收藏条数")]
        public int UserFavoriteCnt { get; set; }
        [Display(Name = "其他")]
        public string Other { get; set; }
    }
}
