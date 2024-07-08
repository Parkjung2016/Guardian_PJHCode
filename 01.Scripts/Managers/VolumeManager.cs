using System;
using DG.Tweening;
using OccaSoftware.RadialBlur.Runtime;
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
        private Sequence _lensDistortionSequence;
        private Sequence _chromaticAberrationSequence;
        private Sequence _radialBlurSequence;
        private RadialBlurPostProcess _radialBlur;

        public VolumeManager()
        {
            Volume volume = GameObject.FindObjectOfType<Volume>();
            volume.profile.TryGet(out _beautify);
            volume.profile.TryGet(out _lensDistortion);
            volume.profile.TryGet(out _radialBlur);
        }

        public static void SetChromaticAberration(float value, Action callBack = null)
        {
            if (Instance._chromaticAberrationSequence != null && Instance._chromaticAberrationSequence.IsActive())
                Instance._chromaticAberrationSequence.Kill();
            float duration = .12f;
            Instance._chromaticAberrationSequence = DOTween.Sequence();

            Instance._chromaticAberrationSequence.Append(DOTween
                .To(() => Instance._beautify.chromaticAberrationIntensity.value,
                    x => Instance._beautify.chromaticAberrationIntensity.value = x, value, duration).OnComplete(() =>
                {
                    callBack?.Invoke();
                }));
        }

        public static void SetLensDistortion(float intensity, Action callBack = null)
        {
            if (Instance._lensDistortionSequence != null && Instance._lensDistortionSequence.IsActive())
                Instance._lensDistortionSequence.Kill();
            float duration = .12f;
            Instance._lensDistortionSequence = DOTween.Sequence();
            Instance._lensDistortionSequence.Append(DOTween.To(() => Instance._lensDistortion.intensity.value,
                x => Instance._lensDistortion.intensity.value = x, intensity, duration).OnComplete(() =>
            {
                callBack?.Invoke();
            }));
        }

        public static void SetBlur(float intensity)
        {
            Instance._beautify.blurIntensity.value = intensity;
        }

        public static void SetRadialBur(float intensity, Action callBack = null)
        {
            if (Instance._radialBlurSequence != null && Instance._radialBlurSequence.IsActive())
                Instance._radialBlurSequence.Kill();
            float duration = .12f;
            Instance._radialBlurSequence = DOTween.Sequence();

            Instance._radialBlurSequence.Append(DOTween.To(() => Instance._radialBlur.intensity.value,
                    x => Instance._radialBlur.SetIntensity(x), intensity, duration)
                .OnComplete(() => { callBack?.Invoke(); }));
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