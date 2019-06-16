using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void setProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                onPropertyChanged(name);
            }
        }

        protected void onPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region ICommand

        public class RelayCommand : ICommand
        {
            #region Fields
            readonly Action<object> _execute;
            readonly Predicate<object> _canExecute;
            #endregion // Fields

            #region Constructors

            public RelayCommand(Action<object> execute)
                : this(execute, null)
            {
            }

            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                _execute = execute ?? throw new ArgumentNullException("execute");
                _canExecute = canExecute;
            }
            #endregion // Constructors

            #region ICommand Members
            
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            [DebuggerStepThrough]
            public bool CanExecute(object parameter) => _canExecute == null ? true : _canExecute(parameter);
            public void raiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
            public void Execute(object parameter) => _execute(parameter);

            #endregion // ICommand Members
        }
        #endregion

        #region private fields

        #endregion
    }
}