using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointVelocity : MonoBehaviour
{
    [SerializeField]
    private KinectManager kinectManager;
    [SerializeField]
    private KinectInterop.JointType joint;
    [SerializeField, Range(0.05f, 0.5f)]
    private const float updateInterval = 0.1f;

    private float prevTime = 0;
    private Vector3 prevPos = new Vector3();
    private bool firstFrame = true;
    private float nextTime = 0;
    public float Speed { get; private set; }

    private void Update()
    {
        UpdateSpeed();
    }

    public void UpdateSpeed()
    {
        List<long> userID = kinectManager.GetAllUserIds();
        if (userID.Count > 0)
        {
            Vector3 pos = kinectManager.GetJointKinectPosition(userID[0], (int)joint);
            if (firstFrame)
            {
                firstFrame = false;
                prevPos = pos;
                prevTime = Time.time;
                Speed = -10000f;
            }
            else
            {
                if (Time.time >= nextTime)
                {
                    float distance = Vector3.Distance(pos, prevPos);
                    float deltaTime = Time.time - prevTime;
                    Speed = distance / deltaTime;
                    //Debug.Log("Dis: " + distance);
                    //Debug.Log("time: " + deltaTime);
                    //Debug.Log("speed: " + speed);
                    nextTime = Time.time + updateInterval;
                    prevPos = pos;
                    prevTime = Time.time;
                }
            }

        }
        else
        {
            Speed = -10000f;
        }
    }

    public static Color GetSpeedColor(float speed)
    {
        
        return Color.HSVToRGB(GetSpeedColorHue(speed), 1, 1);
        //    if (speed < 0.2f)
        //    {
        //        return Color.cyan;
        //    }
        //    else if (speed < 0.6f)
        //    {
        //        return Color.green;
        //    }
        //    else if (speed < 1.2f)
        //    {
        //        return Color.yellow;
        //    }
        //    else
        //    {
        //        return Color.red;
        //    }
    }

    public static float GetSpeedColorHue(float speed)
    {
        speed = Mathf.Clamp(speed, 0, 1.5f);
        float hue = 0.5f - Mathf.Floor(speed / 1.5f * 0.5f * 10.0f) * 0.1f;
        //Debug.Log(hue);
        return hue;
    }

}
