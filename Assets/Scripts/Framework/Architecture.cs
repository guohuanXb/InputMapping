using System;
using System.Collections.Generic;

namespace Framework
{
    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>,new()
    {
        private bool _inited = false;
        
        private static T _instance = null;

        public static IArchitecture Interface
        {
            get
            {
                if(_instance == null)
                    MakeSureArchitecture();
                return _instance;
            }
        }

        private IOCContainer _container = new IOCContainer();

        private List<IModel> _models = new();
        
        private List<ISystem> _systems = new();
        public static Action<T> OnRegisterPatch = architecture => { };

        static void MakeSureArchitecture()
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Init();

                OnRegisterPatch?.Invoke(_instance);
                
                foreach (var model in _instance._models)
                {
                    model.Init();
                }
                
                _instance._models.Clear();

                foreach (var system in _instance._systems)
                {
                    system.Init();
                }
                _instance._systems.Clear();
                _instance._inited = true;
            }
        }

        protected abstract void Init();

        public static void Register<T>(T instance) where T:class
        {
            MakeSureArchitecture();
            _instance._container.Register(instance);
        }

        public static T Get<T>() where T : class
        {
            MakeSureArchitecture();
            return _instance._container.Get<T>();
        }
        
        public void RegisterSystem<T>(T instance) where T : class,ISystem 
        {
            instance.SetArchitecture(this);
            _container.Register<T>(instance);
            
            if (_inited)
            {
                instance.Init();
            }
            else
            {
                _systems.Add(instance);
            }
        }

        public void RegisterModel<T>(T instance) where T : class, IModel
        {
            instance.SetArchitecture(this);
            _container.Register(instance);

            if (_inited)
            {
                instance.Init();
            }
            else
            {
                _models.Add(instance);
            }
        }

        public T1 GetSystem<T1>() where T1 : class, ISystem
        {
            return _container.Get<T1>();
        }

        public T GetModel<T>() where T : class, IModel
        {
            return _container.Get<T>();
        }
        
        public void RegisterUtility<T>(T instance) where T:class,IUtility
        {
            _container.Register(instance);
        }

        public T GetUtility<T>() where T : class,IUtility
        {
            return _container.Get<T>();
        }

        public void SendCommand<T1>(T1 command) where T1 : ICommand, new()
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        public void SendCommand<T1>() where T1 : ICommand, new()
        {
            var command = new T1();
            command.SetArchitecture(this);
            command.Execute();
        }
        private ITypeEventSystem _typeEventSystem = new TypeEventSystem();
        public void SendEvent<T1>() where T1 : new()
        {
            _typeEventSystem.Send<T1>();
        }

        public void SendEvent<T1>(T1 e)
        {
            _typeEventSystem.Send(e);
        }

        public IUnRegister RegisterEvent<T1>(Action<T1> onEvent)
        {
            return _typeEventSystem.Register<T1>(onEvent);
        }

        public void UnRegisterEvent<T1>(Action<T1> onEvent)
        {
            _typeEventSystem.UnRegister<T1>(onEvent);
        }

        
        
        
    }
}