using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    private bool changeColor = false;
    [SerializeField]
    private Transform ObjectToTrack;
    [SerializeField]
    private Transform LineParent;
    [SerializeField]
    JointVelocity jointVelocity;
    [SerializeField, Range(0, 1.0f)]
    private float minDistanceInterval = 0.2f;
    [SerializeField, Range(0, 1.0f)]
    private float lineDefaultWidth = 0.01f;
    [SerializeField]
    private Color defaultColor = Color.cyan;
    [SerializeField]
    int maxColor = 3;
    [SerializeField]
    private Material defaultLineMaterial;

    private Vector3 prevPointDistance = Vector3.zero;
    private int positionCount = 0;
    private List<LineRenderer> lines = new List<LineRenderer>();
    private LineRenderer currentLine;
    private bool isDrawing = false;
    private float lineLength = 0f;

    private Color prevColor = Color.white;
    private bool firstColor = true;
    private float prevTime = 0f;
    private float prevLength = 0f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isDrawing)
            {
                isDrawing = true;
            }
            DeleteLine();
            AddNewLine();
        }
        else if (isDrawing)
        {
            UpdateLine();
        }
    }
    void AddNewLine()
    {
        positionCount = 0;
        GameObject lineObj = new GameObject($"Line_{lines.Count}");
        lineObj.transform.position = ObjectToTrack.position;
        lineObj.transform.parent = LineParent;

        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineDefaultWidth;
        lineRenderer.endWidth = lineDefaultWidth;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = defaultLineMaterial;
        lineRenderer.startColor = defaultColor;
        lineRenderer.endColor = defaultColor;
        lineRenderer.positionCount = 1;
        lineRenderer.numCapVertices = 5;

        currentLine = lineRenderer;

        lines.Add(lineRenderer);
    }

    void DeleteLine()
    {
        foreach (LineRenderer line in lines)
        {
            if (!line) return;
            Destroy(line.gameObject);
        }
        lines.Clear();
        //Debug.Log("DeleteLine");

    }

    

    void UpdateLine()
    {
        Vector3 currentPosition = currentLine.transform.InverseTransformPoint( ObjectToTrack.position);

        if (prevPointDistance == null)
        {
            prevPointDistance = currentPosition;
        }

        float distance = Mathf.Abs(Vector3.Distance(prevPointDistance, currentPosition));

        if (prevPointDistance != null && distance >= minDistanceInterval)
        {
            AddPoint(currentPosition);
            if (changeColor)
            {
                ChangeLineColor(lineLength, distance);
            }
            prevPointDistance = currentPosition;
            lineLength += distance;
        }
    }

    void AddPoint(Vector3 position)
    {
        currentLine.SetPosition(positionCount, position);
        positionCount++;
        currentLine.positionCount = positionCount + 1;
        currentLine.SetPosition(positionCount, position);
    }

    void ChangeLineColor(float lineLength, float addDistance)
    {
        float speed = jointVelocity.Speed;
        Debug.Log(speed);
        if(speed >= 0)
        {

            float newLength = lineLength + addDistance;
            Color color = JointVelocity.GetSpeedColor(speed);
            if (prevColor == color) return;

            //Debug.Log($"{Time.time} Color change from {prevColor } to {color}");
            

            if (firstColor)
            {
                firstColor = false;
                prevTime = Time.time;
                prevColor = color;
                prevLength = newLength;

                return;
            }

            prevColor = color;
            prevLength = newLength;
            float shrink = prevTime / Time.time;

            Gradient originalGradient = currentLine.colorGradient;
            Gradient newGradient = new Gradient();

            if(originalGradient.colorKeys.Length >= maxColor)
            {
                AddNewLine();
                currentLine.startColor = color;
                currentLine.endColor = color;
                return;
            }

            GradientColorKey[] originalColorKeys = originalGradient.colorKeys;
            GradientAlphaKey[] originalAlphaKeys = originalGradient.alphaKeys;

            GradientColorKey[] newColorKeys = new GradientColorKey[originalColorKeys.Length + 1];
            GradientAlphaKey[] newAlphaKeys = new GradientAlphaKey[originalAlphaKeys.Length + 1];

            for(int i=0; i<originalColorKeys.Length; i++)
            {
                float t = originalColorKeys[i].time * shrink;

                newColorKeys[i].time = t;
                newColorKeys[i].color = originalColorKeys[i].color;
                newAlphaKeys[i].time = t;
                newAlphaKeys[i].alpha = 1;
                //Debug.Log($"key {i}- color {newColorKeys[i].color} - alpha {newAlphaKeys[i].alpha}");
                //Debug.Log($"key {i}- original time: {originalColorKeys[i].time} -> {newColorKeys[i].time}");

            }
            newColorKeys[newColorKeys.Length - 1] = new GradientColorKey(color, 1);
            newGradient.colorKeys = newColorKeys;
            newGradient.alphaKeys = newAlphaKeys;
            currentLine.colorGradient = newGradient;
        }   
    }
}
