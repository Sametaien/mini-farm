#region

using Data;
using Infrastructure;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

#endregion

namespace UI
{
    public class ResourceUIView : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private TextMeshProUGUI breadAmountText;

        [SerializeField] private TextMeshProUGUI flourAmountText;
        [SerializeField] private TextMeshProUGUI wheatAmountText;
        private readonly CompositeDisposable _disposable = new();


        private IResourceManager _resourceManager;

        private void OnEnable()
        {
            _resourceManager.OnResourceChanged
                .Subscribe(resourceInfo =>
                {
                    var type = resourceInfo.resourceType;
                    var newAmount = resourceInfo.newAmount;

                    switch (type)
                    {
                        case ResourceType.Bread:
                            breadAmountText.text = newAmount.ToString();
                            break;
                        case ResourceType.Flour:
                            flourAmountText.text = newAmount.ToString();
                            break;
                        case ResourceType.Wheat:
                            wheatAmountText.text = newAmount.ToString();
                            break;
                    }
                })
                .AddTo(_disposable);

            UpdateAllResourceTexts();
        }

        private void OnDisable()
        {
            _disposable.Clear();
        }

        [Inject]
        public void Construct(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        private void UpdateAllResourceTexts()
        {
            breadAmountText.text = _resourceManager.GetResourceAmount(ResourceType.Bread).ToString();
            flourAmountText.text = _resourceManager.GetResourceAmount(ResourceType.Flour).ToString();
            wheatAmountText.text = _resourceManager.GetResourceAmount(ResourceType.Wheat).ToString();
        }
    }
}