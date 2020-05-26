﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Convert;

namespace ESO_Lang_Editor.Model
{
    class ExportFromDB
    {
        public void ExportAsText()
        {

            var outputIDFile = new List<string>();
            var outputTextFile = new List<string>();

            var connDB = new SQLiteController();

            var dbData = connDB.FullSearchData(false);

            if (!Directory.Exists("_tmp"))
                Directory.CreateDirectory("_tmp");

            foreach (var item in dbData)
            {
                outputIDFile.Add(item.ID_Type + "-" + item.ID_Unknown + "-" + item.ID_Index);
                outputTextFile.Add(item.Text_SC);
            }

            using (StreamWriter sw = new StreamWriter("_tmp/ID.txt"))
            {
                foreach (string s in outputIDFile)
                {
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Close();
            }

            using (StreamWriter sw = new StreamWriter("_tmp/Text.txt"))
            {
                foreach (string s in outputTextFile)
                {
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Close();
            }

        }

        public void ExportIDArray(int[] ID_Array)
        {
            var outputFile = new List<string>();
            var connDB = new SQLiteController();
            var dbData = connDB.FullSearchData(false);

            if (!Directory.Exists("_tmp"))
                Directory.CreateDirectory("_tmp");

            foreach (var item in dbData)
            {
                if (ID_Array.Contains(ToInt32(item.ID_Type)))
                    outputFile.Add(item.ID_Type
                        + "-"
                        + item.ID_Unknown
                        + "-"
                        + item.ID_Index
                        + "&"
                        + item.Text_EN
                        + "&"
                        + item.Text_SC);
            }
            using (StreamWriter sw = new StreamWriter("_tmp/ExportIDArray.txt"))
            {
                foreach (string s in outputFile)
                {
                    sw.WriteLine(s);
                }

                sw.Flush();
                sw.Close();
            }

        }

        public string ExportTranslateDB(List<View.LangSearchModel> SearchData)
        {
            var connDB = new SQLiteController();
            string number = GetRandomNumber();
            List<View.LangSearchModel> data = SearchData;

            if (!Directory.Exists("Export"))
                Directory.CreateDirectory("Export");

            string dbPath = @"Export\Translate_" + number + ".db";

            if(File.Exists(dbPath))
            {
                ExportTranslateDB(data);
            }
            else
            {
                connDB.CreateTranslateDBwithData(data, dbPath);
            }

            return dbPath;
        }

        public string ExportTranslateDB(List<UIstrFile> Data)
        {
            var connDB = new UI_StrController();
            string number = GetRandomNumber();
            //List<UIstrFile> data = SearchData;

            if (!Directory.Exists("Export"))
                Directory.CreateDirectory("Export");

            string dbPath = @"Export\Translate_Str_" + number + ".db";

            if (File.Exists(dbPath))
            {
                ExportTranslateDB(Data);
            }
            else
            {
                connDB.CreateTranslateStrDBwithData(Data, dbPath);
            }

            return dbPath;
        }

        public void ExportStrDB(string TableName)
        {
            List<string> exportData = new List<string>();
            var StrDB = new UI_StrController();
            string line;

            var data = StrDB.FullSearchStrDB(false);

            StreamReader file = new StreamReader("Data/FontLib.txt");

            while ((line = file.ReadLine()) != null)
            {
                exportData.Add(line);
            }
            file.Close();

            foreach (var d in data)
            {
                if(TableName.Contains(d.UI_Table))
                {
                    exportData.Add("[" + d.UI_ID + "]"
                        + " = "
                        + "\"" + d.UI_ZH + "\"");
                }
            }

            if (!Directory.Exists("Export"))
                Directory.CreateDirectory("Export");

            using (StreamWriter sw = new StreamWriter("Export/zh_" + TableName + ".str"))
            {
                
                foreach (string s in exportData)
                {
                    sw.WriteLine(s);
                }

                sw.Flush();
                sw.Close();
            }


        }

        private string GetRandomNumber()
        {
            Random rnd = new Random();
            string number = rnd.Next(1234, 9876).ToString();
            return number;
        }
    }

}