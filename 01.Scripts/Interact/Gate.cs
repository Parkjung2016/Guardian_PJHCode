using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gate :  MonoBehaviour, IInteractAble
{
    [field: SerializeField] public string InteractName { get; set; }
    public bool CanInteract { get; set; } = true;

    public Transform Trm { get; set; }

    [SerializeField] private Collider _collider;
    [SerializeField] private EventReference _enterGateSound;

    private void Awake()
    {
        Trm = transform;

    }

    public void Interact()
    {
        if (!_collider.enabled) return;
        _collider.enabled = false;
        SoundManager.PlaySFX(_enterGateSound);
        GameManager.ParentInstance.EnterGate(transform.rotation, () =>
            _collider.enabled = true);
    }
}