using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Noise : MonoBehaviour
{
    [SerializeField] AudioClip wind_clip = null;
    private AudioSource source = null;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = wind_clip;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
