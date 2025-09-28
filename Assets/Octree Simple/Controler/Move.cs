using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5.0f;
    public float accuray = 1.0f;            // 位置精度
    
    // 实时导航数据
    private int currentWayPoint = 0;
    private OctreeNode currentNode;

    // 八叉树导航数据
    public GameObject createOctree;
    private Octree octree;
    private Graph graph;
    public List<Node> aStarPathList = new();
    
    void Start()
    {
        Invoke(nameof(DataInit), 1);
    }

    void Update()
    {
        if (octree == null  || graph == null)
        {
            return;
        }

        if (currentWayPoint >= GetAStarPathCount())
        {
            ReStartMove();
        }
        else
        {
            float distance = Vector3.Distance(GetAstarPathNode(currentWayPoint).octreeNode.nodeBounds.center, transform.position);
            if (distance <= accuray)
            {
                currentWayPoint++;
            }

            if (currentWayPoint < GetAStarPathCount())
            {
                currentNode = GetAstarPathNode(currentWayPoint).octreeNode;
                Vector3 direction = currentNode.nodeBounds.center - transform.position;
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            }
        }
    }

    private void DataInit()
    {
        CreateOctree createOctreeComp = createOctree.GetComponent<CreateOctree>();
        octree = createOctreeComp.Octree;
        graph = createOctreeComp.WayPointGraph;

        ReStartMove();
    }

    public void ReStartMove()
    {
        var startNode = (currentNode != null)
            ? currentNode
            : graph.nodeList[Random.Range(0, graph.nodeList.Count)].octreeNode;
        var endNode = graph.nodeList[Random.Range(0, graph.nodeList.Count)].octreeNode;
        
        bool randomSuc = graph.AStar(startNode, endNode, ref aStarPathList);
        if (randomSuc)
        {
            currentWayPoint = 0;
            transform.position = GetAstarPathNode(0).octreeNode.nodeBounds.center;
        }
    }

    private int GetAStarPathCount()
    {
        return aStarPathList.Count;
    }

    private Node GetAstarPathNode(int index)
    {
        return aStarPathList[index];
    }
}
