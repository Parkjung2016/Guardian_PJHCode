using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnUI : MonoSingleton<LockOnUI>
{
    private Transform _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main.transform;
        Instance.gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(_mainCam);
    }
}