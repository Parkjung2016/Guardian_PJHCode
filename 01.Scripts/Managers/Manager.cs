using System;
using PJH;
using UnityEngine;

public abstract class Manager : MonoSingleton<Manager>
{
    [SerializeField] protected InputReader _inputReader;

    protected virtual void Awake()
    {
        CreateVolumeManager();
        CreateCameraManager();
        CreateTimeManager();
    }

    private void OnEnable()
    {
        _inputReader.ESCEvent += HandleESCEvent;
    }

    private void OnDisable()
    {
        _inputReader.ESCEvent -= HandleESCEvent;
    }

    private void CreateVolumeManager()
    {
        VolumeManager.Instance = new VolumeManager();
    }

    private void CreateCameraManager()
    {
        CameraManager.Instance = new CameraManager();
        CameraManager.ChangeCamera(CameraTypeEnum.FreeLook);
    }

    private void CreateTimeManager()
    {
        TimeManager.Instance = new GameObject("TimeManager", typeof(TimeManager)).GetComponent<TimeManager>();
        Transform timeManagerTrm = TimeManager.Instance.transform;
        timeManagerTrm.SetParent(transform);
        timeManagerTrm.localPosition = Vector3.zero;
    }

    private void HandleESCEvent()
    {
        MainUI.Instance.PauseToggle();
    }
}