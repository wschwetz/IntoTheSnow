/*This source code was originally provided by the Digital Simulations Lab (VARLab) at Conestoga College in Ontario, Canada.
 * It was provided as a foundation of learning for participants of our 2022 Introduction to Unity Boot Camp.
 * Participants are welcome to use, extend and share projects derived from this code under the Creative Commons Attribution-NonCommercial 4.0 International license as linked below:
        Summary: https://creativecommons.org/licenses/by-nc/4.0/
        Full: https://creativecommons.org/licenses/by-nc/4.0/legalcode
 * You may not sell works derived from this code, but we hope you learn from it and share that learning with others.
 * We hope it inspires you to make more games or consider a career in game development.
 * To learn more about the opportunities for computer science and software engineering at Conestoga College please visit https://www.conestogac.on.ca/applied-computer-science-and-information-technology */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.InputSystem;
using TMPro;

namespace TigerTail.FPSController
{
    [DisallowMultipleComponent]




    public class Player : MonoBehaviour, IPickerUpper
    {
        private bool energized = false;
        private float energyStart = 0;
        private float energyTimeLeft = 5;
        private int currentAmmo = 20;
        public GameObject bullet;
        public Transform spawnTransform;

        private int coinCount = 0;
        [SerializeField] AudioClip canonShot = null;
        [SerializeField] AudioClip dryFire = null;
        [SerializeField] private GameObject fallingSnowBall = null;
        private AudioSource source = null;
        float secondsLeft;
        private float timeLeft = 60;
        public TextMeshProUGUI countText;
        public TextMeshProUGUI TimerText;
        public TextMeshProUGUI gameOverDisplay;

        [Tooltip("The transform pickups should be parented to for holding/throwing.")]
        [SerializeField] private Transform pickupLocation;

        /// <summary>Currently held pickup.</summary>
        private IPickup pickup;

        [Tooltip("Force to apply to thrown objects. (Mass-dependant)")]
        [Range(500,5000)]
        [SerializeField] private float throwForce = 2000f;

        private void Start()
        {
            gameOverDisplay.enabled = false;
            SetCountText();
            source = GetComponent<AudioSource>();
            if(source == null)
            {
                Debug.Log("Audio source is null");
            }
            else
            {
                
            }
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            secondsLeft = Mathf.FloorToInt(timeLeft % 60);
            if(secondsLeft > 0 && (secondsLeft % 10) ==0)
            {
                float x = Random.Range(-100f, 100f);
                float y = Random.Range(-100f, 100f);
                Vector3 newPoint = new Vector3(x, 50, y);
                var snowball = Instantiate(fallingSnowBall, newPoint, Quaternion.identity) as GameObject;
            }
            if (secondsLeft > 0) UpDateCounter();
            else
            {
                gameOverDisplay.text = "GAME OVER\n" + "Final score: " + coinCount;
                gameOverDisplay.enabled = true;
            }
            if (currentAmmo > 0)
            {
                source.clip = canonShot;
            }
            else source.clip = dryFire;

            HandleThrowing();
            if (Input.GetKeyUp(KeyCode.F) && pickup != null && currentAmmo > 0)
            {
                FireCanon();
                currentAmmo--;
                SetCountText();
            }
            else if (Input.GetKeyUp(KeyCode.F) && pickup != null && currentAmmo <= 0) source.Play();

            if(energized)
            {
                energyTimeLeft -= Time.deltaTime;
                if (Mathf.FloorToInt(energyTimeLeft % 60) == 0)
                {
                    energyStart = 0;
                    energized = false;
                    energyTimeLeft = 5;
                    GetComponent<FPSMovement>().moveSpeed = 10;
                }
            }
        }

        private void FireCanon()
        {
            source.Play();
            var projectile = Instantiate(bullet, spawnTransform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().AddForce(spawnTransform.forward * 25, ForceMode.Impulse);
        }

        private void HandleThrowing()
        {
            if (Input.GetMouseButtonUp(0) && pickup != null)
            {
                if (pickup is IThrowable)
                {
                    (pickup as IThrowable).Throw(gameObject, pickupLocation.forward * throwForce);
                    pickup = null;
                }
            }
        }

        public void PickupObject(IPickup pickup)
        {
            if (this.pickup != null) // Don't pick up an object if we already have one picked up.
                return;

            pickup.SetParentPoint(pickupLocation);        
            
            this.pickup = pickup;
        }

        void SetCountText()
        {
            countText.text = "Coins: " + coinCount + "\nSnowballs: " + currentAmmo;
        }

        void UpDateCounter()
        {
            TimerText.text = "Time: " + secondsLeft;
        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("coin"))
            {
                // Deactivate because we still need the sound
                other.gameObject.GetComponent<Coin>().Deactivate();
                coinCount++;
                SetCountText();
            }

            if (other.gameObject.CompareTag("magazine"))
            {
                other.gameObject.GetComponent<ammo_pickup>().Deactivate();
                currentAmmo += 5;
                SetCountText();
            }

            if (other.gameObject.CompareTag("SnowCanon"))
            {
                other.enabled = false;
            }

            if (other.gameObject.CompareTag("energy_can"))
            {
                energized = true;
                energyStart = Time.deltaTime;
                GetComponent<FPSMovement>().moveSpeed = GetComponent<FPSMovement>().moveSpeed * 1.5f;
                other.gameObject.GetComponent<energy_pickup>().Deactivate();
            }

        }
    }
}
