using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    private Coroutine _freezeCoroutine;

    public static void SetFreezeTime(float freezeValue, float freezeTime)
    {
        if (Instance._freezeCoroutine != null) Instance.StopCoroutine(Instance._freezeCoroutine);
        Instance._freezeCoroutine = Instance.StartCoroutine(Instance.Freeze(freezeValue, freezeTime));
    }

    private IEnumerator Freeze(float freezeValue, float freezeTime)
    {
        Time.timeScale = freezeValue;
        yield return new WaitForSecondsRealtime(freezeTime);
        if (!MainUI.Instance.GamePaused) 
        Time.timeScale = 1;
    }
}