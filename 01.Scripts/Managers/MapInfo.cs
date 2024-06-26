using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class MapInfo : MonoSingleton<MapInfo>
{
    [SerializeField] private string _mapName;
    
    public void ShowMapInfo()
    {
        MainUI.Instance.EnableMapInfo(_mapName);
    }
    
}