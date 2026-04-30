using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Example
{
    public class InputMappingController : MonoBehaviour, IController
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private GameObject bindingRowPrefab;
        [SerializeField] private Transform bindingListContainer;
        [SerializeField] private Button resetAllButton;
        [SerializeField] private GameObject rebindOverlay;
        [SerializeField] private TMP_Text rebindOverlayPromptText;
        [SerializeField] private Button rebindCancelButton;

        public IArchitecture GetArchitecture() => GameArchitecture.Interface;

        private void Awake()
        {
            var map = inputActionAsset.FindActionMap("PC");
            var system = this.GetSystem<IPlayerInputSystem>();
            system.Initialize(map);
            
            // foreach (var action in map)
            // {
            //     var row = Instantiate(bindingRowPrefab, bindingListContainer);
            //     var rowController = row.GetComponent<BindingRowController>();
            //     rowController.Setup(action.name);
            // }
            
            // foreach (var actionMap in inputActionAsset.actionMaps)
            // {
            //     foreach (var action in actionMap.actions)
            //     {
            //         var row = Instantiate(bindingRowPrefab, bindingListContainer);
            //         var rowController = row.GetComponent<BindingRowController>();
            //         rowController.Setup(action.name);
            //     }
            // }

            this.RegisterEvent<RebindStateChangedEvent>(OnRebindStateChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start()
        {
            // resetAllButton.onClick.AddListener(() =>
            // {
            //     this.SendCommand<ResetAllBindingsCommand>();
            // });
            //
            // if (rebindCancelButton != null)
            // {
            //     rebindCancelButton.onClick.AddListener(() =>
            //     {
            //         var system = this.GetSystem<IPlayerInputSystem>();
            //         system.CancelRebind();
            //     });
            // }
        }

        private void OnRebindStateChanged(RebindStateChangedEvent e)
        {
            if (rebindOverlay != null)
            {
                rebindOverlay.SetActive(e.IsRebinding);
            }
            if (rebindOverlayPromptText != null && e.IsRebinding)
            {
                rebindOverlayPromptText.text = "Press a key for " + e.ActionName + "...";
            }
        }

        private void OnDestroy()
        {
            // var system = this.GetSystem<IPlayerInputSystem>();
            // if (system.IsRebinding)
            // {
            //     system.CancelRebind();
            // }
        }
    }
}
