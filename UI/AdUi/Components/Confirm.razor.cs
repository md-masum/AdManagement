using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;

namespace AdUi.Components
{
    public partial class Confirm
    {
        private bool ConfirmDlgVisible { get; set; } = false;
        [Parameter] public string Title { get; set; }
        [Parameter] public string Content { get; set; }
        [Parameter] public EventCallback<string> ConfirmDelete { get; set; }
        private string Value { get; set; }

        private ResizeDirection[] DialogResizeDirections { get; set; } = new ResizeDirection[] { ResizeDirection.All };

        public void Show(string value)
        {
            ConfirmDlgVisible = true;
            Value = value;
        }

        public async Task OnConfirmChange(bool value)
        {
            ConfirmDlgVisible = false;
            if (value)
            {
                await ConfirmDelete.InvokeAsync(Value);
            }
        }

        private void HideDialog(Object e)
        {
            ConfirmDlgVisible = false;
        }
    }
}
