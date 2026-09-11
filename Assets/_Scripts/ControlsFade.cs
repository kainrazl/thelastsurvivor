using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsFade : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color opacity;
    private float alpha;
    private bool fade = true;
    private void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        opacity = sr.color;
        alpha = opacity.a;
    }

    private void Update()
    {
        if(fade && opacity.a > 0)
        {
            StartCoroutine(FadeImage());
            sr.color = new Color(1, 1, 1, alpha);
        }
    }

    private IEnumerator FadeImage()
    {
        fade = false;
        WaitForSeconds waiting = new WaitForSeconds(1);
        alpha -= 0.1f;
        yield return waiting;
        fade = true;
    }
}
