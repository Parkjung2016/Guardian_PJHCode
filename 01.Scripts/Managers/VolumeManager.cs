using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PJH
{
    public class VolumeManager
    {
        public static VolumeManager Instance;
        private Beautify.Universal.Beautify _beautify;
        private LensDistortion _lensDistortion;
        private Sequence _bloodFrameSequence;

        public VolumeManager()
        {
            Volume volume = GameObject.FindObjectOfType<Volume>();
            volume.profile.TryGet(out _beautify);
            volume.profile.TryGet(out _lensDistortion);
        }

        public static void SetChromaticAberration(float value, Action callBack = null)
        {
            float duration = .12f;
            DOTween.To(() => Instance._beautify.chromaticAberrationIntensity.value,
                x => Instance._beautify.chromaticAberrationIntensity.value = x, value, duration).OnComplete(() =>
            {
                callBack?.Invoke();
            });
        }

        public static void SetLensDistortion(float intensity, Action callBack = null)
        {
            float duration = .12f;
            DOTween.To(() => Instance._lensDistortion.intensity.value,
                x => Instance._lensDistortion.intensity.value = x, intensity, duration).OnComplete(() =>
            {
                callBack?.Invoke();
            });
        }

        public static void SetBlur(float intensity)
        {
            Instance._beautify.blurIntensity.value = intensity;
        }

        public static void EnableBloodFrame()
        {
            if (Instance._bloodFrameSequence != null && Instance._bloodFrameSequence.IsActive())
                Instance._bloodFrameSequence.Kill();
            float duration = .5f;
            Instance._bloodFrameSequence = DOTween.Sequence();
            Color color = Instance._beautify.frameColor.value;
            color.a = .45f;
            Instance._bloodFrameSequence.Append(DOTween.To(() => Instance._beautify.frameColor.value,
                x => Instance._beautify.frameColor.value = x, color,
                duration));
            Instance._bloodFrameSequence.AppendInterval(.3f);
            color.a = 0;
            Instance._bloodFrameSequence.Append(DOTween.To(() => Instance._beautify.frameColor.value,
                x => Instance._beautify.frameColor.value = x, color,
                duration));
        }
    }
}