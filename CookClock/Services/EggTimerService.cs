using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CookClock.Services
{
    public class EggTimerService : IDisposable
    {
        private readonly Lock _lock = new();

        private System.Timers.Timer? _timer;

        private int _remainingSeconds;
        private int _totalSeconds;
        private bool _running;
        private bool _paused;

        public event Action? StateChanged;
        public event Action? Finished;

        public int RemainingSeconds => _remainingSeconds;
        public int TotalSeconds => _totalSeconds;

        public bool IsRunning => _running;
        public bool IsPaused => _paused; 

        public double Progress => _totalSeconds == 0 ? 0 : (double)_remainingSeconds / TotalSeconds; //прогресс заполнения колечка 

        public string FormatedTime => TimeSpan.FromSeconds(_remainingSeconds).ToString(@"mm\:ss"); //выбор формата

        public void Start(int seconds)
        {
            lock (_lock) {
                StopInternal();

                _totalSeconds = seconds;
                _remainingSeconds = seconds;

                _running = true;
                _paused = false;

                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = true;
                _timer.Start();
            }

            NotifyStateChanged();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            bool completed = false;

            lock (_lock)
            {
                if (!_running || _paused)
                    return;

                _remainingSeconds--;

                if (_remainingSeconds <= 0)
                {
                    _remainingSeconds = 0;
                    completed = true;

                    StopInternal();
                }
            }

            NotifyStateChanged();

            if (completed)
            {
                Finished?.Invoke();
            }
        }

        public void Pause()
        {
            lock (_lock)
            {
                if (!_running || _paused)
                    return;

                _timer?.Stop();
                _paused = true;
            }

            NotifyStateChanged();
        }

        public void Resume()
        {
            lock (_lock)
            {
                if (!_running || !_paused)
                    return;

                _timer?.Start();
                _paused = false;
            }

            NotifyStateChanged();
        }

        public void Stop()
        {
            lock (_lock)
            {
                StopInternal();
            }

            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StateChanged?.Invoke();
            });
        }

        private void StopInternal()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            _running = false;
            _paused = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
