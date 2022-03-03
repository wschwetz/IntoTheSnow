using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour
{
    [SerializeField] AudioClip music_clip = null;
    private AudioSource source = null;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = music_clip;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
