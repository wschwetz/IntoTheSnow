/*This source code was originally provided by the Digital Simulations Lab (VARLab) at Conestoga College in Ontario, Canada.
 * It was provided as a foundation of learning for participants of our 2022 Introduction to Unity Boot Camp.
 * Participants are welcome to use, extend and share projects derived from this code under the Creative Commons Attribution-NonCommercial 4.0 International license as linked below:
        Summary: https://creativecommons.org/licenses/by-nc/4.0/
        Full: https://creativecommons.org/licenses/by-nc/4.0/legalcode
 * You may not sell works derived from this code, but we hope you learn from it and share that learning with others.
 * We hope it inspires you to make more games or consider a career in game development.
 * To learn more about the opportunities for computer science and software engineering at Conestoga College please visit https://www.conestogac.on.ca/applied-computer-science-and-information-technology */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Snowball : MonoBehaviour
    {
        /// <summary>Density of packed snow in kg/m^3.</summary>
        private const float DENSITY_OF_SNOWBALL = 200;

        /// <summary>Rigidbody attached to this snowball.</summary>
        private Rigidbody rb;

        /// <summary>SphereCollider attached to this snowball.</summary>
        private SphereCollider sc;

        /// <summary>Total number of completed rotations.</summary>
        private float completedRotations;

        /// <summary>Current size of the snowball.</summary>
        private float currentSize;

        [Tooltip("Particle system for the mist effect below the snowball.")]
        [SerializeField] private ParticleSystem ps;

        [Tooltip("Number of rotations needed to double the snowball's size.")]
        [SerializeField] private float rotationsToDoubleInSize = 15f;

        private void Awake()
        {
            sc = GetComponent<SphereCollider>();
            rb = GetComponent<Rigidbody>();
            CalculateMass();
        }

        private void Update()
        {
            CalculateSize();
            CalculateMass();
            HandleParticleSystem();
        }

        private void HandleParticleSystem()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, sc.radius * currentSize + 0.5f))
            {
                ps.transform.position = hit.point;
                ps.transform.rotation = Quaternion.identity;
            }
        }

        private void FixedUpdate()
        {
            completedRotations += rb.angularVelocity.magnitude * Time.fixedDeltaTime; // Add our current angular speed to the number of completed rotations.
        }

        private void CalculateMass()
        {
            const float SPHERE_VOLUME_CONSTANT = 4 * Mathf.PI / 3;
            var r = sc.radius * currentSize;
            rb.mass = DENSITY_OF_SNOWBALL * SPHERE_VOLUME_CONSTANT * r * r * r; // Multiplying by r 3 times is faster than Mathf.Pow.
        }

        private void CalculateSize()
        {
            currentSize = 1 + completedRotations / rotationsToDoubleInSize;

            transform.localScale = Vector3.one * currentSize;
        }
    }
}
