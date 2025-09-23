using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public OctreeNode rootNode;
    
    public Octree(GameObject[] worldObjects, float minNodeSize)
    {
        Bounds bounds = new();
        
        // 根包围盒
        foreach (var wObject in worldObjects)
        {
            Collider collider = wObject.GetComponent<Collider>();
            if (!collider)
            {
                continue;
            }
            bounds.Encapsulate(collider.bounds);
        }
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) / 2;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
        
        // 根节点构建
        rootNode = new OctreeNode(bounds, minNodeSize);
        AddWorldObject(worldObjects);
    }

    public void AddWorldObject(GameObject[] worldObjects)
    {
        foreach (var go in worldObjects)
        {
            rootNode.AddWorldObject(go);
        }
    }
}
