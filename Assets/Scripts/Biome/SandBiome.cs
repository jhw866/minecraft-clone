﻿using UnityEngine;

public class SandBiome : BaseBiome
{
    public override GameObject GetObjectForPosition(Vector3 position)
    {
        return PrefabManager.GetPrefab(PrefabType.Sand);
    }
    
    protected override float AdjustHeightIfNecessary(float newX, float newZ, float currentTotalY)
    {
        return currentTotalY / 2;
    }

    public override GameObject GetColumnCube()
    {
        return PrefabManager.GetPrefab(PrefabType.Sand);
    }

    public override GameObject GetAdjacentCube()
    {
        return GetColumnCube();
    }
}
