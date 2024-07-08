using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeUI : MonoSingleton<FadeUI>
{
    private CanvasGroup _canvasGroup;

    public bool ExistsCanvasGroup => _canvasGroup;

    private void Awake()
    {
        FadeUI[] manager = FindObjectsOfType<FadeUI>();
        if (manager.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        FadeIn(0);
    }


    public static void FadeIn(float duration = .5f, Action callBack = null, bool ignoreTime = false)
    {
        Instance.transform.GetChild(0).gameObject.SetActive(true);

        Instance._canvasGroup.DOFade(1, duration).OnComplete(() => { callBack?.Invoke(); }).SetUpdate(ignoreTime);
    }

    public static void FadeOut(float duration = .5f, Action callBack = null, bool ignoreTime = false)
    {
        Instance._canvasGroup.DOFade(0, duration).OnComplete(() =>
        {
            Instance.transform.GetChild(0).gameObject.SetActive(false);
            callBack?.Invoke();
        }).SetUpdate(ignoreTime);
    }
}