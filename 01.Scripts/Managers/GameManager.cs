using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using PJH;
using UnityEngine;
using UnityEngine.Playables;
using Task = System.Threading.Tasks.Task;

public enum BossTimeLineType
{
    Begin,
    Step2
}

[Serializable]
public struct BossTimeLine
{
    public BossTimeLineType type;
    public PlayableAsset PlayableAsset;
}

public class GameManager : Manager
{
    [SerializeField] private EventReference _bgm, _battleBgm, _battleBgm2;
    [SerializeField] private List<BossTimeLine> _bossTimeLines;
    [SerializeField] private string _bossName;
    [SerializeField] private GameObject _enemy, _timeLineGroup;
    [SerializeField] private GameObject _endBossGoToLobbyInteract;
    [SerializeField] private Transform _beginStep2PlayerPos, _beginStep2EnemyPos;
    private PlayableDirector _playableDirector;
    private Transform _beginBattlePlayerTrm;

    public static GameManager ParentInstance => Instance as GameManager;
    private Action _beginBattleAction;
    private Action _beginStep2Action;
    private Enemy _boss;

    protected override void Awake()
    {
        base.Awake();

        _beginBattlePlayerTrm = transform.Find("BeginBattlePlayerPosition");
        _playableDirector = GetComponent<PlayableDirector>();
        _boss = _enemy.GetComponent<Enemy>();
    }

    private IEnumerator Start()
    {
        _timeLineGroup.SetActive(false);
        _boss.HealthCompo.OnHealthHalfEvent += HandleBossHealthHalfEvent;
        _boss.HealthCompo.OnDeadEvent += HandleBossDeadEvent;
        yield return new WaitUntil(() => PlayerManager.Instance.Player);
        PlayerController player = PlayerManager.Instance.Player;
        player.canCombat = true;
        player.canReduceEnergy = true;
        player.canLockOn = false;
        // player.canUseSkill = false;
        SoundManager.PlayBGM(_bgm);
        _endBossGoToLobbyInteract.SetActive(false);
        _enemy.gameObject.SetActive(false);
    }

    private void HandleBossHealthHalfEvent()
    {
        PlayerController player = PlayerManager.Instance.Player;

        SoundManager.SetSubMasterVolume(0);

        FadeUI.FadeIn(0, callBack: async () =>
        {
            _boss.BehaviorTreeCompo.DisableBehavior(true);

            _enemy.gameObject.SetActive(false);
            player.InputReader.EnableUIInput(false);
            player.InputReader.EnablePlayerInput(false);
            player.gameObject.SetActive(false);
            SoundManager.SetSubMasterVolume(1);
            await Task.Delay(1000);

            MainUI.Instance.gameObject.SetActive(false);
            SoundManager.StopBGM();
            _timeLineGroup.SetActive(true);
            PlayTimeLine(BossTimeLineType.Step2);
            LockOnUI.Instance.gameObject.SetActive(false);
            FadeUI.FadeOut(1);
        });
    }

    public void EnterGate(Quaternion playerRotation, Action callBack)
    {
        PlayerController player = PlayerManager.Instance.Player;
        player.Model.DORotateQuaternion(playerRotation, .5f);
        player.stopRotate = true;
        player.useRootMotion = true;
        player.InputReader.EnableUIInput(false);
        player.InputReader.EnablePlayerInput(false);
        SoundManager.SetSubMasterVolumeWithDuration(0, 2f);
        player.AnimatorCompo.CrossFadeInFixedTime("WalkFrontRootMotion", .1f);
        _beginBattleAction = callBack;

        FadeUI.FadeIn(2.5f, callBack: async () =>
        {
            if (PlayerManager.Instance.enteredBossGate)
            {
                SoundManager.SetSubMasterVolume(1);
                BeginBattle();
            }
            else
            {
                await Task.Delay(1000);
                SoundManager.SetSubMasterVolume(1);
                MainUI.Instance.gameObject.SetActive(false);
                SoundManager.StopBGM();

                _timeLineGroup.SetActive(true);
                PlayTimeLine(BossTimeLineType.Begin);
                LockOnUI.Instance.gameObject.SetActive(false);
                FadeUI.FadeOut(2);
            }
        });
    }

