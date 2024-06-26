using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
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
    private GameObject _checkNewGame;
    private bool _loadDataResult;

    private void Awake()
    {
        _checkNewGame = _canvas.Find("CheckNewGame").gameObject;
        Transform panel = _canvas.Find("Panel");
        _panelOtherGroup = panel.Find("Other");
        _menuGroup = new()
        {
            { MenuType.Main, panel.Find("Main").gameObject },
            { MenuType.Crew, _panelOtherGroup.Find("Crew").gameObject },
            { MenuType.Option, _panelOtherGroup.Find("Option").gameObject }
        };
        ChangeMenu(MenuType.Main);
        _checkNewGame.SetActive(false);
    }

    private IEnumerator Start()
    {
        CursorUtility.EnableCursor(true);
        yield return new WaitForSeconds(1);

        SoundManager.SetSubMasterVolume(1);
        FindObjectOfType<PlayableDirector>().Play();
        _continueBtn.SetActive(PlayerDataManager.ExistsData());
        _loadDataResult = PlayerDataManager.Load();

        PlayerDataManager.PlayerData playerData = PlayerDataManager.playerData;
        _sfxSlider.value = playerData.sfxVolume;
        _musicSlider.value = playerData.bgmVolume;
        _masterSlider.value = playerData.masterVolume;
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
            _checkNewGame.SetActive(true);
        }
        else
        {
            ClickNewGameBtn();
        }
    }

    public void ClickCancelNewGameBtn()
    {
        SoundManager.PlaySFX(_btnClickSound);
        _checkNewGame.SetActive(false);
    }

    public void ClickNewGameBtn()
    {
        SoundManager.PlaySFX(_btnClickSound);
        PlayerDataManager.playerData = new()
        {
            bossStep = 1,
            currentWeapon = 0
        };
        PlayerDataManager.Save();
        FadeUI.FadeIn(callBack: () => { LoadSceneManager.LoadScene("Tutorial"); });
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