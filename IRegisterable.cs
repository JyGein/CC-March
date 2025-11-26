using Nanoray.PluginManager;
using Nickel;

namespace JyGein.March;

internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}