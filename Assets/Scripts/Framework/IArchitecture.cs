using System;

namespace Framework
{
    public interface IArchitecture
    {
        void RegisterSystem<T>(T instance) where T : class,ISystem;
        void RegisterModel<T>(T instance) where T : class,IModel;
        void RegisterUtility<T>(T instance) where T :class,IUtility;
        T GetSystem<T>() where T : class,ISystem;
        T GetModel<T>() where T : class,IModel;
        T GetUtility<T>() where T : class,IUtility;
        void SendCommand<T>(T command) where T : ICommand,new();
        void SendCommand<T>() where T : ICommand,new();
        void SendEvent<T>() where T : new(); 
        void SendEvent<T>(T e); 
        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void UnRegisterEvent<T>(Action<T> onEvent); 
    }
}