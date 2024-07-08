using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Obi;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbRopeTrigger : MonoBehaviour,IInteractAble
{
    [field: SerializeField] public string InteractName { get; set; }
    public bool CanInteract { get; set; } = true;

    public Transform Trm { get; set; }

    [SerializeField] private ObiRope _obiRope;
    [SerializeField] private float _ropeUpSpeed;
    [SerializeField] private Transform _climbUpTrm;
    [SerializeField] private EventReference _ropeSound;
    [HideInInspector] public Transform handLPos, handRPos;
    private ObiRopeCursor _obiRopeCursor;
    private PlayerController _player;
    private Transform _originHandLTarget, _originHandRTarget;

    private EventInstance _ropeEventInstance;

    private void Awake()
    {
        Trm = transform;

        _obiRopeCursor = _obiRope.GetComponent<ObiRopeCursor>();
        handLPos = transform.Find("HandLPos");
        handRPos = transform.Find("HandRPos");
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerManager.Instance.Player);
        _player = PlayerManager.Instance.Player;
    }


    IEnumerator ChangeRopeLength()
    {
        while (_obiRope.restLength > 1)
        {
            _player.transform.position = _obiRope.GetParticlePosition(153) + transform.forward * .2f;
            _obiRopeCursor.ChangeLength(_obiRope.restLength - _ropeUpSpeed * Time.deltaTime);
            yield return null;
        }

        _ropeEventInstance.setParameterByName("End", 1);
        _player.transform.position = _climbUpTrm.position;
        _player.CharacterControllerCompo.enabled = true;
        _player.SetHandLIKImmediately(_originHandLTarget);
        _player.SetHandRIKImmediately(_originHandRTarget);
        _player.AnimatorCompo.CrossFadeInFixedTime("Locomotion", .2f);
        _player.InputReader.EnablePlayerInput(true);
        _player.EndRootMotion();
        MainUI.Instance.canChangePlayerInputReader = true;
        yield return new WaitForFixedUpdate();
        _obiRope.enabled = false;
        yield return null;
        _obiRope.enabled = true;

        yield return CoroutineHelper.WaitForSeconds(6);
        CanInteract = true;
    }

    private void Update()
    {
        handLPos.position = _obiRope.GetParticlePosition(143);
    }

    public void Interact()
    {
        CanInteract = false;
        _player.CharacterControllerCompo.enabled = false;
        MainUI.Instance.canChangePlayerInputReader = false;

        _player.InputReader.EnablePlayerInput(false);
        _originHandRTarget = _player.FullBodyBipedIKCompo.solver.rightHandEffector.target;
        _originHandLTarget = _player.FullBodyBipedIKCompo.solver.leftHandEffector.target;
        _player.transform.DOMove(_obiRope.GetParticlePosition(153) + transform.forward * .2f, .5f).OnComplete(
            async () =>
            {
                StartCoroutine(ChangeRopeLength());
                await Task.Delay(300);
                _player.SetHandLIK(handLPos);
                _player.SetHandRIK(handLPos);
                _ropeEventInstance = SoundManager.PlaySFX(_ropeSound);
                _player.AnimatorCompo.CrossFadeInFixedTime("RopeUp", .2f);
            });
    }
}