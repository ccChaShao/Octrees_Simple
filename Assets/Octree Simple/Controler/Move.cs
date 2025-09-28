using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5.0f;
    public float accuray = 1.0f;            // 位置精度
    
    // 实时导航数据
    private int m_CurWayPoint = 0;
    private OctreeNode m_CurNode;

    // 八叉树导航数据
    public GameObject createOctree;
    private Octree m_Octree;
    private Graph m_Graph;
    private List<Node> m_AStarPathList = new();
    
    void Start()
    {
        Invoke(nameof(DataInit), 1);
    }

    void Update()
    {
        if (m_Octree == null  || m_Graph == null)
        {
            return;
        }

        if (m_CurWayPoint >= GetAStarPathCount())
        {
            ReStartMove();
        }
        else
        {
            float distance = Vector3.Distance(GetAstarPathNode(m_CurWayPoint).octreeNode.nodeBounds.center, transform.position);
            if (distance <= accuray)
            {
                m_CurWayPoint++;
            }

            if (m_CurWayPoint < GetAStarPathCount())
            {
                m_CurNode = GetAstarPathNode(m_CurWayPoint).octreeNode;
                Vector3 direction = m_CurNode.nodeBounds.center - transform.position;
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            }
        }
    }

    private void DataInit()
    {
        CreateOctree createOctreeComp = createOctree.GetComponent<CreateOctree>();
        m_Octree = createOctreeComp.Octree;
        m_Graph = createOctreeComp.WayPointGraph;

        ReStartMove();
    }

    public void ReStartMove()
    {
        var startNode = (m_CurNode != null)
            ? m_CurNode
            : m_Graph.nodeList[Random.Range(0, m_Graph.nodeList.Count)].octreeNode;
        var endNode = m_Graph.nodeList[Random.Range(0, m_Graph.nodeList.Count)].octreeNode;
        
        bool randomSuc = m_Graph.AStar(startNode, endNode, ref m_AStarPathList);
        if (randomSuc)
        {
            m_CurWayPoint = 0;
            transform.position = GetAstarPathNode(0).octreeNode.nodeBounds.center;
        }
    }

    private int GetAStarPathCount()
    {
        return m_AStarPathList.Count;
    }

    private Node GetAstarPathNode(int index)
    {
        return m_AStarPathList[index];
    }
}
