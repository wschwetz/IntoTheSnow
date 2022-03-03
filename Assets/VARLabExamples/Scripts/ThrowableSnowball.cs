/*This source code was originally provided by the Digital Simulations Lab (VARLab) at Conestoga College in Ontario, Canada.
 * It was provided as a foundation of learning for participants of our 2022 Introduction to Unity Boot Camp.
 * Participants are welcome to use, extend and share projects derived from this code under the Creative Commons Attribution-NonCommercial 4.0 International license as linked below:
        Summary: https://creativecommons.org/licenses/by-nc/4.0/
        Full: https://creativecommons.org/licenses/by-nc/4.0/legalcode
 * You may not sell works derived from this code, but we hope you learn from it and share that learning with others.
 * We hope it inspires you to make more games or consider a career in game development.
 * To learn more about the opportunities for computer science and software engineering at Conestoga College please visit https://www.conestogac.on.ca/applied-computer-science-and-information-technology */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableSnowball : MonoBehaviour, IPickup, IThrowable
    {
        [Tooltip("Prefab for the particle effect to play when this snowball impacts after throwing.")]
        [SerializeField] private GameObject impactEffectPrefab;

        /// <summary>Rigidbody attached to this object.</summary>
        private Rigidbody rb;

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

        [Tooltip("Damage dealt by a snowball impact to any classes that implement the IDamageable interface.\nNote: Nothing in the base project implements IDamageable.")]
        [SerializeField] private float impactDamage = 5f;

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
                    collision.collider.isTrigger = true;
                    break;

                case State.Thrown:
                    collision.collider.isTrigger = false;
                    Impact(collision.gameObject);
                    break;
            }
        }

        /// <summary>Handles impact visuals and damage dealing..</summary>
        private void Impact(GameObject obj)
        {
            if (Helpers.TryGetInterface(out IDamageable victim, obj))
            {
                victim.TakeDamage(impactDamage);
            }

            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

            state = State.Pickup;
        }

        /// <summary>Handles being picked up by another object.</summary>
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
    }
}
