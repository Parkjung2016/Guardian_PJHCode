using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BossRoom : InteractAble
{
    private readonly int IsOpenedHash = Shader.PropertyToID("_IsOpened");
    private Light[] _lights;
    [SerializeField] private MeshRenderer _roomMeshRenderer;

    [ValueDropdown("GetAllScenes", IsUniqueList = true, DropdownTitle = "Select Scene",
         DrawDropdownForListElements = false, ExcludeExistingValuesInList = true), SerializeField]
    private string _enterBossRoomSceneName;

    private bool _isOpened;
    private BoxCollider _boxCollider;

#if UNITY_EDITOR
    private static IEnumerable GetAllScenes()
    {
        return EditorBuildSettings.scenes.Select(x =>
        {
            string str = x.path;
            string[] split = str.Split('/');
            str = split.Last();
            int strIdx = str.LastIndexOf('.');
            str = str.Substring(0, strIdx);
            return str;
        }).Where(x => x.Contains("Boss"));
    }
#endif
    private void Awake()
    {
        _lights = GetComponentsInChildren<Light>();
        _boxCollider = GetComponent<BoxCollider>();
        OpenBossRoom(false);
    }

    public void OpenBossRoom(bool isOpened)
    {
        Material[] mats = _roomMeshRenderer.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetInt(IsOpenedHash, Convert.ToByte(isOpened));
        }

        for (int i = 0; i < _lights.Length; i++)
        {
            _lights[i].enabled = isOpened;
        }

        _roomMeshRenderer.materials = mats;
        _isOpened = isOpened;
        _boxCollider.enabled = isOpened;
    }


    public override void Interact()
    {
        canInteract = false;
        PlayerController player = PlayerManager.Instance.Player;
        player.stopRotate = true;
        player.useRootMotion = true;
        SoundManager.SetSubMasterVolumeWithDuration(0, .4f);
        player.InputReader.EnableUIInput(false);
        player.InputReader.EnablePlayerInput(false);
        FadeUI.FadeIn(callBack: () => { LoadSceneManager.LoadSceneWithLoading(_enterBossRoomSceneName); });
    }
}