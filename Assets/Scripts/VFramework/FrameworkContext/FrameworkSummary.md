# 框架知识摘要

## 架构概览

这是一个 Unity C# **MVCS 架构框架**（参考 QFramework），通过 `Architecture<T>` 单例中枢统一管理 Model、System、Command、Query、Utility 和 Event 之间的通信。

---

## 模块列表

| 文件 | 职责 |
|---|---|
| `Architecture.cs` | 框架中枢，单例门面，负责注册/获取/发送一切 |
| `IArchitecture.cs` | 顶层 API 接口 |
| `IOCContainer.cs` | 简易 DI 容器（按类型存储/检索实例） |
| `Singleton.cs` | 泛型单例基类（反射调用私有无参构造函数） |
| `IModel.cs` / `AbstractModel` | Model 层接口与抽象类 |
| `ISystem.cs` / `AbstractSystem` | System 层接口与抽象类 |
| `ICommand.cs` / `AbstractCommand` | Command 接口（void / `TResult`）与抽象类 |
| `IQuery.cs` / `AbstractQuery<T>` | 查询接口（只读，返回 `TResult`）与抽象类 |
| `IUtility.cs` | Utility 层标记接口（空接口） |
| `Rule.cs` + `FrameworkExtension` | 各 "Rule 接口"（`IGetSystem`、`ISendCommand` 等）及**扩展方法实现** |
| `TypeEventSystem.cs` | 类型事件系统 + `IUnRegister` + `UnRegisterTrigger` 自动注销 |
| `EasyEvent.cs` | 无参/1~3 参 `EasyEvent`、`EasyEvents` 字典 + `OrEvent` |
| `Event.cs` | 空文件（Unity using 占位） |
| `IOnEvent.cs` | `IOnEvent<T>` + 全局事件快捷注册扩展 |
| `ITypeEventSystem.cs` | 空接口文件 |
| `BindableProperty.cs` | `BindableProperty<T>`，值变化时触发回调 |
| `IInit.cs` | `IInit` 接口（`Init` / `Deinit` / `Initialized`） |
| `IGetArchitecture.cs` / `ISetArchitecture.cs` | 获取/设置 `IArchitecture` 的接口 |

---

## 关键 API

### Architecture\<T\>（中枢，继承后使用）

```csharp
// 初始化（触发所有 Model/System 的 Init）
GameArchitecture.InitArchitecture();  // 或访问 GameArchitecture.Interface 自动触发

// 注册（仅在 Init() 中调用）
RegisterModel<TModel>(instance);
RegisterSystem<TSystem>(instance);
RegisterUtility<TUtility>(instance);

// 发送
SendCommand(new SomeCommand());            // void 命令
TResult result = SendCommand(new SomeCmd<TResult>());  // 有返回值命令
TResult result = SendQuery(new SomeQuery<TResult>());   // 查询
SendEvent<MyEvent>();                      // 发送事件（new()）
SendEvent(myEventInstance);                // 发送事件实例

// 事件注册
IUnRegister token = RegisterEvent<T>(callback);
UnRegisterEvent<T>(callback);

// 销毁
Deinit();  // 反初始化所有 System → Model → 清空容器
```

### 各层通过扩展方法访问（来自 `Rule.cs`）

```csharp
// 在任何实现了对应 Rule 接口的类中直接使用：
this.GetSystem<T>();         // IGetSystem
this.GetModel<T>();          // IGetModel
this.GetUtility<T>();        // IGetUtility
this.SendCommand(new T());   // ISendCommand
this.SendCommand<T>(cmd);    // ISendCommand (泛型)
this.SendQuery(query);       // ISendQuery
this.SendEvent<T>();         // ISendEvent
this.RegisterEvent<T>(cb);   // IRegisterEvent → 返回 IUnRegister
```

### 接口继承链（谁拥有哪些能力）

| 接口 | 继承的 Rule 接口 | 说明 |
|---|---|---|
| `IController` | `IGetSystem, IGetModel, ISendCommand, IRegisterEvent, ISendQuery, IGetUtility` | MonoBehaviour 实现此接口 |
| `ISystem` | `ISetArchitecture, IGetModel, IGetUtility, IRegisterEvent, ISendEvent, IGetSystem, IInit` | 业务逻辑层 |
| `IModel` | `ISetArchitecture, IGetUtility, ISendEvent, IInit` | 数据层 |
| `ICommand` | `ISetArchitecture, IGetSystem, IGetModel, IGetUtility, ISendEvent, ISendCommand, ISendQuery` | 命令（void / `TResult`） |
| `IQuery<TResult>` | `ISetArchitecture, IGetModel, IGetSystem, ISendQuery` | 只读查询 |
| `IUtility` | 无（空接口标记） | 工具类 |

### 事件系统

