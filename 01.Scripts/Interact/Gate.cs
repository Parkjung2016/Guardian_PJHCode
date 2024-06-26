using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gate : InteractAble
{
    [SerializeField] private Collider _collider;
    [SerializeField] private EventReference _enterGateSound;

    public override void Interact()
    {
        if (!_collider.enabled) return;
        _collider.enabled = false;
        SoundManager.PlaySFX(_enterGateSound);
        GameManager.ParentInstance.EnterGate(transform.rotation, () =>
            _collider.enabled = true);
    }
}