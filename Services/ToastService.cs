using Timer = System.Timers.Timer;

namespace CardZen.Services
{
    public class ToastService
    {
        public class ToastItem
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Message { get; set; } = string.Empty;
            public string Background { get; set; } = "#323232";
        }

        public event Action? OnToastsUpdated;
        public List<ToastItem> Toasts { get; } = new();


        public void ShowToast(string message, string background = "#323232", int durationMs = 3000)
        {
            var toast = new ToastItem { Message = message, Background = background };
            Toasts.Add(toast);
            OnToastsUpdated?.Invoke();
            var timer = new Timer(durationMs) { AutoReset = false };
            timer.Elapsed += (s, e) => RemoveToast(toast.Id);
            timer.Start();
        }

        public void ShowSuccess(string message, int durationMs = 3000) => ShowToast(message, "#22c55e", durationMs); // green-500
        public void ShowInfo(string message, int durationMs = 3000) => ShowToast(message, "#2563eb", durationMs); // blue-600
        public void ShowWarning(string message, int durationMs = 3000) => ShowToast(message, "#f59e42", durationMs); // amber-500
        public void ShowError(string message, int durationMs = 3000) => ShowToast(message, "#ef4444", durationMs); // red-500

        public void RemoveToast(Guid id)
        {
            var toast = Toasts.Find(t => t.Id == id);
            if (toast != null)
            {
                Toasts.Remove(toast);
                OnToastsUpdated?.Invoke();
            }
        }
    }
}