#region

using Infrastructure;
using Zenject;

#endregion

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IResourceManager>().To<ResourceManager>().AsSingle();
    }
}