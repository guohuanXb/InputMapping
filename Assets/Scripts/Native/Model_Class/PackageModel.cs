using VFramework;

namespace Native
{
    public interface IPackageModel : IModel
    {
        string DefaultPackageName { get; }
    }

    public class PackageModel :AbstractModel,IPackageModel
    {
        protected override void OnInit()
        {
            DefaultPackageName = "DefaultPackage";
        }

        public string DefaultPackageName { get; private set; }
    }
}