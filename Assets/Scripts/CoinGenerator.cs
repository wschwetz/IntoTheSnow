using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{

    public class CoinGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        
        // Start is called before the first frame update
        void Start()
        {

        }

        void GeneratCoin()
        {
            var coin = Instantiate(prefab) as GameObject;
            coin.transform.position = transform.position;

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}