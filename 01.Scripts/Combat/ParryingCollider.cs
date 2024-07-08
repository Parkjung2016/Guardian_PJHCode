using System;
using System.Collections;
using FMODUnity;
using ObjectPooling;
using PJH;
using UnityEngine;

public class ParryingCollider : MonoBehaviour
{
    private Collider _parryingCol;

    [SerializeField] private float _parryingAbleTime;
    [SerializeField] private EventReference _parryingSound, _guardSound;
    private bool _parryingAble;

    private PlayerController _player;

    private void Awake()
    {
        _player = transform.root.GetComponent<PlayerController>();
        _parryingCol = GetComponent<Collider>();
        EnableCollider(false);
    }


    public void EnableCollider(bool enable)
    {
        _parryingCol.enabled = enable;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Weapon weapon))
        {
            Enemy enemy = weapon.transform.root.GetComponent<Enemy>();
            if (enemy.WeaponDamageType != WeaponDamageType.VeryHeavy)
            {
                Vector3 direction = enemy.transform.position - _player.transform.position;
                float angle = Vector3.Angle(direction, _player.Model.forward);
                if (angle >= 50)
                    return;
            }

            if (enemy.WeaponDamageType == WeaponDamageType.VeryHeavy || _player.IsAttacking || _player.IsEvasion)
            {
                return;
            }

            if (_player.IsHitting) return;
            Vector3 point = _parryingCol.ClosestPoint(weapon.transform.position);
            (PoolManager.Instance.Pop(PoolingType.Effect_GuardSpark) as EffectPlayer).PlayEffects(point,
                _player.Model.eulerAngles);
            if (_parryingAble)
            {
                _parryingAble = false;
                if (TutorialManager.Instance)
                {
                    TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
                    if (tutorialManager)
                        if (tutorialManager.currentStep == 4 && !tutorialManager.ExistsGoal(1))
                        {
                            tutorialManager.ClearGoal();
                            tutorialManager.UpdateGoal(1);
                        }
                }

                enemy.IsStun = true;
                VolumeManager.SetChromaticAberration(.03f,
                    () => { VolumeManager.SetChromaticAberration(0); });
                VolumeManager.SetLensDistortion(.4f, () => { VolumeManager.SetLensDistortion(0); });
                TimeManager.SetFreezeTime(.3f, .6f);
                CameraManager.ShakeCamera(.2f);
                EffectPlayer effectPlayer = (PoolManager.Instance.Pop(PoolingType.Effect_ShockWave) as EffectPlayer);
                _player.EndRootMotion();
                SoundManager.PlaySFX(_parryingSound);
                effectPlayer.transform.position = point;
                effectPlayer.PlayEffects();
            }
            else
            {
                if (TutorialManager.Instance)
                {
                    TutorialManager tutorialManager = (TutorialManager.Instance as TutorialManager);
                    if (tutorialManager)
                        if (tutorialManager.currentStep == 4 && !tutorialManager.ExistsGoal(0))
                        {
                            tutorialManager.ClearGoal();
                            tutorialManager.UpdateGoal(0);
                        }
                }

                SoundManager.PlaySFX(_guardSound, point);
                _player.Guard(enemy.WeaponDamageType);
            }

            weapon.EnableCollider(false);
        }
    }

    public void SetParryingAble(bool enable)
    {
        _parryingAble = enable;
    }
}