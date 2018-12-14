using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAreaExample : MonoBehaviour {

    public GameObject point;
    public MeshUtility mesh;

    public Color pointInColor;
    public Color pointOutColor;

    public Color meshInColor;
    public Color meshOutColor;

    public LineRenderer normalEdge;
    public LineRenderer closeEdge;

	void Update () {
        ShowArea();
        ShowEdge();
    }

    void ShowArea()
    {
        List<Vector2> polygon = new List<Vector2>();
        for(int i = 0;i < mesh.points.Length; i++)
        {
            polygon.Insert(polygon.Count, mesh.points[i].position);
        }
        if (MathUtility.IsPointInPolygon(ref polygon, point.transform.position))
        {
            mesh.gameObject.GetComponent<Renderer>().material.SetColor("_Color", meshInColor);
            point.GetComponent<SpriteRenderer>().color = pointInColor;
        }
        else
        {
            mesh.gameObject.GetComponent<Renderer>().material.SetColor("_Color", meshOutColor);
            point.GetComponent<SpriteRenderer>().color = pointOutColor;
        }

    }

    void ShowEdge()
    {
        int closeIndex = 0;
        float minDis = float.MaxValue;
        float currentDis;
        for(int i = 0;i < mesh.points.Length-1; i++)
        {
            currentDis = MathUtility.PointToSegment(
                                        point.transform.position,
                                        mesh.points[i].position,
                                        mesh.points[i + 1].position);
            if(currentDis < minDis)
            {
                closeIndex = i;
                minDis = currentDis;
            }
        }
        currentDis = MathUtility.PointToSegment(
                                        point.transform.position,
                                        mesh.points[mesh.points.Length-1].position,
                                        mesh.points[0].position);
        if (currentDis < minDis)
        {
            closeIndex = mesh.points.Length - 1;
            minDis = currentDis;
        }

        List<Vector3> closePoints = new List<Vector3>();
        closePoints.Insert(closePoints.Count, mesh.points[closeIndex].position);
        closePoints.Insert(closePoints.Count, mesh.points[(closeIndex + 1) % mesh.points.Length].position);
        List<Vector3> normalPoints = new List<Vector3>();
        for(int i = 0;i < mesh.points.Length;i++)
        {
            normalPoints.Insert(normalPoints.Count, mesh.points[(i+ closeIndex+1) % mesh.points.Length].position);
        }
        normalEdge.positionCount = normalPoints.Count;
        normalEdge.SetPositions(normalPoints.ToArray());
        closeEdge.positionCount = closePoints.Count;
        closeEdge.SetPositions(closePoints.ToArray());
    }
}
