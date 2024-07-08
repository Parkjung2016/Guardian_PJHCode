using System;
using System.Collections.Generic;
using DG.Tweening;
using FIMSpace.FLook;
using FIMSpace.FProceduralAnimation;
using FMODUnity;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.VFX;

[Serializable]
public struct MovementSpeed
{
    public float walkSpeed;
    public float runSpeed;
}


public partial class PlayerController
{
    [SerializeField] private InputReader _inputReader;

    #region animation hash

    private readonly int rootMotionMultiplierHash = Animator.StringToHash("RootMotionMultiplier");

    private readonly int velocityHash = Animator.StringToHash("Velocity");
    private readonly int horizontalInputHash = Animator.StringToHash("HorizontalInput");
    private readonly int verticalInputHash = Animator.StringToHash("VerticalInput");
    private readonly int strafeHash = Animator.StringToHash("Strafe");
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private readonly int isGuardHash = Animator.StringToHash("IsGuard");
    private readonly int attackSpeedhash = Animator.StringToHash("AttackSpeed");

    #endregion

    #region energy

    [TabGroup("multi row", "Energy", SdfIconType.Battery, TextColor = "orange", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private int _maxEnergy = 100;

    public int MaxEnergy => _maxEnergy;

    [TabGroup("multi row", "Energy", SdfIconType.Battery, TextColor = "orange", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private float _startIncreaseEnergyTime = 1f;

    [TabGroup("multi row", "Energy", SdfIconType.Battery, TextColor = "orange", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private int _increaseEnergy = 1;

    [TabGroup("multi row", "Energy", SdfIconType.Battery, TextColor = "orange", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private float _increaseEnergyTime = .5f, _decreaseEnergyTimeWhenRunning = .2f;

    [TabGroup("multi row", "Energy", SdfIconType.Battery, TextColor = "orange", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private int _reduceEnergyWhenRunning = 5,
        _reduceEnergyWhenBlock = 6,
        _reduceEnergyWhenParrying = 5,
        _reduceEnergyWhenEvasion = 10;

    public event Action<float> OnEnergyChangedEvent;
    private int _currentEnergy;
    private float _currentDecreaseEnergyWhenRunningTime;
    private Coroutine _startIncreaseEnergyCoroutine;

    private int reduceEnergyWhenNormalAttack => _currentWeaponR.CurrentWeaponData.reduceEnergyWhenNormalAttack;

    private int reduceEnergyWhenRunningAttack => _currentWeaponR.CurrentWeaponData.reduceEnergyWhenRunningAttack;

    private int reduceEnergyWhenStrongAttack => _currentWeaponR.CurrentWeaponData.reduceEnergyWhenStrongAttack;

    public int CurrentEnergy
    {
        get => _currentEnergy;
        set
        {
            if (value < _currentEnergy)
            {
                if (_startIncreaseEnergyCoroutine != null) StopCoroutine(_startIncreaseEnergyCoroutine);
                _startIncreaseEnergyCoroutine = StartCoroutine(StartIncreaseEnergy());
            }

            _currentEnergy = Mathf.Clamp(value, 0, _maxEnergy);
            if (_currentEnergy <= _reduceEnergyWhenBlock && _isGuard)
            {
                Guard(false);
                _isParrying = false;
                SetHandIKWeight1();
            }

            OnEnergyChangedEvent?.Invoke((float)_currentEnergy / _maxEnergy);
        }
    }

    #endregion

    #region sound

    [TabGroup("multi row", "Sound", SdfIconType.Soundwave, TextColor = "yellow", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private Dictionary<FootstepType, EventReference> _footstepSounds;

    [TabGroup("multi row", "Sound", SdfIconType.Soundwave, TextColor = "yellow", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private EventReference _playRollStartSound, _playLandInRollSound;

    [TabGroup("multi row", "Sound", SdfIconType.Soundwave, TextColor = "yellow", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private EventReference _hitSound;

    [TabGroup("multi row", "Sound", SdfIconType.Soundwave, TextColor = "yellow", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private EventReference _recoveryHPSound;

    [TabGroup("multi row", "Sound", SdfIconType.Soundwave, TextColor = "yellow", TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private EventReference _appearImpossibleParryingIconSound;

    #endregion

    #region components

    public Transform Model { get; private set; }
    public Animator AnimatorCompo { get; private set; }
    public Health HealthCompo { get; private set; }

    public FullBodyBipedIK FullBodyBipedIKCompo { get; private set; }
    private CharacterController _controller;
    private Camera _mainCamera;
    private ParryingCollider _parryingCollider;
    private LegsAnimator _legsAnimator;
    private FLookAnimator _lookAnimator;
    private FieldOfView _fov;
    private PlayerWeapon _currentWeaponL, _currentWeaponR;
    private Transform _impossibleParringIcon;

    public CharacterController CharacterControllerCompo => _controller;
    public PlayerWeapon CurrentWeaponL => _currentWeaponL;
    public PlayerWeapon CurrentWeaponR => _currentWeaponR;

    [SerializeField] private Transform _lockOnTarget;
    [SerializeField] private Transform _weaponSocketL, _weaponSocketR;
    [SerializeField] private ParticleSystem _recoveryHPParticle;
    [SerializeField] private InputActionReference _interactInputActionReference;
    [SerializeField] private SkinnedMeshRenderer _capeSkinnedMeshRenderer;
    [SerializeField] private MeshRenderer _hatSkinnedMeshRenderer;

    #endregion

    #region locomotion

    [HideInInspector] public bool stopMove;
    [HideInInspector] public bool stopRotate;

    [TabGroup("multi row", " Locomotion", SdfIconType.Activity, TextColor = "blue",
        TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private float _rotationSpeed = 16;

    [TabGroup("multi row", " Locomotion", SdfIconType.Activity, TextColor = "blue",
        TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private float _extraGravity = -9.8f;


    [TabGroup("multi row", " Locomotion", SdfIconType.Activity, TextColor = "blue",
        TabLayouting = TabLayouting.MultiRow)]
    [SerializeField]
    private float _maxAfterEvasionAttackTime = .4f;


    private MovementSpeed locomotionSpeed => _currentWeaponR.locomotionSpeed;
    private MovementSpeed strafeSpeed => _currentWeaponR.strafeSpeed;

    private float rollSpeed => _currentWeaponR.rollSpeed;

    public float Speed
    {
        get
        {
            MovementSpeed speed = locomotionSpeed;
            speed = _isLockOn ? strafeSpeed : locomotionSpeed;
            return _isRunning && CurrentEnergy > 0 ? speed.runSpeed : speed.walkSpeed;
        }
    }

    #endregion

    #region coroutine

    private Coroutine _checkAfterEvasionAttackCoroutine;
    private Coroutine _hideImpossibleParryingIconCoroutine;
    private Coroutine _setHandIKWeight1Coroutine;

    #endregion

    #region property

    public bool IsGrounded => _controller.isGrounded;
    public bool IsEvasion => _isEvasion;
    public bool AfterEvasionAttacking => _afterEvasionAttacking;
    public bool StrongAttacking => _strongAttacking;
    public bool IsAttacking => _isAttacking;
    public bool IsGuard => _isGuard;
    public bool IsHitting => _isHitting;
    public InputReader InputReader => _inputReader;

    public int CurrentPotionCount
    {
        get => _currentPotionCount;
        set
        {
            _currentPotionCount = value;
            MainUI.Instance.ChangePotionCount(_currentPotionCount);
        }
    }

    #endregion

    public bool UseRootMotion => AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsTag("UseRootMotion") ||
                                 AnimatorCompo.GetNextAnimatorStateInfo(0).IsTag("UseRootMotion") ||
                                 AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsTag("Attack") ||
                                 AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsTag("KnockDown");


    [HideInInspector] public Vector3 input;
    [HideInInspector] public float skillCooldown;
    [HideInInspector] public bool canCombat = true;
    [HideInInspector] public bool canEvasion = true;
    [HideInInspector] public bool canReduceEnergy = true;
    [HideInInspector] public bool canLockOn = true;
    [HideInInspector] public bool canUseSkill = true;
    [HideInInspector] public bool isUsingSkill;
    [HideInInspector] public bool canSkillCooldown;
    [SerializeField] private int _maxPotionCount = 3;
    [SerializeField] private int _potionRecoveryHP = 10;
    private Sequence _handLIKSequence;
    private Sequence _handRIKSequence;
    private Vector3 _moveVelocity;
    private bool _canRecoveryHP = true;
    private bool _isRunning;
    private bool _isAttacking;
    private bool _isEvasion;
    private bool _isLockOn;
    private bool _isParrying;
    private bool _isGuard;
    private bool _isDead;
    private bool _isUsingSkillAnimation;
    private bool _comboPossible;
    private bool _canAfterEvasionAttack;
    private bool _isHitting;
    private bool _afterEvasionAttacking;
    private bool _strongAttacking;
    private bool _canIncreasePressInputTime;
    private float _velocityY;
    private float _currentSkillCooldownTime;
    private float _currentPressInputTime;
    private int _comboCount;
    private int _currentPotionCount;
}