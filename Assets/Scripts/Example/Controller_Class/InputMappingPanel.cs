using System;
using System.Collections.Generic;
using Example.Command_Class;
using Example.Controller_Class.Component;
using Example.Event_Class;
using Example.Query_Class;
using Example.System_Class;
using Native.UIKit.Framework;
using UnityEngine;
using VFramework;

namespace Example.Controller_Class
{
    public class InputMappingPanel : UIPanel,IController
    {
        public IArchitecture GetArchitecture() => HotUpdateEntry.Architecture;
        public ControlMappingComponent[] bindingComponents;
        public override UILayer Layer { get; protected set; } = UILayer.First;
        private void Start()
        {
            foreach (var comp in bindingComponents)
            {
                var captured = comp; // 闭包保护

                // 初始化显示
                captured.SetDisplay(
                    this.SendQuery(new BindingNameQuery(captured.actionName, captured.bindingIndex))
                );

                // 绑定按钮
                captured.button.onClick.AddListener(() =>
                {
                    this.SendCommand(
                        new BeginRebindCommand(captured.actionName, captured.bindingIndex)
                    );
                });
            }
            
            
            this.RegisterEvent<BeginRebindEvent>(OnBeginRebinding)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<RebindCompleteEvent>(OnRebindCompleted)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            
            this.RegisterEvent<CancelRebindEvent>(OnCancelRebinding)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            
        }

        private void OnDestroy()
        {
            foreach (var comp in bindingComponents)
            {
                comp.button.onClick.RemoveAllListeners();
            }
        }

        private void OnBeginRebinding(BeginRebindEvent e)
        {
            foreach (var comp in bindingComponents)
            {
                if (comp.actionName == e.ActionName && comp.bindingIndex == e.BindingIndex)
                {
                    comp.ClearDisplay();
                    return;
                }
            }
        }

        private void OnRebindCompleted(RebindCompleteEvent e)
        {
            foreach (var comp in bindingComponents)
            {
                if (comp.actionName == e.ActionName && comp.bindingIndex == e.BindingIndex)
                {
                    comp.SetDisplay(e.DisplayContent);
                    return;
                }
            }
        }

        private void OnCancelRebinding(CancelRebindEvent e)
        {
            foreach (var comp in bindingComponents)
            {
                if (comp.actionName == e.ActionName && comp.bindingIndex == e.BindingIndex)
                {
                    
                    comp.SetDisplay(this.SendQuery(new BindingNameQuery(e.ActionName, e.BindingIndex)));
                    return;
                }
            }
        }


        
    }
}