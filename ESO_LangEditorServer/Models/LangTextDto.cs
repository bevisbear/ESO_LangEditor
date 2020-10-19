﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ESO_LangEditorServer.Models
{
    public class LangTextDto
    {
        public Guid Id { get; set; }
        public string TextId { get; set; }
        public int IdType { get; set; }
        public string TextEn { get; set; }
        public string TextZh { get; set; }
        public int IsTranslated { get; set; }
        public string UpdateStats { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-DD HH:MM}")]
        [DataType(DataType.Date)]
        public DateTime EnLastModifyTimestamp { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-DD HH:MM}")]
        public DateTime ZhLastModifyTimestamp { get; set; }
        public Guid UserId { get; set; }
    }
}