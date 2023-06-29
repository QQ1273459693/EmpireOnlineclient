using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitSpineInfo", menuName = "新单位骨骼信息")]
public class UnitSpineInfo : ScriptableObject
{
    public TextAsset SkelData;
    public TextAsset Atlas;
    public Texture2D[] Textures;
    public Material Materials;
}