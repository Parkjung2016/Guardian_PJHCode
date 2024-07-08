using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BossRoom : MonoBehaviour, IInteractAble
{
    private readonly int IsOpenedHash = Shader.PropertyToID("_IsOpened");

    [field: SerializeField] public string InteractName { get; set; }
    public bool CanInteract { get; set; } = true;
    public Transform Trm { get; set; }

    [SerializeField] private MeshRenderer _roomMeshRenderer;
    [SerializeField] private TextMeshPro _timerTMP;

    [ValueDropdown("GetAllScenes", IsUniqueList = true, DropdownTitle = "Select Scene",
         DrawDropdownForListElements = false, ExcludeExistingValuesInList = true), SerializeField]
    private string _enterBossRoomSceneName;

    private Light[] _lights;
    private BoxCollider _boxCollider;
    private bool _isOpened;

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
        Trm = transform;
        _lights = GetComponentsInChildren<Light>();
        _boxCollider = GetComponent<BoxCollider>();
        OpenBossRoom(false);
        SetTimer(new Timer());
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

    public void SetTimer(Timer timer)
    {
        _timerTMP.gameObject.SetActive(!timer.IsNull());
        if (!timer.IsNull())
        {
            _timerTMP.SetText(timer.ToString());
        }
    }

    public void Interact()
    {
        CanInteract = false;
        PlayerController player = PlayerManager.Instance.Player;
        player.stopRotate = true;
        player.stopMove = true;
        SoundManager.SetSubMasterVolumeWithDuration(0, .4f);
        player.InputReader.EnableUIInput(false);
        player.InputReader.EnablePlayerInput(false);
        FadeUI.FadeIn(callBack: () => { LoadSceneManager.LoadSceneWithLoading(_enterBossRoomSceneName); });
    }
}