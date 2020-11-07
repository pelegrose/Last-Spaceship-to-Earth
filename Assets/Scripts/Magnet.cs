using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Magnet : MonoBehaviour
{
    private PointEffector2D _magnet;
    private SpriteRenderer _renderer;
    private bool _turnOn = true;
    private Light2D _light;
    private AudioSource _source;

    [SerializeField] private float _magnitude = -7f;

    [SerializeField] private bool _special = false;

    private bool _triggered;

    private bool _useCounter;
    private int _activationCounter;
    // Start is called before the first frame update
    void Start()
    {
        _activationCounter = 0;
        _useCounter = false;
        _magnet = GetComponent<PointEffector2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _light = GetComponent<Light2D>();
        _source = GetComponent<AudioSource>();
        _magnet.forceMagnitude = 0;
        _triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_special)
        {
            if (Player.triggered && !_triggered)
            {
                _triggered = true;
                StartCoroutine(Special());
            }
        }
        else
        {
            if (_turnOn)
            {
                StartCoroutine(OnOff());    
            }    
        }
    }

    IEnumerator OnOff()
    {
        if (_activationCounter < 3)
        {
            _source.Play();
            _turnOn = false;
            _magnet.forceMagnitude = _magnitude;
            _light.color = Color.red;
            yield return new WaitForSeconds(3);
            _source.Stop();
            _magnet.forceMagnitude = 0;
            _light.color = Color.green;
            yield return new WaitForSeconds(6);
            _turnOn = true;
            if (_useCounter)
            {
                _activationCounter += 1;    
            }
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
    }
    
    IEnumerator Special()
    {
        _source.Play();
        _turnOn = false;
        _magnet.forceMagnitude = _magnitude;
        _light.color = Color.red;
        yield return new WaitForSeconds(GameManager.Level2TransitionTime);
        _source.Stop();
        _magnet.forceMagnitude = 0;
        _light.color = Color.green;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player") && (transform.position.y > 75 && transform.position.y < 85) && !_useCounter)
        {
            _useCounter = true;
        }
    }
}
