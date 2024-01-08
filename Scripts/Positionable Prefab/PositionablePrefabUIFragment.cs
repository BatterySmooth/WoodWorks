using TimberApi.UiBuilderSystem;
using TimberApi.UiBuilderSystem.ElementSystem;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

namespace Battery.WoodWorks.PositionablePrefab
{
  public class PositionablePrefabUIFragment : IEntityPanelFragment
  {
    private readonly IResourceAssetLoader _resourceAssetLoader;
    private readonly UIBuilder _uiBuilder;
    private VisualElement _root;
    private ILoc _loc;
    private readonly InputService _inputService;
    private PositionablePrefab _positionablePrefab;
    private readonly DevModeManager _devModeManager;
    
    // Internal Params
    private bool DevMode => _devModeManager.Enabled;

    // Controls
    private VisualElement _xyPositioner;
    private Button _leftArrow;
    private Button _rightArrow;
    private Button _upArrow;
    private Button _downArrow;
    private Button _plus;
    private Button _minus;
    
    // Fields
    private VisualElement _xyChip;
    private VisualElement _zChip;
    private Label _vectorDisplayX;
    private Label _vectorDisplayY;
    private Label _vectorDisplayZ;

    public PositionablePrefabUIFragment(ILoc loc, UIBuilder uiBuilder, IResourceAssetLoader resourceAssetLoader, InputService inputService)
    {
      _loc = loc;
      _uiBuilder = uiBuilder;
      _resourceAssetLoader = resourceAssetLoader;
      _inputService = inputService;
    }
    
    public VisualElement InitializeFragment()
    {
      _root = GenerateUI();
      
      /*
       * Buttons:
       * Left/Right = X
       * Up/Down = Z
       * Height = Y
       */
      
      // Assign Controls
      _leftArrow = _root.Q<Button>("XYPositioner-LeftArrow");
      _leftArrow.clicked += () => _positionablePrefab.Move(new Vector3(-_positionablePrefab.DisplacementStep, 0f, 0f));
      _rightArrow = _root.Q<Button>("XYPositioner-RightArrow");
      _rightArrow.clicked += () => _positionablePrefab.Move(new Vector3(_positionablePrefab.DisplacementStep, 0f, 0f));
      _upArrow = _root.Q<Button>("XYPositioner-UpArrow");
      _upArrow.clicked += () => _positionablePrefab.Move(new Vector3(0f, 0f, _positionablePrefab.DisplacementStep));
      _downArrow = _root.Q<Button>("XYPositioner-DownArrow");
      _downArrow.clicked += () => _positionablePrefab.Move(new Vector3(0f, 0f, -_positionablePrefab.DisplacementStep));
      _plus = _root.Q<Button>("ZPositioner-Plus");
      _plus.clicked += () => _positionablePrefab.Move(new Vector3(0f, _positionablePrefab.DisplacementStep, 0f));
      _minus = _root.Q<Button>("ZPositioner-Minus");
      _minus.clicked += () => _positionablePrefab.Move(new Vector3(0f, -_positionablePrefab.DisplacementStep, 0f));
      
      // Assign Fields
      _xyPositioner = _root.Q<VisualElement>("XYPositioner");
      _xyChip = _root.Q<VisualElement>("XYPositioner-Chip");
      _zChip = _root.Q<VisualElement>("ZPositioner-Chip");
      _vectorDisplayX = _root.Q<Label>("Vector3Field-X");
      _vectorDisplayY = _root.Q<Label>("Vector3Field-Y");
      _vectorDisplayZ = _root.Q<Label>("Vector3Field-Z");

      _root.ToggleDisplayStyle(false);
      
      return _root;
    }

    public void ShowFragment(BaseComponent entity)
    {
      _positionablePrefab = entity.GetComponentFast<PositionablePrefab>();
      if (!(bool)_positionablePrefab)
      {
        _root.ToggleDisplayStyle(false);
        return;
      }
      
      _root.ToggleDisplayStyle(true);
    }

    public void ClearFragment()
    {
      _root.ToggleDisplayStyle(false);
    }

    public void UpdateFragment()
    {
      if (!(bool)_positionablePrefab)
      {
        _root.ToggleDisplayStyle(false);
        return;
      }
      
      // Fetch the active prefab displacement (coalesce to empty)
      Vector3 activeDisplacement = (bool)_positionablePrefab ? _positionablePrefab.Displacement : new Vector3();
      
      // Set chip positions to match displacement
      _xyChip.style.left = ConvertChipPosition(activeDisplacement.x);
      _xyChip.style.bottom = ConvertChipPosition(activeDisplacement.z);
      _zChip.style.bottom = ConvertChipPosition(activeDisplacement.y);
      
      // Set XYZ text fields
      _vectorDisplayX.text = activeDisplacement.x.ToString("0.0");
      _vectorDisplayY.text = activeDisplacement.y.ToString("0.0");
      _vectorDisplayZ.text = activeDisplacement.z.ToString("0.0");
      
      // Rotate Controls to respect camera position
      float prefabRotation = _positionablePrefab.TransformFast.rotation.eulerAngles.y;
      float cameraRotation = Camera.main.transform.localEulerAngles.y;
      float angleBetween = prefabRotation - cameraRotation;
      _xyPositioner.style.rotate = new StyleRotate(new Rotate(new Angle(angleBetween, AngleUnit.Degree)));
      
    }

