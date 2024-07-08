using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using FMODUnity;
using ObjectPooling;
using PJH;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;

public enum WeaponDamageType
{
    Light,
    Medium,
    Heavy,
    VeryHeavy
}


public abstract class Weapon : SerializedMonoBehaviour
{
    protected Collider _weaponCol;
    [SerializeField] protected LayerMask _whatIsTarget;
    [SerializeField] protected Dictionary<ImpactType, EventReference> _impactSounds;

    protected TrailEffect _trailEffect;


    protected virtual void Awake()
    {
        _trailEffect = GetComponent<TrailEffect>();
        _weaponCol = GetComponent<Collider>();
        EnableCollider(false);
        EnableTrailEffect(false);
    }

    public void EnableCollider(bool enable)
    {
        _weaponCol.enabled = enable;
    }

    public void OnTriggerEnter(Collider other)
    {
        TriggerCollider(other);
    }

    protected abstract void TriggerCollider(Collider other);

    public void EnableTrailEffect(bool enable)
    {
        if (!_trailEffect) return;
        _trailEffect.active = enable;
    }
}