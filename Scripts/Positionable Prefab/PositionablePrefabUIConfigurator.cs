using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.EntityPanelSystem;

namespace Battery.WoodWorks.PositionablePrefab
{
  [Configurator(SceneEntrypoint.InGame)]
  public class PositionablePrefabUIConfigurator : IConfigurator
  {
    public void Configure(IContainerDefinition containerDefinition)
    {
      containerDefinition.Bind<PositionablePrefabUIFragment>().AsSingleton();
      containerDefinition.MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
    }

    private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
    {
      private readonly PositionablePrefabUIFragment _positionablePrefabUIFragment;

      public EntityPanelModuleProvider(PositionablePrefabUIFragment positionablePrefabUIFragment)
      {
        _positionablePrefabUIFragment = positionablePrefabUIFragment;
      }

      public EntityPanelModule Get()
      {
        EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
        builder.AddTopFragment(_positionablePrefabUIFragment);
        return builder.Build();
      }
    }
  }
}