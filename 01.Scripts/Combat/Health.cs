using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    public int MaxHealth => _maxHealth;
    private int _currentHealth;
    public bool isEnemy = false;
    private Enemy _enemy;

    public event Action<float> OnHealthChangedEvent;
    public event Action OnHealthHalfEvent;
    public event Action OnDeadEvent;
    public event Action OnApplyDamagedEvent;

    private void Awake()
    {
        if (isEnemy)
        {
            _enemy = GetComponent<Enemy>();
        }

        CurrentHealth = _maxHealth;
    }

    public void ApplyDamage(int damage)
    {
        if (_enemy is TutorialEnemy) return;
        int value = _currentHealth - damage;
        CurrentHealth = value;
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
            if (isEnemy)
            {
                if (!_enemy.IsSecondStep && _currentHealth <= _maxHealth / 2 + 5)
                {
                    OnHealthHalfEvent?.Invoke();
                }
            }

            if (_currentHealth <= 0)
            {
                OnDeadEvent?.Invoke();
            }

            OnHealthChangedEvent?.Invoke((float)_currentHealth / _maxHealth);
        }
    }
}