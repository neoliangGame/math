using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentsExample : MonoBehaviour {

    public GameObject pointA1;
    public GameObject pointA2;
    public LineRenderer lineA;

    public GameObject pointB1;
    public GameObject pointB2;
    public LineRenderer lineB;

    public GameObject pointCross;

    public Gradient lineNormalGradient;
    public Gradient lineCrossGradient;

    bool isCross = false;

    private void LateUpdate()
    {
        Vector2 cross = Vector2.zero;
        isCross = MathUtility.SegmentCrossPoint(
            new Vector2(pointA1.transform.position.x, pointA1.transform.position.y),
            new Vector2(pointA2.transform.position.x, pointA2.transform.position.y),
            new Vector2(pointB1.transform.position.x, pointB1.transform.position.y),
            new Vector2(pointB2.transform.position.x, pointB2.transform.position.y),
            ref cross);
        UpdateShow(cross);
    }

    void UpdateShow(Vector2 vectorCross)
    {
        lineA.SetPosition(0, pointA1.transform.position);
        lineA.SetPosition(1, pointA2.transform.position);

        lineB.SetPosition(0, pointB1.transform.position);
        lineB.SetPosition(1, pointB2.transform.position);

        if (isCross)
        {
            lineA.colorGradient = lineCrossGradient;
            lineB.colorGradient = lineCrossGradient;
            pointCross.SetActive(true);
            pointCross.transform.position = new Vector3(vectorCross.x, vectorCross.y, pointCross.transform.position.z);
        }
        else
        {
            lineA.colorGradient = lineNormalGradient;
            lineB.colorGradient = lineNormalGradient;
            pointCross.SetActive(false);
        }
    }
}
