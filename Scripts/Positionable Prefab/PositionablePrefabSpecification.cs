using System.Runtime.CompilerServices;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Battery.WoodWorks.PositionablePrefab
{
  public class PositionablePrefabSpecification : BaseComponent
  {
    [SerializeField]
    private GameObject positionableRoot;
    public GameObject PositionableRoot => this.positionableRoot;
    
    [SerializeField]
    private float displacementStep = 0.1f;
    public float DisplacementStep => this.displacementStep;
    
    [SerializeField]
    private float maximumDisplacement = 0.5f;
    public float MaximumDisplacement => this.maximumDisplacement;
    
    [SerializeField]
    private int displacementRounding = 1;
    public int DisplacementRounding => this.displacementRounding;
  }
}