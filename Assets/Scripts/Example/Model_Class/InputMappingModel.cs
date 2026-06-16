using VFramework;

namespace Example.Model_Class
{
    public interface IInputMappingModel : IModel
    {
        
    }

    public class InputMappingModel : AbstractModel, IInputMappingModel
    {
        protected override void OnInit()
        {
        }
    }
}
