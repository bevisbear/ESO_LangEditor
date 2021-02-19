﻿using ESO_LangEditor.Core.EnumTypes;
using ESO_LangEditor.Core.Models;
using ESO_LangEditorGUI.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESO_LangEditorGUI.ViewModels
{
    public class ImportDbRevProgressDialogViewModel : BaseViewModel
    {
        private string _currentExcuteText;
        private Visibility _closeButtonVisibility = Visibility.Collapsed;
        private bool _closeButtonEnable = false;
        private bool _progressbarDisplay;
        private string _downloadSpeed;

        //public ICommand RunDialogCommand => new ExcuteViewModelMethod(ExportLangAsync);

        public string CurrentExcuteText
        {
            get { return _currentExcuteText; }
            set { _currentExcuteText = value; NotifyPropertyChanged(); }
        }

        public Visibility CloseButtonVisibility
        {
            get { return _closeButtonVisibility; }
            set { _closeButtonVisibility = value; NotifyPropertyChanged(); }
        }

        public bool CloseButtonEnable
        {
            get { return _closeButtonEnable; }
            set { _closeButtonEnable = value; NotifyPropertyChanged(); }
        }

        public bool ProgressbarDisplay
        {
            get { return _progressbarDisplay; }
            set { _progressbarDisplay = value; NotifyPropertyChanged(); }
        }

        public string DownloadSpeed
        {
            get { return _downloadSpeed; }
            set { _downloadSpeed = value; NotifyPropertyChanged(); }
        }


        private int _DatabaseRevServer;
        private List<string> DatabaseRevDownloadList = new List<string>();

        private static string DatabaseRevPath = @"database/data_rev";
        private static string DownloadFolder = @"_Tmp/";

        private int RevCompareNum = 0;
        private int TaskCount = 0;

        private HttpClient apiClient { get; set; }

     

        public ImportDbRevProgressDialogViewModel(bool isDbRev)
        {
            //CurrentExcuteText = "导出为.lang，等待点击开始按钮执行";
            ProgressbarDisplay = false;
            //BeginButtonEnable = true;
            CloseButtonEnable = false;
            CloseButtonVisibility = Visibility.Collapsed;

            if (isDbRev)
                DatabaseRevCal();
            else
                DatabaseVersionCal();
        }



        private async void DatabaseRevCal()
        {
            int localRev = App.LangConfig.DatabaseRev;
            int serverRev = App.LangConfigServer.LangDatabaseRevised;

            RevCompareNum = serverRev - localRev;
            int count = 0;

            if (!Directory.Exists(DownloadFolder))
                Directory.CreateDirectory(DownloadFolder);

            var db = new LangTextRepository();

            for (int i = 1; i <= RevCompareNum; i++)
            {
                DatabaseRevDownloadList.Add(App.ServerPath + DatabaseRevPath + (localRev + i).ToString() + ".zip");
            }

            CurrentExcuteText = "共有 " + RevCompareNum.ToString() + " 个数据文件下载。";

            foreach (var path in DatabaseRevDownloadList)
            {
                Debug.WriteLine(path);
                count++;

                CurrentExcuteText = "当前正在下载和导入第 " + count.ToString() + " 项，共有 " + RevCompareNum.ToString() + " 个数据文件下载。" +
                    "\n如果窗口突然假死请勿强行结束程序，目前导入时会导致此问题。\n如果完成后没有自动退出此窗口，可以点击关闭按钮。";

                Uri uri = new Uri(path);
                string filename = Path.GetFileName(uri.LocalPath);

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadProgressChanged += Editor_DownloadProgressChanged;
                        await client.DownloadFileTaskAsync(uri, DownloadFolder + filename);
                        //client.DownloadFileCompleted += DatabaseZipExtractAndImport(DownloadFolder + filename);
                    }

                    catch(WebException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    
                }

                DatabaseZipExtractAndImport(DownloadFolder + filename);

                //await Task.Run(() => DownloadDatabaseRevFile(path));
                //Task.Wait(DownloadDatabaseRevFile(path));

            }

            CloseButtonEnable = true;
            CloseButtonVisibility = Visibility.Visible;

        }

        private void CompletedImportCheck()
        {
            if (TaskCount == RevCompareNum)
            {
                var langconfig = App.LangConfig;
                langconfig.DatabaseRev = App.LangConfigServer.LangDatabaseRevised;
                AppConfigClient.Save(langconfig);
                DialogHost.CloseDialogCommand.Execute(null, null);
                App.LangNetworkService.CompareServerConfig();
            }

        }

        private async void DatabaseVersionCal()
        {
            string downloadPath = App.ServerPath + App.LangConfigServer.LangDatabasePath;
            
            Uri uri = new Uri(downloadPath);
            string filename = Path.GetFileName(uri.LocalPath);

            string localPath = App.WorkingDirectory + @"\" + filename;

            var fileExist = GetFileExistAndSha256(localPath, App.LangConfigServer.LangDatabasePackSha256);

            if (fileExist)
            {

                try
                {
                    CurrentExcuteText = "正在处理数据库相关任务……";

                    DownloadSpeed = "正在解压……";
                    ZipFile.ExtractToDirectory(filename, App.WorkingDirectory, true);

                    var dbCheck = new StartupDBCheck(@"Data\LangData_v3.db", @"Data\LangData_v3.update");

                    DownloadSpeed = "对比数据库……";
                    await dbCheck.ProcessUpdateMerge();

                    DownloadSpeed = "完成";


                    var langconfig = App.LangConfig;
                    langconfig.LangDatabaseVersion = App.LangConfigServer.LangDatabaseVersion;
                    AppConfigClient.Save(langconfig);
                    App.LangNetworkService.CompareServerConfig();
                    File.Delete(localPath);

                    DialogHost.CloseDialogCommand.Execute(null, null);
                    CloseButtonEnable = true;
                    CloseButtonVisibility = Visibility.Visible;

                }
                catch (Exception e)
                {
                    MessageBox.Show("解压出错！" + Environment.NewLine + e.ToString());
                }
            }
            else
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        CurrentExcuteText = "正在下载全量数据库，下载后会自动导出已翻译的内容。";

                        client.DownloadProgressChanged += Editor_DownloadProgressChanged;
                        await client.DownloadFileTaskAsync(uri, localPath);
                        //client.DownloadFileCompleted += DatabaseZipExtractAndImport(DownloadFolder + filename);
                    }

                    catch (WebException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                }

                DatabaseVersionCal();
            }

        }


        //private void DownloadDatabaseRevFile(string path)
        //{
        //    //JsonDto langText;
        //    Uri uri = new Uri(path);
        //    string filename = Path.GetFileName(uri.LocalPath);

        //    using (WebClient client = new WebClient())
        //    {
        //        client.DownloadProgressChanged += Editor_DownloadProgressChanged;
        //        client.DownloadFileAsync(uri, DownloadFolder + filename);
        //        //client.DownloadFileCompleted += DatabaseZipExtractAndImport(DownloadFolder + filename);
                
                
        //    }
            
        //}




        //private Task DelegateUnzip(object s, AsyncCompletedEventArgs e)
        //{
        //    DatabaseZipExtractAndImport((string)s);

        //    return Task.CompletedTask;
        //}

        private void DatabaseZipExtractAndImport(string filename)
        {
            List<string> fileList = new List<string>();
            LangFileParser parseLangFile = new LangFileParser();

            using (ZipArchive archive = ZipFile.OpenRead(filename))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    fileList.Add(entry.FullName);
                    Debug.WriteLine(entry.FullName);
                }
            }
            ZipFile.ExtractToDirectory(filename, DownloadFolder, true);

            foreach (var path in fileList)
            {
                ImportDataToDb(parseLangFile.JsonToDtoReader(DownloadFolder + path));
            }
            TaskCount++;
            CompletedImportCheck();
            //Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
            //{
                
            //};
            //return new AsyncCompletedEventHandler(action);
        }

        private void ImportDataToDb(JsonFileDto json)
        {
            var db = new LangTextRepository();

            switch (json.ChangeType)
            {
                case LangChangeType.Added:
                    db.AddNewLangs(json.LangTexts);
                    break;
                case LangChangeType.ChangedEN:
                    db.UpdateLangsEN(json.LangTexts);
                    break;
                case LangChangeType.ChangedZH:
                    db.UpdateLangsZH(json.LangTexts);
                    break;
                case LangChangeType.Removed:
                    db.DeleteLangs(json.LangTexts);
                    break;
            }
        }

        void Editor_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadSpeed = "进度：" 
                + e.ProgressPercentage 
                + "%，已下载大小：" 
                + SizeSuffix(e.BytesReceived) 
                + "，总大小："
                + SizeSuffix(e.TotalBytesToReceive);
        }

        private bool GetFileExistAndSha256(string filePath, string fileSHA265)
        {
            string hashReslut;

            if (File.Exists(filePath))
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    Debug.WriteLine(filePath);
                    SHA256Managed sha = new SHA256Managed();
                    byte[] hash = sha.ComputeHash(stream);
                    hashReslut = BitConverter.ToString(hash).Replace("-", String.Empty);
                }
                return fileSHA265 == hashReslut;
            }
            else
            {
                return false;
            }
        }

        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
