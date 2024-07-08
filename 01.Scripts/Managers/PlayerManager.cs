using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    public PlayerController Player { get; private set; }
    public PlayerWeapon[] weapons;
    public event Action SuccessTeleportPlayerEvent;
    public bool enteredBossGate;

    private void Awake()
    {
        PlayerManager[] manager = FindObjectsOfType<PlayerManager>();
        if (manager.Length > 1)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;

            Destroy(gameObject);
        }
        else
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Player = FindObjectOfType<PlayerController>();
        if (Player)
            StartCoroutine(TeleportPlayerToStartPos());
    }

    public IEnumerator TeleportPlayerToStartPos()
    {
        yield return CoroutineHelper.WaitForSeconds(.2f);
        if (FindObjectOfType<TitleManager>()) yield break;
        Player.InputReader.EnableUIInput(false);
        Player.InputReader.EnablePlayerInput(false);
        Transform trm = GameObject.Find("PlayerStartPos")?.transform;
        if (!trm) yield break;
        Player.CharacterControllerCompo.enabled = false;
        yield return null;
        Player.transform.position = trm.position;
        Player.Model.rotation = trm.rotation;
        yield return null;
        Player.CharacterControllerCompo.enabled = true;
        yield return CoroutineHelper.WaitForSeconds(1.1f);
        FadeUI.FadeOut(1);
        yield return CoroutineHelper.WaitForSeconds(.3f);
        MapInfo.Instance.ShowMapInfo();
        SoundManager.SetSubMasterVolumeWithDuration(1, 0);
        Player.InputReader.EnableUIInput(true);
        Player.InputReader.EnablePlayerInput(true);
        SuccessTeleportPlayerEvent?.Invoke();
    }

    private void OnApplicationQuit()
    {
        PlayerDataManager.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (MainUI.Instance)
            MainUI.Instance.PauseToggle(true);
    }
}