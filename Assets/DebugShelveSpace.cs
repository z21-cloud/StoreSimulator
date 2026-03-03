using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugShelveSpace : MonoBehaviour
{
    public GameObject prefab;
    private int x;
    private int z;
    
    private BoxCollider boxCollider;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        Vector3 min = boxCollider.bounds.min;
        Vector3 size = boxCollider.bounds.size;
        Vector3 nodeSize = prefab.GetComponent<Collider>().bounds.size;

        int countX = Mathf.FloorToInt(size.x / nodeSize.x);
        int countZ = Mathf.FloorToInt(size.z / nodeSize.z);

        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countZ; j++)
            {
                float posX = min.x + (i * nodeSize.x) + (nodeSize.x / 2f);
                float posZ = min.z + (j * nodeSize.z) + (nodeSize.z / 2f);

                Vector3 worldPosition = new Vector3(posX, min.y, posZ);

                Instantiate(prefab, worldPosition, Quaternion.identity, transform);
            }
        }
    }
}
