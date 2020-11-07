using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Doors : MonoBehaviour
{
    private Transform _right;
    private Transform _left;

    private Light2D _lightL;
    private Light2D _lightR;
    private AudioSource _source;

    private float _time;

    private float _elapsed;

    private bool _open;
    private bool _triggered;

    private float _initialXR;
    private float _initialXL;

    private Vector2 _initialPosR;
    private Vector2 _endPosR;
    private Vector2 _initialPosL;
    private Vector2 _endPosL;
    // Start is called before the first frame update
    void Start()
    {
        _right = transform.Find("InnerR").transform;
        _left = transform.Find("InnerL").transform;
        _lightR = _right.Find("LightR").GetComponent<Light2D>();
        _lightL = _left.Find("LightL").GetComponent<Light2D>();
        _source = GetComponent<AudioSource>();
        _time = 1;
        _elapsed = 0;
        _open = false;
        _triggered = false;
        _initialPosR = new Vector2(_right.position.x, _right.position.y);
        _endPosR = new Vector2(_right.position.x + 4, _right.position.y);
        _initialPosL = new Vector2(_left.position.x, _left.position.y);
        _endPosL = new Vector2(_left.position.x - 4, _left.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (_open)
        {
            Open();
        }
    }

    private void Open()
    {
        _lightL.color = Color.green;
        _lightR.color = Color.green;
        if (_elapsed < _time)
        {
            _right.position = Vector2.Lerp(_initialPosR,  _endPosR, _elapsed / _time);
            _left.position = Vector2.Lerp(_initialPosL,  _endPosL, _elapsed / _time);
            _elapsed += Time.deltaTime;
        }
        else
        {
            _open = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && !_triggered)
        {
            StartCoroutine(Timer());
            _triggered = true;
        }
            
    }
    
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3);
        _open = true;
        _source.Play();
    }
}
