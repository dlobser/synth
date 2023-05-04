using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinewave : MonoBehaviour
{
    public BreathListener myBreath;
    public LineRenderer myLine;
    public int points;
    public float amplitude = 1f;
    public float speed = 0f;
    public float frequency = 1f;
    private float y;
    private float pausedY;
    private float holdY;

    void Start()
    {
        myLine = GetComponent<LineRenderer>();
    }

    void Draw()
    {
        myLine.positionCount = points;
        for (int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(0, 6, progress);

            float y = Mathf.Sin(x + Time.timeSinceLevelLoad);
            myLine.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }

    void Update()
    {
        Draw();
    }
}
