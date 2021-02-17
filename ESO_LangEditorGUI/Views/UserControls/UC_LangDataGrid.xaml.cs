﻿using ESO_LangEditor.Core.EnumTypes;
using ESO_LangEditor.Core.Models;
using ESO_LangEditorGUI.Command;
using ESO_LangEditorGUI.Converter;
using ESO_LangEditorGUI.EventAggres;
using ESO_LangEditorGUI.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ESO_LangEditorGUI.Views.UserControls
{
    /// <summary>
    /// UC_LangDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class UC_LangDataGrid : UserControl
    {
        //WindowController windowControll = new WindowController();
        private ContextMenu _menu = new ContextMenu();
        public ContextMenu _rowRightClickMenu = new ContextMenu();
        private LangTextDto _selectedItem;
        private List<LangTextDto> _selectedItems;

        private EnumDescriptionConverter _enumDescriptionConverter = new EnumDescriptionConverter();

        public ICommand LangDataGridCommand { get; }

        public IEnumerable<LangDataGridContextMenu> RowRightClickMenuEnum
        {
            get { return Enum.GetValues(typeof(LangDataGridContextMenu)).Cast<LangDataGridContextMenu>(); }
            //set { _searchPostion =  }
        }



        public UC_LangDataGrid()
        {
            InitializeComponent();
            //LangDataGridCommand = new LangDataGridCommand(this);

        }


        private void LangSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DataGrid datagrid = sender as DataGrid;
            Point aP = e.GetPosition(datagrid);
            IInputElement obj = datagrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;


            while (target != null)
            {
                if (target is DataGridRow & GetDataGridInWindowTag() == "Mainwindow")
                {
                    if (datagrid.SelectedIndex != -1)
                    {
                        //MakeSelectedItemToEventArgs();
                        //var vm = DataContext as MainWindowViewModel;
                        _selectedItem = (LangTextDto)datagrid.SelectedItem;
                        //vm.GridSelectedItem = _selectedItem;
                        MakeDouleClickSelectedItemToEventArgs(_selectedItem);
                        //new LangtextEditor((LangTextDto)datagrid.SelectedItem).Show();
                        //RaiseEvent(new RoutedEventArgs(DataGridDoubleClick));
                    }
                }

                //MessageBox.Show(target.ToString());
                target = VisualTreeHelper.GetParent(target);
            }


        }
        private void LangSearchDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            var tag = GetDataGridInWindowTag();

            if (tag == "Mainwindow")
            {
                _selectedItems = datagrid.SelectedItems.OfType<LangTextDto>().ToList();
                MakeSelectedItemToEventArgs(_selectedItems);
            }
            else if (tag == "LangtextEditor")
            {
                _selectedItem = (LangTextDto)datagrid.SelectedItem;
                MakeSelectedItemToEditorEventArgs(_selectedItem);
            }
            else if (tag == "ExportTranslateWindow")
            {
                _selectedItems = datagrid.SelectedItems.OfType<LangTextDto>().ToList();
                MakeSelectedItemToEventArgs(_selectedItems);
            }
        }



        private void LangDataGridRightClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            Point aP = e.GetPosition(datagrid);
            IInputElement obj = datagrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;
            //target = VisualTreeHelper.GetParent(target);

            while (target != null)
            {
                if (target is DataGridColumnHeader)
                {
                    HeaderRightClickMenuGenerater();
                }
                else if (target is DataGridRow & GetDataGridInWindowTag() == "Mainwindow")
                {
                    if (datagrid.SelectedIndex != -1)
                    {
                        RowRightClickMenuGenerater();
                    }
                }

                //MessageBox.Show(target.ToString());
                target = VisualTreeHelper.GetParent(target);
            }

        }

        private void HeaderRightClickMenuGenerater()
        {
            var visibleColumns = LangDataGrid.Columns.Where(c => c.Visibility == Visibility.Visible).Count();

            if (_menu.Items.Count == 0)
            {
                foreach (var column in LangDataGrid.Columns)
                {
                    var menuItem = new MenuItem
                    {
                        Header = column.Header.ToString(),
                        IsChecked = column.Visibility == Visibility.Visible,
                        IsCheckable = true,
                        // Don't allow user to hide all columns
                        IsEnabled = visibleColumns > 1 || column.Visibility != Visibility.Visible
                    };
                    // Bind events
                    menuItem.Checked += (object a, RoutedEventArgs ea)
                        => column.Visibility = Visibility.Visible;
                    menuItem.Unchecked += (object b, RoutedEventArgs eb)
                        => column.Visibility = Visibility.Collapsed;
                    _menu.Items.Add(menuItem);
                }
            }
            else
            {
                foreach (MenuItem item in _menu.Items)
                {
                    if (visibleColumns == 1 && item.IsChecked == true)
                    {
                        item.IsEnabled = false;
                    }
                    else
                    {
                        item.IsEnabled = true;
                    }
                }
            }
            _menu.IsOpen = true;
        }

        private void RowRightClickMenuGenerater()
        {

            if (_rowRightClickMenu.Items.Count == 0)
            {
                //_rowRightClickMenu.ItemsSource = RowRightClickMenuEnum;

                //foreach (MenuItem item in _rowRightClickMenu.Items)
                //{
                //    item.Header = _enumDescriptionConverter.GetEnumDescription((LangDataGridContextMenu)item.DataContext);
                //    item.Click += RowRightClickMenu_OnClick;
                //}

                foreach (var item in RowRightClickMenuEnum)
                {
                    var menuItem = new MenuItem
                    {
                        Header = _enumDescriptionConverter.GetEnumDescription(item),
                        DataContext = item,
                        //IsChecked = column.Visibility == Visibility.Visible,
                        //IsCheckable = true,
                        // Don't allow user to hide all columns
                        //IsEnabled = visibleColumns > 1 || column.Visibility != Visibility.Visible
                        //Command = LangDataGridCommand,
                        //CommandParameter = item,
                        //ItemsSource = RowRightClickMenuEnum,

                    };
                    menuItem.Click += RowRightClickMenu_OnClick;
                    //(object a, RoutedEventArgs ea)
                    //=> MessageBox.Show(menuItem.DataContext.ToString());
                    _rowRightClickMenu.Items.Add(menuItem);
                }
            }


            _rowRightClickMenu.IsOpen = true;
        }

        private void RowRightClickMenu_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            //ContextMenu cm = mi.Parent as ContextMenu;
            LangDataGridContextMenu _menuEnum = (LangDataGridContextMenu)menuitem.DataContext;

            switch (_menuEnum)
            {
                //LangDataGridContextMenu.EditCurrentItem => "编辑当前",
                case LangDataGridContextMenu.EditMutilItem:
                    //new TextEditor(GetSeletedItems()).Show();
                    new LangtextEditor(GetSeletedItems()).Show();
                    break;
                case LangDataGridContextMenu.SearchAndReplace:
                    new TextEditor_SearchReplace(_selectedItems).Show();
                    break;

                    //LangDataGridContextMenu.SearchAndReplace => "查找替换",

            };

            //MessageBox.Show(word);

        }

        private List<LangTextDto> GetSeletedItems()
        {
            var list = new List<LangTextDto>();

            foreach (var selectedItem in LangDataGrid.SelectedItems)
            {
                if (selectedItem != null)
                    list.Add((LangTextDto)selectedItem);
            }
            return list;
        }

        private string GetDataGridInWindowTag()
        {
            Window parentWindow = Window.GetWindow(this);

            return parentWindow.Tag.ToString();
        }

        public static readonly RoutedEvent DataGridDoubleClick = 
            EventManager.RegisterRoutedEvent("DataGridDoubleClick", RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(UC_LangDataGrid));


        public event RoutedEventHandler DataGridDoubleClicked
        {
            add { AddHandler(DataGridDoubleClick, value); }
            remove { RemoveHandler(DataGridDoubleClick, value); }
        }


        public static readonly RoutedEvent DataGridSelectionChangedEvent =
            EventManager.RegisterRoutedEvent("DataGridSelectChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UC_LangDataGrid));


        public event RoutedEventHandler DataGridSelectChanged
        {
            add { AddHandler(DataGridSelectionChangedEvent, value); }
            remove { RemoveHandler(DataGridSelectionChangedEvent, value); }
        }

        private void MakeSelectedItemToEventArgs(List<LangTextDto> langtextList)
        {
            DataGridSelectedChangedEventArgs args = new DataGridSelectedChangedEventArgs(DataGridSelectionChangedEvent, langtextList);
            this.RaiseEvent(args);
        }

        private void MakeDouleClickSelectedItemToEventArgs(LangTextDto langtext)
        {
            DataGridSelectedItemEventArgs args = new DataGridSelectedItemEventArgs(DataGridDoubleClick, langtext);
            this.RaiseEvent(args);
        }
        private void MakeSelectedItemToEditorEventArgs(LangTextDto langtext)
        {
            DataGridSelectedItemEventArgs args = new DataGridSelectedItemEventArgs(DataGridSelectionChangedEvent, langtext);
            this.RaiseEvent(args);
        }



        //private void OpenLangEditorWindow()
        //{
        //    //List<LangTextDto> selectedItems = (List<LangTextDto>)LangDataGrid.SelectedItems;

        //    TextEditor langEditor = new TextEditor(_selectedItems);

        //    //langEditor.UpdateData(_selectedItems);
        //    langEditor.Show();
        //}
    }
}