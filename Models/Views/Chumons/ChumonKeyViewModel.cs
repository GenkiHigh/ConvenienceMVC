﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace ConvenienceMVC.Models.Views.Chumons
{
    public class ChumonKeyViewModel
    {
        [Column("shiire_saki_code")]
        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        [Required]
        public string ShiireSakiId { get; set; }

        [Column("chumon_date")]
        [DisplayName("注文日")]
        public DateTime ChumonDate { get; set; }

        public List<SelectListItem>? ShiireSakiIdList { get; set; }
    }
}
