using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VolumeManager = PJH.VolumeManager;

public class MainUI : MonoSingleton<MainUI>
{
    [SerializeField] private Slider _masterSoundSlider, _bgmSoundSlider, _sfxSoundSlider;
    private GameObject _interactionGroup;
    private Transform _playerHealthProgressBar, _easePlayerHealthProgressBar;
    private Image _bossHealthProgressBar, _easeBossHealthProgressBar;
    private Transform _energyProgressBar, _easeEnergyProgressBar;
    private CanvasGroup _mapInfo;
    private TextMeshProUGUI _mapName;
    private TextMeshProUGUI _bossName;
    private TextMeshProUGUI _interactionTMP;
    private TextMeshProUGUI _interactionInputTMP;
    private TextMeshProUGUI _potionCountTMP;
    private Image _skillCoolProgress;
    private Image _skillIcon;

    private CanvasGroup _deathUICanvasGroup;
    public bool canChangePlayerInputReader = true;

    #region sounds

    [SerializeField] private EventReference _mapInfoVisibleSound;
    [SerializeField] private EventReference _buttonClickSound, _pauseSound;

    #endregion

    private bool _gamePaused;
    public bool GamePaused => _gamePaused;
    private GameObject _pauseGroup;
    private GameObject _optionGroup;

    private void Awake()
    {
        _interactionGroup = transform.Find("Interaction").gameObject;
        Transform bg = _interactionGroup.transform.Find("BG");
        _interactionTMP = bg.Find("Text").GetComponent<TextMeshProUGUI>();
        _interactionInputTMP = bg.Find("Input").GetComponent<TextMeshProUGUI>();
        _deathUICanvasGroup = transform.Find("Death").GetComponent<CanvasGroup>();
        _mapInfo = transform.Find("MapInfo").GetComponent<CanvasGroup>();
        _mapName = _mapInfo.transform.Find("MapName").GetComponent<TextMeshProUGUI>();
        Transform healthBar = transform.Find("HealthBar");
        Transform energyBar = transform.Find("EnergyBar");
        _playerHealthProgressBar = healthBar.Find("ProgressPivot");
        _easePlayerHealthProgressBar = healthBar.Find("EaseProgressPivot");
        Transform bossHealthBar = transform.Find("BossHP");
        _bossHealthProgressBar = bossHealthBar.Find("Progress").GetComponent<Image>();
        _easeBossHealthProgressBar = bossHealthBar.Find("EaseProgress").GetComponent<Image>();
        _bossName = bossHealthBar.Find("BossName").GetComponent<TextMeshProUGUI>();

        _energyProgressBar = energyBar.Find("ProgressPivot");
        _easeEnergyProgressBar = energyBar.Find("EaseProgressPivot");
        _mapInfo.alpha = 0;
        _deathUICanvasGroup.gameObject.SetActive(false);

        _deathUICanvasGroup.alpha = 0;
        _bossHealthProgressBar.transform.parent.gameObject.SetActive(false);
        _pauseGroup = transform.Find("Pause").gameObject;
        _optionGroup = _pauseGroup.transform.Find("Option").gameObject;

        Transform skillIcon = transform.Find("SkillIcon");
        _skillIcon = skillIcon.Find("Icon").GetComponent<Image>();
        _skillCoolProgress = skillIcon.Find("Progress").GetComponent<Image>();
        _potionCountTMP = transform.Find("Potion/Count").GetComponent<TextMeshProUGUI>();
    }


    private IEnumerator Start()
    {
        Enemy boss = FindObjectOfType<Enemy>();
        if (boss)
            boss.HealthCompo.OnHealthChangedEvent += HandleBossHealthChanged;
        _pauseGroup.SetActive(false);
        _optionGroup.SetActive(false);
        yield return new WaitUntil(() => PlayerManager.Instance.Player);
        PlayerController player = PlayerManager.Instance.Player;
        player.HealthCompo.OnHealthChangedEvent += HandlePlayerHealthChanged;
        player.OnEnergyChangedEvent += HandleEnergyChanged;

        PlayerDataManager.PlayerData playerData = PlayerDataManager.playerData;
        HideInteractionUI();
        _masterSoundSlider.value = playerData.masterVolume;
        _bgmSoundSlider.value = playerData.bgmVolume;
        _sfxSoundSlider.value = playerData.sfxVolume;
        SoundManager.SetSFXVolume(playerData.sfxVolume);
        SoundManager.SetMusicVolume(playerData.bgmVolume);
        SoundManager.SetMasterVolume(playerData.masterVolume);
    }


    private void HandleEnergyChanged(float value)
    {
        float duration = .5f;
        _energyProgressBar.DOScaleX(value, duration);
        _easeEnergyProgressBar.DOScaleX(value, duration * 2f).SetEase(Ease.Flash);
    }

    private void HandlePlayerHealthChanged(float value)
    {
        float duration = .5f;
        _playerHealthProgressBar.DOScaleX(value, duration);
        _easePlayerHealthProgressBar.DOScaleX(value, duration * 2f).SetEase(Ease.Flash);
    }

