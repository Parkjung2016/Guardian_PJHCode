using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public partial class PlayerController
{
    private void CalculateMovement()
    {
        var right = _mainCamera.transform.right;
        right.y = 0;
        var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
        _moveVelocity = (input.x * right) + (input.z * forward);
        _moveVelocity *= Speed * Time.fixedDeltaTime;
        if (_isRunning && CurrentEnergy > 0 && CharacterControllerCompo.velocity.sqrMagnitude > 0 && !_isEvasion &&
            !_isAttacking)
        {
            if (_currentDecreaseEnergyWhenRunningTime <= _decreaseEnergyTimeWhenRunning)
            {
                _currentDecreaseEnergyWhenRunningTime += Time.deltaTime;
            }
            else
            {
                _currentDecreaseEnergyWhenRunningTime = 0;
                if (canReduceEnergy)
                    CurrentEnergy -= _reduceEnergyWhenRunning;
            }
        }

        if (_isLockOn)
        {
            Vector3 dir = (_lockOnTarget.position - transform.position).normalized;
            dir.y = 0;
            Quaternion look = Quaternion.LookRotation(dir);
            Model.rotation = Quaternion.Lerp(Model.rotation, look, 15 * Time.fixedDeltaTime);
        }
        else if (_moveVelocity.sqrMagnitude > 0 && !stopRotate)
        {
            Quaternion look = Quaternion.LookRotation(_moveVelocity);
            Model.rotation = Quaternion.Lerp(Model.rotation, look, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (IsGrounded && _velocityY < 0)
        {
            _velocityY = -.03f;
        }
        else
        {
            _velocityY += _extraGravity * Time.fixedDeltaTime;
        }

        _moveVelocity.y = _velocityY;
    }

    public void Move(Vector3 velocity)
    {
        if (stopMove || !_controller.enabled)
        {
            return;
        }

        _controller.Move(velocity);
    }

    public void EvasionEnd()
    {
        EndRootMotion();
    }


    public void HitEnd()
    {
        EndRootMotion();
        _isHitting = false;
        _isParrying = false;
    }

    public void Guard(WeaponDamageType damageType)
    {
        CurrentEnergy -= _reduceEnergyWhenBlock;
        float shakeValue = 0;
        switch (damageType)
        {
            case WeaponDamageType.Light:
                shakeValue = .1f;
                break;
            case WeaponDamageType.Medium:
                shakeValue = .25f;
                break;
            case WeaponDamageType.Heavy:
                shakeValue = .45f;
                break;
        }

        CameraManager.ShakeCamera(shakeValue);

        stopRotate = true;
        useRootMotion = true;
        AnimatorCompo.Play($"Guard{(int)damageType}");
    }

    public void EndRootMotion()
    {
        if (AnimatorCompo.GetNextAnimatorStateInfo(0).IsTag("Attack")) return;
        _comboPossible = true;
        _comboCount = 0;
        _isAttacking = false;
        useRootMotion = false;
        _isEvasion = false;
        stopRotate = false;
        _isParrying = false;
        _isHitting = false;
        _afterEvasionAttacking = false;
        _strongAttacking = false;
        EnableWeaponTrail(0, false);
        EnableWeaponTrail(1, false);
        if (!_isGuard && !_isLockOn)
            SetHandLIK(1, .3f);
    }

    IEnumerator StartIncreaseEnergy()
    {
        yield return CoroutineHelper.WaitForSeconds(_startIncreaseEnergyTime);

        while (CurrentEnergy < _maxEnergy)
        {
            CurrentEnergy += _increaseEnergy;
            yield return CoroutineHelper.WaitForSeconds(_increaseEnergyTime);
        }
    }

    private async void Evasion()
    {
        useRootMotion = true;
        if (_isLockOn)
        {
            if (input == Vector3.zero)
            {
                AnimatorCompo.CrossFadeInFixedTime($"Evasion180", .1f);
            }
            else
            {
                float angle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
                AnimatorCompo.CrossFadeInFixedTime($"Evasion{angle}", .1f);
            }
        }
        else
        {
            AnimatorCompo.CrossFadeInFixedTime("Evasion0", .1f);
        }

        stopRotate = true;
        _isEvasion = true;
        if (input.sqrMagnitude > 0)
        {
            await Task.Delay(10);
            Vector3 velocity = _moveVelocity;
            velocity.y = 0;
            Quaternion look = Quaternion.LookRotation(velocity);
            Model.transform.rotation = look;
        }

        if (canReduceEnergy)
            CurrentEnergy -= _reduceEnergyWhenEvasion;

        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 5 && tutorialManager.CheckPlayerEvasion())
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(0);
                }
        }
    }

    private void Strafe(bool isStrafe)
    {
        _isLockOn = isStrafe;
        _lookAnimator.ObjectToFollow = _isLockOn ? _lockOnTarget : null;

        AnimatorCompo.SetBool(strafeHash, isStrafe);
    }

    private void Guard(bool isGuard)
    {
        _isGuard = isGuard;
        AnimatorCompo.SetBool(isGuardHash, _isGuard);
        if (isGuard)
            PlayRaiseSwordSound();
        _parryingCollider.EnableCollider(_isGuard);
        if (isGuard)
        {
            if (_setHandIKWeight1Coroutine != null) StopCoroutine(_setHandIKWeight1Coroutine);

            SetHandLIK(0);
            SetHandRIK(0);
        }
        else
        {
            if (_isParrying || _isLockOn) return;
            SetHandIKWeight1();
        }
    }
}