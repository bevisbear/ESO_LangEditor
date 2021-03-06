﻿using AutoMapper;
using ESO_LangEditor.Core.Entities;
using ESO_LangEditor.Core.EnumTypes;
using ESO_LangEditor.Core.Models;
using ESO_LangEditor.GUI.EventAggres;
using NLog;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ESO_LangEditor.GUI.Services
{
    public class StartupCheck : IStartupCheck
    {
        private int RevCompareNum = 0;
        private int TaskCount = 0;

        private int _RevNumberLocal = 0;
        private int _RevNumberServer = 0;

        private AppConfigServer _langConfigServer;

        private IEventAggregator _ea;
        private ILangTextRepoClient _langTextRepo;
        private ILangTextAccess _langTextAccess;
        private IUserAccess _userAccess;
        private IMapper _mapper;
        private JsonSerializerOptions _jsonOption;
        private ILogger _logger;
        

        public StartupCheck(IEventAggregator ea, ILangTextRepoClient langTextRepo, 
            ILangTextAccess langTextAccess, IUserAccess userAccess, IMapper Mapper, ILogger logger)
        {
            _ea = ea;
            _langTextRepo = langTextRepo;
            _langTextAccess = langTextAccess;
            _userAccess = userAccess;
            _mapper = Mapper;
            _logger = logger;

            _jsonOption = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task StartupTaskList()
        {
            _ea.GetEvent<ConnectProgressString>().Publish("正在读取服务器版本数据……");
            _ea.GetEvent<ConnectStatusChangeEvent>().Publish(ClientConnectStatus.Connecting);

            _langConfigServer = await GetServerRespondAndConfig();
            

            if (_langConfigServer != null)
            {
               
                _logger.Debug("获取服务器配置成功");

                if (App.LangConfig.LangUpdaterVersion != _langConfigServer.LangUpdaterVersion)
                {
                    await UpdateUpdater();
                }
                else
                {
                    _logger.Debug("更新器版本已最新");
                }

                if (App.LangConfig.LangEditorVersion != _langConfigServer.LangEditorVersion)
                {
                    UpdateEditor();
                }
                else
                {
                    _logger.Debug("编辑器版本已最新");
                }

            }
        }

        public async Task Login()
        {
            if (App.LangConfig.UserAuthToken != "" & App.LangConfig.UserRefreshToken != "")
            {
                var Logintoken = await _userAccess.GetTokenByDto(new TokenDto
                {
                    AuthToken = App.LangConfig.UserAuthToken,
                    RefreshToken = App.LangConfig.UserRefreshToken,
                });

                if (Logintoken != null)
                {
                    _logger.Debug("登录并获取Token成功");

                    _userAccess.SaveToken(Logintoken);
                    await LoginTaskList();
                }
            }

            if (App.LangConfig.UserAuthToken == "" || App.LangConfig.UserRefreshToken == "")
            {
                _ea.GetEvent<LoginRequiretEvent>().Publish();
            }
        }

        private async Task LoginTaskList()
        {
            await GetConfigFromDb();
            _RevNumberServer = await _langTextAccess.GetLangTextRevisedNumber();

            if (_RevNumberServer != 0 && _RevNumberLocal != _RevNumberServer)
            {
                _logger.Debug("本地步进号：" + _RevNumberLocal);
                _logger.Debug("服务器端步进号：" + _RevNumberServer);
                await SyncRevDatabase();
            }
            else
            {
                _logger.Debug("步进号已最新");
            }
        }


        public void UpdateEditor()
        {
            _logger.Debug("发现编辑器版本更新");

            string args = App.ServerPath + _langConfigServer.LangFullpackPath    //服务器地址 + 编辑器包名
                + " " + _langConfigServer.LangFullpackSHA256   //编辑器服务器端SHA256
                + " " + _langConfigServer.LangEditorVersion;   //编辑器服务器端版本号
            Debug.WriteLine(args);

            ProcessStartInfo startUpdaterInfo = new ProcessStartInfo
            {
                FileName = "ESO_LangEditorUpdater.exe",
                Arguments = args,
            };

            Process updater = new Process
            {
                StartInfo = startUpdaterInfo
            };
            updater.Start();
            Application.Current.Shutdown();

        }

        public async Task UpdateUpdater()
        {
            _logger.Debug("发现更新器版本更新");
            _ea.GetEvent<ConnectProgressString>().Publish("正在下载更新器……");

            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DelegateHashAndUnzip);
                await client.DownloadFileTaskAsync(new Uri(App.ServerPath + _langConfigServer.LangUpdaterPackPath),
                "UpdaterPack.zip");
            }
        }

        private void DelegateHashAndUnzip(object sender, AsyncCompletedEventArgs e)
        {
            _ea.GetEvent<ConnectProgressString>().Publish("下载完成！");
            Debug.WriteLine("下载完成！");

            Task.Delay(1000);

            if (GetFileExistAndSha256("UpdaterPack.zip", _langConfigServer.LangUpdaterPackSha256))
            {
                _ea.GetEvent<ConnectProgressString>().Publish("SHA256校验通过，准备解压文件。");
                Debug.WriteLine("SHA256校验通过，准备解压文件。");

                ZipFile.ExtractToDirectory("UpdaterPack.zip", App.WorkingDirectory, true);

                App.LangConfig.LangUpdaterDllSha256 = _langConfigServer.LangUpdaterDllSha256;
                App.LangConfig.LangUpdaterVersion = _langConfigServer.LangUpdaterVersion;

                AppConfigClient.Save(App.LangConfig);

                File.Delete("UpdaterPack.zip");
            }
            else
            {
                _ea.GetEvent<ConnectProgressString>().Publish("校验SHA256失败！");
                _logger.Fatal("=====新版更新器更新失败======");
            }
        }


        public Task DownloadFullDatabase()
        {
            throw new NotImplementedException();
        }

        public async Task SyncRevDatabase()
        {
            _ea.GetEvent<ConnectProgressString>().Publish("发现更新数据库……");
            _ea.GetEvent<DatabaseUpdateEvent>().Publish(true);

            int RevCount = _RevNumberServer - _RevNumberLocal;
            int revised = _RevNumberLocal + 1;

            Debug.WriteLine("Revised number: {0}", revised);
            _logger.Debug("步进计数：" + RevCount);
            _logger.Debug("当前新步进号：" + revised);

            Dictionary<Guid, ReviewReason> langtextDeletedDict = new Dictionary<Guid, ReviewReason>();
            Dictionary<Guid, ReviewReason> langtextAddedDict = new Dictionary<Guid, ReviewReason>();
            Dictionary<Guid, ReviewReason> langtextUpdateDict = new Dictionary<Guid, ReviewReason>();

            for (int i = 1; i <= RevCount; i++)
            {
                _logger.Debug("当前步进：" + i);
                _ea.GetEvent<ImportDbRevDialogStringMainEvent>().Publish("正在下载需要同步的文本列表(" + i + "/" + RevCount + ")");
                //获取当前步进号的步进列表
                var langRevisedDto = await _langTextAccess.GetLangTextRevisedDtos(revised);

                if (langRevisedDto != null)
                {
                    _logger.Debug("当前langRevisedDto数量：" + langRevisedDto.Count());
                    foreach (var rev in langRevisedDto)
                    {
                        switch (rev.ReasonFor)
                        {
                            case ReviewReason.Deleted:
                                langtextDeletedDict.Add(rev.LangtextID, rev.ReasonFor);
                                break;
                            case ReviewReason.NewAdded:
                                langtextAddedDict.Add(rev.LangtextID, rev.ReasonFor);
                                break;
                            case ReviewReason.EnChanged:
                                langtextUpdateDict.Add(rev.LangtextID, rev.ReasonFor);
                                break;
                            case ReviewReason.ZhChanged:
                                langtextUpdateDict.Add(rev.LangtextID, rev.ReasonFor);
                                break;
                        }
                    }

                    _ea.GetEvent<ImportDbRevDialogStringSubEvent>().Publish("正在获取服务器文本");
                    //查询langtext的步进编号
                    var currentRevLangDto = await _langTextAccess.GetLangTexts(revised);
                    if (currentRevLangDto != null)
                    {
                        Debug.WriteLine("langtext from server count: {0}", currentRevLangDto.Count);
                        _logger.Debug("当前步进服务器端lang文本数量：" + currentRevLangDto.Count);

                        List<LangTextClient> newLangtexts = new List<LangTextClient>();
                        List<LangTextClient> langtextToUpdateList = new List<LangTextClient>();

                        foreach (var lang in currentRevLangDto)
                        {
                            if (langtextAddedDict.ContainsKey(lang.Id))
                            {
                                var langclient = _mapper.Map<LangTextClient>(lang);
                                newLangtexts.Add(langclient);
                            }

                            if (langtextUpdateDict.ContainsKey(lang.Id))
                            {
                                var langtext = await _langTextRepo.GetLangTextByGuidAsync(lang.Id);
                                _mapper.Map(lang, langtext, typeof(LangTextDto), typeof(LangTextClient));
                                langtextToUpdateList.Add(langtext);
                            }
                        }

                        _ea.GetEvent<ImportDbRevDialogStringMainEvent>().Publish("更新文本列表正在写入数据库(" + i + "/" + RevCount + ")");

                        if (await _langTextRepo.AddLangtexts(newLangtexts))//应用新增文本
                        {
                            _logger.Debug("当前步进新增：" + newLangtexts.Count);
                            _logger.Debug("当前步进新增文本完成");
                        }
                        
                        if (await _langTextRepo.UpdateLangtexts(langtextToUpdateList))//应用修改文本
                        {
                            _logger.Debug("当前步进修改：" + langtextToUpdateList.Count);
                            _logger.Debug("当前步进修改文本完成");
                        }
                           
                        if (langtextDeletedDict.Count >= 1)
                        {
                            if (await _langTextRepo.DeleteLangtexts(langtextDeletedDict.Keys.ToList()))//应用删除文本
                            {
                                _logger.Debug("当前步进删除：" + langtextDeletedDict.Count);
                                _logger.Debug("当前步进删除文本完成");
                            }
                        }

                        if(await _langTextRepo.UpdateRevNumber(revised))    //更新步进号
                        {
                            _logger.Debug("当前步进号新增完成");
                            revised++;
                        }

                        langtextDeletedDict.Clear();
                        langtextAddedDict.Clear();
                        langtextUpdateDict.Clear();
                    }
                }
                else
                {
                    _logger.Fatal("====当前步进号文本列表下载失败====");
                    _ea.GetEvent<ImportDbRevDialogStringMainEvent>().Publish("文本列表下载失败！");
                    _ea.GetEvent<ImportDbRevDialogStringSubEvent>().Publish("请尝试重启工具");
                }
                _logger.Debug("全部步进号更新完成");
                _ea.GetEvent<ImportDbRevDialogStringMainEvent>().Publish("更新完成！");
                _ea.GetEvent<ImportDbRevDialogStringSubEvent>().Publish("如果本窗口没有自动关闭，请点击下边的关闭按钮");
                _ea.GetEvent<CloseMainWindowDrawerHostEvent>().Publish();
            }
        }

        private async Task<AppConfigServer> GetServerRespondAndConfig()
        {
            AppConfigServer result = null;
            HttpClient client = App.HttpClient;

            HttpResponseMessage response = await client.GetAsync("AppConfig.json");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<AppConfigServer>(responseContent, _jsonOption);
            }
            return result;
        }

        private async Task GetConfigFromDb()
        {
            if (App.LangConfig.UserGuid != new Guid())
            {
                var user = await _langTextRepo.GetUserInClient(App.LangConfig.UserGuid);
                var userDto = _mapper.Map<UserInClientDto>(user);

                App.User = userDto;

                //Debug.WriteLine("id: {0}, name: {1}", App.User.Id, App.User.UserNickName);
            }

            _RevNumberLocal = await _langTextRepo.GetLangtextRevNumber();
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

        
    }
}
