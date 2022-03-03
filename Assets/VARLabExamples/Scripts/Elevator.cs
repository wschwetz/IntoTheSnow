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
using TigerTail.FPSController;
using UnityEngine;
using UnityEngine.Events;

namespace TigerTail
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class Elevator : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";

        public enum Mode
        {
            /// <summary>Elevator will loop through all destinations and start back at index 0 after reaching the end.</summary>
            Looping,
            /// <summary>Elevator will loop through all destinations and go backwards through the list when it reaches the end.</summary>
            PingPong,
            /// <summary>Elevator will choose a random new destination from its list each time it reaches a destination.</summary>
            Random
        }

        public enum Activator
        {
            /// <summary>The elevator will automatically begin moving.</summary>
            Automatic,
            /// <summary>The elevator will activate when the player steps on it.</summary>
            Player,
            /// <summary>The elevator will activate when any object collides with it.</summary>
            Everything,
            /// <summary>Elevator will not move without calling Move.</summary>
            Manual
        }

        public enum Deactivation
        {
            /// <summary>When there are no colliders on the elevator, it will stop moving.</summary>
            Stop,
            /// <summary>When there are no colliders on the elevator, it will continue moving to its next destination before stopping.</summary>
            Continue,
            /// <summary>When there are no colliders on the elevator, it will return to its last destination before stopping.</summary>
            Return
        }

        [Tooltip("Array of destinations for the elevator to loop between.")]
        [SerializeField] private Transform[] destinations;

        [Tooltip("Adds the starting location of this elevator to the list of destinations.")]
        [SerializeField] private bool useStartingLocationAsDestination = true;

        [Tooltip("Mode for the elevator to use.")]
        [SerializeField] private Mode mode = Mode.Looping;

        [Tooltip("Speed of the elevator in m/s.")]
        [Range(0f, 10f)]
        [SerializeField] private float moveSpeed = 4f;

        [Tooltip("Method used to activate the elevator.")]
        [SerializeField] private Activator activator = Activator.Player;

        [Tooltip("What the elevator does when all of its activators fall off.")]
        [SerializeField] private Deactivation deactivateMode = Deactivation.Return;

        /// <summary>If the elevator is currently moving.</summary>
        private bool active;

        /// <summary>Current destination index to move towards.</summary>
        private int destinationIndex = 0;

        /// <summary>Is the elevator currently moving backwards when set to PingPong mode.</summary>
        private bool pong = false;

        /// <summary>Current destination to move towards.</summary>
        private Transform destination;

        /// <summary>GameObjects of anything currently riding the elevator.</summary>
        private List<GameObject> activators;

        /// <summary>If the elevator is currently returning to its last destination.</summary>
        private bool reversing = false;

        [Tooltip("Event to fire when the Elevator reaches its destination.")]
        public UnityEvent<Transform> OnArrival;

        /// <summary>Player movement script to apply external velocity.</summary>
        private FPSMovement playerMovement;

        private void Awake()
        {
            activators = new List<GameObject>();

            active = activator == Activator.Automatic; // Activate the elevator if it's in automatic mode.

            destinationIndex = 0;

            if (useStartingLocationAsDestination)
            {
                AddStartingLocationToDestinations();
                destinationIndex = 1;
            }

            destination = destinations[destinationIndex];
        }

        private void Update()
        {
            HandlePlayerVelocitySync();

            if (active)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination.position, moveSpeed * Time.deltaTime);

                var sqrDist = (transform.position - destination.position).sqrMagnitude;

                const float ERROR_MARGIN = 0.1f; // How close we need to be to the destination to count as having arrived.
                                                 // This is left squared since using square root to get distance is expensive and unnecessary.
                if (sqrDist < ERROR_MARGIN)
                {
                    OnArrival?.Invoke(destination);

                    switch (activator)
                    {
                        case Activator.Manual:
                            active = false; // We arrived, shut deactivate since we're in manual mode.
                            return;

                        case Activator.Everything:
                        case Activator.Player:
                            active = activators.Count > 0; // Nothing's on the elevator, deactivate it.
                            break;
                    }

                    GetDestinationByMode();
                }
            }
        }

        /// <summary>Sets the destination of the elevator based on its current <see cref="Mode"/>.</summary>
        private void GetDestinationByMode(bool reverse = false)
        {
            switch (mode)
            {
                case Mode.Looping:
                    GetLoopingDestination(reverse);
                    break;

                case Mode.PingPong:
                    GetPingPongDestination(reverse);
                    break;

                case Mode.Random:
                    GetRandomDestination();
                    break;
            }

            destination = destinations[destinationIndex];
            reversing = reverse;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var objectCanUseElevator = false;
            var isPlayer = IsAPlayerObject(collision.gameObject);

            switch (activator)
            {
                case Activator.Everything:
                    objectCanUseElevator = true;
                    break;

                case Activator.Player:    
                    objectCanUseElevator = isPlayer;
                    break;
            }

            if (objectCanUseElevator)
            {
                active = true;
                activators.Add(collision.gameObject);

                if(isPlayer)
                    playerMovement = collision.gameObject.GetComponent<FPSMovement>(); // ew coupling

                if (reversing) // If the elevator was returning to its last destination, we want to go forward again.
                    GetDestinationByMode();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            var isPlayer = IsAPlayerObject(collision.gameObject);

            switch (activator)
            {
                case Activator.Everything:
                    activators.Remove(collision.gameObject);

                    if (isPlayer)
                    {
                        DetachPlayer();
                    }

                    DeactivateByMode();
                    break;

                case Activator.Player:
                    if (isPlayer)
                    {
                        activators.Remove(collision.gameObject);
                        DetachPlayer();
                    }

                    DeactivateByMode();
                    break;
            }
        }

        /// <summary>Stops applying its own velocity to the player, used previously to keep them in sync.</summary>
        private void DetachPlayer()
        {
            playerMovement.ExternalVelocity = Vector3.zero;
            playerMovement = null;
        }

        /// <summary>Handles the deactivation logic that should run when no activators are present on the elevator.</summary>
        private void DeactivateByMode()
        {
            switch (deactivateMode)
            {
                case Deactivation.Return:
                    if (activators.Count <= 0)
                        GetDestinationByMode(true);

                    break;

                case Deactivation.Stop:
                    active = activators.Count > 0;
                    break;
            }
        }

        /// <summary>Checks if <paramref name="obj"/> is tagged with the Player tag in Unity.</summary>
        private bool IsAPlayerObject(GameObject obj)
        {
            return obj.CompareTag(PLAYER_TAG);
        }

        /// <summary>Forces the elevator to move to a specific transform.</summary>
        /// <remarks>This transform does not need to be part of the destinations array.</remarks>
        public void Move(Transform destination)
        {
            this.destination = destination;
            active = true;
        }

        /// <summary>Gets a random index to use with the destinations array.</summary>
        private void GetRandomDestination()
        {
            destinationIndex = UnityEngine.Random.Range(0, destinations.Length); // Didn't care to exclude the current destination index since nobody will notice.
        }

        /// <summary>Gets the next index to use with the destinations array, restarting from 0 if it reaches the end.</summary>
        private void GetLoopingDestination(bool reverse)
        {
            int nextIndex;

            if (reverse)
                nextIndex = destinationIndex - 1;
            else
                nextIndex = destinationIndex + 1;

            destinationIndex = Helpers.Modulo(nextIndex, destinations.Length);
        }

        /// <summary>Gets the next index to use with the destinations array, reversing through the array when it reaches the start/finish.</summary>
        private void GetPingPongDestination(bool reverse)
        {
            int nextIndex;

            if (pong ^ reverse) // XOR so we can reverse it without having to duplicate a lot more code.
            {
                nextIndex = destinationIndex - 1;
                if (nextIndex < 0)
                {
                    destinationIndex++;
                    pong = false;
                }
                else
                {
                    destinationIndex--;
                }
            }
            else
            {
                nextIndex = destinationIndex + 1;
                if (nextIndex >= destinations.Length)
                {
                    destinationIndex--;
                    pong = true;
                }
                else
                {
                    destinationIndex++;
                }
            }
        }

        /// <summary>Adds the velocity of this platform to the Player so that they travel with the platform instead of sliding off.</summary>
        private void HandlePlayerVelocitySync()
        {
            if (playerMovement != null)
            {
                if (active)
                    playerMovement.ExternalVelocity = (destination.position - transform.position).normalized * moveSpeed;
                else
                    playerMovement.ExternalVelocity = Vector3.zero;
            }
        }

        /// <summary>Adds the starting location to the destinations array so that it may be part of the loop.</summary>
        private Transform AddStartingLocationToDestinations()
        {
            var temp = new Transform[destinations.Length + 1];

            var newDestination = new GameObject("Elevator Starting Position").transform;
            newDestination.position = transform.position;
            temp[0] = newDestination;

            for (int i = 1; i < temp.Length; i++)
            {
                temp[i] = destinations[i - 1];
            }

            destinations = temp;

            return newDestination;
        }
    }
}