using System.Collections.Generic;
using DG.Tweening;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class SoundManager
{
    private static SoundManager _instance;
    private const string masterVCA = "Master";
    private const string subMasterVCA = "SubMaster";
    private const string musicVCA = "Music";
    private const string sfxVCA = "SFX";

    private VCA _masterVCAController;
    private VCA _subMasterVCAController;
    private VCA _musicVCAController;
    private VCA _sfxVCAController;
    private EventInstance _bgm;
    private Bus _masterBus;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SoundManager();
            return _instance;
        }
    }

    public SoundManager()
    {
        string vca = "vca:/";
        _masterVCAController = RuntimeManager.GetVCA(vca + masterVCA);
        _subMasterVCAController = RuntimeManager.GetVCA(vca + subMasterVCA);
        _musicVCAController = RuntimeManager.GetVCA(vca + musicVCA);
        _sfxVCAController = RuntimeManager.GetVCA(vca + sfxVCA);
        _masterBus = RuntimeManager.GetBus("Bus:/InGame");
    }


    public static void PlayBGM(EventReference bgm)
    {
        if (Instance._bgm.isValid()) StopBGM();
        Instance._bgm = RuntimeManager.CreateInstance(bgm);
        Instance._bgm.start();
        Instance._bgm.release();
    }

    public static void EndBGM()
    {
        Instance._bgm.setParameterByName("End", 1);
    }

    public static void StopBGM()
    {
        if (!Instance._bgm.isValid()) return;
        Instance._bgm.stop(STOP_MODE.IMMEDIATE);
    }

    public static void StopAll() => Instance._masterBus.stopAllEvents(STOP_MODE.IMMEDIATE);

    public static void PauseBGM()
    {
        if (!Instance._bgm.isValid()) return;
        Instance._bgm.setPaused(true);
    }

    public static void ResumeBGM()
    {
        if (!Instance._bgm.isValid()) return;
        Instance._bgm.setPaused(false);
    }

    public static EventInstance PlaySFX(EventReference sfx)
    {
        if (sfx.IsNull) return new EventInstance();
        EventInstance instance = RuntimeManager.CreateInstance(sfx);
        Instance.PlaySFX(instance);
        return instance;
    }

    public static EventInstance PlaySFX(EventReference sfx, Vector3 position)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sfx);
        instance.set3DAttributes(position.To3DAttributes());
        Instance.PlaySFX(instance);
        return instance;
    }

    private void PlaySFX(EventInstance instance)
    {
        instance.start();
        instance.release();
    }

    public static void PlayerDeath()
    {
        // EventInstance instance = RuntimeManager.CreateInstance("event://WindBGM");
        // Instance.PlaySFX(instance);
        SetSubMasterVolumeWithDuration(0, .5f);
    }

    public static void PlayerReSpawn()
    {
        SetSubMasterVolume(1);
    }


    public static void PauseGame()
    {
        _instance._masterBus.setPaused(true);
    }

    public static void ResumeGame()
    {
        _instance._masterBus.setPaused(false);
    }


    #region get set

    public static void SetMasterVolume(float volume)
    {
        Instance._masterVCAController.setVolume(volume);
    }

    public static void SetSubMasterVolume(float volume)
    {
        Instance._subMasterVCAController.setVolume(volume);
    }


    public static void SetSubMasterVolumeWithDuration(float volume, float duration)
    {
        Instance._subMasterVCAController.getVolume(out float currentVolume);
        DOTween.To(() => currentVolume, x => currentVolume = x,
            volume, duration).OnUpdate(() => { _instance._subMasterVCAController.setVolume(currentVolume); });
    }

    public static void SetMusicVolume(float volume)
    {
        Instance._musicVCAController.setVolume(volume);
    }

    public static void SetSFXVolume(float volume)
    {
        Instance._sfxVCAController.setVolume(volume);
    }

    public static float GetMasterVolume()
    {
        Instance._musicVCAController.getVolume(out var volume);
        return volume;
    }

    public static float GetMusicVolume()
    {
        Instance._musicVCAController.getVolume(out var volume);
        return volume;
    }

    public static float GetSFXVolume()
    {
        Instance._musicVCAController.getVolume(out var volume);
        return volume;
    }

    #endregion
}