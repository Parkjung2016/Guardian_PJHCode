using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using VolumeManager = PJH.VolumeManager;

[Serializable]
public struct WeaponSlashEffectInfo
{
    public Vector3 pos;
    public Vector3 angle;
}


public enum PlayerWeaponDamageType
{
    Normal,
    Strong,
    AfterEvasion,
    AfterRun
}

[Serializable]
public struct PlayerWeaponColliderEffect
{
    public float chromaticAberrationValue;
    public float lensDistortionValue;
    public float freezeTime;
    public float freezeValue;
    public float shakeValue;
}

[Serializable]
public struct PlayerWeaponData
{
    public SerializedDictionary<PlayerWeaponDamageType, int> weaponDamage;

    public Vector3 weaponColliderCenter;
    public Vector3 weaponColliderSize;

    #region energy

    [TabGroup("multi row", "Energy", SdfIconType.Battery, TextColor = "orange", TabLayouting = TabLayouting.MultiRow)]
    public int reduceEnergyWhenNormalAttack,
        reduceEnergyWhenRunningAttack,
        reduceEnergyWhenStrongAttack;

    #endregion

    public EventReference normalAttackSound,
        strongAttackSound,
        afterEvasionAttackSound,
        raiseWeaponSound;


    public PlayerWeaponColliderEffect normalAttackColliderEffect,
        afterEvasionAttackColliderEffect,
        strongAttackColliderEffect;
}

public enum PlayerWeaponSocketType
{
    LeftHand,
    RightHand
}

public class PlayerWeapon : Weapon
{
    #region hash

    private readonly int VoronoiColorHash = Shader.PropertyToID("Color_7FBEC34D");
    private readonly int FresnelColorHash = Shader.PropertyToID("Color_B154983C");

    #endregion

    #region runtima animator controller

    [SerializeField] private RuntimeAnimatorController _skillAnimatorController;
    [SerializeField] private RuntimeAnimatorController _animatorController;

    #endregion

    #region color

    [SerializeField] private Color _playerCapeColor;

    [SerializeField, ColorUsage(true, true)]
    private Color _skillWeaponColor;

    #endregion

    [SerializeField] private float _attackSpeed = 1, _skillAttackSpeed = 1;
    [SerializeField] private float _skillCooldown;
    [SerializeField] private float _endSkillTime;
    [SerializeField] private float _hitGroundShakeCameraValue = .12f;

    [SerializeField] private Sprite _skillIconTexture;
    [SerializeField] private List<WeaponSlashEffectInfo> _weaponEffectInfos;
    [SerializeField] private List<WeaponSlashEffectInfo> _weaponEffect2Infos;
    [SerializeField] private PlayerWeaponData _weaponData, _weaponDataWhenSkill;
    [SerializeField] private PoolingType _weaponEffect1, _weaponEffect2;
    [SerializeField] private EventReference _useSkillSound;

    private Action _endSkillCallBack;
    private VisualEffect _skillVFX;
    private MeshRenderer _weaponMeshRenderer;
    private PlayerController _player;
    private List<Transform> _weaponEffectTrm;
    private List<Transform> _weaponEffect2Trm;
    private Transform _playerHandLIK;
    private RuntimeAnimatorController _playerOriginAnimator;

    public PlayerWeaponData CurrentWeaponData => _endSkillCallBack != null ? _weaponDataWhenSkill : _weaponData;

    public PlayerWeaponSocketType socketType;
    public MovementSpeed locomotionSpeed, strafeSpeed;
    public float rollSpeed;
    public float animationCrossFadeDuration = .1f;
    [HideInInspector] public PlayerWeaponDamageType damageType;
    [HideInInspector] public bool willEndSkill;


    protected override void Awake()
    {
        base.Awake();
        _skillVFX = GetComponentInChildren<VisualEffect>();
        _weaponMeshRenderer = GetComponent<MeshRenderer>();
        _playerHandLIK = transform.Find("PlayerHandLIK");
    }

    private void Start()
    {
        SetWeaponEffectTrm();
        SetWeaponEffect2Trm();
    }

    protected override void TriggerCollider(Collider other)
    {
        Ray ray = new Ray(transform.position, transform.up);
        bool result = Physics.Raycast(ray, out RaycastHit hit, 10, _whatIsTarget);
        Vector3 point = other.ClosestPoint(transform.position);

        if (other.TryGetComponent(out BodyPart part))
        {
            EnableCollider(false);
            PlayerController player = PlayerManager.Instance.Player;
            bool afterEvasionAttacking = player.AfterEvasionAttacking;
            bool strongAttacking = player.StrongAttacking;
            PlayerWeaponColliderEffect effect;
            if (strongAttacking)
            {
                effect = CurrentWeaponData.strongAttackColliderEffect;
            }
            else if (afterEvasionAttacking)
            {
                effect = CurrentWeaponData.afterEvasionAttackColliderEffect;
            }
            else
            {
                effect = CurrentWeaponData.normalAttackColliderEffect;
            }


            float chromaticAberrationValue = effect.chromaticAberrationValue;
            float lensDistortionValue = effect.lensDistortionValue;
            float freezeValue = effect.freezeValue;
            float freezeTime = effect.freezeTime;
            float shakeValue = effect.shakeValue;

            VolumeManager.SetChromaticAberration(chromaticAberrationValue,
                () => { VolumeManager.SetChromaticAberration(0); });
            VolumeManager.SetLensDistortion(lensDistortionValue, () => { VolumeManager.SetLensDistortion(0); });
            VolumeManager.SetRadialBur(.38f, async () =>
            {
                await Task.Delay(500);
                VolumeManager.SetRadialBur(0);
            });
            TimeManager.SetFreezeTime(freezeValue, freezeTime);
            CameraManager.ShakeCamera(shakeValue);

            Quaternion rot = Quaternion.identity;
            if (result)
            {
                rot = Quaternion.LookRotation(-hit.normal);
            }

            EffectPlayer effectPlayer = (PoolManager.Instance.Pop(PoolingType.Effect_Spark) as EffectPlayer);
            effectPlayer.transform.SetPositionAndRotation(point, rot);
            effectPlayer.PlayEffects();
            SoundManager.PlaySFX(_impactSounds[part.enemy.ImpactType], point);
            part.ApplyDamage(CurrentWeaponData.weaponDamage[damageType]);
        }
        else
        {
            CameraManager.ShakeCamera(_hitGroundShakeCameraValue);
            SoundManager.PlaySFX(_impactSounds[ImpactType.Rock], point);
            Quaternion rot = Quaternion.identity;
            if (result)
            {
                rot = Quaternion.LookRotation(-hit.normal);
            }

            EffectPlayer effectPlayer = (PoolManager.Instance.Pop(PoolingType.Effect_ImpactOnRock) as EffectPlayer);
            effectPlayer.transform.SetPositionAndRotation(point, rot);
            effectPlayer.PlayEffects();
        }
    }


