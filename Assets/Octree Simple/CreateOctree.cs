using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public int minNodeSize;
    public GameObject[] worldObjects;
    
    private Octree m_Octree;
    private Graph m_WayPointGraph;

    public Octree Octree => m_Octree;
    public Graph WayPointGraph => m_WayPointGraph;

    private void Awake()
    {
        m_WayPointGraph = new Graph();
        m_Octree = new Octree(worldObjects, minNodeSize, m_WayPointGraph);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        m_Octree.DrawDebug();
        m_Octree.rootNode.DrawDebug();
        m_Octree.navigationGraph.DrawDebug();
    }
}
