using MelonLoader;
using System.Collections;
using UnityEngine;

namespace BloomEngine.Utilities;

public static class SceneHelper
{
    private static readonly Dictionary<RectTransform, object> fadeCoroutines = new();

    public static T FindComponent<T>(this Transform obj, string path) where T : MonoBehaviour => obj?.Find(path)?.GetComponentInChildren<T>(true);
    public static T FindComponent<T>(this GameObject obj, string path) where T : MonoBehaviour => obj?.transform?.Find(path)?.GetComponentInChildren<T>(true);

    public static T[] FindComponents<T>(this Transform obj, string path) where T : MonoBehaviour => obj?.Find(path)?.GetComponentsInChildren<T>(true);
    public static T[] FindComponents<T>(this GameObject obj, string path) where T : MonoBehaviour => obj?.transform?.Find(path)?.GetComponentsInChildren<T>(true);

    public static void FadeUIAlpha(RectTransform uiRect, float target, float duration)
    {
        if(!uiRect) return;

        if(fadeCoroutines.TryGetValue(uiRect, out object fadeCoToken))
            MelonCoroutines.Stop(fadeCoToken);

        fadeCoroutines[uiRect] = MelonCoroutines.Start(CoFadeAlphaTo(uiRect, target, duration));
    }

    private static IEnumerator CoFadeAlphaTo(RectTransform uiRect, float target, float duration)
    {
        CanvasGroup group = uiRect.GetComponent<CanvasGroup>() ?? uiRect.gameObject.AddComponent<CanvasGroup>();

        float start = group.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        group.alpha = target;
        fadeCoroutines.Remove(uiRect, out _);
    }
}