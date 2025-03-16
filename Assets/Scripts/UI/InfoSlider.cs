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

            _factoryModel.CurrentProductionAmount
                .Subscribe(amount => { depositText.text = $"{amount}"; })
                .AddTo(_disposable);

            _factoryModel.RemainingTime
                .Subscribe(remaining =>
                {
                    var progress = 1f - remaining / _factoryModel.ProductionTime;
                    progressSlider.value = Mathf.Clamp01(progress);

                    if (_factoryModel.IsFull.Value)
                        statusText.text = "Full";
                    else
                        statusText.text = $"{Mathf.Ceil(remaining)}s";
                })
                .AddTo(_disposable);

            _factoryModel.IsFull
                .Subscribe(isFull =>
                {
                    if (isFull) statusText.text = "Full";
                })
                .AddTo(_disposable);
        }
    }
}