    private void SetWeaponEffectTrm()
    {
        _weaponEffectTrm = new();
        for (int i = 0; i < _weaponEffectInfos.Count; i++)
        {
            Transform trm = new GameObject("WeaponEffectTrm").transform;
            trm.SetParent(_player.Model);
            trm.localPosition = _weaponEffectInfos[i].pos;
            trm.localEulerAngles = _weaponEffectInfos[i].angle;
            _weaponEffectTrm.Add(trm);
        }
    }

    private void SetWeaponEffect2Trm()
    {
        _weaponEffect2Trm = new();
        for (int i = 0; i < _weaponEffect2Infos.Count; i++)
        {
            Transform trm = new GameObject("WeaponEffect2Trm").transform;
            trm.SetParent(_player.Model);
            trm.localPosition = _weaponEffect2Infos[i].pos;
            trm.localEulerAngles = _weaponEffect2Infos[i].angle;
            _weaponEffect2Trm.Add(trm);
        }
    }

    public void UseSkill(Action callBack)
    {
        _skillVFX.Play();
        if (socketType == PlayerWeaponSocketType.RightHand)
        {
            _playerOriginAnimator = _player.AnimatorCompo.runtimeAnimatorController;
            _player.SetAttackSpeed(_skillAttackSpeed);
            _player.AnimatorCompo.runtimeAnimatorController = _skillAnimatorController;
            SoundManager.PlaySFX(_useSkillSound);
        }

        DOTween.To(() => _weaponMeshRenderer.material.GetColor(VoronoiColorHash),
            x => _weaponMeshRenderer.material.SetColor(VoronoiColorHash, x), _skillWeaponColor, 1f);
        DOTween.To(() => _weaponMeshRenderer.material.GetColor(FresnelColorHash),
            x => _weaponMeshRenderer.material.SetColor(FresnelColorHash, x), _skillWeaponColor, 1f);
        ChangeCollider(_weaponDataWhenSkill);
        StartCoroutine(StartEndSkillTime(callBack));
    }

    IEnumerator StartEndSkillTime(Action callBack)
    {
        yield return CoroutineHelper.WaitForSeconds(_endSkillTime);
        _endSkillCallBack = callBack;
        if (_player.IsAttacking)
        {
            willEndSkill = true;
        }
        else
        {
            EndSkill();
        }
    }

    private void ChangeCollider(PlayerWeaponData data)
    {
        BoxCollider collider = (_weaponCol as BoxCollider);
        collider.size = data.weaponColliderSize;
        collider.center = data.weaponColliderCenter;
    }

    public void EndSkill(float duration = 1)
    {
        if (_playerOriginAnimator == null) return;
        _endSkillCallBack?.Invoke();
        _endSkillCallBack = null;
        willEndSkill = false;
        _skillVFX.Stop();
        ChangeCollider(_weaponData);
        if (socketType == PlayerWeaponSocketType.RightHand)
        {
            _player.AnimatorCompo.runtimeAnimatorController = _playerOriginAnimator;

            _player.SetAttackSpeed(_attackSpeed);
        }

        DOTween.To(() => _weaponMeshRenderer.material.GetColor(VoronoiColorHash),
            x => _weaponMeshRenderer.material.SetColor(VoronoiColorHash, x), Color.black, duration);
        DOTween.To(() => _weaponMeshRenderer.material.GetColor(FresnelColorHash),
            x => _weaponMeshRenderer.material.SetColor(FresnelColorHash, x), Color.black, duration);
    }

    public void PlayWeaponEffect(int idx)
    {
        Transform trm = _weaponEffectTrm[idx];
        (PoolManager.Instance.Pop(_weaponEffect1) as EffectPlayer).PlayEffects(
            trm.position, trm.eulerAngles);
    }

    public void PlayWeaponEffect2(int idx)
    {
        Transform trm = _weaponEffect2Trm[idx];
        (PoolManager.Instance.Pop(_weaponEffect2) as EffectPlayer).PlayEffects(
            trm.position, trm.eulerAngles);
    }


    public void EquipWeapon(PlayerController player)
    {
        _player = player;
        if (socketType == PlayerWeaponSocketType.RightHand)
        {
            _player.ChangeCapeAndHatColor(_playerCapeColor);
            _player.skillCooldown = _skillCooldown;
            MainUI.Instance.ChangeSkillIcon(_skillIconTexture);
        }

        _player.SetHandLIK(_playerHandLIK);
        _playerOriginAnimator = _animatorController;
        EndSkill(duration: 0f);
    }
}