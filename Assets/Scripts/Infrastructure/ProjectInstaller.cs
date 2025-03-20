#region

using Zenject;

#endregion

namespace Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IResourceManager>().To<ResourceManager>().AsSingle().NonLazy();
        }
    }
}