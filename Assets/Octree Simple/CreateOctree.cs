using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public int minNodeSize;
    public GameObject[] worldObjects;
    
    private Octree m_Octree;
    
    void Start()
    {
        m_Octree = new Octree(worldObjects, minNodeSize);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        m_Octree.rootNode.DrawGizoms();
    }
}