    private void HandleBossHealthChanged(float value)
    {
        float duration = .5f;
        _bossHealthProgressBar.DOFillAmount(value, duration);
        _easeBossHealthProgressBar.DOFillAmount(value, duration * 2f).SetEase(Ease.Flash);
    }

    public void EnableMapInfo(string mapName)
    {
        SoundManager.PlaySFX(_mapInfoVisibleSound);
        _mapInfo.alpha = 1;
        _mapName.SetText(mapName);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.Append(_mapInfo.DOFade(0, 1));
    }

    public void EnableDeathUI()
    {
        PauseToggle(false);
        Sequence seq = DOTween.Sequence();
        _deathUICanvasGroup.gameObject.SetActive(true);
        seq.Append(_deathUICanvasGroup.DOFade(1, 3));
        seq.AppendInterval(1);
        seq.AppendCallback(() =>
        {
            FadeUI.FadeIn(callBack: async () =>
            {
                _deathUICanvasGroup.DOFade(0, 0);
                _deathUICanvasGroup.gameObject.SetActive(false);
                await Task.Delay(3000);
                PlayerManager.Instance.Player.InputReader.EnablePlayerInput(true);
                PlayerManager.Instance.Player.InputReader.EnableUIInput(true);
                SoundManager.PlayerReSpawn();
                SoundManager.StopBGM();
                LoadSceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        });
    }


    public void SetBossName(string name)
    {
        _bossName.SetText(name);
        _bossHealthProgressBar.transform.parent.gameObject.SetActive(true);
    }

    public void HideBossHealthBar()
    {
        _bossHealthProgressBar.transform.parent.gameObject.SetActive(false);
    }

    public void ShowInteractionUI(string text, string input)
    {
        _interactionGroup.SetActive(true);
        _interactionInputTMP.SetText(input);
        _interactionTMP.SetText(text);
    }

    public void HideInteractionUI()
    {
        _interactionGroup.SetActive(false);
    }

    public void PauseToggle()
    {
        if (_optionGroup.activeSelf)
        {
            SoundManager.PlaySFX(_buttonClickSound);

            _optionGroup.SetActive(false);
        }
        else
            PauseToggle(!_gamePaused);
    }

    public void PauseToggle(bool enable)
    {
        if (!PlayerManager.Instance.Player || FadeUI.Instance.transform.GetChild(0).gameObject.activeSelf ||
            !gameObject.activeSelf) return;
        _gamePaused = enable;
        int pause = Convert.ToByte(!_gamePaused);
        Time.timeScale = pause;
        VolumeManager.SetBlur(~pause * 1.7f);

        if (canChangePlayerInputReader)
            PlayerManager.Instance.Player.InputReader.EnablePlayerInput(!_gamePaused);
        CursorUtility.EnableCursor(_gamePaused);
        _pauseGroup.SetActive(_gamePaused);

        if (_gamePaused)
        {
            SoundManager.PauseGame();
            if (!_pauseGroup.activeSelf)
            {
                SoundManager.PlaySFX(_pauseSound);
            }
        }
        else
        {
            SoundManager.ResumeGame();
        }
    }

    public void ClickResumeBtn()
    {
        SoundManager.PlaySFX(_buttonClickSound);

        PauseToggle();
    }

    public void ClickOptionBtn()
    {
        SoundManager.PlaySFX(_buttonClickSound);
        _optionGroup.SetActive(true);
    }

    public void ClickTitleBtn()
    {
        SoundManager.PlaySFX(_buttonClickSound);
        FadeUI.FadeIn(callBack: () =>
        {
            Time.timeScale = 1;
            SoundManager.ResumeGame();
            PlayerManager.Instance.enteredBossGate = false;
            PlayerDataManager.Save();
            LoadSceneManager.LoadScene("Title");
        }, ignoreTime: true);
    }

    public void ClickExitOptionBtn()
    {
        SoundManager.PlaySFX(_buttonClickSound);

        _optionGroup.SetActive(false);
    }

    public void StartSkillCool(float cooldown)
    {
        _skillCoolProgress.DOFillAmount(0, cooldown + .2f);
    }

    public void LockSkill()
    {
        _skillCoolProgress.fillAmount = 1;
    }

    public void EndSkillCool()
    {
        _skillCoolProgress.DOKill();
        _skillCoolProgress.fillAmount = 0;
    }

    public void ChangeMasterSound(Single value)
    {
        SoundManager.SetMasterVolume(value);
        PlayerDataManager.playerData.masterVolume = value;
    }

    public void ChangeBGMSound(Single value)
    {
        SoundManager.SetMusicVolume(value);
        PlayerDataManager.playerData.bgmVolume = value;
    }

    public void ChangeSFXSound(Single value)
    {
        SoundManager.SetSFXVolume(value);
        PlayerDataManager.playerData.sfxVolume = value;
    }

    public void ChangePotionCount(int value)
    {
        _potionCountTMP.SetText(value.ToString());
    }

    public void ChangeSkillIcon(Sprite icon)
    {
        _skillIcon.sprite = icon;
    }

    public void ClickQuitBtn()
    {
        SoundManager.PlaySFX(_buttonClickSound);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}