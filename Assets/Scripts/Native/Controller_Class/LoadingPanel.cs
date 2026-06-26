using Native.Event_Class;
using Native.UIKit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VFramework;

namespace Native
{
    public class LoadingPanel : MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture() => GameManager.Interface;
        private IUnRegister _percentage;
        private IUnRegister _info;
        public Slider percentageSlider;
        public TMP_Text percentageText;
        public TMP_Text progressText;
        private void OnEnable()
        {
            _percentage = this.RegisterEvent<UpdateProgressPercEvent>(e =>
            {
                UpdateProgressPercentage(e.ProgressPercentage);
            });
            _info = this.RegisterEvent<UpdateProgressInfoEvent>(e =>
            {
                UpdateProgressInfo(e.Info);
            });
        }

        void UpdateProgressPercentage(float value)
        {
            percentageSlider.value = value;
            percentageText.text = value.ToString("F1");
        }

        void UpdateProgressInfo(string info)
        {
            progressText.text = info;
        }

        private void OnDisable()
        {
            _percentage.UnRegister();
            _info.UnRegister();
            
        }
    }
}