using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{
    public class damageable_bucket : MonoBehaviour
    {
        [SerializeField] private float bucketHealth = 4;
        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.collider.tag == "hammer")
            {
                GameObject tmp = collision.collider.gameObject;
                //Debug.Log("hammer hit");
                bucketHealth -= tmp.GetComponent<ThrowableHammer>().impactDamage;
                Destroy(tmp);

            }
        }

        // Update is called once per frame
        void Update()
        {
            if (bucketHealth <= 0) Destroy(gameObject);
        }
    }
}
