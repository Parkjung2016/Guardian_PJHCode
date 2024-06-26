using FIMSpace.FLook;
using FIMSpace.FProceduralAnimation;
using RootMotion.FinalIK;
using UnityEngine;

public partial class PlayerController
{
    private void Awake()
    {
        Model = transform.Find("Model");
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        AnimatorCompo = Model.GetComponent<Animator>();
        FullBodyBipedIKCompo = Model.GetComponent<FullBodyBipedIK>();
        HealthCompo = GetComponent<Health>();
        CursorUtility.EnableCursor(false);
        CurrentEnergy = _maxEnergy;
        ComboPossible();
        _parryingCollider = GetComponentInChildren<ParryingCollider>();
        _lookAnimator = Model.GetComponent<FLookAnimator>();
        _legsAnimator = Model.GetComponent<LegsAnimator>();
        _fov = Model.GetComponent<FieldOfView>();
        _impossibleParringIcon = transform.Find("ImpossibleParryingIcon");
    }

    private void Start()
    {
        byte weaponIdx = PlayerDataManager.playerData.currentWeapon;
        PlayerWeapon weapon = PlayerManager.Instance.weapons[weaponIdx];
        SwapWeapon(weapon);
        EnableImpossibleParryingIcon(false);
        CurrentPotionCount = _maxPotionCount;
    }

    private void Update()
    {
        CheckLockOnWhileRunning();
        IncreaseCurrentPressInputTime();
        UpdateAnimatorParameters();

        DetectInteractAble();
        CheckSkillCooldown();
    }

    private void FixedUpdate()
    {
        CalculateMovement();
        ImpossibleParryingIconLookAtCamera();
        ApplyGravity();
        if (!useRootMotion)
            Move(_moveVelocity);
    }
}