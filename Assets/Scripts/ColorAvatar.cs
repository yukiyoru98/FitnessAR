using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAvatar : MonoBehaviour
{
    [SerializeField]
    private JointVelocity jointVelocity;
    [SerializeField]
    private Renderer userRenderer;

    private Color prevColor = Color.cyan;
    private bool isChangingColor = false;
    [SerializeField, Range(0.1f, 1f)]
    private float lerpDuration = 0.1f;
    private float prevHue = 0.5f;

    void Update()
    {
        float speed = jointVelocity.Speed;
        if(speed >= 0f)
        {
            ChangeRendererColor(speed);
        }
    }


    //void ChangeRendererColor(float speed)
    //{
    //    if (userRenderer)
    //    {
    //        Color nextColor = JointVelocity.GetSpeedColor(speed);
    //        if(prevColor != nextColor)
    //        {
    //            if (isChangingColor)
    //            {
    //                prevColor = userRenderer.material.color;
    //            }
    //            StartCoroutine(LerpColor(nextColor));
    //        }
    //    }
    //}
    void ChangeRendererColor(float speed)
    {
        if (userRenderer)
        {
            float nextColor = JointVelocity.GetSpeedColorHue(speed);
            if (prevHue != nextColor)
            {
                if (isChangingColor)
                {
                    Color.RGBToHSV(userRenderer.material.color, out float h, out float s, out float v);
                    prevHue = h;
                }
                StartCoroutine(LerpColor(nextColor));
            }
        }
    }
    IEnumerator LerpColor(float nextColor)
    {
        isChangingColor = true;
        float timeElapsed = 0f;
        while (timeElapsed < lerpDuration)
        {
            float h = Mathf.Lerp(prevHue, nextColor, timeElapsed / lerpDuration);
            userRenderer.material.color = Color.HSVToRGB(h, 1, 1);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isChangingColor = false;
        userRenderer.material.color = Color.HSVToRGB(nextColor, 1, 1);
        Debug.Log(nextColor);
    }

    IEnumerator LerpColor(Color nextColor)
    {
        float timeElapsed = 0f;
        while(timeElapsed < lerpDuration)
        {
            userRenderer.material.color = Color.Lerp(prevColor, nextColor, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        userRenderer.material.color = nextColor;
    }
}
