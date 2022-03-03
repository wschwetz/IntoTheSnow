using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TigerTail {
    public class damagable_barrel : MonoBehaviour
    {
        [SerializeField] AudioClip barrelHit = null;
        private AudioSource source = null;
        [SerializeField] private float barrelHealth = 5;

        [SerializeField] private GameObject prefab = null;
        [SerializeField] private GameObject ammo = null;
        [SerializeField] private GameObject energy_can = null;

        public enum State
        {
            /// <summary>This object is waiting to be picked up.</summary>
            Pickup,
            /// <summary>This object is being held.</summary>
            Held,
            /// <summary>This object is being thrown.</summary>
            Thrown
        }
        State state;

        // Start is called before the first frame update
        void Start()
        {
            source = GetComponent<AudioSource>();
            if (source == null)
            {
                Debug.Log("Audio source is null");
            }
            Destroy(gameObject, 60);
        }

        private void OnCollisionEnter(Collision collision)
        {
            
            if (collision.collider.tag == "hammer")
            {
                GameObject tmp = collision.collider.gameObject;
                barrelHealth -= tmp.GetComponent<ThrowableHammer>().impactDamage;
                tmp.GetComponent<ThrowableHammer>().hammerHealth -= 4;
                source.clip = barrelHit;
                source.Play();
            }

            if (collision.collider.tag == "SnowCanon")
            {
                GameObject tmp = collision.collider.gameObject;
                if (!tmp.GetComponent<SnowCanon>().IsHeld())
                {
                    barrelHealth -= tmp.GetComponent<SnowCanon>().impactDamage;
                    Destroy(tmp);
                    source.clip = barrelHit;
                    source.Play();
                }
    
            }

            if (collision.collider.tag == "bullet")
            {
                GameObject tmp = collision.collider.gameObject;
                barrelHealth -= tmp.GetComponent<Projectile>().impactDamage;
                Destroy(tmp);
                source.clip = barrelHit;
                source.Play();
            }

            if (collision.collider.tag == "fired_snowball")
            {
                GameObject tmp = collision.collider.gameObject;
                barrelHealth -= tmp.GetComponent<Projectile>().impactDamage;
                Destroy(tmp);
                source.clip = barrelHit;
                source.Play();
            }

            if (collision.collider.tag == "gun")
            {
                GameObject tmp = collision.collider.gameObject;
                barrelHealth -= tmp.GetComponent<Gun>().impactDamage;
                Destroy(tmp);
                source.clip = barrelHit;
                source.Play();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (barrelHealth <= 0)
            {
                Destroy(gameObject);
                int x = Random.Range(1, 11);
                
                if ((x % 2) == 0)
                {
                    var reward = Instantiate(prefab) as GameObject;

                    Vector3 newPostion = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    reward.transform.position = newPostion;

                }
                else if ((x % 3) == 0)
                {
                    var reward = Instantiate(ammo) as GameObject;
                    Vector3 newPostion = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    reward.transform.position = newPostion;
                }
                else if ((x % 5) == 0)
                {
                    var reward = Instantiate(energy_can) as GameObject;
                    Vector3 newPostion = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    reward.transform.position = newPostion;
                }

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

    }
}
