#region

using Infrastructure;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace UI
{
    public class InfoSlider : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Slider progressSlider;

        [SerializeField] private TextMeshProUGUI depositText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI capacityStatusText;

        private readonly CompositeDisposable _disposable = new();
        private FactoryModel _factoryModel;

        private void OnDestroy()
        {
            _disposable.Dispose();
        }

        public void SetFactoryModel(FactoryModel factoryModel)
        {
            _factoryModel = factoryModel;

            _disposable.Clear();

            _factoryModel.Deposit
                .Subscribe(amount => { depositText.text = $"{amount}"; })
                .AddTo(_disposable);

            _factoryModel.RemainingTime
                .CombineLatest(_factoryModel.IsDepositFull, (remaining, isFull) => (remaining, isFull))
                .Subscribe(tuple =>
                {
                    var (remaining, isFull) = tuple;

                    var progress = 1f - remaining / _factoryModel.ProductionTime;
                    progressSlider.value = Mathf.Clamp01(progress);

                   
                    if (isFull)
                        statusText.text = "Full";
                    else if (remaining > 0f)
                        statusText.text = $"{Mathf.Ceil(remaining)}s";
                    else
                        statusText.text = "Idle";
                })
                .AddTo(_disposable);

            _factoryModel.QueueCount
                .Subscribe(_ => UpdateCapacityStatusText())
                .AddTo(_disposable);

            UpdateCapacityStatusText();
        }

        private void UpdateCapacityStatusText()
        {
            var total = _factoryModel.QueueCount.Value + _factoryModel.Deposit.Value;
            var cap = _factoryModel.Capacity;
            capacityStatusText.text = $"{total}/{cap}";
        }
    }
}