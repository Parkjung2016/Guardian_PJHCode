using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleTriggerDamageForPlayer : ParticleTriggerDamage
{
    private bool _hitEnemy;
    
    private void OnParticleTrigger()
    {
        int numEnter = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _enterTrigger);
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = _enterTrigger[i];
            Collider[] hits =
                Physics.OverlapSphere(p.position, 3, _whatIsTarget);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    print(hit.gameObject);
                    if (hit == null || _hitEnemy) continue;
                    _colliderBufferList.Add(hit);
                    if (hit.TryGetComponent(out Health health))
                        health.ApplyDamage(_damage);
                    else if (hit.TryGetComponent(out BodyPart bodyPart))
                    {
                        bodyPart.enemy.HealthCompo.ApplyDamage(_damage);
                    }
                    _hitEnemy= true;
                }
            }
        }
    }
    public override void Reset()
    {
        _hitEnemy = false;
        base.Reset();
    }
}