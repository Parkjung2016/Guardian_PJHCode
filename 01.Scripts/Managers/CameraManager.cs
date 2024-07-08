using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public enum CameraTypeEnum
{
    FreeLook,
    LockOn,
}

public class CameraManager
{
    public static CameraManager Instance;
    private Dictionary<CameraTypeEnum, CinemachineVirtualCameraBase> _cameraDictionary;
    public CinemachineVirtualCameraBase currentCam;
    private float _originFreeLookXSpeed, _originFreeLookYSpeed;
    private CinemachineImpulseSource _shakeImpulse;
    private Sequence _changneFOVCameraSequence;

    public CameraManager()
    {
        _cameraDictionary = new();
        _cameraDictionary.Add(CameraTypeEnum.FreeLook,
            GameObject.Find("FreeLook Camera").GetComponent<CinemachineVirtualCameraBase>());
        _cameraDictionary.Add(CameraTypeEnum.LockOn,
            GameObject.Find("LockOn Camera").GetComponent<CinemachineVirtualCameraBase>());
        CinemachineFreeLook freeLook = (_cameraDictionary[CameraTypeEnum.FreeLook] as CinemachineFreeLook);
        _originFreeLookXSpeed = freeLook.m_XAxis.m_MaxSpeed;
        _originFreeLookYSpeed = freeLook.m_YAxis.m_MaxSpeed;
        _shakeImpulse = Camera.main.GetComponent<CinemachineImpulseSource>();
    }

    public static void ChangeCamera(CameraTypeEnum type)
    {
        foreach (var camera in Instance._cameraDictionary)
        {
            camera.Value.Priority = camera.Key == type ? 15 : 10;
        }

        Instance.currentCam = Instance._cameraDictionary[type];
    }

    public static void EnableFreeLookAxis(bool enable)
    {
        CinemachineFreeLook freeLook = (Instance._cameraDictionary[CameraTypeEnum.FreeLook] as CinemachineFreeLook);
        if (enable)
        {
            freeLook.m_XAxis.Value = Camera.main.transform.eulerAngles.y;
        }

        freeLook.m_XAxis.m_MaxSpeed = enable ? Instance._originFreeLookXSpeed : 0;
        freeLook.m_YAxis.m_MaxSpeed = enable ? Instance._originFreeLookYSpeed : 0;
    }

    public static void ChangeFOVCamera(float fovValue, float duration = 1f)
    {
        if (Instance._changneFOVCameraSequence != null && Instance._changneFOVCameraSequence.IsActive())
            Instance._changneFOVCameraSequence.Kill();
        Instance._changneFOVCameraSequence = DOTween.Sequence();
        foreach (var cam in Instance._cameraDictionary)
        {
            if (cam.Value is CinemachineVirtualCamera vCam)
            {
                Instance._changneFOVCameraSequence.Join(DOTween.To(() => vCam.m_Lens.FieldOfView,
                    x => vCam.m_Lens.FieldOfView = x, fovValue, duration));
            }
            else if (cam.Value is CinemachineFreeLook freeLook)
                Instance._changneFOVCameraSequence.Join(DOTween.To(() => freeLook.m_Lens.FieldOfView,
                    x => freeLook.m_Lens.FieldOfView = x, fovValue, duration));
        }
    }

    public static void ShakeCamera(float shakePower)
    {
        Instance._shakeImpulse.GenerateImpulse(shakePower);
    }

    public static CinemachineVirtualCameraBase GetCamera(CameraTypeEnum type) => Instance._cameraDictionary[type];
}