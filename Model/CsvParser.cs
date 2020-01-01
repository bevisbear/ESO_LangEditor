﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESO_Lang_Editor.Model
{
    class CsvParser
    {


        /// <summary>
        /// 将读取的csv文件把数组转换成List<csvItem>。
        /// </summary>
        /// <param name="content">csvItem[] 格式</param>
        /// <returns>List<csvItem></returns>
        public List<FileModel_Csv> CsvFileParser(FileModel_Csv[] content)
        {
            var csvContent = new List<FileModel_Csv>();
            //Dictionary<string, csvItem> csvContent = new Dictionary<string, csvItem>();
            foreach (var item in content)
            {
                //Console.WriteLine("ID: {0}, 未知: {1}, 索引: {2}, 文本: {3}",item.stringID, item.stringUnknow,
                //         item.stringIndex, item.textContent);
                //csvContent.Add(csv.
                csvContent.Add(item);

            }
            //Console.WriteLine(csvContent.Count);
            return csvContent;
        }

        /// <summary>
        /// 从传进的CSV文本，按照ID和Index从小到大排序。
        /// </summary>
        /// <param name="csvContent"></param>
        /// <returns></returns>
        public List<FileModel_Csv> CsvSort(List<FileModel_Csv> csvContent)
        {
            List<FileModel_Csv> OrderdList = csvContent.OrderBy(t => t.stringID).
                ThenBy(t => t.stringIndex).ToList();

            //foreach (var item in OrderdList)
            //{
            //    Console.WriteLine("ID: {0}, 未知: {1}, 索引: {2}, 文本: {3}", item.stringID, item.stringUnknow,
            //            item.stringIndex, item.textContent);
            //}
            return OrderdList;
        }


        public Dictionary<string, string> CsvCompareNonChange(Dictionary<string, string> OldDict, Dictionary<string, string> NewDict)
        {
            var ContentnNonChange = new Dictionary<string, string>();

            foreach (var entry in DiffDictionary(OldDict, NewDict))
            {
                if (entry.Value == "same")
                    ContentnNonChange.Add(entry.Key, OldDict[entry.Key]);

            }
            return ContentnNonChange;
        }

        public Dictionary<string, string> CsvCompareEdited(Dictionary<string, string> OldDict, Dictionary<string, string> NewDict)
        {
            var ContentEdited = new Dictionary<string, string>();

            foreach (var entry in DiffDictionary(OldDict, NewDict))
            {
                if (entry.Value == "different")
                    ContentEdited.Add(entry.Key, OldDict[entry.Key]);

            }
            return ContentEdited;
        }

        public Dictionary<string, string> CsvCompareAdded(Dictionary<string, string> OldDict, Dictionary<string, string> NewDict)
        {
            var ContentAdded = new Dictionary<string, string>();

            foreach (var entry in DiffDictionary(OldDict, NewDict))
            {
                if (entry.Value == "added")
                    ContentAdded.Add(entry.Key, OldDict[entry.Key]);

            }
            return ContentAdded;
        }

        public Dictionary<string, string> CsvCompareRemove(Dictionary<string, string> OldDict, Dictionary<string, string> NewDict)
        {
            var ContentRemoved = new Dictionary<string, string>();

            foreach (var entry in DiffDictionary(OldDict, NewDict))
            {
                if (entry.Value == "removed")
                    ContentRemoved.Add(entry.Key, OldDict[entry.Key]);
            }
            return ContentRemoved;
        }


        public void CsvCompare(Dictionary<string, string> OldDict, Dictionary<string, string> NewDict)
        {
            foreach (var entry in DiffDictionary(OldDict, NewDict))
            {

            }

        }

        private static Dictionary<string, string> DiffDictionary(Dictionary<string, string> first, Dictionary<string, string> second)
        {
            var diff = first.ToDictionary(e => e.Key, e => "removed");
            foreach (var other in second)
            {
                string firstValue;
                if (first.TryGetValue(other.Key, out firstValue))
                {
                    diff[other.Key] = firstValue.Equals(other.Value) ? "same" : "different";
                }
                else
                {
                    diff[other.Key] = "added";
                }
            }
            return diff;
        }

    }
}