```csharp
// 架构内事件
this.SendEvent<MyEvent>();             // 无参结构体事件
this.SendEvent(myInstance);            // 实例事件
var token = this.RegisterEvent<MyEvent>(e => { ... });
token.UnRegisterWhenGameObjectDestroyed(gameObject);  // 绑定到 GO 生命周期

// 全局事件（绕过架构）
TypeEventSystem.Global.Register<T>(callback);
TypeEventSystem.Global.Send<T>(e);

// EasyEvent 直接使用
var easyEvent = new EasyEvent();
easyEvent.Register(() => { ... });
easyEvent.Trigger();
```

### BindableProperty\<T\>

```csharp
var prop = new BindableProperty<string> { Value = "initial" };
prop.OnValueChanged += newVal => { /* UI 更新 */ };
prop.Value = "changed";  // 值不等时触发回调
// T 必须实现 IEquatable<T>
```

### IOCContainer

```csharp
container.Register<ISomeInterface>(instance);   // 类型→实例映射
var obj = container.Get<ISomeInterface>();      // 按类型检索
var list = container.GetInstancesByType<IModel>(); // 获取某接口的所有实例
container.Clear();                               // 清空
```

---

## 配置方式

1. **创建自定义 Architecture**：继承 `Architecture<T>`（T 为自身），如 `GameArchitecture : Architecture<GameArchitecture>`
2. **覆写 `Init()`**：在其中调用 `RegisterModel`、`RegisterSystem`、`RegisterUtility`
3. **初始化时机**：访问 `GameArchitecture.Interface` 自动触发，或显式调用 `GameArchitecture.InitArchitecture()`
4. **Controller 接入**：MonoBehaviour 实现 `IController`，`GetArchitecture()` 返回 `GameArchitecture.Interface`

---

## 示例流程（按键重绑定）

```
1. GameArchitecture.InitArchitecture()
   └─ Init():
        RegisterModel<IInputMappingModel>(new InputMappingModel())     // 存储绑定数据
        RegisterUtility<IStorage>(new PlayerPrefsStore())              // 持久化
        RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem())    // 输入系统逻辑

2. InputMappingController (MonoBehaviour, implements IController)
   └─ Awake():
        var system = this.GetSystem<IPlayerInputSystem>();            // 获取 System
        system.Initialize(inputActionMap);                             // 初始化输入映射
        this.RegisterEvent<RebindStateChangedEvent>(OnChanged)        // 订阅 UI 事件
            .UnRegisterWhenGameObjectDestroyed(gameObject);           // 自动注销

3. BindingRowController (MonoBehaviour, implements IController)
   └─ Setup(actionName, bindingIndex):
        var model = this.GetModel<IInputMappingModel>();              // 获取 Model
        model.GetBindingData(...).BindingPath.OnValueChanged += ...   // 绑定数据变化 → UI
   └─ OnRebindClick():
        this.GetSystem<IPlayerInputSystem>().BeginRebind(...)         // 直接调用 System

4. ResetBindingCommand (extends AbstractCommand)
   └─ OnExecute():
        var model = this.GetModel<IInputMappingModel>();              // Command 内访问 Model
        var system = this.GetSystem<IPlayerInputSystem>();            // Command 内访问 System
        system.ResetBindingOverride(...);
        this.SendEvent(new BindingChangedEvent{...});                 // 发送事件通知 UI
```

---

## 注意事项

1. **Architecture 是单例**：全局只有一个 `_instance`，`Deinit()` 后才可重新初始化
2. **T 自引用约束**：`Architecture<T>` 要求 `T : Architecture<T>, new()`，继承时必须 `class MyArch : Architecture<MyArch>`
3. **访问 Architecture 的口子是 `GetArchitecture()`**：所有扩展方法都通过 `this.GetArchitecture()` 路由；Controller 必须实现它
4. **Controller 不直接持有 Model/System 引用**：通过 `this.GetSystem<T>()` 等扩展方法按需获取，框架自动从 IOC 容器定位
5. **Model 不能发 Command/Query**：`IModel` 未继承 `ISendCommand` / `ISendQuery`，只能发事件
6. **System 也不能发 Command/Query**：`ISystem` 未继承 `ISendCommand` / `ISendQuery`
7. **Command 权限最大**：可访问 System、Model、Utility、发送事件、发送子 Command、发送 Query
8. **BindableProperty\<T\> 仅当 `T : IEquatable<T>` 时可用**；值变更检测依赖 `Equals()`
9. **事件注册必须管理生命周期**：使用 `UnRegisterWhenGameObjectDestroyed()` / `UnRegisterWhenDisabled()` 等方法防止内存泄漏
10. **热注册支持**：`_inited` 为 true 后注册的 Model/System 会立即调用 `Init()`
11. **`OnRegisterPatch`**：静态 Action，可在 `InitArchitecture()` 的 `Init()` 之后、批量 Init Model/System 之前注入额外逻辑（如注册第三方模块）
12. **`Singleton<T>` 仅能通过私有无参构造函数实例化**，要求类显式声明 private 无参构造器
