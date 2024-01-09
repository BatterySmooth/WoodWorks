<div align="center">
  <img src="StaticFiles/img/WoodWorks Logo Small.png"  alt="WoodWorks Logo"/>
</div>

# Introduction

WoodWorks is a work-in-progress collection of scripts, tools, and systems to help in making your Timberborn mods.

At present, the features and functions are very limited, and I hope to be adding to them more over time.

# Systems

## Positionable Prefab

The Positionable Prefab system allows the player to change a GameObject's position after being placed. By default, the user can change in increments of 0.1 blocks, within the initial placement block.

### Usage

To add the Positionable Prefab system to an object, attach the `PositionablePrefabSpecification` script to an object.

The system will add the relevant components as a decorator.

### Parameters

| Parameter Name        | Type        | Details                                                                                                                                                                                                                                                                |
|-----------------------|-------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Positionable Root     | Game Object | This is the object to use as the parent object when applying the transformation. <br/> _Note: you cannot use the `#Finished` object, as the local transform for that Game Object is operated on by the BlockObject system, and the transforms will not work correctly_ |
| Displacement Step     | Float       | The amount to move the Game Object on each transform step. <br/> _The default value is 0.1_                                                                                                                                                                            |
| Maximum Displacement  | Float       | The upper and lower bounds for the displacement limiting. <br/> _The default is 0.5 (within the initial block)_                                                                                                                                                        |
| Displacement Rounding | Int         | The number of decimal places to use when rounding the displacement vector. Set this to appropriately match your configured `Displacement Step` and `Maximum Displacement`.                                                                                             |