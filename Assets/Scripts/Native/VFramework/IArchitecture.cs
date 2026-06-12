using System;

namespace VFramework
{
    public interface IArchitecture
    {
        void RegisterSystem<T>(T instance) where T : ISystem;
        void RegisterModel<T>(T instance) where T : IModel;
        void RegisterUtility<T>(T instance) where T :IUtility;
        T GetSystem<T>() where T : class,ISystem;
        T GetModel<T>() where T : class,IModel;
        T GetUtility<T>() where T : class,IUtility;
        void SendCommand<T>(T command) where T : ICommand;
        TResult SendCommand<TResult>(ICommand<TResult> command);
        TResult SendQuery<TResult>(IQuery<TResult> query);
        void SendEvent<T>() where T : new(); 
        void SendEvent<T>(T e); 
        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void UnRegisterEvent<T>(Action<T> onEvent);
        void Deinit();
    }
}