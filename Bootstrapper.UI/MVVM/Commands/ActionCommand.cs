using System;
using System.Windows.Input;

namespace Bootstrapper.UI.MVVM.Commands
{
    public class ActionCommand : ICommand
    {
        private readonly Action Action;

        public ActionCommand(Action action)
        {
            Action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action();
        }
    }
}