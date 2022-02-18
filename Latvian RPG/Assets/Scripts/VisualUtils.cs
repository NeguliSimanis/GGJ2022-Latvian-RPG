using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class VisualUtils
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fadeSpeed">rate at which image fades in/out (0<fadeSpeed<1f)</param>
    /// <param name="imageToFade"></param>
    /// <param name="fadeIn"></param>
    /// <returns></returns>
    public static IEnumerator FadeImage(float fadeSpeed, Image imageToFade, Color startColor, Color endColor)
    {
        // R COMPONENT
        if (startColor.r != endColor.r)
        {
            if (startColor.r < endColor.r)
            {
                for (float r = startColor.r; r <= endColor.r; r += fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.r = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                for (float r = startColor.r; r >= endColor.r; r -= fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.r = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        // G COMPONENT
        if (startColor.g != endColor.g)
        {
            if (startColor.g < endColor.g)
            {
                for (float r = startColor.g; r <= endColor.g; r += fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.g = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                for (float r = startColor.g; r >= endColor.g; r -= fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.g = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        // B COMPONENT
        if (startColor.b != endColor.b)
        {
            if (startColor.b < endColor.b)
            {
                for (float r = startColor.b; r <= endColor.b; r += fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.b = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                for (float r = startColor.b; r >= endColor.b; r -= fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.b = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        // A COMPONENT
        if (startColor.a != endColor.a)
        {
            if (startColor.a < endColor.a)
            {
                for (float r = startColor.a; r <= endColor.a; r += fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.a = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                for (float r = startColor.a; r >= endColor.a; r -= fadeSpeed)
                {
                    Color c = imageToFade.color;
                    c.a = r;
                    imageToFade.color = c;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
        yield return null;
    }

}
