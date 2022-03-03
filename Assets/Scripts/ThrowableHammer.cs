using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableHammer : MonoBehaviour, IPickup, IThrowable
    {
        [SerializeField] AudioClip barrelHit = null;
        private AudioSource source = null;
        [SerializeField] private GameObject impactEffectPrefab;
        public int hammerHealth = 10;

        public enum State
        {
            /// <summary>This object is waiting to be picked up.</summary>
            Pickup,
            /// <summary>This object is being held.</summary>
            Held,
            /// <summary>This object is being thrown.</summary>
            Thrown
        }
        private State state = State.Pickup;
        public float impactDamage = 5f;
        private Rigidbody rb;

        private void Start()
        {
            source = GetComponent<AudioSource>();
        }
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (state)
            {
                case State.Pickup:
                    Pickup(collision.gameObject);
                    break;

                case State.Thrown:
                    Impact(collision.gameObject);
                    break;
            }

            if (collision.collider.tag == "barrel")
            {
                source.clip = barrelHit;
                source.Play();
            }
        }

        private void Impact(GameObject obj)
        {
            if (Helpers.TryGetInterface(out IDamageable victim, obj))
            {
                victim.TakeDamage(impactDamage);
            }

            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

            state = State.Pickup;
        }

        public void Pickup(GameObject obj)
        {
            if (Helpers.TryGetInterface(out IPickerUpper pickerUpper, obj))
            {
                pickerUpper.PickupObject(this);
                rb.constraints = RigidbodyConstraints.FreezeAll;
                state = State.Held;



            }
        }

        /// <summary>Handles being thrown by another object.</summary>
        public void Throw(GameObject thrower, Vector3 forceVector)
        {
            transform.SetParent(null);
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(forceVector);
            state = State.Thrown;
        }

        /// <summary>Sets the parent transform for this snowball while it's being held and resets its local position.</summary>
        public void SetParentPoint(Transform point)
        {
            transform.SetParent(point);
            transform.localPosition = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (hammerHealth <= 0) Destroy(gameObject);
        }
    }
}
