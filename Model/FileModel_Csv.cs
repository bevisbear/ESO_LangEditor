﻿using System;
using static System.Convert;
using FileHelpers;

namespace ESO_Lang_Editor.Model
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class FileModel_Csv
    {
        [FieldQuoted]
        public UInt32 stringID { get; set; }
        [FieldQuoted]
        public UInt16 stringUnknow { get; set; }
        [FieldQuoted]
        public UInt32 stringIndex { get; set; }
        [FieldQuoted]
        public UInt32 stringOffset { get; set; }
        [FieldQuoted]
        public string textContent { get; set; }

        public string GetUniqueID(bool isGetFieldID)
        {

            string fieldID = ToUInt32(stringID).ToString(); 
            string fieldUnknown = ToUInt32(stringUnknow).ToString(); 
            string fieldIndex = ToUInt32(stringIndex).ToString();
            if (isGetFieldID)
            {
                return fieldID;
            }
            else
            {
                return fieldID + '-' + fieldUnknown + '-' + fieldIndex;
            }
        }
    }
}
