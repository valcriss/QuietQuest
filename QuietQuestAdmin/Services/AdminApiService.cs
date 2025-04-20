using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuietQuestAdmin.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace QuietQuestAdmin.Services
{
    public class AdminApiService
    {
        private readonly HttpClient _http;

        public AdminApiService(string baseAddress)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public Task<ClientStatus?> GetStatusAsync()
            => _http.GetFromJsonAsync<ClientStatus>("status");

        public Task<ClientStatus?> UpdateConfigAsync(bool? active, int? threshold)
            => _http.PostAsJsonAsync("config", new { active, threshold })
                    .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<ClientStatus>())
                    .Unwrap();

        public Task TriggerPenaltyAsync()
            => _http.PostAsync("penalty", null);
    }
}
