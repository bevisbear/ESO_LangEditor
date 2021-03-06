﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESO_LangEditor.GUI.Command
{
    public class LangOpenDialogCommand : CommandBase
    {
        private readonly Action<object> _executeMethod;

        public LangOpenDialogCommand(Action<object> execute)
        {
            _executeMethod = execute;
        }

        public override void ExecuteCommand(object parameter)
        {
            _executeMethod(parameter);
        }
    }
}
