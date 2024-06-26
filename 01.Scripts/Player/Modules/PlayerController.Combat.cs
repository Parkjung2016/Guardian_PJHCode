using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class PlayerController
{
    public void ComboPossible()
    {
        _comboPossible = true;
    }

    public void ComboPossibleWithRestCombo()
    {
        _comboCount = 0;
        ComboPossible();
    }


    public void UseWeaponSkill()
    {
        _isUsingSkillAnimation = false;
        if (_currentWeaponL)
            _currentWeaponL.UseSkill(null);
        _currentWeaponR.UseSkill(() =>
        {
            isUsingSkill = false;
            canSkillCooldown = true;
            EndRootMotion();
            if (TutorialManager.Instance)
            {
                TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
                if (tutorialManager)
                    if (tutorialManager.currentStep == 6)
                    {
                        tutorialManager.ClearGoal();
                        tutorialManager.UpdateGoal(0);
                    }
            }

            MainUI.Instance.StartSkillCool(skillCooldown);
        });
    }


    public void CheckEndSkillWhenEndCombo()
    {
        if (_currentWeaponR.willEndSkill)
        {
            EndRootMotion();
            _currentWeaponR.EndSkill();
            if (_currentWeaponL)
                _currentWeaponL.EndSkill();
        }
    }

    public void EnableParryingCollider(bool enable)
    {
        _parryingCollider.SetParryingAble(enable);
        _parryingCollider.EnableCollider(enable);
    }

    public void CanEvasionAttack()
    {
        if (_checkAfterEvasionAttackCoroutine != null) StopCoroutine(_checkAfterEvasionAttackCoroutine);
        _checkAfterEvasionAttackCoroutine = StartCoroutine(StartCheckAfterEvasionAttack());
    }

    IEnumerator StartCheckAfterEvasionAttack()
    {
        _canAfterEvasionAttack = true;
        yield return CoroutineHelper.WaitForSeconds(_maxAfterEvasionAttackTime);
        _canAfterEvasionAttack = false;
    }

    private void CheckSkillCooldown()
    {
        if (canSkillCooldown)
        {
            if (_currentSkillCooldownTime <= skillCooldown)
            {
                _currentSkillCooldownTime += Time.deltaTime;
            }
            else
            {
                _currentSkillCooldownTime = 0;
                canSkillCooldown = false;
            }
        }
    }

    private void AfterEvasionAttack()
    {
        _currentWeaponR.damageType = PlayerWeaponDamageType.AfterEvasion;

        AnimatorCompo.CrossFadeInFixedTime($"AfterEvasionAttack", _currentWeaponR.animationCrossFadeDuration);
        _canAfterEvasionAttack = false;
        _afterEvasionAttacking = true;
        if (canReduceEnergy)
            CurrentEnergy -= reduceEnergyWhenNormalAttack;
        _isAttacking = true;
        useRootMotion = true;
        stopRotate = true;
        _comboPossible = false;
        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 2 && !tutorialManager.ExistsGoal(4))
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(4);
                }
        }
    }

    private void RunningAttack()
    {
        _currentWeaponR.damageType = PlayerWeaponDamageType.AfterRun;
        _isAttacking = true;
        useRootMotion = true;
        stopRotate = true;
        AnimatorCompo.CrossFadeInFixedTime($"RunningAttack", _currentWeaponR.animationCrossFadeDuration);
        if (canReduceEnergy)
            CurrentEnergy -= reduceEnergyWhenRunningAttack;
        _comboPossible = false;
        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 2 && !tutorialManager.ExistsGoal(2))
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(2);
                }
        }
    }

    private void NormalAttack()
    {
        _currentWeaponR.damageType = PlayerWeaponDamageType.Normal;
        _isAttacking = true;
        useRootMotion = true;
        stopRotate = true;
        AnimatorCompo.CrossFadeInFixedTime($"NormalAttack_{_comboCount++}",
            _currentWeaponR.animationCrossFadeDuration);
        if (canReduceEnergy)
            CurrentEnergy -= reduceEnergyWhenNormalAttack;
        _comboPossible = false;

        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 2 && !tutorialManager.ExistsGoal(0))
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(0);
                }
        }
    }

    private void StrongAttack()
    {
        _currentWeaponR.damageType = PlayerWeaponDamageType.Strong;
        useRootMotion = true;
        _strongAttacking = true;

        AnimatorCompo.CrossFadeInFixedTime($"StrongAttack", _currentWeaponR.animationCrossFadeDuration);
        _comboPossible = false;
        stopRotate = true;
        _isAttacking = true;
        if (canReduceEnergy)
            CurrentEnergy -= reduceEnergyWhenStrongAttack;
        if (TutorialManager.Instance)
        {
            TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
            if (tutorialManager)
                if (tutorialManager.currentStep == 2 && !tutorialManager.ExistsGoal(1))
                {
                    tutorialManager.ClearGoal();
                    tutorialManager.UpdateGoal(1);
                }
        }
    }
}