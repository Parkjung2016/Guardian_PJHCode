using ObjectPooling;
using UnityEngine;

public class BossWeapon : Weapon
{
    [SerializeField] public int _damage;
    private Enemy _enemy;
    CapsuleCollider _collider;
    public bool isElf = false;

    protected override void Awake()
    {
        base.Awake();
        _enemy = transform.root.GetComponent<Enemy>();
        _collider = GetComponent<CapsuleCollider>();
    }

    public void ColliderBig()
    {
        if (isElf)
        {
            _collider.radius = 0.4f;
            _collider.height = 2;
        }
    }

    protected override void TriggerCollider(Collider other)
    {
        PlayerController player = PlayerManager.Instance.Player;
        if (player.IsGuard && _enemy.WeaponDamageType != WeaponDamageType.VeryHeavy)
        {
            Vector3 direction = _enemy.transform.position - player.transform.position;
            float angle = Vector3.Angle(direction, player.Model.forward);
            if (angle < 50)
                return;
        }

        Ray ray = new Ray(transform.position, transform.up);
        bool result = Physics.Raycast(ray, out RaycastHit hit, 10, _whatIsTarget);
        Vector3 point = other.ClosestPoint(transform.position);
        if (other.TryGetComponent(out PlayerController health))
        {
            Enemy enemy = transform.root.GetComponent<Enemy>();
            enemy.CombatData.hitPoint = enemy.transform.position;
            health.ApplyDamage(_damage, enemy.CombatData);
            EnableCollider(false);
        }
        else
        {
            CameraManager.ShakeCamera(.012f);
            SoundManager.PlaySFX(_impactSounds[ImpactType.Rock], point);
            Quaternion rot = Quaternion.identity;
            if (result)
            {
                rot = Quaternion.LookRotation(-hit.normal);
            }

            EffectPlayer effectPlayer = (PoolManager.Instance.Pop(PoolingType.Effect_ImpactOnRock) as EffectPlayer);
            effectPlayer.transform.SetPositionAndRotation(point, rot);
            effectPlayer.PlayEffects();
        }
    }
}