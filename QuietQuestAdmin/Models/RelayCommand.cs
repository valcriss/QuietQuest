using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuietQuestAdmin.Models
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _exec;
        public RelayCommand(Action<object?> exec) => _exec = exec;
        public bool CanExecute(object? _) => true;
        public void Execute(object? p) => _exec(p);
        public event EventHandler? CanExecuteChanged;
    }
}
