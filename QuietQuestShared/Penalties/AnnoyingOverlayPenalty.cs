using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuietQuestShared.Penalties
{
    public sealed class AnnoyingOverlayPenalty : IPenalty
    {
        public string Name => "Fenêtre gênante";

        private readonly string _gifPath;

        public AnnoyingOverlayPenalty(string gifPath)
        {
            _gifPath = gifPath;
        }

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            var tcs = new TaskCompletionSource();
            var thread = new Thread(() =>
            {
                var window = new System.Windows.Window
                {
                    WindowStyle = System.Windows.WindowStyle.None,
                    AllowsTransparency = true,
                    Background = System.Windows.Media.Brushes.Transparent,
                    Topmost = true,
                    ShowInTaskbar = false,
                    WindowState = System.Windows.WindowState.Maximized,
                    Content = new System.Windows.Controls.Image
                    {
                        Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_gifPath, UriKind.RelativeOrAbsolute)),
                        Opacity = 0.35,
                        Stretch = System.Windows.Media.Stretch.Uniform
                    }
                };

                window.Show();

                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = duration
                };
                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    window.Close();
                };
                timer.Start();

                System.Windows.Threading.Dispatcher.Run();
                tcs.SetResult();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            await Task.WhenAny(tcs.Task, Task.Delay(duration, token));
        }
    }
}
