﻿using ESO_LangEditor.Core.Entities;
using ESO_LangEditor.Core.EnumTypes;
using ESO_LangEditor.Core.Models;
using ESO_LangEditor.GUI.NetClient.Old;
using ESO_LangEditor.GUI.Command;
using ESO_LangEditor.GUI.Services;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESO_LangEditor.GUI.ViewModels
{
    public class LangTextReviewWindowViewModel : BindableBase
    {
        private string _searchResultInfo;
        private string _selectedInfo;
        private string _networkInfo;
        private ObservableCollection<LangTextForReviewDto> _gridData;
        private ObservableCollection<UserInClientDto> _userList;
        private UserInClientDto _selectedUser;
        private List<LangTextForReviewDto> _gridSelectedItems;
        private LangTextForReviewDto _gridSelectedItem;
        private bool _isReviewSelectedItems = true;
        private LangtextNetService _langtextNetService = new LangtextNetService(App.ServerPath);
        private ILangTextRepoClient _langTextRepoClient; // = new LangTextRepoClientService();
        private Dictionary<Guid, UserInClient> _userDict;
        private string _selectedItemInfo;

        public string SearchResultInfo
        {
            get { return _searchResultInfo; }
            set { SetProperty(ref _searchResultInfo, "查询到 " + value + " 条文本"); }
        }

        public string SelectedInfo
        {
            get { return _selectedInfo; }
            set { SetProperty(ref _selectedInfo, "已选择 " + value + " 条文本"); }
        }

        public string NetworkInfo
        {
            get { return _networkInfo; }
            set { SetProperty(ref _networkInfo, value); }
        }

        public ObservableCollection<LangTextForReviewDto> GridData
        {
            get { return _gridData; }
            set { SetProperty(ref _gridData, value); }
        }

        public List<LangTextForReviewDto> GridSelectedItems
        {
            get { return _gridSelectedItems; }
            set { SetProperty(ref _gridSelectedItems, value); }
        }

        public ObservableCollection<UserInClientDto> UserList
        {
            get { return _userList; }
            set { SetProperty(ref _userList, value); }
        }

        public UserInClientDto SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }

        public bool IsReviewSelectedItems
        {
            get { return _isReviewSelectedItems; }
            set { SetProperty(ref _isReviewSelectedItems, value); }
        }

        public string SelectedItemInfo
        {
            get { return _selectedItemInfo; }
            set { SetProperty(ref _selectedItemInfo, value); }
        }

        public LangTextForReviewDto GridSelectedItem 
        {
            get { return _gridSelectedItem; }
            set { SetProperty(ref _gridSelectedItem, value); }
        }

        public ExcuteViewModelMethod QueryReviewPendingItems => new ExcuteViewModelMethod(QueryReviewUserList);
        public ExcuteViewModelMethod SubmitApproveItems => new ExcuteViewModelMethod(SubmitApproveItemsToServer);
        public ExcuteViewModelMethod SubmitDenyItems => new ExcuteViewModelMethod(SubmitDenyItemsToServer);

        IEventAggregator _ea;
        
        public LangTextReviewWindowViewModel(IEventAggregator ea, ILangTextRepoClient langTextRepoClient)
        {
            _ea = ea;
            _langTextRepoClient = langTextRepoClient;
        }

        public async void QueryReviewItemsBySelectedUser(object o)
        {
            var token = App.LangConfig.UserAuthToken;
            GridData = null;
            NetworkInfo = "正在尝试读取……";

            try
            {
                var list = await _langtextNetService.GetLangtextInReviewAsync(SelectedUser.Id.ToString(), token);

                if (list == null && list.Count == 0)
                {
                    NetworkInfo = "读取完成，无待审核条目";
                }
                else
                {
                    GridData = new ObservableCollection<LangTextForReviewDto>(list);
                    SearchResultInfo = GridData.Count.ToString();
                    NetworkInfo = "读取完成";
                }
            }
            catch(HttpRequestException ex)
            {
                NetworkInfo = ex.Message;
            }
        }

        public async void QueryReviewUserList(object o)
        {
            var token = App.LangConfig.UserAuthToken;
            UserList = null;
            NetworkInfo = "正在尝试读取……";
            var userDtoList = new List<UserInClientDto>();

            if (_userDict == null)
            {
                //_userDict = await _langTextRepoClient.GetUsersToDict();
            }

            try
            {
                var list = await _langtextNetService.GetUsersInReviewAllAsync(token);
                
                foreach(var user in list)
                {
                    userDtoList.Add(new UserInClientDto{ Id = user, UserNickName = _userDict[user].UserNickName });
                }
                UserList = new ObservableCollection<UserInClientDto>(userDtoList);
                NetworkInfo = "读取完成";
            }
            catch (HttpRequestException ex)
            {
                NetworkInfo = ex.Message;
            }

        }

        public async void SubmitApproveItemsToServer(object o)
        {
            var token = App.LangConfig.UserAuthToken;
            NetworkInfo = "正在上传并等待服务器执行……";

            List<Guid> langIdList;
            List<LangTextForReviewDto> langTextForReviewDtos;

            if (IsReviewSelectedItems)
            {
                langTextForReviewDtos = GridSelectedItems;
            }
            else
            {
                langTextForReviewDtos = GridData.ToList();
            }

            langIdList = langTextForReviewDtos.Select(lang => lang.Id).ToList();
            Debug.WriteLine("langIdList count: " + langIdList.Count);

            try
            {
                var respondCode = await _langtextNetService.PutReviewListIdAsync(langIdList, token);

                if (respondCode == HttpStatusCode.OK)
                {
                    foreach(var selected in langTextForReviewDtos)
                    {
                        GridData.Remove(selected);
                    }
                    NetworkInfo = "执行完成";
                }
            }
            catch (HttpRequestException ex)
            {
                NetworkInfo = ex.Message;
            }
        }

        public async void SubmitDenyItemsToServer(object o)
        {

        }

        public async void SetSelectedItemInfo()
        {
            var localOldLang = await FindLangtext();
            string localOldTextZh = "读取本地修改前文本出错！";

            if (GridSelectedItem != null)
            {
                if (GridSelectedItem.ReasonFor == ReviewReason.ZhChanged 
                    || GridSelectedItem.ReasonFor == ReviewReason.Deleted)
                {
                    localOldTextZh = "修改前中文(本地)：" + localOldLang.TextZh;
                }

                if (GridSelectedItem.ReasonFor == ReviewReason.EnChanged)
                {
                    localOldTextZh = "修改前英文(本地)：" + localOldLang.TextEn;
                }

                SelectedItemInfo = "文本唯一ID：" + GridSelectedItem.TextId
                + "\n" + "文本类型：(" + GridSelectedItem.IdType + ")" + App.iDCatalog.GetCategory(GridSelectedItem.IdType)
                + "\n" + "英文：" + GridSelectedItem.TextEn
                + "\n" + "中文：" + GridSelectedItem.TextZh
                + "\n" + localOldTextZh;

            }
            else
            {
                localOldTextZh = "当前文本不存在于本地数据库。";
                SelectedItemInfo = localOldTextZh;
            }


        }

        public async Task<LangTextClient> FindLangtext()
        {
            LangTextClient lang = new LangTextClient();

            if (GridSelectedItem != null && GridSelectedItem.ReasonFor != ReviewReason.NewAdded)
            {
                lang = await _langTextRepoClient.GetLangTextByGuidAsync(GridSelectedItem.Id);
            }
            return lang;
        }


    }
}
