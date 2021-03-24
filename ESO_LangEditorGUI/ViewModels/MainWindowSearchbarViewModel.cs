﻿using ESO_LangEditor.Core.EnumTypes;
using ESO_LangEditorGUI.Command;
using ESO_LangEditorGUI.EventAggres;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ESO_LangEditorGUI.ViewModels
{
    public class MainWindowSearchbarViewModel : BindableBase
    {

        private SearchPostion _selectedSearchPostion;
        private SearchTextType _selectedSearchTextType;
        private SearchTextType _selectedSearchTextTypeSecond;
        private string _keyword;
        private string _keywordSecond;
        private bool _doubleKeyWordSearch;
        private bool _serverSideSearch;
        private ClientConnectStatus _connectStatus;

        public ICommand SearchLangCommand { get; }

        public SearchPostion SelectedSearchPostion 
        {
            get { return _selectedSearchPostion; } 
            set { SetProperty(ref _selectedSearchPostion, value); }
        }

        public IEnumerable<SearchPostion> SearchPostionCombox
        {
            get { return Enum.GetValues(typeof(SearchPostion)).Cast<SearchPostion>(); }
            //set { _searchPostion =  }
        }

        public IEnumerable<SearchTextType> SearchTextTypeCombox
        {
            get { return Enum.GetValues(typeof(SearchTextType)).Cast<SearchTextType>(); }
            //set { _searchTextType = value; NotifyPropertyChanged(); }
        }

        public SearchTextType SelectedSearchTextType 
        {
            get { return _selectedSearchTextType; }
            set { SetProperty(ref _selectedSearchTextType, value); }
        }

        public SearchTextType SelectedSearchTextTypeSecond
        {
            get { return _selectedSearchTextTypeSecond; }
            set { SetProperty(ref _selectedSearchTextTypeSecond, value); }
        }

        public string Keyword
        {
            get { return _keyword; }
            set { SetProperty(ref _keyword, value); }
        }

        public string KeywordSecond
        {
            get { return _keywordSecond; }
            set { SetProperty(ref _keywordSecond, value); }
        }

        public bool DoubleKeyWordSearch
        {
            get { return _doubleKeyWordSearch; }
            set { SetProperty(ref _doubleKeyWordSearch, value); }
        }

        public bool ServerSideSearch
        {
            get { return _serverSideSearch; }
            set { SetProperty(ref _serverSideSearch, value); }
        }

        public ClientConnectStatus ConnectStatus
        {
            get { return _connectStatus; }
            set { SetProperty(ref _connectStatus, value); }
        }

        IEventAggregator _ea;

        public MainWindowSearchbarViewModel(IEventAggregator ea)
        {
            _ea = ea;
            SearchLangCommand = new SearchLangCommand(this, ea);

            _ea.GetEvent<ConnectStatusChangeEvent>().Subscribe(ChangeConnectStatus);
        }

        private void ChangeConnectStatus(ClientConnectStatus obj)
        {
            ConnectStatus = obj;
        }




        //protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


    }
}