    private VisualElement GenerateUI()
    {
      VisualElementBuilder root = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      
      // Positioning Header
      root.AddComponent(builder =>
      {
        builder.AddClass("tba-bg-3");
        builder.SetPadding(10);
        builder.AddPreset(factory => factory.Labels().DefaultBold("battery.WoodWorks.PositionablePrefab.UI.PositionablePrefab.Header", builder: builder => builder.SetStyle(style => {style.alignSelf = Align.Center;})));
      });
      
      // Positioning UI
      root.AddComponent(builder =>
      {
        // Styling
        builder.SetFlexDirection(FlexDirection.Column);
        builder.SetFlexWrap(Wrap.Wrap);
        builder.SetJustifyContent(Justify.SpaceAround);
        builder.SetAlignItems(Align.Center);
        builder.SetPadding(5);
        builder.AddClass("tba-bg-3");

        // Components
        builder.AddComponent(builder =>
        {
          // Styling
          builder.SetFlexDirection(FlexDirection.Row);
          builder.SetFlexWrap(Wrap.Wrap);
          builder.SetJustifyContent(Justify.SpaceAround);
          builder.SetAlignItems(Align.Center);
          
          builder.SetMargin(10);
          
          // Components
          builder.AddComponent(XYPositioner());
          builder.AddComponent(Spacer());
          builder.AddComponent(ZPositioner());
        });
        
        // Vector3 Display
        builder.AddComponent(Vector3Display());
      });
      
      return root.Build();
    }

    private VisualElement Header(string locKey)
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      fragment.AddComponent(builder =>
      {
        // Styling
        builder.SetJustifyContent(Justify.SpaceAround);
        builder.SetAlignItems(Align.Center);
        // builder.SetPadding(10);
        builder.SetStyle(style =>
        {
          style.paddingTop = 5;
          style.paddingBottom = 2;
        });

        // Component
        builder.AddPreset(factory => factory.Labels().DefaultText(locKey));
      });

      return fragment.Build();
    }
    
    private VisualElement XYPositioner()
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      // Header
      fragment.AddComponent(Header("battery.WoodWorks.PositionablePrefab.UI.PositionablePrefab.XYHeader"));
      
      // Components
      fragment.AddComponent(builder =>
      {
        // Styling
        builder.SetBackgroundImage(new StyleBackground(_resourceAssetLoader.Load<Sprite>("battery.WoodWorks/battery_WoodWorks/positionable-blueprint-dial-bg")));
          
        // Main component
        builder.AddComponent(builder =>
        {
          builder.SetName("XYPositioner");
          // Styling
          builder.SetFlexDirection(FlexDirection.Column);
          builder.SetFlexWrap(Wrap.Wrap);
          builder.SetJustifyContent(Justify.SpaceAround);
          builder.SetAlignItems(Align.Center);
          builder.SetPadding(9);
          builder.SetMargin(9);

          // Top Row
          builder.AddComponent(builder =>
          {
            // Style
            builder.SetFlexDirection(FlexDirection.Row);
            builder.SetFlexWrap(Wrap.Wrap);
            builder.SetJustifyContent(Justify.SpaceAround);
            builder.SetAlignItems(Align.Center);

            // Controls
            builder.AddPreset(factory => factory.Buttons().ArrowUp("XYPositioner-UpArrow"));
          });
          
          // Middle Row
          builder.AddComponent(builder =>
          {
            // Style
            builder.SetFlexDirection(FlexDirection.Row);
            builder.SetFlexWrap(Wrap.Wrap);
            builder.SetJustifyContent(Justify.SpaceAround);
            builder.SetAlignItems(Align.Center);

            // Controls
            builder.AddPreset(factory => factory.Buttons().ArrowLeft("XYPositioner-LeftArrow"));
            builder.AddComponent(XYPositionerDisplay());
            builder.AddPreset(factory => factory.Buttons().ArrowRight("XYPositioner-RightArrow"));
          });
          
          // Bottom Row
          builder.AddComponent(builder =>
          {
            // Style
            builder.SetFlexDirection(FlexDirection.Row);
            builder.SetFlexWrap(Wrap.Wrap);
            builder.SetJustifyContent(Justify.SpaceAround);
            builder.SetAlignItems(Align.Center);

            // Controls
            builder.AddPreset(factory => factory.Buttons().ArrowDown("XYPositioner-DownArrow"));
          });
        });
      });
      
