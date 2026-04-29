using System;

namespace Framework
{
    public interface IGetUtility : IGetArchitecture
    {
        
    }

    public interface IGetModel : IGetArchitecture
    {
        
    }

    public interface IGetSystem : IGetArchitecture
    {
        
    }
    
    public interface ISendCommand :IGetArchitecture
    {
        
    }


    public interface ISendEvent : IGetArchitecture
    {
    }

    public interface IRegisterEvent : IGetArchitecture
    {
    }

    public static class FrameworkExtension
    {
        public static T GetUtility<T>(this IGetUtility self) where T : class, IUtility
        {
            return self.GetArchitecture().GetUtility<T>();
        }
        public static T GetModel<T>(this IGetModel self) where T : class, IModel
        {
            return self.GetArchitecture().GetModel<T>();
        }
        
        public static T GetSystem<T>(this IGetSystem self) where T : class, ISystem
        {
            return self.GetArchitecture().GetSystem<T>();
        }
        
        
        public static void SendCommand<T>(this ISendCommand self) where T : ICommand, new()
        {
            self.GetArchitecture().SendCommand<T>();
        }
    
        public static void SendCommand<T>(this ISendCommand self,T command) where T : ICommand,new()
        {
            self.GetArchitecture().SendCommand<T>(command);
        }
        
        public static void SendEvent<T>(this ISendEvent self) where T : new()
        {
            self.GetArchitecture().SendEvent<T>();
        }

        public static void SendEvent<T>(this ISendEvent self, T e)
        {
            self.GetArchitecture().SendEvent<T>(e);
        }
        public static IUnRegister RegisterEvent<T>(this IRegisterEvent self,Action<T> onEvent)
        {
            return self.GetArchitecture().RegisterEvent<T>(onEvent);
        }
        
        public static void UnRegisterEvent<T>(this IRegisterEvent self,Action<T> onEvent)
        {
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
        }
        
    }
}