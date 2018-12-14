using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基本数学计算方法库
/// </summary>
public static class MathUtility {

    /// <summary>
    /// 判断两点是否在边的同一侧
    /// </summary>
    /// <param name="LA1">向量端点1</param>
    /// <param name="LA2">向量端点2</param>
    /// <param name="P1">点1</param>
    /// <param name="P2">点2</param>
    /// <returns>两点在同一边正确与否</returns>
    public static bool IsPointsSameSide(Vector2 LA1, Vector2 LA2, Vector2 P1, Vector2 P2)
    {
        Vector2 lineP1A = P1 - LA1;
        Vector2 lineP2A = P2 - LA1;
        Vector2 lineA = LA2 - LA1;
        float crossA1 = lineP1A.x * lineA.y - lineP1A.y * lineA.x;
        float crossA2 = lineP2A.x * lineA.y - lineP2A.y * lineA.x;

        return crossA1 * crossA2 >= 0;
    }

    /// <summary>
    /// 叉乘判断两线段A和B是否相交
    /// </summary>
    /// <param name="A1"></param>
    /// <param name="A2"></param>
    /// <param name="B1"></param>
    /// <param name="B2"></param>
    /// <returns></returns>
    public static bool IsSegmentCross(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
    {
        Vector2 vectorA1B = A1 - B1;
        Vector2 vectorA2B = A2 - B1;
        Vector2 vectorB = B2 - B1;
        float crossA1 = vectorA1B.x * vectorB.y - vectorA1B.y * vectorB.x;
        float crossA2 = vectorA2B.x * vectorB.y - vectorA2B.y * vectorB.x;

        Vector2 vectorB1A = B1 - A1;
        Vector2 vectorB2A = B2 - A1;
        Vector2 vectorA = A2 - A1;
        float crossB1 = vectorB1A.x * vectorA.y - vectorB1A.y * vectorA.x;
        float crossB2 = vectorB2A.x * vectorA.y - vectorB2A.y * vectorA.x;

        return (crossA1 * crossA2 < 0) && (crossB1 * crossB2 < 0);
    }

    /// <summary>
    /// 判断两线段A和B是否相交，并且返回相交点
    /// 根据两点式联立计算得出：
    ///  x-x1      y-y1
    /// -----  =  -----
    /// x2-x1     y2-y1
    /// </summary>
    /// <param name="A1">线段A起点</param>
    /// <param name="A2">线段A终点</param>
    /// <param name="B1">线段B起点</param>
    /// <param name="B2">线段B终点</param>
    /// <param name="cross">交点</param>
    /// <returns>返回是否相交</returns>
    public static bool SegmentCrossPoint(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, ref Vector2 cross)
    {
        float paral = (A2.x - A1.x) * (B2.y - B1.y) - (B2.x - B1.x) * (A2.y - A1.y);
        if (Mathf.Approximately(paral, 0f))
        {
            //平行
            float same1 = (A1.x - B1.x) * (B2.y - B1.y);
            float same2 = (A1.y - B1.y) * (B2.x - B1.x);
            if (Mathf.Approximately(same1, same2))
            {
                //共线
                float maxX = (B1.x > B2.x) ? B1.x : B2.x;
                float minX = (B1.x < B2.x) ? B1.x : B2.x;
                float maxY = (B1.y > B2.y) ? B1.y : B2.y;
                float minY = (B1.y < B2.y) ? B1.y : B2.y;
                if ((minX <= A1.x && A1.x <= maxX) && (minY <= A1.y && A1.y <= maxY))
                {
                    cross = A1;
                    return true;
                }else if ((minX <= A2.x && A2.x <= maxX) && (minY <= A2.y && A2.y <= maxY))
                {
                    cross = A2;
                    return true;
                }
            }
            return false;
        }
        if (IsSegmentCross(A1, A2, B1, B2) == false)
        {
            return false;
        }

        float y = (B1.x - A1.x) * (B2.y - B1.y) * (A2.y - A1.y)
                    + A1.y * (A2.x - A1.x) * (B2.y - B1.y)
                    - B1.y * (B2.x - B1.x) * (A2.y - A1.y);
        y /= paral;

        float x = (B1.y - A1.y) * (A2.x - A1.x) * (B2.x - B1.x)
                    + A1.x * (A2.y - A1.y) * (B2.x - B1.x)
                    - B1.x * (B2.y - B1.y) * (A2.x - A1.x);
        x /= -paral;
        cross = new Vector2(x,y);
        return true;
    }

    public static bool LineSegmentCrossPoint(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, ref Vector2 cross)
    {
        float maxX = (B1.x > B2.x) ? B1.x : B2.x;
        float minX = (B1.x < B2.x) ? B1.x : B2.x;

        float paral = (A2.x - A1.x) * (B2.y - B1.y) - (B2.x - B1.x) * (A2.y - A1.y);
        if (Mathf.Approximately(paral, 0f))
        {
            //平行
            float same1 = (A1.x - B1.x) * (B2.y - B1.y);
            float same2 = (A1.y - B1.y) * (B2.x - B1.x);
            if (Mathf.Approximately(same1, same2))
            {
                //共线
                cross = B1;
                return true;
            }
            return false;
        }

        float y = (B1.x - A1.x) * (B2.y - B1.y) * (A2.y - A1.y)
                    + A1.y * (A2.x - A1.x) * (B2.y - B1.y)
                    - B1.y * (B2.x - B1.x) * (A2.y - A1.y);
        y /= paral;

        float x = (B1.y - A1.y) * (A2.x - A1.x) * (B2.x - B1.x)
                    + A1.x * (A2.y - A1.y) * (B2.x - B1.x)
                    - B1.x * (B2.y - B1.y) * (A2.x - A1.x);
        x /= -paral;
        if(minX <= x && x <= maxX)
        {
            cross = new Vector2(x, y);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断线段是否与射线相交，射线沿X轴正方向
    /// </summary>
    /// <param name="rayPoint">射线起始点</param>
    /// <param name="S1">线段一端</param>
    /// <param name="S2">线段另一端</param>
    /// <returns>射线与线段相交与否</returns>
    public static bool IsRaySegmentCross(Vector2 rayPoint, Vector2 S1, Vector2 S2)
    {
        /*
         (x - S1.x) * (S2.y - S1.y) = (y - S1.y) * (S2.x - S1.x)
         */
        float segmentY = S2.y - S1.y;
        if (Mathf.Approximately(segmentY, 0f))
        {
            //平行
            if(Mathf.Approximately(segmentY, rayPoint.y))
            {
                //共线
                if(rayPoint.x < S1.x || rayPoint.x < S2.x)
                {
                    return true;
                }
            }
            return false;
        }
        float x = (rayPoint.y - S1.y) * (S2.x - S1.x) / segmentY + S1.x;
        float segMinX = (S1.x < S2.x) ? S1.x : S2.x;
        float segMaxX = (S1.x > S2.x) ? S1.x : S2.x;
        if (rayPoint.x <= x && (segMinX <= x && x <= segMaxX))
            return true;
        return false;
    }

    /// <summary>
    /// 射线法判断点是否在多边形内
    /// </summary>
    /// <param name="polygonPoints">按顺序排好的多边形点</param>
    /// <param name="point">需要判断的点</param>
    /// <returns>点在多边形内与否</returns>
    public static bool IsPointInPolygon(ref List<Vector2> polygonPoints, Vector2 point)
    {
        int crossCount = 0;
        for(int i = 1;i < polygonPoints.Count; i++)
        {
            if(IsRaySegmentCross(point, polygonPoints[i - 1], polygonPoints[i]))
            {
                crossCount++;
            }
        }
        if (IsRaySegmentCross(point, polygonPoints[0], polygonPoints[polygonPoints.Count-1]))
        {
            crossCount++;
        }
        return crossCount % 2 != 0;
    }

    /// <summary>
    /// 计算点到线段的最短距离，具体需要考虑：
    /// 1.点在线段所在直线上的投影点刚好在线段之间，则返回点到直线距离；
    /// 2.点都在线段所在直线上的投影点在线段范围外，则返回其与两端点距离较近的；
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="S1">线段端点1</param>
    /// <param name="S2">线段端点2</param>
    /// <returns>点离线段的最短距离</returns>
    public static float PointToSegment(Vector2 point, Vector2 S1, Vector2 S2)
    {
        Vector2 PS1 = point - S1;
        Vector2 S2S1 = S2 - S1;
        float distancePS1 = Vector2.Distance(point, S1);
        float distancePS2 = Vector2.Distance(point, S2);
        float distanceS2S1 = Vector2.Distance(S2, S1);
        if (Mathf.Approximately(distancePS1, 0f))
            return 0f;
        if (Mathf.Approximately(distanceS2S1, 0f))
            return distancePS1;

        float dot = Vector2.Dot(PS1, S2S1);
        float refDis = dot / distanceS2S1;
        Vector2 refPoint = S1 + S2S1 * refDis;
        float minX = (S1.x < S2.x) ? S1.x : S2.x;
        float maxX = (S1.x > S2.x) ? S1.x : S2.x;
        float minDistance = Vector2.Distance(refPoint, point);
        if (refPoint.x < minX || refPoint.x > maxX)
        {
            minDistance = (distancePS1 < distancePS2) ? distancePS1 : distancePS2;
        }
        return minDistance;
    }

    /// <summary>
    /// 警告：未完成。未完成。未完成。别用
    /// 判断三维空间下的直线是否穿过三角形面片
    /// </summary>
    /// <param name="pp1"></param>
    /// <param name="pp2"></param>
    /// <param name="pp3"></param>
    /// <param name="lp"></param>
    /// <param name="ldir"></param>
    /// <returns></returns>
    public static bool IsLineCrossTriangle(Vector3 pp1, Vector3 pp2, Vector3 pp3, Vector3 lp, Vector3 ldir, ref Vector3 cross)
    {
        //1.计算出三角形组成的平面的坐标系
        Vector3 xAxis = pp2 - pp1;
        Vector3 yAxis = Vector3.Cross(pp3 - pp1, xAxis);
        Vector3 zAxis = Vector3.Cross(xAxis, yAxis);
        Matrix4x4 planeToWorld = new Matrix4x4();
        planeToWorld.SetColumn(0, new Vector4(xAxis.x, xAxis.y, xAxis.z, 0f));
        planeToWorld.SetColumn(1, new Vector4(yAxis.x, yAxis.y, yAxis.z, 0f));
        planeToWorld.SetColumn(2, new Vector4(zAxis.x, zAxis.y, zAxis.z, 0f));
        planeToWorld.SetColumn(3, new Vector4(pp1.x, pp1.y, pp1.z, 1f));
        Matrix4x4 worldToPlane = planeToWorld.inverse;

        //2.把直线中两个点转换到平面所在三维坐标系中
        Vector3 trilp = worldToPlane.MultiplyPoint(lp);
        Vector3 trildir = worldToPlane.MultiplyVector(ldir);
        /*
         ( x-x1)   ( y-y1)   ( z-z1)
         ------- = ------- = --------
         (x2-x1)   (y2-y1)   (z2-z1)
         */
        Vector3 tripp2 = worldToPlane.MultiplyPoint(pp2);
        Vector3 tripp3 = worldToPlane.MultiplyPoint(pp3);
        if(Mathf.Approximately(trildir.y, 0f))
        {
            //线与面在同一个平面
            Vector2 crossP = Vector2.zero;
            if(LineSegmentCrossPoint(
                new Vector2(trilp.x,trilp.z),
                new Vector2(trilp.x + trildir.x,trilp.z + trildir.z),
                new Vector2(tripp2.x,tripp2.z),
                Vector2.zero,
                ref crossP)){
                cross = planeToWorld.MultiplyPoint(new Vector3(crossP.x, 0, crossP.y));
                return true;
            }else if (LineSegmentCrossPoint(
                new Vector2(trilp.x, trilp.z),
                new Vector2(trilp.x + trildir.x, trilp.z + trildir.z),
                Vector2.zero,
                new Vector2(tripp3.x, tripp3.z),
                ref crossP))
            {
                cross = planeToWorld.MultiplyPoint(new Vector3(crossP.x, 0, crossP.y));
                return true;
            }else if (LineSegmentCrossPoint(
                new Vector2(trilp.x, trilp.z),
                new Vector2(trilp.x + trildir.x, trilp.z + trildir.z),
                new Vector2(tripp2.x, tripp2.z),
                new Vector2(tripp3.x, tripp3.z),
                ref crossP))
            {
                cross = planeToWorld.MultiplyPoint(new Vector3(crossP.x, 0, crossP.y));
                return true;
            }
            return false;
        }

        float x = trilp.x - trilp.y * trildir.x / trildir.y;
        float z = trilp.z - trilp.y * trildir.z / trildir.y;

        List<Vector2> polygon = new List<Vector2>();
        polygon.Insert(polygon.Count, Vector2.zero);
        polygon.Insert(polygon.Count, new Vector2(tripp2.x, tripp2.z));
        polygon.Insert(polygon.Count, new Vector2(tripp3.x, tripp3.z));
        if(IsPointInPolygon(ref polygon, new Vector2(x, z)))
        {
            cross = planeToWorld.MultiplyPoint(new Vector3(x, 0f, z));
            return true;
        }
        return false;
    }
}
