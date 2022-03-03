//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarrelRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private Vector3 spawnLocation = new Vector3(1, 1, 1);
    [SerializeField] private const int maxBarrels = 120;
    private int currentNumBarrels = 0;

    // Start is called before the first frame update
    void Start()
    {
        while (currentNumBarrels < maxBarrels)
        {
            float x = Random.Range(-100f, 100f);
            float y = Random.Range(-100f, 100f);
            Vector3 newPoint = GetTerrainPos(x, y);
            var barrel = Instantiate(prefab, newPoint, Quaternion.identity) as GameObject;
            currentNumBarrels++;
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
