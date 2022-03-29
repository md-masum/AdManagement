using AdUi.Shared;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Notifications;

namespace AdUi.Service
{
    public enum ToastPositions
    {
        Center,
        Right,
        Left
    }

    public enum ToastTypes
    {
        Notification,
        VideoCall
    }

    public class ToastService
    {
        private readonly IJSInProcessRuntime _js;
        public ToastService(IJSInProcessRuntime js)
        {
            SfToastObj = new SfToast();
            ToastPosition = ToastPositions.Right.ToString();
            _js = js;
        }

        public SfToast SfToastObj { get; set; }
        public string ToastPosition { get; set; }
        public ToastTypes ToastTypes { get; set; }
        public void ShowInfo(string message, int timeOut)
        {
            ToastTypes = ToastTypes.Notification;
            var toastModel = new ToastModel
            {
                Title = "Information!",
                Icon = "e-info toast-icons",
                Content = message,
                Timeout = timeOut,
                CssClass = "e-toast-info"
            };
            ShowNotification(toastModel);
        }

        public void ShowWarn(string message, int timeOut)
        {
            ToastTypes = ToastTypes.Notification;
            var toastModel = new ToastModel
            {
                Title = "Warning!",
                Icon = "e-warning toast-icons",
                Content = message,
                Timeout = timeOut,
                CssClass = "e-toast-warning"
            };
            ShowNotification(toastModel);
        }

        public void ShowSuccess(string message, int timeOut)
        {
            ToastTypes = ToastTypes.Notification;
            var toastModel = new ToastModel
            {
                Title = "Success!",
                Icon = "e-success toast-icons",
                Content = message,
                Timeout = timeOut,
                CssClass = "e-toast-success"
            };
            ShowNotification(toastModel);
        }

        public void ShowError(string message, int timeOut)
        {
            ToastTypes = ToastTypes.Notification;
            var toastModel = new ToastModel
            {
                Title = "Error!",
                Icon = "e-error toast-icons",
                Content = message,
                Timeout = timeOut,
                CssClass = "e-toast-danger"
            };
            ShowNotification(toastModel);
        }

        private void ShowNotification(ToastModel model)
        {
            _js.InvokeVoid(JsInteropConstant.PlatNotification);
            SfToastObj.Show(model);
        }
    }
}
