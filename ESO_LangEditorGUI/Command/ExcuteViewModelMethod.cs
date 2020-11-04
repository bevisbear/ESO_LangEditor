﻿using ESO_LangEditorGUI.Services;
using ESO_LangEditorGUI.ViewModels;
using ESO_LangEditorLib.Services.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESO_LangEditorGUI.Command
{
    public class ExcuteViewModelMethod : CommandBase
    {
        private readonly Action<object> _executeMethod;

        public ExcuteViewModelMethod(Action<object> execute)
        {
            _executeMethod = execute;
        }

        public override void ExecuteCommand(object parameter)
        {
            //ExportToLang();
            _executeMethod(parameter);
        }
    }
}