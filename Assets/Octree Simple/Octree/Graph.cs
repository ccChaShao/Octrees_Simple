using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Edge> edgeList = new();
    public Dictionary<int, Node> nodeMap = new();
    
    // A*
    public List<Node> pathCacheList = new();            // 缓存最新寻路找到的所有路径节点；

    public void AddNode(OctreeNode otn)
    {
        if (FindNode(otn.id) == null)
        {
            Node node = new Node(otn);           // 一个树节点 = 一个路径节点
            nodeMap[otn.id] = node;
        }
    }

    public void AddEdge(OctreeNode fromOtn, OctreeNode toOtn)
    {
        Node from = FindNode(fromOtn.id);
        Node to = FindNode(toOtn.id);
        if (from != null && to != null)
        {
            Edge edge = new(from, to);          // 正向
            edgeList.Add(edge);
            from.edgeList.Add(edge);
            
            Edge reverseEdge = new(to, from);   // 反向
            edgeList.Add(reverseEdge);
            to.edgeList.Add(reverseEdge);
        }
    }

    public Node FindNode(int otnId)
    {
        return nodeMap[otnId];
    }

    public void DrawDebug()
    {
        // 边界球体绘制
        foreach (var kv in nodeMap)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(kv.Value.octreeNode.nodeBounds.center, 0.25f);
        }
        
        // 边界路径绘制
        for (int i = 0; i < edgeList.Count; i++)
        {
            Debug.DrawLine(
                edgeList[i].startNode.octreeNode.nodeBounds.center,
                edgeList[i].endNode.octreeNode.nodeBounds.center,
                Color.yellow
            );
        }
    }

    public bool AStar(OctreeNode startNode, OctreeNode endNode)
    {
        pathCacheList.Clear();
        Node start = FindNode(startNode.id);
        Node end = FindNode(endNode.id);

        if (start == null || end == null)
        {
            return false;
        }

        // 一开始就是终点
        if (startNode.id == endNode.id)
        {
            pathCacheList.Add(start);
            return true;
        }

        List<Node> openList = new();            // open
        List<Node> closeList = new();           // close（记录的是走过的路径）
        
        // float betterGS = float.MaxValue;
        // bool isBetter = false;

        // 代价计算
        start.g = 0;
        start.h = Vector3.SqrMagnitude(endNode.nodeBounds.center - startNode.nodeBounds.center);
        
        openList.Add(start);
        while (openList.Count > 0)
        {
            int thisI = GetLowestFIndex(openList);
            Node thisN = openList[thisI];
            
            // 到达终点则结束
            if (thisN.octreeNode.id == endNode.id)
            {
                ReconstructPath(start, end);
                return true;
            }
            
            // 数列更新
            openList.RemoveAt(thisI);         // 待考察的节点
            closeList.Add(thisN);             // 已经考察过的节点
            
            Node neighbourN;
            foreach (Edge edge in thisN.edgeList)
            {
                neighbourN = edge.endNode;

                if (closeList.IndexOf(neighbourN) > -1)
                {
                    continue; 
                }

                bool isBetterG = false;
                float newG = thisN.g + Vector3.SqrMagnitude
                (
                    neighbourN.octreeNode.nodeBounds.center -
                    thisN.octreeNode.nodeBounds.center
                );

                // 首次被发现
                if (openList.IndexOf(neighbourN) == -1)
                {
                    openList.Add(neighbourN);
                    isBetterG = true;
                }
                // 有更近的入口点
                else if (newG <= neighbourN.g)
                {
                    isBetterG = true;
                }
                
                // 数据更新
                if (isBetterG)
                {
                    neighbourN.cameFrom = thisN;                // 更新来源点
                    neighbourN.g = newG;                        // 已消耗代价更新
                    neighbourN.h = Vector3.SqrMagnitude(        // 启发代价更新
                        endNode.nodeBounds.center -
                        neighbourN.octreeNode.nodeBounds.center
                    );
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// 找到最低f的节点
    /// </summary>
    private int GetLowestFIndex(List<Node> openList)
    {
        float lowestF = -9999;
        int lowestIndex = 0;
        for (int i = 0; i < openList.Count; i++)
        {
            if (openList[i].f <= lowestF)
            {
                lowestF = openList[i].f;
                lowestIndex = i;
            }
        }

        return lowestIndex;
    }

    /// <summary>
    ///  路径回溯
    /// </summary>
    private void ReconstructPath(Node startNode, Node endNode)
    {
        pathCacheList.Clear();
        pathCacheList.Add(endNode);

        var from = endNode.cameFrom;
        while (from != null && from != startNode)
        {
            pathCacheList.Insert(0, from);          // 添加到数列首部
            from = from.cameFrom;
        }

        pathCacheList.Insert(0, startNode);
    }
}