    public void PlayTimeLine(BossTimeLineType type)
    {
        _playableDirector.playableAsset = _bossTimeLines.Find(x => x.type == type).PlayableAsset;
        _playableDirector.Play();
    }

    public void PlayBossBGM()
    {
        SoundManager.PlayBGM(_battleBgm);
    }

    public void BeginBattle()
    {
        FadeUI.FadeIn(callBack: () =>
        {
            PlayerManager.Instance.enteredBossGate = true;
            StartCoroutine(SetPlayerPosToBeginBattle());
            _beginBattleAction?.Invoke();
        });
    }

    public void BeginStep2()
    {
        SoundManager.SetSubMasterVolumeWithDuration(0, .3f);
        FadeUI.FadeIn(callBack: () =>
        {
            PlayerController player = PlayerManager.Instance.Player;
            _enemy.transform.SetPositionAndRotation(_beginStep2EnemyPos.position, _beginStep2EnemyPos.rotation);
            player.transform.position = _beginStep2PlayerPos.position;
            player.Model.rotation = _beginStep2PlayerPos.rotation;
            _enemy.gameObject.SetActive(true);
            player.gameObject.SetActive(true);
            player.InputReader.EnableUIInput(true);
            player.InputReader.EnablePlayerInput(true);
            player.EndRootMotion();
            player.stopMove = false;
            if (player.CurrentWeaponL)
                player.CurrentWeaponL.EndSkill();
            player.CurrentWeaponR.EndSkill();
            player.isUsingSkill = false;
            player.canUseSkill = true;
            player.canSkillCooldown = false;
            player.LockOn(false);
            SoundManager.SetSubMasterVolume(1);

            MainUI.Instance.EndSkillCool();
            _boss.StartStep2();
            _boss.BehaviorTreeCompo.EnableBehavior();

            _timeLineGroup.SetActive(false);
            SoundManager.PlayBGM(_battleBgm2);

            MainUI.Instance.gameObject.SetActive(true);
            LockOnUI.Instance.gameObject.SetActive(false);
            _beginStep2Action?.Invoke();
            _playableDirector.Stop();
            FadeUI.FadeOut(1);
        });
    }

    public void HandleBossDeadEvent()
    {
        SoundManager.EndBGM();
        TimeManager.SetFreezeTime(.3f, 2f);
        int bossStep = PlayerDataManager.playerData.bossStep;
        PlayerDataManager.playerData.bossStep = (byte)Mathf.Clamp(bossStep + 1, 1, 2);
        PlayerDataManager.Save();
        _endBossGoToLobbyInteract.SetActive(true);
        PlayerController player = PlayerManager.Instance.Player;
        MainUI.Instance.HideBossHealthBar();
        player.canLockOn = false;
        player.LockOn(false);
    }

    private IEnumerator SetPlayerPosToBeginBattle()
    {
        PlayerController player = PlayerManager.Instance.Player;
        player.CharacterControllerCompo.enabled = false;
        player.transform.position = _beginBattlePlayerTrm.position;
        player.Model.rotation = _beginBattlePlayerTrm.rotation;
        _playableDirector.Stop();
        yield return CoroutineHelper.WaitForSeconds(2f);
        _enemy.SetActive(true);
        _timeLineGroup.SetActive(false);
        FadeUI.FadeOut(1);
        yield return null;
        player.CharacterControllerCompo.enabled = true;
        MainUI.Instance.SetBossName(_bossName);
        MainUI.Instance.gameObject.SetActive(true);
        PlayBossBGM();

        yield return CoroutineHelper.WaitForSeconds(.8f);
        player.InputReader.EnablePlayerInput(true);
        player.InputReader.EnableUIInput(true);

        player.canLockOn = true;
        player.canUseSkill = true;
        player.isUsingSkill = false;
        player.canSkillCooldown = false;
        MainUI.Instance.EndSkillCool();
        player.EndRootMotion();
    }
}