using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class DoorLevel2 : MonoBehaviour
{
    private Transform _right;
    private Transform _left;

    private Light2D _lightL;
    private Light2D _lightR;
    private AudioSource _source;
    private AudioSource _alarm;
    [SerializeField] private float _delay;

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

    [SerializeField] private float _closeDis;
    // Start is called before the first frame update
    void Start()
    {
        if (name == "doors (2)")
        {
            _alarm = transform.Find("Alarm").GetComponent<AudioSource>();
        }
        _right = transform.Find("InnerR").transform;
        _left = transform.Find("InnerL").transform;
        _lightR = _right.Find("LightR").GetComponent<Light2D>();
        _lightL = _left.Find("LightL").GetComponent<Light2D>();
        _lightR.color = Color.green;
        _lightL.color = Color.green;
        _source = GetComponent<AudioSource>();
        _time = 1;
        _elapsed = 0;
        _open = true;
        _triggered = false;
        _initialPosR = new Vector2(_right.position.x, _right.position.y);
        _endPosR = new Vector2(_right.position.x - _closeDis, _right.position.y);
        _initialPosL = new Vector2(_left.position.x, _left.position.y);
        _endPosL = new Vector2(_left.position.x + _closeDis, _left.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_open)
        {
            Close();
        }
    }

    private void Close()
    {
        _lightL.color = Color.red;
        _lightR.color = Color.red;
        if (_elapsed < _time)
        {
            _right.position = Vector2.Lerp(_initialPosR,  _endPosR, _elapsed / _time);
            _left.position = Vector2.Lerp(_initialPosL,  _endPosL, _elapsed / _time);
            _elapsed += Time.deltaTime;
        }
        else
        {
            _open = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && !_triggered)
        {
            _triggered = true;
            StartCoroutine(AllowClose(other));
        }
    }

    IEnumerator AllowClose(Collider2D player)
    {
        yield return new WaitForSeconds(_delay);
        _source.Play();
        _open = false;
        if (name == "doors (2)")
        {
            _alarm.Play();
            player.GetComponent<Player>().BeginLevel2();
            yield return new WaitForSeconds(GameManager.Level2TransitionTime);
            _source.volume = 0.6f;
        }
    }

    public void TurnOffAlarm()
    {
        if (_alarm != null)
        {
            _alarm.Stop();
        }
    }
}
