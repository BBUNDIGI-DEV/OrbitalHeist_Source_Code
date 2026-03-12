using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OutlineSnapshot", menuName = "DataContainer/OutlineSnapshot")]
public class OutlineSnapshotConfig : ScriptableObject
{
    public OutlineSnapShot SheildOutlineSnapshot;
    public OutlineSnapShot PowerOverwalmingSnapshot;
    public OutlineSnapShot ForceShieldOutlineSnapshot;
}

[System.Serializable]
public struct OutlineSnapShot
{
    public Color OutlineColor;
    [Range(0f, 0.05f)] public float OutlineWidth;
}
