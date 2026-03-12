using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTypeDefiner : MonoBehaviour
{
    [field: SerializeField]
    public eObstacleType SFType
    {
        get; private set;
    }
    private void Awake()
    {
        Debug.Assert(gameObject.tag == 
            "Obstacle", $"ObstacleTypeDefiner can be only used when gameobject is tagged Obstacle [{gameObject.name}]");
    }
}


[System.Flags]
public enum eObstacleType
{
    RushStoppable = 1 << 1,
    ProjectilePassable = 1 << 2,
}