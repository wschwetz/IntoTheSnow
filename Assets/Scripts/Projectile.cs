using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{
    public class Projectile : MonoBehaviour
    {

        public float speed = 1000f;
        public float lifespan = 3f;
        public float impactDamage = 3f;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, lifespan);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

