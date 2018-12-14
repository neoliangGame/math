using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多边形三角形划分填充
/// 利用二分递归算法
/// 理论上：最好时间效率O((nlogn) * k)，k为常数
/// 理论上：最坏时间效率O((n^2) * k)，k为常数
/// </summary>
public class MeshUtility : MonoBehaviour {

    public Transform[] points;

	void Start () {
        if(GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        if (GetComponent<Renderer>() == null)
            gameObject.AddComponent<Renderer>();

    }
	
	void Update () {
        RefreshMesh();
    }

    void RefreshMesh()
    {
        List<int> triangles = new List<int>();
        List<Vector2> verties2 = new List<Vector2>();
        List<Vector3> verties3 = new List<Vector3>();
        List<int> section = new List<int>();
        for (int i = 0;i < points.Length; i++)
        {
            verties2.Insert(verties2.Count, points[i].position);
            verties3.Insert(verties3.Count, points[i].position);
            section.Insert(section.Count, i);
        }
        FillMesh(ref verties2, ref triangles, ref section);
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.SetVertices(verties3);
        mesh.triangles = triangles.ToArray();
    }

    #region 切面填充
    /// <summary>
    /// 三角形分割填充多边形点列组成的多边形
    /// 在两个理论的基础上实现：
    /// 理论1：任何多边形，都存在三角形化方案；
    /// 理论2：如果理论1成立，则任何多边形肯定存在不相邻两点，把多边形划分成两个子多边形，
    ///        并且两个子多边形依然是可以三角形化的。
    /// 通过以上两个理论依据，本文进行二分递归划分三角形
    /// </summary>
    /// <param name="verties">多边形顶点，尾部不需要添加首部</param>
    /// <param name="triangles">输出的三角形划分数据，每三个为一组</param>
    /// <param name="section">当前多边形点序列</param>
    void FillMesh(ref List<Vector2> verties, ref List<int> triangles, ref List<int> section)
    {
        if (section.Count < 3)
            return;
        if (section.Count == 3)
        {
            triangles.Insert(triangles.Count, section[0]);
            triangles.Insert(triangles.Count, section[1]);
            triangles.Insert(triangles.Count, section[2]);
            return;
        }
        else if (section.Count == 4)
        {
            //两个对角线，判断哪个对角线满足：通过此线把另外两点划分到线的两边
            Vector2 vector10 = verties[section[1]] - verties[section[0]];
            Vector2 vector20 = verties[section[2]] - verties[section[0]];
            Vector2 vector30 = verties[section[3]] - verties[section[0]];
            float cross1 = vector10.x * vector20.y - vector10.y * vector20.x;
            float cross2 = vector30.x * vector20.y - vector30.y * vector20.x;
            int indexOffset = 0;
            if (cross1 * cross2 >= 0)
                indexOffset = 1;

            triangles.Insert(triangles.Count, section[indexOffset]);
            triangles.Insert(triangles.Count, section[(indexOffset + 1) % 4]);
            triangles.Insert(triangles.Count, section[(indexOffset + 2) % 4]);

            triangles.Insert(triangles.Count, section[(indexOffset + 2) % 4]);
            triangles.Insert(triangles.Count, section[(indexOffset + 3) % 4]);
            triangles.Insert(triangles.Count, section[indexOffset]);
            return;
        }

        //多边形边数>=5需要二分分割，递归处理
        List<int> subSection1 = new List<int>();
        List<int> subSection2 = new List<int>();
        int center = SplitSection(ref verties, ref section);
        if(center == -1)
        {
            int secntion0 = section[0];
            section.RemoveAt(0);
            section.Insert(section.Count, secntion0);
            center = SplitSection(ref verties, ref section);

            if (center == -1)
            {
                string errorStr = "section.Count:" + section.Count + "\n";
                for (int i = 0; i < section.Count; i++)
                {
                    errorStr += section[i] + " ";
                }
                Debug.LogError("center not should be -1\n" + errorStr);
                return;
            }
        }
        
        for (int i = 0; i <= center; i++)
        {
            subSection1.Insert(subSection1.Count, section[i]);
        }
        for (int i = center; i < section.Count; i++)
        {
            subSection2.Insert(subSection2.Count, section[i]);
        }
        subSection2.Insert(subSection2.Count, section[0]);
        FillMesh(ref verties, ref triangles, ref subSection1);
        FillMesh(ref verties, ref triangles, ref subSection2);
    }

    /// <summary>
    /// 拆分多边形，一分为二；
    /// 以坐标0为起点，分割点从数组中间往两边开始查找（为了时间更优优先）；
    /// </summary>
    /// <param name="verties">多边形原始点位置数组</param>
    /// <param name="section">当前点数组对应原始数组的映射</param>
    /// <returns>分割点下标</returns>
    int SplitSection(ref List<Vector2> verties, ref List<int> section)
    {
        int center = (int)((float)section.Count * 0.5);
        if (IsConnectOK(center, ref section, ref verties) == false)
        {
            int i;
            int splitIndex = -1;
            for (int offset = 1; offset < center+1; offset++)
            {
                i = center + offset;
                if (i < section.Count - 1)
                {
                    if (IsConnectOK(i, ref section, ref verties))
                    {
                        splitIndex = i;
                        break;
                    }
                }
                i = center - offset;
                if (i > 1)
                {
                    if (IsConnectOK(i, ref section, ref verties))
                    {
                        splitIndex = i;
                        break;
                    }
                }
            }
            center = splitIndex;
        }
        return center;
    }

    /// <summary>
    /// 判断此分割点与坐标0形成的边是否能合理分割此多边形
    /// 如若起点和分割点连接的边同时满足：
    /// 1.与任何其他边都不相交；
    /// 2.边在多边形里面。
    /// 则以此分割点分割此多边形
    /// </summary>
    /// <param name="connectIndex">分割点</param>
    /// <param name="section"></param>
    /// <param name="verties"></param>
    /// <returns>是否满足</returns>
    bool IsConnectOK(int connectIndex, ref List<int> section, ref List<Vector2> verties)
    {
        //1.判断分割点和0形成的边是否和其他边相交
        for (int i = 2; i < section.Count; i++)
        {
            if (i == connectIndex)
            {
                i++;
                continue;
            }
            if (MathUtility.IsSegmentCross(verties[section[0]], verties[section[connectIndex]], verties[section[i - 1]], verties[section[i]]))
            {
                return false;
            }
        }
        //在满足连接边与任何已有边都不想交的前提下
        //2.通过判断连接边的中点是否在多边形里面来判断此边是否在多边形里面
        Vector2 inPoint = (verties[section[0]] + verties[section[connectIndex]]) * 0.5f;
        List<Vector2> polygon = new List<Vector2>();
        for(int i = 0;i < section.Count; i++)
        {
            polygon.Insert(polygon.Count, verties[section[i]]);
        }
        if(MathUtility.IsPointInPolygon(ref polygon, inPoint))
        {
            return true;
        }
        return false;
    }

    #endregion
}
