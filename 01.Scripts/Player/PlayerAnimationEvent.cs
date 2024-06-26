using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerController _player;

    private void Awake()
    {
        _player = transform.parent.GetComponent<PlayerController>();
    }

    private void OnAnimatorMove()
    {
        _player.OnAnimatorMove();
    }

    private void ComboPossible()
    {
        _player.ComboPossible();
    }

    private void ComboPossibleWithResetCombo()
    {
        _player.ComboPossibleWithRestCombo();
    }


    private void LookTarget()
    {
        _player.LookTarget();
    }

    private void EvasionEnd()
    {
        _player.EvasionEnd();
    }

    private void CanEvasionAttack()
    {
        _player.CanEvasionAttack();
    }

    private void EnableWeaponCollider(int idx)
    {
        _player.EnableWeaponCollider(idx, true);
    }

    private void DisableWeaponCollider(int idx)
    {
        _player.EnableWeaponCollider(idx, false);
    }

    private void EnableWeaponTrail(int idx)
    {
        _player.EnableWeaponTrail(idx, true);
    }

    private void DisableWeaponTrail(int idx)
    {
        _player.EnableWeaponTrail(idx, false);
    }


    private void SpawnDistortionEffect()
    {
        _player.SpawnDistortionEffect();
    }

    private void PlayFootstepSound()
    {
        if (_player.input.sqrMagnitude > 0 && _player.CharacterControllerCompo.velocity.sqrMagnitude > .5f)
            _player.PlayFootstepSound();
    }

    private void PlayWeaponNormalAttackSound()
    {
        _player.PlayWeaponNormalAttackSound();
    }

    private void PlayWeaponStrongAttackSound()
    {
        _player.PlayWeaponStrongAttackSound();
    }

    private void PlayWeaponAfterEvasionAttackSound()
    {
        _player.PlayWeaponAfterEvasionAttackSound();
    }

    private void PlayRollStartSound()
    {
        _player.PlayRollStartSound();
    }

    private void PlayLandInRollSound()
    {
        _player.PlayLandInRollSound();
    }


    private void ZoomIn()
    {
        CameraManager.ChangeFOVCamera(37);
    }

    private void ZoomOut()
    {
        CameraManager.ShakeCamera(.3f);
        CameraManager.ChangeFOVCamera(50, .3f);
    }

    private void HitEnd()
    {
        _player.HitEnd();
    }

    private void EnableParryingCollider()
    {
        _player.EnableParryingCollider(true);
    }

    private void DisableParryingCollider()
    {
        _player.EnableParryingCollider(false);
    }

    private void EndRootMotion()
    {
        _player.EndRootMotion();
    }

    private void UseWeaponSkill()
    {
        _player.UseWeaponSkill();
    }

    private void PlayWeaponEffect(int idx)
    {
        _player.PlayWeaponEffect(idx);
    }

    private void PlayWeaponEffect2(int idx)
    {
        _player.PlayWeaponEffect2(idx);
    }

    private void CheckEndSkillWhenEndCombo()
    {
        _player.CheckEndSkillWhenEndCombo();
    }

    private void SetHandIKWeight1()
    {
        _player.SetHandIKWeight1();
    }
}