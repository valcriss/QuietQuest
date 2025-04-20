using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using QuietQuestAdmin.Models;
using QuietQuestAdmin.Services;
using System.Windows.Input;

namespace QuietQuestAdmin.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AdminApiService _api;
        private ClientStatus _status;

        public MainViewModel()
        {
            _api = new AdminApiService("http://127.0.0.1:5005/"); // adresse du client
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync());
            ToggleActiveCommand = new RelayCommand(async _ => await ToggleActiveAsync());
            SetThresholdCommand = new RelayCommand(async _ => await SetThresholdAsync());
            TriggerPenaltyCommand = new RelayCommand(async _ => await _api.TriggerPenaltyAsync());
            // on démarre la première récup
            Task.Run(RefreshAsync);
        }

        public ClientStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public string ThresholdInput { get; set; } = "";

        public ICommand RefreshCommand { get; }
        public ICommand ToggleActiveCommand { get; }
        public ICommand SetThresholdCommand { get; }
        public ICommand TriggerPenaltyCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        private async Task RefreshAsync()
            => Status = await _api.GetStatusAsync();

        private async Task ToggleActiveAsync()
            => Status = await _api.UpdateConfigAsync(!Status.Active, null);

        private async Task SetThresholdAsync()
        {
            if (int.TryParse(ThresholdInput, out var t))
                Status = await _api.UpdateConfigAsync(null, t);
        }
    }
}
