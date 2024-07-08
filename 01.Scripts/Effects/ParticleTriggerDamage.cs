using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ParticleTriggerDamage : MonoBehaviour
{
    [SerializeField] protected LayerMask _whatIsTarget;
    [SerializeField] protected int _damage;
    protected ParticleSystem _ps;
    protected List<ParticleSystem.Particle> _enterTrigger = new List<ParticleSystem.Particle>();
    protected List<Collider> _colliderBufferList = new();

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();

    }

    private void Start()
    {
        int idx = Mathf.RoundToInt(Mathf.Log(_whatIsTarget.value, 2));
        FindObjectsOfType<Health>().ToList().ForEach(x =>
        {
            if (x.gameObject.layer == idx)
                _ps.trigger.AddCollider(x.GetComponent<Collider>());
        });
    }

    public virtual void Reset()
    {
        _colliderBufferList.Clear();
    }
}