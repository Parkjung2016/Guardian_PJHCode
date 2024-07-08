using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TutorialManager : Manager
{
    [SerializeField] private TypeWriter _noticeText;
    [SerializeField] private TextMeshProUGUI _goalText;
    [SerializeField] private GameObject _goalPos;
    [SerializeField] private GameObject _portal;
    public int currentStep;
    [HideInInspector] public int currentGoal;
    [HideInInspector] public int maxGoal;

    private List<int> _goalList = new();
    private List<string> _stepGoalText = new();
    private TutorialEnemy _enemy;
    private Action _nextNoticeEvent;

    protected override void Awake()
    {
        base.Awake();
        _goalText.gameObject.SetActive(false);
        _enemy = FindObjectOfType<TutorialEnemy>();
    }

    private IEnumerator Start()
    {
        _portal.SetActive(false);
        _goalPos.SetActive(false);
        _noticeText.SetTypeWriter("");
        yield return new WaitUntil(() => PlayerManager.Instance.Player);
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return new WaitForFixedUpdate();
        PlayerManager.Instance.SuccessTeleportPlayerEvent += HandleSuccessTeleportPlayerEvent;
    }

    private void Update()
    {
        DetectGoalPos();
    }

    private void DetectGoalPos()
    {
        if (currentStep != 0) return;
        float distance =
            Vector3.Distance(PlayerManager.Instance.Player.transform.position, _goalPos.transform.position);
        if (distance <= .45f)
        {
            if (currentStep == 0)
            {
                currentGoal = 3;
                ClearGoal();
                UpdateGoal(0);
            }
        }
    }

    private void HandleSuccessTeleportPlayerEvent()
    {
        StartCoroutine(BeginTutorialText());
        PlayerManager.Instance.SuccessTeleportPlayerEvent -= HandleSuccessTeleportPlayerEvent;
    }

    IEnumerator BeginTutorialText()
    {
        currentStep = 0;
        MainUI.Instance.canChangePlayerInputReader = false;
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return CoroutineHelper.WaitForSeconds(1);
        yield return _noticeText.SetTypeWriterCoroutine("튜토리얼에 오신 것을 환영합니다.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        yield return _noticeText.SetTypeWriterCoroutine("먼저 기본적인 움직임에 대해 알아보겠습니다.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        yield return _noticeText.SetTypeWriterCoroutine("WASD로 움직일 수 있으며 Shift로 달리기할 수 있습니다.");
        yield return CoroutineHelper.WaitForSeconds(1);
        yield return _noticeText.SetTypeWriterCoroutine("또한, Space Bar를 눌러 회피할 수 있습니다.");
        yield return CoroutineHelper.WaitForSeconds(1);
        yield return _noticeText.SetTypeWriterCoroutine("목표지점까지 이동해보세요.");
        yield return CoroutineHelper.WaitForSeconds(.5f);
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "목표지점까지 가기 : 0/1",
            "이동 : <sprite=10> <sprite=9> <sprite=11> <sprite=12>",
            "회피 : <sprite=6>",
            "달리기 : <sprite=5>",
        };

        SetGoal(goals);
        _nextNoticeEvent = () => StartCoroutine(FirstTutorialText());

        MainUI.Instance.canChangePlayerInputReader = true;

        _goalPos.SetActive(true);
    }

    IEnumerator FirstTutorialText()
    {
        MainUI.Instance.canChangePlayerInputReader = false;
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("잘하셨습니다. 이제 적을 소환하여 공격에 대해 알려 드리겠습니다.");
        yield return CoroutineHelper.WaitForSeconds(1.5f);
        yield return _noticeText.SetTypeWriterCoroutine("적을 공격하기 전에 락온을 한 상태에서 공격하면 훨씬 수월합니다.");
        yield return CoroutineHelper.WaitForSeconds(1.5f);
        yield return _noticeText.SetTypeWriterCoroutine("락온은 휠 클릭을 통해 가능합니다.");
        yield return CoroutineHelper.WaitForSeconds(1);
        yield return _noticeText.SetTypeWriterCoroutine("락온을 해보세요.");
        yield return CoroutineHelper.WaitForSeconds(.5f);
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "락온 : 0/1 <sprite=4>"
        };

        SetGoal(goals);
        _nextNoticeEvent = () => StartCoroutine(SecondTutorialText());
        _enemy.ShowEnemy();
        MainUI.Instance.canChangePlayerInputReader = true;

        _goalPos.SetActive(true);
    }

    private void SetGoal(string[] goals)
    {
        _stepGoalText.Clear();
        _goalText.gameObject.SetActive(true);
        maxGoal = goals.Length;
        for (int i = 0; i < maxGoal; i++)
        {
            _goalList.Add(0);
            _stepGoalText.Add(goals[i]);
        }

        SetGoalText();
    }

    public bool CheckPlayerEvasion()
    {
        return _enemy.AnimatorCompo.GetCurrentAnimatorStateInfo(0).IsName("Atk_Grab") &&
               _enemy.AnimatorCompo.GetCurrentAnimatorStateInfo(0).normalizedTime >= .35f;
    }

    IEnumerator SecondTutorialText()
    {
        _goalPos.SetActive(false);

        MainUI.Instance.canChangePlayerInputReader = false;
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("잘하셨습니다. 이제 공격을 해볼 차례입니다.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        yield return _noticeText.SetTypeWriterCoroutine("기본공격, 강 공격, 달리기 후 공격, 회피 후 공격이 존재합니다.");
        yield return CoroutineHelper.WaitForSeconds(1);
        yield return _noticeText.SetTypeWriterCoroutine("모든 공격을 한번 씩 사용해보세요.");
        yield return CoroutineHelper.WaitForSeconds(.5f);

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "기본 공격 : 0/1 <sprite=3>",
            "강 공격 : 0/1 <sprite=3>~",
            "달리기 후 공격 : 0/1 <sprite=5>~ + <sprite=3>",
            "<size=22>(약간의 거리가 있어야 됩니다)</size>",
            "회피 공격 : 0/1 <sprite=6> + <sprite=3>"
        };
        SetGoal(goals);
        _nextNoticeEvent = () => StartCoroutine(ThirdTutorialText());
        MainUI.Instance.canChangePlayerInputReader = true;
    }

    IEnumerator ThirdTutorialText()
    {
        MainUI.Instance.canChangePlayerInputReader = false;
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("잘하셨습니다. 다음으로는 포션 사용법입니다.");
        yield return CoroutineHelper.WaitForSeconds(1);
        yield return _noticeText.SetTypeWriterCoroutine("포션은 R키로 사용할 수 있으며 체력이 다 찼거나 포션이 없으면 사용이 불가능합니다.");
        yield return CoroutineHelper.WaitForSeconds(2);
        yield return _noticeText.SetTypeWriterCoroutine("포션을 사용해 보세요.");
        yield return CoroutineHelper.WaitForSeconds(.5f);
        PlayerController player = PlayerManager.Instance.Player;
        player.InputReader.EnablePlayerInput(true);
        CombatData combatData = new CombatData();
        combatData.hitAnimationType = 0;
        combatData.hitPoint = player.transform.position;
        player.ApplyDamage(10, combatData);

        string[] goals = new string[]
        {
            "포션 사용 : 0/1 <sprite=0>"
        };
        SetGoal(goals);
        _nextNoticeEvent = () => StartCoroutine(FourthTutorialText());
        MainUI.Instance.canChangePlayerInputReader = true;
    }

    IEnumerator FourthTutorialText()
    {
        MainUI.Instance.canChangePlayerInputReader = false;

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("잘하셨습니다. 다음으로는 패링과 가드입니다.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        yield return _noticeText.SetTypeWriterCoroutine("패링과 가드는 우클릭으로 가능합니다.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        yield return _noticeText.SetTypeWriterCoroutine("가드와 패링을 한번 씩 해보세요.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        PlayerController player = PlayerManager.Instance.Player;
        player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "가드 : 0/1 <sprite=2>~",
            "패링 : 0/1 <sprite=2>"
        };
        SetGoal(goals);
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);

        _nextNoticeEvent = () => StartCoroutine(FifthTutorialText());
        MainUI.Instance.canChangePlayerInputReader = true;
    }

    IEnumerator FifthTutorialText()
    {
        MainUI.Instance.canChangePlayerInputReader = false;
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", 0);

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("가드와 패링이 불가능한 공격이 존재합니다.");
        yield return CoroutineHelper.WaitForSeconds(1f);
        yield return _noticeText.SetTypeWriterCoroutine("플레이어 머리 위 한자가 뜨면 가드와 패링이 불가능한 공격입니다.");
        yield return CoroutineHelper.WaitForSeconds(2f);
        yield return _noticeText.SetTypeWriterCoroutine("특수 공격 이펙트 또한 가드와 패링이 불가능한 공격입니다.");
        yield return CoroutineHelper.WaitForSeconds(1.5f);
        yield return _noticeText.SetTypeWriterCoroutine("이때는 회피로만 피할 수 있습니다.");
        yield return CoroutineHelper.WaitForSeconds(.7f);
        yield return _noticeText.SetTypeWriterCoroutine("공격을 회피해보세요.");
        yield return CoroutineHelper.WaitForSeconds(.5f);
        PlayerController player = PlayerManager.Instance.Player;
        player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "회피 : 0/1 <sprite=6>"
        };
        SetGoal(goals);
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);
        _nextNoticeEvent = () => StartCoroutine(SixTutorialText());

        SetGoalText();
        MainUI.Instance.canChangePlayerInputReader = true;
    }

    IEnumerator SixTutorialText()
    {
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", 0);
        MainUI.Instance.canChangePlayerInputReader = false;

        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("다음으로는 스킬입니다.");
        yield return CoroutineHelper.WaitForSeconds(.4f);
        yield return _noticeText.SetTypeWriterCoroutine("스킬은 F키로 사용 가능하며 스킬을 사용했을때는 대미지, 모션, 속도 등이 달라지며 이펙트가 추가됩니다.");
        yield return CoroutineHelper.WaitForSeconds(2.5f);
        yield return _noticeText.SetTypeWriterCoroutine("또한, 스킬 시전 중에는 공격을 받지 않는 무적 상태가 됩니다.");
        yield return CoroutineHelper.WaitForSeconds(1f);
        yield return _noticeText.SetTypeWriterCoroutine("스킬을 다 사용하고 난 뒤에는 쿨타임이 존재해 쿨타임동안에는 스킬을 사용할 수 없습니다.");
        yield return CoroutineHelper.WaitForSeconds(2);
        yield return _noticeText.SetTypeWriterCoroutine("스킬을 사용해보세요.");
        yield return CoroutineHelper.WaitForSeconds(.5f);
        PlayerController player = PlayerManager.Instance.Player;
        player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "스킬 사용 : 0/1 <sprite=1>"
        };
        SetGoal(goals);
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);

        _nextNoticeEvent = () => StartCoroutine(SevenTutorialText());

        SetGoalText();
        MainUI.Instance.canChangePlayerInputReader = true;
    }

    IEnumerator SevenTutorialText()
    {
        MainUI.Instance.canChangePlayerInputReader = false;
        _enemy.BehaviorTreeCompo.SetVariableValue("currentStep", currentStep);
        PlayerManager.Instance.Player.canLockOn = false;
        PlayerManager.Instance.Player.LockOn(false);
        _portal.SetActive(true);
        _enemy.HideEnemy();
        PlayerManager.Instance.Player.InputReader.EnablePlayerInput(false);
        yield return _noticeText.SetTypeWriterCoroutine("마지막으로 상호작용입니다.");
        yield return CoroutineHelper.WaitForSeconds(.4f);
        yield return _noticeText.SetTypeWriterCoroutine("상호작용은 E키입니다.");
        yield return CoroutineHelper.WaitForSeconds(.4f);
        yield return _noticeText.SetTypeWriterCoroutine("앞에 생긴 포탈은 상호작용이 가능한 오브젝트입니다.");
        yield return CoroutineHelper.WaitForSeconds(1f);
        yield return _noticeText.SetTypeWriterCoroutine("포탈에 다가가 상호작용키를 누르면 로비로 나가집니다.");
        yield return CoroutineHelper.WaitForSeconds(1f);
        yield return _noticeText.SetTypeWriterCoroutine("포탈에 들어가 로비로 가는 것을 끝으로 튜토리얼을 마치겠습니다.");
        yield return CoroutineHelper.WaitForSeconds(1f);
        yield return _noticeText.SetTypeWriterCoroutine("앞으로의 여정을 응원합니다.");
        yield return CoroutineHelper.WaitForSeconds(.8f);
        PlayerController player = PlayerManager.Instance.Player;
        player.InputReader.EnablePlayerInput(true);
        string[] goals = new string[]
        {
            "포탈 상호작용하기 : 0/1 <sprite=8>"
        };
        SetGoal(goals);

        _nextNoticeEvent = () =>
        {
            PlayerDataManager.playerData.completeTutorial = true;
            PlayerDataManager.Save();
        };

        SetGoalText();
        MainUI.Instance.canChangePlayerInputReader = true;
    }

    public void SetGoalText()
    {
        string str = "";
        for (int i = 0; i < _stepGoalText.Count; i++)
        {
            str += _stepGoalText[i];
            str += "\n";
        }

        _goalText.SetText(str);
    }

    public void ClearGoal()
    {
        _goalList[currentGoal] = 1;
        currentGoal++;
        if (currentGoal >= maxGoal)
        {
            currentGoal = 0;
            currentStep++;
            _nextNoticeEvent?.Invoke();
        }
    }

    public void UpdateGoal(byte idx)
    {
        _stepGoalText[idx] = _stepGoalText[idx].Replace("0/1", "1/1");


        SetGoalText();
    }

    public bool ExistsGoal(byte idx)
    {
        return _stepGoalText[idx].Contains("1/1");
    }
}