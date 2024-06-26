using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleTriggerDamageForEnemy : ParticleTriggerDamage
{
    [SerializeField] private int _combatType = 4;
    [SerializeField] private float _radius = 3;
    [SerializeField] private bool _rockEffect = false;


    private void OnParticleTrigger()
    {
        int numEnter = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _enterTrigger);

        if (!_rockEffect)
        {
            if (numEnter > 0)
            {
                Collider[] hits =
                    Physics.OverlapSphere(transform.position, _radius, _whatIsTarget);
                foreach (var hit in hits)
                {
                    if (hit == null || _colliderBufferList.Contains(hit)) continue;
                    _colliderBufferList.Add(hit);
                    CombatData combatData = new CombatData()
                    {
                        hitAnimationType = _combatType,
                        //hitPoint = p.position
                    };
                    hit.GetComponent<PlayerController>().ApplyDamage(_damage, combatData);
                }
            }
        }
        else
        {
            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle p = _enterTrigger[i];
                Collider[] hits =
                    Physics.OverlapSphere(transform.position, 100, _whatIsTarget);
                if (hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
                        if (hit == null || _colliderBufferList.Contains(hit)) continue;
                        _colliderBufferList.Add(hit);
                        CombatData combatData = new CombatData()
                        {
                            hitAnimationType = _combatType,
                            hitPoint = p.position
                        };
                        hit.GetComponent<PlayerController>().ApplyDamage(_damage, combatData);
                    }
                }
            }
        }

    }


}