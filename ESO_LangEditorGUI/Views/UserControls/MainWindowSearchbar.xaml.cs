﻿using ESO_LangEditorGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ESO_LangEditorGUI.Views.UserControls
{
    /// <summary>
    /// MainWindowSearchbar.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowSearchbar : UserControl
    {
        public MainWindowSearchbar()
        {
            InitializeComponent();
            //DataContext = new MainWindowSearchbarViewModel();
        }

    }
}
