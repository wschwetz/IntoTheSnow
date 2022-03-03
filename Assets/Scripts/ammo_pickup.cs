using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammo_pickup : MonoBehaviour
{

    [SerializeField] AudioClip pickupNoise = null;
    private AudioSource source = null;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            source.clip = pickupNoise;
            source.Play();
        }
    }

    public void Deactivate()
    {
        this.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        this.GetComponent<BoxCollider>().enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
