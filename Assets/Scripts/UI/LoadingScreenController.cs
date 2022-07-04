using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    private CanvasGroup loadingScreen;

    private void Awake()
    {
        loadingScreen = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        StartCoroutine(LoadingScreenFade(0, 1f));
    }

    //function that can be used to either fade in or fade out
    public IEnumerator LoadingScreenFade(float targetValue, float duration)
    {
        float startValue = loadingScreen.alpha;
        float time = 0f;

        while (time < duration)
        {
            loadingScreen.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        //this is here to guarantee that the alpha is 1 (or 0) instead of a very close float value
        loadingScreen.alpha = targetValue;
    }
}
