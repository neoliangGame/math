using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentAreaExample : MonoBehaviour {

    public GameObject point1;
    public GameObject point2;
    public LineRenderer segmentLR;
    public GameObject[] crossPoints;
    public MeshUtility mesh;

    public Gradient segmentInGradient;
    public Gradient segmentOutGradient;

    public Color meshInColor;
    public Color meshOutColor;

    void Update()
    {
        bool isCrossEdge = IsCrossEdge();
        SetEdge(segmentLR, point1.transform.position, point2.transform.position, isCrossEdge);

        SetMesh(IsInArea() || isCrossEdge);
    }

    bool IsCrossEdge()
    {
        bool isCross = false;
        Vector2 cp = Vector2.zero;
        for(int i = 0;i < mesh.points.Length-1; i++)
        {
            if(MathUtility.SegmentCrossPoint(
                point1.transform.position,
                point2.transform.position,
                mesh.points[i].position,
                mesh.points[i+1].position,
                ref cp))
            {
                SetEdge(mesh.points[i].gameObject.GetComponent<LineRenderer>(),
                    mesh.points[i].position, mesh.points[i + 1].position, true);
                crossPoints[i].transform.position = new Vector3(cp.x, cp.y, mesh.points[i].position.z);
                crossPoints[i].SetActive(true);
                isCross = true;
            }
            else
            {
                SetEdge(mesh.points[i].gameObject.GetComponent<LineRenderer>(),
                    mesh.points[i].position, mesh.points[i + 1].position, false);
                crossPoints[i].SetActive(false);
            }
        }
        if (MathUtility.SegmentCrossPoint(
                point1.transform.position,
                point2.transform.position,
                mesh.points[mesh.points.Length-1].position,
                mesh.points[0].position,
                ref cp))
        {
            SetEdge(mesh.points[mesh.points.Length - 1].gameObject.GetComponent<LineRenderer>(),
                    mesh.points[mesh.points.Length - 1].position,
                    mesh.points[0].position, true);
            isCross = true;
            crossPoints[mesh.points.Length - 1].transform.position = new Vector3(cp.x, cp.y, mesh.points[mesh.points.Length - 1].position.z);
            crossPoints[mesh.points.Length - 1].SetActive(true);
        }
        else
        {
            SetEdge(mesh.points[mesh.points.Length - 1].gameObject.GetComponent<LineRenderer>(),
                    mesh.points[mesh.points.Length - 1].position,
                    mesh.points[0].position, false);
            crossPoints[mesh.points.Length - 1].SetActive(false);
        }
        return isCross;
    }

    void SetEdge(LineRenderer lr, Vector3 p1, Vector3 p2, bool isCross)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, p1);
        lr.SetPosition(1, p2);
        if (isCross)
        {
            lr.colorGradient = segmentInGradient;
        }
        else
        {
            lr.colorGradient = segmentOutGradient;
        }
    }

    bool IsInArea()
    {
        List<Vector2> polygon = new List<Vector2>();
        for(int i = 0;i < mesh.points.Length; i++)
        {
            polygon.Insert(polygon.Count, mesh.points[i].position);
        }
        return MathUtility.IsPointInPolygon(ref polygon, point1.transform.position);
    }

    void SetMesh(bool isInArea)
    {
        if (isInArea)
        {
            mesh.gameObject.GetComponent<Renderer>().material.SetColor("_Color", meshInColor);
        }
        else
        {
            mesh.gameObject.GetComponent<Renderer>().material.SetColor("_Color", meshOutColor);
        }

    }

    
}
