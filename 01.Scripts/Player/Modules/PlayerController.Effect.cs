using ObjectPooling;

public partial class PlayerController
{
    public void EnableWeaponCollider(int idx, bool enable)
    {
        switch (idx)
        {
            case 0:
                _currentWeaponR.EnableCollider(enable);
                break;
            case 1:
                if (_currentWeaponL)
                    _currentWeaponL.EnableCollider(enable);
                break;
        }
    }


    public void EnableWeaponTrail(int idx, bool enable)
    {
        switch (idx)
        {
            case 0:
                _currentWeaponR.EnableTrailEffect(enable);
                break;
            case 1:
                if (_currentWeaponL)
                    _currentWeaponL.EnableTrailEffect(enable);
                break;
        }
    }
    public void SpawnDistortionEffect()
    {
        EffectPlayer effect = (PoolManager.Instance.Pop(PoolingType.Effect_ShockWave) as EffectPlayer);
        effect.transform.position = _currentWeaponR.transform.position;
        effect.PlayEffects();
    }
    
    public void PlayWeaponEffect(int idx)
    {
        if (!isUsingSkill) return;
        _currentWeaponR.PlayWeaponEffect(idx);
    }

    public void PlayWeaponEffect2(int idx)
    {
        if (!isUsingSkill) return;
        _currentWeaponR.PlayWeaponEffect2(idx);
    }
}