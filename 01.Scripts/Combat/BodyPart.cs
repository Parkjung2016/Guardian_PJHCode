using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [SerializeField] private int _damageMultiplier = 1;
    [HideInInspector] public Enemy enemy;

    private void Awake()
    {
        enemy = transform.root.GetComponent<Enemy>();
    }

    public void ApplyDamage(int damage)
    {
        enemy.HealthCompo.ApplyDamage(_damageMultiplier * damage);
    }
}