using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using PJH;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : SerializedMonoBehaviour
{
    private void IncreaseCurrentPressInputTime()
    {
        if (!_canIncreasePressInputTime || _isAttacking || !_isRunning)
        {
            _currentPressInputTime = 0;
            return;
        }

        _currentPressInputTime += Time.deltaTime;
    }

    private void DetectInteractAble()
    {
        if (_fov.IsTargetExist)
        {
            int bindingIdx =
                _interactInputActionReference.action.GetBindingIndexForControl(
                    _interactInputActionReference.action
                        .controls[0]);
            string key = InputControlPath.ToHumanReadableString(
                _interactInputActionReference.action
                    .bindings[bindingIdx]
                    .effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

            MainUI.Instance.ShowInteractionUI(_fov.visibleTargets.First().InteractName, key);
        }
        else
        {
            MainUI.Instance.HideInteractionUI();
        }
    }

    private void ImpossibleParryingIconLookAtCamera()
    {
        if (!_impossibleParringIcon.gameObject.activeSelf) return;
        _impossibleParringIcon.LookAt(_mainCamera.transform);
    }


    public void ApplyDamage(int damage, CombatData combatData)
    {
        bool knockDown = AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsTag("KnockDown");
        if (_isEvasion || _isUsingSkillAnimation || AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsName("UseSkill") ||
            knockDown || _isDead) return;
        VolumeManager.EnableBloodFrame();
        stopRotate = true;
        _isHitting = true;
        for (int i = 0; i < 2; i++)
        {
            EnableWeaponCollider(i, false);
            EnableWeaponTrail(i, false);
        }

        canEvasion = false;
        Guard(false);
        SetHandLIK(0);
        CameraManager.ChangeFOVCamera(50, .3f);
        SoundManager.PlaySFX(_hitSound);
        TimeManager.SetFreezeTime(0, .05f);
        CameraManager.ShakeCamera(.5f);
        Vector3 dir = (combatData.hitPoint - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(dir);
        Model.rotation = look;
        CheckEndSkillWhenEndCombo();

        AnimatorCompo.Play($"Hit{combatData.hitAnimationType}");
        HealthCompo.ApplyDamage(damage);
    }

    public void LookTarget()
    {
        float rotateDuration = .2f;
        if (_isLockOn) return;

        if (input.sqrMagnitude > 0)
        {
            Vector3 velocity = _moveVelocity;
            velocity.y = 0;
            if (velocity != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(velocity);
                Model.DORotateQuaternion(rot, rotateDuration);
            }
        }
        else
        {
            float rotY = _mainCamera.transform.eulerAngles.y;
            Model.DORotate(Vector3.up * rotY, rotateDuration);
        }
    }

    public void SwapWeapon(PlayerWeapon weapon)
    {
        if (_currentWeaponL) Destroy(_currentWeaponL.gameObject);
        if (_currentWeaponR) Destroy(_currentWeaponR.gameObject);
        SetHandLIKImmediately(null);

        isUsingSkill = false;
        canUseSkill = true;
        canSkillCooldown = false;
        MainUI.Instance.EndSkillCool();
        if (weapon.socketType == PlayerWeaponSocketType.LeftHand)
        {
            _currentWeaponL = Instantiate(weapon, _weaponSocketL);
            _currentWeaponL.EquipWeapon(this);
            int idx = Array.IndexOf(PlayerManager.Instance.weapons, weapon);
            PlayerWeapon weaponR = PlayerManager.Instance.weapons[idx + 1];
            _currentWeaponR = Instantiate(weaponR, _weaponSocketR);
            _currentWeaponR.EquipWeapon(this);
            EndRootMotion();
            return;
        }

        _currentWeaponR = Instantiate(weapon, _weaponSocketR);
        _currentWeaponR.EquipWeapon(this);
        EndRootMotion();
    }

    public void SetAttackSpeed(float speed)
    {
        AnimatorCompo.SetFloat(attackSpeedhash, speed);
    }

    public void ChangeCapeAndHatColor(Color color)
    {
        _capeSkinnedMeshRenderer.material.color = color;
        _hatSkinnedMeshRenderer.material.color = color;
    }

    public void LockOn(bool isLockOn)
    {
        Strafe(isLockOn);

        CameraManager.EnableFreeLookAxis(!_isLockOn);

        CameraManager.GetCamera(CameraTypeEnum.LockOn).LookAt = _isLockOn ? _lockOnTarget : null;
        CameraManager.ChangeCamera(_isLockOn ? CameraTypeEnum.LockOn : CameraTypeEnum.FreeLook);
        LockOnUI.Instance.gameObject.SetActive(_isLockOn);
        if (_isLockOn)
        {
            if (TutorialManager.Instance)
            {
                TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
                if (tutorialManager)
                    if (tutorialManager.currentStep == 1)
                    {
                        tutorialManager.ClearGoal();
                        tutorialManager.UpdateGoal(0);
                    }
            }

            LockOnUI.Instance.transform.SetParent(_lockOnTarget);
            LockOnUI.Instance.transform.localPosition = Vector3.zero;
        }

        if (FullBodyBipedIKCompo.solver.leftHandEffector.target != null)
        {
            SetHandLIK(Convert.ToByte(!isLockOn && !_isGuard));
        }

        if (_isLockOn && _isRunning)
            Strafe(false);
    }

    public void EnableImpossibleParryingIcon(bool enable)
    {
        if (_hideImpossibleParryingIconCoroutine != null) StopCoroutine(_hideImpossibleParryingIconCoroutine);
        if (enable)
        {
            SoundManager.PlaySFX(_appearImpossibleParryingIconSound);
            _hideImpossibleParryingIconCoroutine = StartCoroutine(HideImpossibleParryingIcon());
        }

        _impossibleParringIcon.gameObject.SetActive(enable);
    }

    public void CanEvasion()
    {
        canEvasion = true;
    }

    IEnumerator HideImpossibleParryingIcon()
    {
        yield return CoroutineHelper.WaitForSeconds(2);
        _impossibleParringIcon.gameObject.SetActive(false);
    }
}