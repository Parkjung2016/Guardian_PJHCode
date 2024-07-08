using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using Slider = UnityEngine.UI.Slider;

public class TitleManager : MonoBehaviour
{
    public enum MenuType
    {
        Main,
        Crew,
        Option,
    }

    private Dictionary<MenuType, GameObject> _menuGroup;
    private Transform _panelOtherGroup;
    [SerializeField] private EventReference _btnClickSound;
    [SerializeField] private GameObject _continueBtn;
    [SerializeField] private Transform _canvas;
    [SerializeField] private Slider _sfxSlider, _musicSlider, _masterSlider;

    private CheckPanel _checkPanel;
    private bool _loadDataResult;

    private void Awake()
    {
        Transform panel = _canvas.Find("Panel");
        _panelOtherGroup = panel.Find("Other");
        _menuGroup = new()
        {
            { MenuType.Main, panel.Find("Main").gameObject },
            { MenuType.Crew, _panelOtherGroup.Find("Crew").gameObject },
            { MenuType.Option, _panelOtherGroup.Find("Option").gameObject }
        };
        ChangeMenu(MenuType.Main);
        _checkPanel = transform.Find("CheckPanel").GetComponent<CheckPanel>();
    }

    private IEnumerator Start()
    {
        _continueBtn.SetActive(PlayerDataManager.ExistsData());
        _loadDataResult = PlayerDataManager.Load();
        CursorUtility.EnableCursor(true);
        yield return new WaitForSeconds(1);

        SoundManager.SetSubMasterVolume(1);
        FindObjectOfType<PlayableDirector>().Play();


        PlayerDataManager.PlayerData playerData = PlayerDataManager.playerData;
        _sfxSlider.SetValueWithoutNotify(playerData.sfxVolume);
        _musicSlider.SetValueWithoutNotify(playerData.bgmVolume);
        _masterSlider.SetValueWithoutNotify(playerData.masterVolume);
        SoundManager.SetSFXVolume(playerData.sfxVolume);
        SoundManager.SetMusicVolume(playerData.bgmVolume);
        SoundManager.SetMasterVolume(playerData.masterVolume);
        FadeUI.FadeOut(0);
    }

    private void ChangeMenu(MenuType type)
    {
        _panelOtherGroup.gameObject.SetActive(type != MenuType.Main);
        foreach (var menu in _menuGroup)
        {
            menu.Value.SetActive(menu.Key == type);
        }
    }

    public void ClickContinueBtn()
    {
        SoundManager.PlaySFX(_btnClickSound);
        if (!_loadDataResult)
        {
            PlayerDataManager.playerData = new()
            {
                bossStep = 1,
                currentWeapon = 0
            };
            PlayerDataManager.Save();
        }

        FadeUI.FadeIn(callBack: () =>
        {
            if (!PlayerDataManager.playerData.completeTutorial)
            {
                PlayerDataManager.playerData.currentWeapon = 0;
                LoadSceneManager.LoadScene("Tutorial");
            }
            else
                LoadSceneManager.LoadSceneWithLoading("Lobby");
        });
    }

    public void ClickCheckNewGameBtn()
    {
        if (PlayerDataManager.ExistsData())
        {
            SoundManager.PlaySFX(_btnClickSound);
            PanelInfo panelInfo = new PanelInfo
            {
                description = "< 데이터 파일이 존재합니다 >",
                btnOneValue = "시작",
                btnTwoValue = "취소",
                BtnOneCallBack = NewGame,
                BtnTwoCallBack = () =>
                {
                    SoundManager.PlaySFX(_btnClickSound);
                    _checkPanel.HideCheckPanel();
                }
            };
            _checkPanel.ShowCheckPanel(panelInfo);
        }
        else
        {
            NewGame();
        }
    }


    public void NewGame()
    {
        SoundManager.PlaySFX(_btnClickSound);
        PlayerDataManager.playerData = new()
        {
            bossStep = 1,
            currentWeapon = 0
        };
        PlayerDataManager.Save();
        PanelInfo panelInfo = new PanelInfo
        {
            description = "< 튜토리얼을 진행하시겠습니까? >",
            btnOneValue = "진행",
            btnTwoValue = "넘기기",
            BtnOneCallBack = () =>
            {
                SoundManager.PlaySFX(_btnClickSound);
                FadeUI.FadeIn(callBack: () => { LoadSceneManager.LoadScene("Tutorial"); });
            },
            BtnTwoCallBack = () =>
            {
                SoundManager.PlaySFX(_btnClickSound);
                PlayerDataManager.playerData.completeTutorial = true;
                PlayerDataManager.Save();
                FadeUI.FadeIn(callBack: () => { LoadSceneManager.LoadSceneWithLoading("Lobby"); });
            }
        };
        _checkPanel.ShowCheckPanel(panelInfo);
    }


    public void ClickCrewBtn()
    {
        ChangeMenu(MenuType.Crew);
        SoundManager.PlaySFX(_btnClickSound);
    }

    public void ClickOptionBtn()
    {
        ChangeMenu(MenuType.Option);
        SoundManager.PlaySFX(_btnClickSound);
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

    public void ClickMenuBtn()
    {
        ChangeMenu(MenuType.Main);
        SoundManager.PlaySFX(_btnClickSound);
    }

    public void ClickQuitBtn()
    {
        SoundManager.PlaySFX(_btnClickSound);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}