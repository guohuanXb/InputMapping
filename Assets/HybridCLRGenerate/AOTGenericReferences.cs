using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Unity.InputSystem.dll",
		"UnityEngine.CoreModule.dll",
		"VFramework.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<Example.Event_Class.BindingChangedEvent>
	// System.Action<Example.Event_Class.RebindStateChangedEvent>
	// System.Action<object>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<object>
	// System.Func<object,byte>
	// System.Predicate<UnityEngine.InputSystem.InputBinding>
	// System.Predicate<object>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray.Enumerator<UnityEngine.InputSystem.InputBinding>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray.Enumerator<object>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray<UnityEngine.InputSystem.InputBinding>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray<object>
	// VFramework.Architecture.<>c<object>
	// VFramework.Architecture<object>
	// }}

	public void RefMethods()
	{
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Resources.Load<object>(string)
		// VFramework.IUnRegister VFramework.FrameworkExtension.RegisterEvent<Example.Event_Class.BindingChangedEvent>(VFramework.IRegisterEvent,System.Action<Example.Event_Class.BindingChangedEvent>)
		// VFramework.IUnRegister VFramework.FrameworkExtension.RegisterEvent<Example.Event_Class.RebindStateChangedEvent>(VFramework.IRegisterEvent,System.Action<Example.Event_Class.RebindStateChangedEvent>)
		// System.Void VFramework.FrameworkExtension.SendCommand<object>(VFramework.ISendCommand,object)
		// System.Void VFramework.FrameworkExtension.SendEvent<Example.Event_Class.BindingChangedEvent>(VFramework.ISendEvent,Example.Event_Class.BindingChangedEvent)
		// System.Void VFramework.FrameworkExtension.SendEvent<Example.Event_Class.RebindStateChangedEvent>(VFramework.ISendEvent,Example.Event_Class.RebindStateChangedEvent)
		// VFramework.IUnRegister VFramework.IArchitecture.RegisterEvent<Example.Event_Class.BindingChangedEvent>(System.Action<Example.Event_Class.BindingChangedEvent>)
		// VFramework.IUnRegister VFramework.IArchitecture.RegisterEvent<Example.Event_Class.RebindStateChangedEvent>(System.Action<Example.Event_Class.RebindStateChangedEvent>)
		// System.Void VFramework.IArchitecture.RegisterModel<object>(object)
		// System.Void VFramework.IArchitecture.RegisterSystem<object>(object)
		// System.Void VFramework.IArchitecture.RegisterUtility<object>(object)
		// System.Void VFramework.IArchitecture.SendCommand<object>(object)
		// System.Void VFramework.IArchitecture.SendEvent<Example.Event_Class.BindingChangedEvent>(Example.Event_Class.BindingChangedEvent)
		// System.Void VFramework.IArchitecture.SendEvent<Example.Event_Class.RebindStateChangedEvent>(Example.Event_Class.RebindStateChangedEvent)
	}
}