using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject Tree1 = null;
    [SerializeField] private GameObject Tree2 = null;

    private int maxTrees = 800;
    private int treeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        while (treeCount < maxTrees)
        {
            float x = Random.Range(-120f, 120f);
            float y = Random.Range(-120f, 120f);
            Vector3 newPoint = GetTerrainPos(x, y);
            if (treeCount % 2 == 0)
            {
                //var tree = Instantiate(Tree1, new Vector3(x, GetTerrainPos(x, y), y), Quaternion.identity) as GameObject;
                var tree = Instantiate(Tree1, newPoint, Quaternion.identity) as GameObject;
            }
            else
            {
                //var tree = Instantiate(Tree2, new Vector3(x, GetTerrainPos(x, y), y), Quaternion.identity) as GameObject;
                var tree = Instantiate(Tree2, newPoint, Quaternion.identity) as GameObject;
            }
            
            treeCount++;
        }

     
    }

    static Vector3 GetTerrainPos(float x, float y)
    {
        //Create object to store raycast data
        RaycastHit hit;

        //Create origin for raycast that is above the terrain. I chose 100.
        Vector3 origin = new Vector3(x, 100, y);

        //Send the raycast.
        Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity);

        return hit.point;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
