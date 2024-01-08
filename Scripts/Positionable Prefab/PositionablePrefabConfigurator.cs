using Bindito.Core;
using System;
using Timberborn.TemplateSystem;

namespace Battery.WoodWorks.PositionablePrefab
{
  public class PositionablePrefabConfigurator: IConfigurator
  {
    public void Configure(IContainerDefinition containerDefinition)
    {
      containerDefinition.MultiBind<TemplateModule>().ToProvider(new Func<TemplateModule>(ProvideTemplateModule)).AsSingleton();
    }

    private static TemplateModule ProvideTemplateModule()
    {
      TemplateModule.Builder builder = new TemplateModule.Builder();
      builder.AddDecorator<PositionablePrefabSpecification, PositionablePrefab>();
      return builder.Build();
    }
  }
}