      return fragment.Build();
    }
    
    private VisualElement XYPositionerDisplay()
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      fragment.AddComponent(builder => 
      {
        // Styling
        builder.SetStyle(style => 
        {
          style.width = new StyleLength(99);
          style.height = new StyleLength(99);
        });
        fragment.SetBackgroundImage(new StyleBackground(_resourceAssetLoader.Load<Sprite>("battery.WoodWorks/battery_WoodWorks/positionable-blueprint-ico")));
      });
      
      fragment.AddComponent(PositionChip("XYPositioner-Chip"));

      return fragment.Build();
    }

    private VisualElement ZPositioner()
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();

      // Styling
      fragment.SetStyle(style =>
      {
        style.flexGrow = 1;
      });
      
      // Header
      fragment.AddComponent(Header("battery.WoodWorks.PositionablePrefab.UI.PositionablePrefab.ZHeader"));
      // Components
      fragment.AddComponent(builder =>
      {
        // Styling
        builder.SetFlexDirection(FlexDirection.Row);
        builder.SetFlexWrap(Wrap.Wrap);
        builder.SetJustifyContent(Justify.SpaceAround);
        builder.SetAlignItems(Align.Center);
        
        // builder.SetPadding(10);
        
        builder.SetStyle(style =>
        {
          style.flexGrow = 1;
        });
        
        // Scale image
        builder.AddComponent(ZPositionerDisplay());
        // Buttons
        builder.AddComponent(builder =>
        {
          // Styling
          builder.SetFlexDirection(FlexDirection.Column);
          builder.SetFlexWrap(Wrap.Wrap);
          builder.SetJustifyContent(Justify.SpaceAround);
          builder.SetAlignItems(Align.Center);
          builder.SetPadding(5);
          
          // Buttons
          builder.AddPreset(factory => factory.Buttons().Plus("ZPositioner-Plus"));
          builder.AddPreset(factory => factory.Buttons().Minus("ZPositioner-Minus"));
        });
        
      });
      
      return fragment.Build();
    }

    private VisualElement ZPositionerDisplay()
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      fragment.AddComponent(builder => 
      {
        // Styling
        builder.SetStyle(style =>
        {
          style.width = new StyleLength(15);
          style.height = new StyleLength(99);
          style.backgroundImage = new StyleBackground( _resourceAssetLoader.Load<Sprite>("battery.WoodWorks/battery_WoodWorks/positionable-scale-ico"));
        });
      });
      
      fragment.AddComponent(PositionArrow("ZPositioner-Chip"));

      return fragment.Build();
    }
    
    private VisualElement PositionChip(string name)
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      
      fragment.SetName(name);
      // Styling
      fragment.SetStyle(style =>
      {
        style.width = 19;
        style.height = 19;
        style.position = new StyleEnum<Position>(Position.Absolute);
        style.right = 0;
        style.bottom = 0;
      });
      fragment.AddClass("tba-slider_holder");

      return fragment.Build();
    }
    
    private VisualElement PositionArrow(string name)
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      
      fragment.SetName(name);
      // Styling
      fragment.SetStyle(style =>
      {
        style.width = 11 + (1/3);
        style.height = 19;
        style.position = new StyleEnum<Position>(Position.Absolute);
        style.right = 12;
        style.bottom = 0;
      });
      fragment.AddClass("tba-right_arrow");

      return fragment.Build();
    }
    
    private static float ConvertChipPosition(float value)
    {
      return (value * 80) + 40;
    }

    private VisualElement Vector3Display()
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      fragment.AddComponent(builder =>
      {
        // Styling
        builder.SetFlexDirection(FlexDirection.Row);
        builder.SetFlexWrap(Wrap.Wrap);
        builder.SetJustifyContent(Justify.SpaceAround);
        builder.SetAlignItems(Align.Center);
        
        builder.SetPadding(5);

        // Components
        builder.AddComponent(builder =>
        {
          // Styling
          builder.SetStyle(style =>
          {
            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            style.flexGrow = 1;
            style.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);
            style.alignItems = new StyleEnum<Align>(Align.Center);
          });
          
          // Components
          builder.AddComponent(Vector3Field("X"));
          builder.AddComponent(Vector3Field("Y"));
          builder.AddComponent(Vector3Field("Z"));
        });
      });

      return fragment.Build();
    }

    private VisualElement Vector3Field(string axis)
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      fragment.AddComponent(builder =>
      {
        // Styling
        builder.SetFlexDirection(FlexDirection.Row);
        builder.SetFlexWrap(Wrap.Wrap);
        builder.SetJustifyContent(Justify.SpaceAround);
        builder.SetAlignItems(Align.Center);
        builder.SetStyle(style =>
        {
          style.marginLeft = 10;
          style.marginRight = 10;
        });
        
        // Fields
        builder.AddPreset(factory => factory.Labels().DefaultText(text: $"<color=yellow>{axis}: </color>"));
        builder.AddPreset(factory => factory.Labels().DefaultText(name: $"Vector3Field-{axis}"));
      });

      return fragment.Build();
    }

    private VisualElement Spacer()
    {
      VisualElementBuilder fragment = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
      // Styling
      fragment.SetStyle(style =>
      {
        style.flexGrow = 1;
        style.width = 20;
      });
      
      return fragment.Build();
    }
    
  }
}
