using UnityEngine;
using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;

namespace Battery.WoodWorks.PositionablePrefab 
{
  public class PositionablePrefab : BaseComponent, IPersistentEntity, IPostInitializableLoadedEntity
  {
    private static readonly ComponentKey PositionManagerKey = new(nameof(PositionablePrefab));
    private static readonly PropertyKey<Vector3> PositionKey = new("Displacement");
    
    private GameObject _positionableRoot;
    private Vector3 _displacement;
    public Vector3 Displacement => _displacement;
    private float _displacementStep;
    public float DisplacementStep => _displacementStep;
    private float _maximumDisplacement;
    private int _displacementRounding;

    private void Awake()
    {
      enabled = true;
      this.InitializeParameters(this.GetComponentFast<PositionablePrefabSpecification>());
    }

    private void InitializeParameters(PositionablePrefabSpecification positionablePrefabSpecification)
    {
      if (!(bool) (UnityEngine.Object) positionablePrefabSpecification)
        return;
      this._positionableRoot = positionablePrefabSpecification.PositionableRoot;
      this._displacementStep = positionablePrefabSpecification.DisplacementStep;
      this._maximumDisplacement = positionablePrefabSpecification.MaximumDisplacement;
      this._displacementRounding = positionablePrefabSpecification.DisplacementRounding;
    }

    public void Save(IEntitySaver entitySaver)
    {
      entitySaver.GetComponent(PositionManagerKey).Set(PositionKey, _displacement);
    }

    public void Load(IEntityLoader entityLoader)
    {
      if (!entityLoader.HasComponent(PositionManagerKey))
        return;
      var component = entityLoader.GetComponent(PositionManagerKey);
      if (component.Has(PositionKey))
      {
        _displacement = component.Get(PositionKey);
      }
    }

    public void PostInitializeLoadedEntity()
    {
      TryUpdateModel();
    }

    public void Move(Vector3 displacementVector)
    {
      _displacement = RestrictedVector(displacementVector);
      TryUpdateModel();
    }

    private void TryUpdateModel()
    {
      if (!(bool)_positionableRoot)
      {
        Plugin.Log.LogWarning("PositionablePrefab could not be repositioned as there was no reference set for the root object");
        return;
      }
      _positionableRoot.gameObject.transform.localPosition = _displacement;
    }

    private Vector3 RestrictedVector(Vector3 displacementVector)
    {
      Vector3 newPos = new Vector3(
        Mathf.Clamp(_displacement.x + displacementVector.x, -_maximumDisplacement, _maximumDisplacement),
        Mathf.Clamp(_displacement.y + displacementVector.y, -_maximumDisplacement, _maximumDisplacement),
        Mathf.Clamp(_displacement.z + displacementVector.z, -_maximumDisplacement, _maximumDisplacement)
      ).Round(_displacementRounding);

      return newPos;
    }
  }
}