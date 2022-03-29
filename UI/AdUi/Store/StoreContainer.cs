using AdUi.Shared;
using Microsoft.JSInterop;

namespace AdUi.Store
{
    public class StoreContainer
    {
        private readonly IJSInProcessRuntime _js;
        public StoreContainer(IJSInProcessRuntime js)
        {
            _js = js;
            PersistState();
        }

        #region Counter
        private int _currentCount;

        public int CurrentCount
        {
            get => _currentCount;
            set
            {
                _currentCount = value;
                _js.InvokeVoid(JsInteropConstant.SetSessionStorage, nameof(CurrentCount), value);
                NotifyStateChanged();
            }
        }
        #endregion

        #region IsLoading
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyStateChanged();
            }
        }
        #endregion

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();


        #region PersistState

        private void PersistState()
        {
            var data = _js.Invoke<string>(JsInteropConstant.GetSessionStorage, nameof(CurrentCount));
            int.TryParse(data, out _currentCount);
        }

        #endregion
    }
}
