﻿using AutoMapper;
using ESO_LangEditor.Core.Entities;
using ESO_LangEditor.Core.EnumTypes;
using ESO_LangEditor.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ESO_LangEditor.GUI.Services
{
    public class BackendService : IBackendService
    {

        private readonly ILangTextRepoClient _langTextRepo;
        private readonly ILangTextAccess _langTextAccess;
        private readonly IUserAccess _userAccess;
        private readonly IMapper _mapper;
        private readonly IEventAggregator _ea;

        public BackendService(IEventAggregator ea, ILangTextRepoClient langTextRepoClient, 
            ILangTextAccess langTextAccess, IUserAccess userAccess, IMapper Mapper)
        {
            _langTextRepo = langTextRepoClient;
            _langTextAccess = langTextAccess;
            _userAccess = userAccess;
            _mapper = Mapper;
            _ea = ea;
        }


        public async Task LangtextZhUpdateUpload(LangTextForUpdateZhDto langTextUpdateZhDto)
        {
            var code = await _langTextAccess.UpdateLangTextZh(langTextUpdateZhDto);

            Debug.WriteLine("langID: {0}, langZh: {1}", langTextUpdateZhDto.Id, langTextUpdateZhDto.TextZh);

            if (code == ApiMessageWithCode.Success)
            {
                langTextUpdateZhDto.IsTranslated = 3;
                await _langTextRepo.UpdateLangtextZh(langTextUpdateZhDto);
            }

            //MainWindowMessageQueue.Enqueue("状态码：" + code.ToString());
        }

        public async Task LangtextZhUpdateUpload(List<LangTextForUpdateZhDto> langTextUpdateZhDtos)
        {
            var code = await _langTextAccess.UpdateLangTextZh(langTextUpdateZhDtos);

            if (code == ApiMessageWithCode.Success)
            {
                foreach (var lang in langTextUpdateZhDtos)
                {
                    lang.IsTranslated = 3;
                }
                await _langTextRepo.UpdateLangtextZh(langTextUpdateZhDtos);
            }

            //MainWindowMessageQueue.Enqueue("状态码：" + code.ToString());
        }

        //public Task StartupConnectSequenceCheck()
        //{
        //    IStartupCheck startupService = new StartupCheck();



        //}

        public async Task SyncToken()
        {
            var timer = new System.Threading.Timer(async e => await _userAccess.GetTokenByDto(new TokenDto
            {
                AuthToken = App.LangConfig.UserAuthToken,
                RefreshToken = App.LangConfig.UserRefreshToken,
            }),
            null, TimeSpan.Zero, TimeSpan.FromMinutes(10));


            //while (ConnectStatus == ClientConnectStatus.Login)
            //{
            //    await Task.Delay(TimeSpan.FromMinutes(10));

            //    await Task.Run(() =>
            //    {
            //        _accountService.LoginByToken();
            //    });

            //}
        }

        public async Task SyncUser()
        {
            Debug.WriteLine("SYNC USER");
            var userListFromServer = await _userAccess.GetUserList(App.LangConfig.UserAuthToken);

            if (userListFromServer != null)
            {
                var userList = _mapper.Map<List<UserInClient>>(userListFromServer);
                await _langTextRepo.UpdateUsers(userList);
            }
        }
    }
}
