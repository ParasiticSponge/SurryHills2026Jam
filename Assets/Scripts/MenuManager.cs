using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MenuManager : MonoBehaviour
{
    public Image screen;

    // Start is called before the first frame update
    void Awake()
    {
        screen.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Actions.Begin += ChangeScene;
        Actions.Hover += Enlarge;
        Actions.HoverExit += Shrink;

    }
    private void OnDisable()
    {
        Actions.Begin -= ChangeScene;
        Actions.Hover -= Enlarge;
        Actions.HoverExit -= Shrink;
    }

    void ChangeScene()
    {
        StartCoroutine(ExitAnimation());
    }

    IEnumerator ExitAnimation()
    {
        screen.gameObject.SetActive(true);
        float duration = 1f;
        float start = 0f;
        float target = 1f;
        float elapsed = 0f;
        Color baseColour = screen.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EasingFunctions.EaseOutCubic(t);
            float a = Mathf.Lerp(start, target, t);
            screen.color = new Color(baseColour.r, baseColour.g, baseColour.b, a);
            yield return null; // wait one frame
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Enlarge(GameObject obj)
    {
        StartCoroutine(ScaleOverTime(obj, new Vector3(2.5f, 2.5f, 1)));
        StartCoroutine(LightControl(obj, 0.4f));
    }

    public void Shrink(GameObject obj)
    {
        StartCoroutine(ScaleOverTime(obj, new Vector3(2, 2, 1)));
        StartCoroutine(LightControl(obj, 0));
    }

    private IEnumerator LightControl(GameObject obj, float target)
    {
        float elapsed = 0f;
        float duration = 0.25f;
        Light2D light = obj.transform.GetChild(0).GetComponent<Light2D>();
        float start = light.intensity;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EasingFunctions.EaseOutCubic(t);
            light.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }

    private IEnumerator ScaleOverTime(GameObject obj, float amount)
    {
        float elapsed = 0f;
        float duration = 0.25f;

        if (obj.GetComponent<RectTransform>() != null)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            Vector3 startScale = rect.localScale;
            Vector3 targetScale = startScale * amount;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float tLerp = elapsed / duration;
                rect.localScale = Vector3.Lerp(startScale, targetScale, tLerp);
                yield return null;
            }
            rect.localScale = targetScale;
        }
        else
        {
            Transform t = obj.transform;
            Vector3 startScale = t.localScale;
            Vector3 targetScale = startScale * amount;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float tLerp = elapsed / duration;
                t.localScale = Vector3.Lerp(startScale, targetScale, tLerp);
                yield return null;
            }
            t.localScale = targetScale;
        }
    }
    private IEnumerator ScaleOverTime(GameObject obj, Vector3 targetScale)
    {
        float elapsed = 0f;
        float duration = 0.25f;

        if (obj.GetComponent<RectTransform>() != null)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            Vector3 startScale = rect.localScale;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float tLerp = elapsed / duration;
                rect.localScale = Vector3.Lerp(startScale, targetScale, tLerp);
                yield return null;
            }
            rect.localScale = targetScale;
        }
        else
        {
            Transform t = obj.transform;
            Vector3 startScale = t.localScale;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float tLerp = elapsed / duration;
                t.localScale = Vector3.Lerp(startScale, targetScale, tLerp);
                yield return null;
            }
            t.localScale = targetScale;
        }
    }
}
