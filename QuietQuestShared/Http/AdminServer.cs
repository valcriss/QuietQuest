using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuietQuestShared.Audio;
using QuietQuestShared.Models;
using QuietQuestShared.Penalties;

namespace QuietQuestShared.Http
{
    public class AdminServer
    {
        private readonly HttpListener _listener = new();
        private readonly AudioMonitor _audioMonitor;
        private readonly PenaltyManager _penaltyManager;
        private readonly Config _config;

        private const string Prefix = "http://*:5005/";

        public AdminServer(AudioMonitor monitor, PenaltyManager penaltyMgr, Config cfg)
        {
            _audioMonitor = monitor;
            _penaltyManager = penaltyMgr;
            _config = cfg;
            _listener.Prefixes.Add(Prefix);
        }

        public void Start()
        {
            _listener.Start();
            _ = Task.Run(ServeLoop);
            Console.WriteLine($"Admin HTTP server listening on {Prefix}");
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Admin HTTP server stopped.");
        }

        private async Task ServeLoop()
        {
            while (_listener.IsListening)
            {
                var ctx = await _listener.GetContextAsync();
                _ = Task.Run(() => Handle(ctx));
            }
        }

        private async Task Handle(HttpListenerContext ctx)
        {
            try
            {
                string path = ctx.Request.Url?.AbsolutePath?.ToLower() ?? "/";
                switch (path)
                {
                    case "/status":
                        await WriteJson(ctx, new
                        {
                            _config.Active,
                            _config.Threshold,
                            _penaltyManager.IsPenaltyRunning,
                            _penaltyManager.LastTriggered,
                            _penaltyManager.LastPenaltyName
                        });
                        break;

                    case "/config" when ctx.Request.HttpMethod == "POST":
                        var cfg = await JsonSerializer.DeserializeAsync<ConfigUpdate>(ctx.Request.InputStream);
                        if (cfg?.Threshold is not null) _config.Threshold = cfg.Threshold.Value;
                        if (cfg?.Active is not null) _config.Active = cfg.Active.Value;
                        await WriteJson(ctx, _config);
                        break;

                    case "/penalty" when ctx.Request.HttpMethod == "POST":
                        _penaltyManager.Trigger();
                        await WriteJson(ctx, new { ok = true });
                        break;

                    default:
                        ctx.Response.StatusCode = 404;
                        break;
                }
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = 500;
                await WriteJson(ctx, new { error = ex.Message });
            }
            finally
            {
                ctx.Response.Close();
            }
        }

        private static async Task WriteJson(HttpListenerContext ctx, object obj)
        {
            ctx.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(obj);
            byte[] buf = Encoding.UTF8.GetBytes(json);
            await ctx.Response.OutputStream.WriteAsync(buf);
        }

        private record ConfigUpdate(int? Threshold, bool? Active);
    }
}
