using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public OctreeNode rootNode = new();
    public Bounds bounds = new();
    
    public Octree(GameObject[] worldObjects, float minNodeSize)
    {
        // 包围盒构建
        foreach (var wObject in worldObjects)
        {
            Collider collider = wObject.GetComponent<Collider>();
            bounds.Encapsulate(collider.bounds);
        }
    }
}
