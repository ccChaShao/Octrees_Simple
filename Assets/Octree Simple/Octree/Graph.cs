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

        // 两点重复，直接返回
        if (startNode.id == endNode.id)
        {
            pathCacheList.Add(start);
            return true;
        }

        List<Node> openList = new();            // open
        List<Node> closeList = new();           // close
        float gScore = 0;
        bool isBetter = false;

        // 代价计算
        start.g = 0;
        start.h = Vector3.SqrMagnitude(endNode.nodeBounds.center - startNode.nodeBounds.center);
        
        openList.Add(start);
        while (openList.Count > 0)
        {
            
        }
        
        return true;
    }

    // private int lowesf(List<Node> openList)
    // {
    //     int lowesf = 0;
    // }
}
