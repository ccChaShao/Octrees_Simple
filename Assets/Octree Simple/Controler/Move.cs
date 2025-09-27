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
        
        var pathCacheListCount = graph.GetChachePathCount();
        Debug.LogError("charsiew : [Update] : 2222-------------"+pathCacheListCount); 
        if (pathCacheListCount <= 0)
        {
            return;
        }
    
        // 到达目的地则重新获取目的地并在下一帧重新开始；
        if (currentWayPoint >= pathCacheListCount)
        {
            GetRandomDestination();
            return;
        }
        
        // 检查是否到达目的地；
        var distance = Vector3.Distance(
            graph.GetCachePathNode(currentWayPoint).octreeNode.nodeBounds.center,
            transform.position
        );
        if (distance <= accuray)
        {
            currentWayPoint++;
        }
        
        // 数据更新
        if (currentWayPoint < graph.GetChachePathCount())
        {
            currentNode = graph.GetCachePathNode(currentWayPoint).octreeNode;
            Vector3 direction = currentNode.nodeBounds.center - transform.position;
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        }
    }

    private void DataInit()
    {
        var createOctreeComp = createOctree.GetComponent<CreateOctree>();
        octree = createOctreeComp.Octree;
        graph = createOctreeComp.WayPointGraph;

        GetRandomDestination();
    }

    private void GetRandomDestination()
    {
        int random = Random.Range(0, graph.nodeList.Count);
        graph.AStar(
            graph.nodeList[0].octreeNode,
            graph.nodeList[random].octreeNode
        );
    }
}
