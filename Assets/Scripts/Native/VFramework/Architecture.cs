using System;
using System.Collections.Generic;
using System.Linq;

namespace VFramework
{
    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>,new()
    {
        private bool _inited = false;
        
        private static T _instance = null;

        public static IArchitecture Interface
        {
            get
            {
                if (_instance == null)
                    InitArchitecture();
                return _instance;
            }
        }

        private IOCContainer _container = new IOCContainer();
        public static Action<T> OnRegisterPatch = architecture => { };

        public static void InitArchitecture()
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Init();

                OnRegisterPatch?.Invoke(_instance);
                
                foreach (var model in _instance._container.GetInstancesByType<IModel>().Where(m=>!m.Initialized))
                {
                    model.Init();
                    model.Initialized = true;
                }
                
                foreach (var system in _instance._container.GetInstancesByType<ISystem>()
                             .Where(m => !m.Initialized))
                {
                    system.Init();
                    system.Initialized = true;
                }
                
                _instance._inited = true;
            }
        }
        
        protected abstract void Init();
        public void Deinit()
        {
            OnDeinit();
            foreach (var system in _container.GetInstancesByType<ISystem>().Where(s => s.Initialized)) system.Deinit();
            foreach (var model in _container.GetInstancesByType<IModel>().Where(m => m.Initialized)) model.Deinit();
            _container.Clear();
            _instance = null;
        }
        protected virtual void OnDeinit()
        {
        }
        
        public void RegisterSystem<TSystem>(TSystem system) where TSystem :ISystem 
        {
            system.SetArchitecture(this);
            _container.Register(system);
            
            if (_inited)
            {
                system.Init();
                system.Initialized = true;
            }
        }

        public void RegisterModel<TModel>(TModel model) where TModel :IModel
        {
            model.SetArchitecture(this);
            _container.Register(model);

            if (_inited)
            {
                model.Init();
                model.Initialized = true;
            }
           
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return _container.Get<TSystem>();
        }

        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return _container.Get<TModel>();
        }
        
        public void RegisterUtility<TUtility>(TUtility utility) where TUtility:IUtility
        {
            _container.Register(utility);
        }

        public TUtility GetUtility<TUtility>() where TUtility : class,IUtility
        {
            return _container.Get<TUtility>();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command) => ExecuteCommand(command);

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand => ExecuteCommand(command);

        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
        {
            command.SetArchitecture(this);
            return command.Execute();
        }
        
        protected virtual void ExecuteCommand(ICommand command)
        {
            command.SetArchitecture(this);
            command.Execute();
        }
        
        public TResult SendQuery<TResult>(IQuery<TResult> query) => DoQuery<TResult>(query);

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }
        
        private TypeEventSystem _typeEventSystem = new TypeEventSystem();
        public void SendEvent<TEvent>() where TEvent : new()
        {
            _typeEventSystem.Send<TEvent>();
        }

        public void SendEvent<TEvent>(TEvent e)
        {
            _typeEventSystem.Send(e);
        }

        public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            return _typeEventSystem.Register<TEvent>(onEvent);
        }

        public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            _typeEventSystem.UnRegister<TEvent>(onEvent);
        }
    }
}