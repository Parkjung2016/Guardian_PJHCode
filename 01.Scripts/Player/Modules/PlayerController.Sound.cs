using FMODUnity;

public partial class PlayerController
{
    public void PlayFootstepSound()
    {
        SoundManager.PlaySFX(_footstepSounds[FootstepType.Rock],
            _mainCamera.transform.position + _mainCamera.transform.forward * 2f);
    }

    public void PlayWeaponNormalAttackSound()
    {

        SoundManager.PlaySFX(_currentWeaponR.CurrentWeaponData.normalAttackSound,
            _mainCamera.transform.position + _mainCamera.transform.forward * 9.5f);
    }

    public void PlayWeaponStrongAttackSound()
    {
        SoundManager.PlaySFX(_currentWeaponR.CurrentWeaponData.strongAttackSound,
            _mainCamera.transform.position + _mainCamera.transform.forward * 9.5f);
    }

    public void PlayWeaponAfterEvasionAttackSound()
    {
        SoundManager.PlaySFX(_currentWeaponR.CurrentWeaponData.afterEvasionAttackSound,
            _mainCamera.transform.position + _mainCamera.transform.forward * 3f);
    }

    public void PlayRollStartSound()
    {
        SoundManager.PlaySFX(_playRollStartSound,
            _mainCamera.transform.position + _mainCamera.transform.forward * 3f);
    }

    public void PlayLandInRollSound()
    {
        SoundManager.PlaySFX(_playLandInRollSound,
            _mainCamera.transform.position + _mainCamera.transform.forward * 3f);
    }

    public void PlayRaiseSwordSound()
    {
        SoundManager.PlaySFX(_currentWeaponR.CurrentWeaponData.raiseWeaponSound);
    }
}