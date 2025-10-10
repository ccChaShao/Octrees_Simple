using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Node> nodeList = new();
    public List<Edge> edgeList = new();

    private Ray m_CacheRay = new ();
    private List<Vector3> m_SixDirs = new()
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down
    };

    public void AddNode(OctreeNode otn)
    {
        if (FindNode(otn.id) == null)
        {
            Node node = new Node(otn);           // 一个树节点 = 一个路径节点
            nodeList.Add(node);
        }
    }

    public Node FindNode(int otnId)
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].octreeNode.id == otnId)
            {
                return nodeList[i];
            }
        }

        return null;
    }

    public void AddEdge(OctreeNode fromOtn, OctreeNode toOtn)
    {
        if (FindEdge(fromOtn, toOtn) != null)
        {
            return;
        }
        
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

    public Edge FindEdge(OctreeNode fromOtn, OctreeNode toOtn)
    {
        Node from = FindNode(fromOtn.id);
        Node to = FindNode(toOtn.id);
        if (from != null && to != null)
        {
            for (int i = 0; i < from.edgeList.Count; i++)
            {
                var element = from.edgeList[i];
                if (element.endNode.octreeNode.id == toOtn.id)
                {
                    return element;
                }
            }
        }
        
        return null;
    }

    public void DrawDebug()
    {
        // 边界球体绘制
        foreach (var node in nodeList)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(node.octreeNode.bounds.center, 0.25f);
        }
        
        // 边界路径绘制
        for (int i = 0; i < edgeList.Count; i++)
        {
            Debug.DrawLine(
                edgeList[i].startNode.octreeNode.bounds.center,
                edgeList[i].endNode.octreeNode.bounds.center,
                Color.red
            );
        }
    }

    public void ConnectNodeNodeNeighbours()
    {
        for (int i = 0; i < nodeList.Count; i++)                // 一层循环
        {
            for (int j = 0; j < nodeList.Count; j++)            // 二层循环
            {
                if (i == j)
                {
                    continue;
                }
                // 六方向检查
                for (int k = 0; k < m_SixDirs.Count; k++)
                {
                    m_CacheRay.origin = nodeList[i].octreeNode.bounds.center;
                    m_CacheRay.direction = m_SixDirs[k];
                    float maxLength = nodeList[i].octreeNode.bounds.size.x / 2.0f + 0.01f;
                    // 单次最多是24个
                    if (nodeList[j].octreeNode.bounds.IntersectRay(m_CacheRay, out float hitLength))
                    {
                        // 仅连接相邻邻居（最短）
                        if (hitLength <= maxLength)
                        {
                            AddEdge(nodeList[i].octreeNode, nodeList[j].octreeNode);
                        }
                    }
                }
            }
        }
    }

    public bool AStar(OctreeNode startNode, OctreeNode endNode, ref List<Node> pathList)
    {
        Node start = FindNode(startNode.id);
        Node end = FindNode(endNode.id);

        if (start == null || end == null)
        {
            return false;
        }

        // 一开始就是终点
        if (start.octreeNode.id == end.octreeNode.id)
        {
            end.cameFrom = start;
            ReconstructPath(start, end, ref pathList);
            return true;
        }

        List<Node> openList = new();            // open
        List<Node> closeList = new();           // close（记录的是走过的路径）

        // 代价计算
        start.g = 0;
        start.h = Vector3.SqrMagnitude(endNode.bounds.center - startNode.bounds.center);
        
        openList.Add(start);
        while (openList.Count > 0)
        {
            int thisI = GetLowestFIndex(openList);
            Node thisN = openList[thisI];
            
            // 到达终点则结束
            if (thisN.octreeNode.id == endNode.id)
            {
                ReconstructPath(start, end, ref pathList);
                return true;
            }
            
            // 数列更新
            openList.RemoveAt(thisI);         // 待考察的节点
            closeList.Add(thisN);             // 已经考察过的节点
            
            foreach (Edge edge in thisN.edgeList)
            {
                Node edgeEndNode = edge.endNode;

                if (closeList.IndexOf(edgeEndNode) > -1)
                {
                    continue; 
                }

                bool updateNode = false;
                float newG = thisN.g + Vector3.SqrMagnitude
                (
                    edgeEndNode.octreeNode.bounds.center -
                    thisN.octreeNode.bounds.center
                );

                // 首次被发现
                if (openList.IndexOf(edgeEndNode) <= -1)
                {
                    openList.Add(edgeEndNode);
                    updateNode = true;
                }
                // 有更近的入口点
                else if (newG <= edgeEndNode.g)
                {
                    updateNode = true;
                }
                
                // 数据更新
                if (updateNode)
                {
                    edgeEndNode.cameFrom = thisN;                // 更新来源点
                    edgeEndNode.g = newG;                        // 已消耗代价更新
                    edgeEndNode.h = Vector3.SqrMagnitude(        // 启发代价更新
                        endNode.bounds.center -
                        edgeEndNode.octreeNode.bounds.center
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
    private void ReconstructPath(Node startNode, Node endNode, ref List<Node> pathList)
    {
        pathList.Clear();
        pathList.Add(endNode);

        var from = endNode.cameFrom;
        while (from != null && from != startNode)
        {
            pathList.Insert(0, from);          // 添加到数列首部
            from = from.cameFrom;
        }

        pathList.Insert(0, startNode);
    }
}
