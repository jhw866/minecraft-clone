﻿using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 50;

    public float startX;
    public float startZ;

    private const float _scaleFactor = 20f;
    private const float _worldScale = 5f;
    private const float _steepnessScale = 200f;
    private const int _offset = 1000;

    private const int SNOW_MAX_Y = 8;
    private static readonly Perlin perlin = new Perlin();

    private static Dictionary<Tuple<int, int>, int> allCubePositions = new Dictionary<Tuple<int, int>, int>();
    private Dictionary<Tuple<int, int>, int> localCubePosition = new Dictionary<Tuple<int, int>, int>();

    // Start is called before the first frame update
    void Start()
    {
        startX = gameObject.transform.position.x;
        startZ = gameObject.transform.position.z;
        BiomeTypeEnum biome = BiomeManager.GetBiome(startX, startZ);

        for (int i = 0; i < CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < CHUNK_SIZE; ++j)
            {
                float newX = (startX + i + _offset) / _scaleFactor;
                float newZ = (startZ + j + _offset) / _scaleFactor;

                // this essentially allows us to generate the steepness. Dividing by _worldScale
                // allows us to have plains and montains because the steepness spans over a longer distance
                float steepnessY = perlin.DoPerlin(newX / _worldScale, newZ / _worldScale) * _steepnessScale;
                float totalY =  perlin.DoPerlin(newX, newZ) * steepnessY;
                Vector3 pos = new Vector3(startX + i, (int)totalY, startZ + j);
                localCubePosition.Add(new Tuple<int, int>((int)startX + i, (int)startZ + j), (int) totalY);
                allCubePositions.Add(new Tuple<int, int>((int)startX + i, (int)startZ + j), (int)totalY);                
                PrefabType prefabType;
                switch (biome)
                {
                    case BiomeTypeEnum.Grass:
                        prefabType = PrefabType.GRASS;
                        if (totalY > SNOW_MAX_Y)
                        {
                            prefabType = PrefabType.SNOW;
                        }
                        break;
                    case BiomeTypeEnum.Snow:
                        prefabType = PrefabType.SNOW;
                        break;
                    default:
                        prefabType = PrefabType.GRASS;
                        Debug.Log($"Unknown BiomeType {biome}");
                        break;
                }
                Instantiate(PrefabManager.GetPrefab(prefabType)).transform.SetPositionAndRotation(pos, new Quaternion());
            }
        }

        BuildColumns();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsPositionInChunk(Vector3 pos)
    {
        return pos.x > startX && pos.x < startX + CHUNK_SIZE && pos.z > startZ && pos.z < startZ + CHUNK_SIZE;
    }

    public string GetKey()
    {
        return GetKey(gameObject.transform.position);
    }

    public static string GetKey(Vector3 position)
    {
        float x = position.x / CHUNK_SIZE;
        float z = position.z / CHUNK_SIZE;

        Debug.Log($"{x.ToString("f0")} {z.ToString("f0")}");

        return $"{x.ToString("f0")} {z.ToString("f0")}";
    }

    private void BuildColumns()
    {
        foreach(KeyValuePair<Tuple<int, int>, int> cubePosition in localCubePosition)
        {
            for (int i = -1; i <= 1; i+=2)
            {
                for (int j = -1; j <= 1; j+=2)
                {
                    Tuple<int, int> adjacentPos = new Tuple<int, int>(cubePosition.Key.Item1 + i, cubePosition.Key.Item2 + j);
                    if (allCubePositions.ContainsKey(adjacentPos))
                    {
                        int adjacentHeight = allCubePositions[adjacentPos];
                        int diffHeight = cubePosition.Value - adjacentHeight;

                        if (diffHeight > 1)
                        {
                            for (int k = 1; k < diffHeight; ++k)
                            {
                                Vector3 newPos = new Vector3(cubePosition.Key.Item1, cubePosition.Value - k, cubePosition.Key.Item2);
                                Instantiate(PrefabManager.GetPrefab(PrefabType.GRASS)).transform.SetPositionAndRotation(newPos, new Quaternion());
                            }
                        }
                    }
                }
            }
        }
    }
}
