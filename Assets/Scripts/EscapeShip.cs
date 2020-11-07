using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class EscapeShip : MonoBehaviour
{
    private AudioSource _source;

    private Animator _animator;

    private Light2D _light;

    // Start is called before the first frame update
    void Start()
    {
        _source = transform.Find("Fire").GetComponent<AudioSource>();
        _animator = transform.Find("Fire").GetComponent<Animator>();
        _light = transform.Find("Fire").transform.Find("FireLight").GetComponent<Light2D>();
        _light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartMoving()
    {
        _animator.SetTrigger("Fire");
        _light.intensity = 4;
        PlayFireSound();
    }

    private void PlayFireSound()
    {
        if (!_source.isPlaying)
        {
            _source.Play();
        }
    }
}