﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                //
                // if (_instance == null)
                // {
                //     _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                // }
            }

            return _instance;
        }
    }
}