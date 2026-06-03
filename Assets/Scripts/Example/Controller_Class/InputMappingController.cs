using System;
using System.Collections.Generic;
using Example.Command_Class;
using Example.Controller_Class.Component;
using Example.Event_Class;
using Example.System_Class;
using UnityEngine;
using VFramework;

namespace Example.Controller_Class
{
    public class InputMappingController : MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture()
            => GameArchitecture.Interface;
        private Dictionary<string,List<RowComponent>> _componentDic = new();
        public RowComponent rowPrefabs;
        public Transform container;
        private void Start()
        {
            var inputSystem = this.GetSystem<IPlayerInputSystem>();
            this.RegisterEvent<BindingChangedEvent>(e =>
            {
                if (_componentDic.TryGetValue(e.ActionName, out var list))
                {
                    if (list[e.BindingIndex] != null)
                    {
                        list[e.BindingIndex].rowValue.text = inputSystem.GetBindingDisplayString(e.ActionName, e.BindingIndex);
                    }
                }
            });
            this.RegisterEvent<RebindStateChangedEvent>(e =>
            {
                if (_componentDic.TryGetValue(e.ActionName, out var list))
                {
                    if (list[e.BindingIndex] != null)
                    {
                        list[e.BindingIndex].rowValue.text = e.IsRebinding
                            ? "——"
                            : inputSystem.GetBindingDisplayString(e.ActionName, e.BindingIndex);
                    }
                }
            });
            foreach (var action in inputSystem.GetAllInputAction())
            {
                var bindingCount = inputSystem.GetBindingCount(action.name);
                var list = new List<RowComponent>(bindingCount);
                for (int i = 0; i < bindingCount; i++)
                    list.Add(null);

                for (int i = 0; i < bindingCount; i++)
                {
                    if (action.bindings[i].isComposite)
                        continue;

                    var row = Instantiate(rowPrefabs, container);
                    var actionName = action.name;
                    var bindingIndex = i;
                    row.rowValue.text = inputSystem.GetBindingDisplayString(actionName, bindingIndex);
                    row.rowTitle.text = action.bindings[bindingIndex].name == ""
                        ? action.bindings[bindingIndex].action
                        : action.bindings[bindingIndex].name;
                    row.rowButton.onClick.AddListener(() =>
                    {
                        this.SendCommand(new BeginRebindCommand(actionName, bindingIndex));
                    });
                    list[bindingIndex] = row;
                }

                _componentDic[action.name] = list;
            }
        }
    }
}