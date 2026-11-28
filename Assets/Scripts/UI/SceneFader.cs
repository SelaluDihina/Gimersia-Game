using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    private static SceneFader instance;
    private CanvasGroup canvasGroup;
    private bool isFading;
    private float fadeDuration = 0.4f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (instance != null) return;

        GameObject obj = new GameObject("AutoSceneFader");
        DontDestroyOnLoad(obj);

        Canvas canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        instance = obj.AddComponent<SceneFader>();
        instance.canvasGroup = obj.AddComponent<CanvasGroup>();
        instance.canvasGroup.alpha = 1f;
        instance.canvasGroup.blocksRaycasts = false;

        GameObject imgObj = new GameObject("BlackScreen");
        imgObj.transform.SetParent(obj.transform, false);
        Image img = imgObj.AddComponent<Image>();
        img.color = Color.black;

        RectTransform rect = img.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        SceneManager.sceneLoaded += instance.OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        isFading = false;
        StartCoroutine(Fade(0f));
    }

    public static void LoadScene(string sceneName)
    {
        if (instance == null || instance.isFading || string.IsNullOrEmpty(sceneName)) return;
        instance.StartCoroutine(instance.FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;
        yield return StartCoroutine(Fade(1f));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        canvasGroup.blocksRaycasts = true;
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = targetAlpha > 0.01f;
    }
}