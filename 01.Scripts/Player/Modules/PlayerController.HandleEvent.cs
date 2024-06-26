using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public partial class PlayerController
{
    private void OnEnable()
    {
        _inputReader.MovementEvent += HandleMovementEvent;
        _inputReader.NormalAttackEvent += HandleNormalAttackEvent;
        _inputReader.StrongAttackEvent += HandleStrongAttackEvent;
        _inputReader.EvasionEvent += HandleEvasionEvent;
        _inputReader.RunEvent += HandleRunEvent;
        _inputReader.LockOnEvent += HandleLockOnEvent;
        _inputReader.GuardEvent += HandleGuardEvent;
        _inputReader.ParryingEvent += HandleParryingEvent;
        _inputReader.SkillEvent += HandleSkillEvent;
        _inputReader.InteractEvent += HandleInteractEvent;
        _inputReader.DrinkPotionEvent += HandleDrinkPotionEvent;
        HealthCompo.OnDeadEvent += HandleDeadEvent;
    }

    private void HandleDrinkPotionEvent()
    {
        if (_currentPotionCount <= 0 || HealthCompo.CurrentHealth >= HealthCompo.MaxHealth || !_canRecoveryHP) return;
        _recoveryHPParticle.Play();
        CurrentPotionCount--;
        HealthCompo.CurrentHealth += _potionRecoveryHP;
        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 3)
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(0);
                }
        }

        SoundManager.PlaySFX(_recoveryHPSound);
        StartCoroutine(RecoveryHPCoolTime());
    }

    IEnumerator RecoveryHPCoolTime()
    {
        _canRecoveryHP = false;
        yield return CoroutineHelper.WaitForSeconds(1);
        _canRecoveryHP = true;
    }

    private void HandleInteractEvent()
    {
        if (!_fov.IsTargetExist) return;
        _fov.visibleTargets[0].Interact();
    }

    private void HandleSkillEvent()
    {
        if (_isParrying || _isEvasion || _isHitting || _isAttacking || isUsingSkill || !canUseSkill ||
            canSkillCooldown || !canCombat)
            return;
        _isUsingSkillAnimation = true;
        useRootMotion = true;
        stopRotate = true;
        isUsingSkill = true;
        HandleGuardEvent(false);
        MainUI.Instance.LockSkill();
        AnimatorCompo.CrossFadeInFixedTime("UseSkill", _currentWeaponR.animationCrossFadeDuration);
    }

    private void HandleDeadEvent()
    {
        useRootMotion = true;
        AnimatorCompo.Play("Dead", -1, 0);
        _inputReader.EnablePlayerInput(false);
        _inputReader.EnableUIInput(false);
        SoundManager.PlayerDeath();
        MainUI.Instance.EnableDeathUI();
        enabled = false;
    }

    private void HandleParryingEvent()
    {
        if (useRootMotion ||
            CurrentEnergy <= _reduceEnergyWhenParrying || !canCombat) return;
        if (_setHandIKWeight1Coroutine != null) StopCoroutine(_setHandIKWeight1Coroutine);

        SetHandLIK(0);
        SetHandRIK(0);
        useRootMotion = true;
        stopRotate = true;
        if (canReduceEnergy)
            CurrentEnergy -= _reduceEnergyWhenParrying;
        _isParrying = true;
        PlayRaiseSwordSound();
        AnimatorCompo.CrossFadeInFixedTime("ParryingUp", _currentWeaponR.animationCrossFadeDuration);
    }

    private void HandleGuardEvent(bool isGuard)
    {
        if (isGuard)
        {
            if (useRootMotion || CurrentEnergy <= _reduceEnergyWhenBlock || !canCombat) return;
        }

        Guard(isGuard);
    }

    private void HandleLockOnEvent()
    {
        if (!_lockOnTarget || !canLockOn) return;

        LockOn(!_isLockOn);
    }

    private void HandleStrongAttackEvent()
    {
        if (CurrentEnergy <= reduceEnergyWhenStrongAttack || useRootMotion ||
            !canCombat) return;

        StrongAttack();
    }

    private void HandleNormalAttackEvent()
    {
        if (!_canAfterEvasionAttack && _isEvasion || _isParrying || _isHitting ||
            AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsName("UseSkill") || !_comboPossible | !canCombat) return;
        if (_canAfterEvasionAttack && CurrentEnergy <= reduceEnergyWhenNormalAttack) return;
        if (_isRunning && CurrentEnergy <= reduceEnergyWhenRunningAttack) return;
        if (CurrentEnergy <= reduceEnergyWhenNormalAttack) return;


        if (_canAfterEvasionAttack)
        {
            AfterEvasionAttack();

            return;
        }

        if (_isRunning && _currentPressInputTime >= .6f)
        {
            RunningAttack();

            return;
        }

        if (_comboPossible)
        {
            NormalAttack();
        }
    }

    private void HandleRunEvent(bool isRunning)
    {
        _isRunning = isRunning;

        if (!_isLockOn && !_isGuard && !_isParrying && input.sqrMagnitude > 0 &&
            FullBodyBipedIKCompo.solver.leftHandEffector.target != null)
        {
            SetHandLIK(Convert.ToByte(isRunning));
        }
    }

    private void CheckLockOnWhileRunning()
    {
        if (!LockOnUI.Instance) return;
        if (LockOnUI.Instance.gameObject.activeSelf)
        {
            Strafe(!_isRunning);
        }
    }

    private void HandleEvasionEvent()
    {
        if (AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsName("UseSkill") ||
            CurrentEnergy <= _reduceEnergyWhenEvasion || useRootMotion) return;
        Evasion();
    }

    private void HandleMovementEvent(Vector3 input)
    {
        this.input = input;
        _canIncreasePressInputTime = input.sqrMagnitude > 0;
    }

    private void OnDisable()
    {
        _inputReader.MovementEvent -= HandleMovementEvent;
        _inputReader.NormalAttackEvent -= HandleNormalAttackEvent;
        _inputReader.StrongAttackEvent -= HandleStrongAttackEvent;
        _inputReader.EvasionEvent -= HandleEvasionEvent;
        _inputReader.RunEvent -= HandleRunEvent;
        _inputReader.LockOnEvent -= HandleLockOnEvent;
        _inputReader.GuardEvent -= HandleGuardEvent;
        _inputReader.ParryingEvent -= HandleParryingEvent;
        _inputReader.SkillEvent -= HandleSkillEvent;
        _inputReader.InteractEvent -= HandleInteractEvent;
        _inputReader.DrinkPotionEvent -= HandleDrinkPotionEvent;
        HealthCompo.OnDeadEvent -= HandleDeadEvent;
    }
}