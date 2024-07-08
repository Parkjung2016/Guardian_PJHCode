using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

public class EffectPlayer : PoolableMono
{
    [SerializeField] private float _effectTime;
    private ParticleSystem[] _particleSystems;
    private VisualEffect[] _visualEffects;

    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _visualEffects = GetComponentsInChildren<VisualEffect>();
    }

    public override void ResetItem()
    {
        ParticleTriggerDamage[] damages = GetComponentsInChildren<ParticleTriggerDamage>();

        for (int i = 0; i < damages.Length; i++)
        {
            damages[i].Reset();
        }
    }

    public void PlayEffects()
    {
        StartCoroutine(PlayEffectsCoroutine());
    }

    public void PlayEffects(Vector3 position, Vector3 angle)
    {
        transform.position = position;
        transform.eulerAngles = angle;
        StartCoroutine(PlayEffectsCoroutine());
    }

    IEnumerator PlayEffectsCoroutine()
    {
        for (int i = 0; i < _particleSystems.Length; i++)
        {
            _particleSystems[i].Play();
        }

        for (int i = 0; i < _visualEffects.Length; i++)
        {
            _visualEffects[i].Play();
        }

        yield return new WaitForSeconds(_effectTime);
        PoolManager.Instance.Push(this);
    }
}