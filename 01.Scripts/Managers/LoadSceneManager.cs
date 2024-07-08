using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    [SerializeField] private Transform _progressBar;
    public static string nextScene;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadSceneWithLoading(string sceneName)
    {
        nextScene = sceneName;
        SoundManager.StopAll();

        SceneManager.LoadScene("Loading");
    }

    public static void LoadScene(string sceneName)
    {
        SoundManager.StopAll();

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        yield return null;
        op.allowSceneActivation = false;
        float timer = 0.0f;
        FadeUI.FadeOut(.1f);
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            Vector3 scale = _progressBar.transform.localScale;
            scale.x = timer / 2f;

            _progressBar.transform.localScale = scale;

            if (scale.x >= 1.0f)
            {
                FadeUI.FadeIn(.2f, () => { op.allowSceneActivation = true; });
                yield break;
            }
        }
    }
}