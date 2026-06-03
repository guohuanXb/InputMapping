using VFramework;

namespace Example.Model_Class
{
    public interface IInputMappingModel : IModel
    {
        // Binding override data is managed by InputActionAsset and persisted via JSON.
        // Model is reserved for future non-input-asset state (e.g. UI preferences).
    }

    public class InputMappingModel : AbstractModel, IInputMappingModel
    {
        protected override void OnInit()
        {
        }
    }
}
