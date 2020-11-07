using System;
using UnityEngine;

public class VentAccelerate : MonoBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private float _maxSpeed;

    [SerializeField] private float _time;
    [SerializeField] private float _upMagnitude;
    [SerializeField] private float _downMagnitude;

    private float _curSpeed;
    private float _elapsed;

    private bool _triggered;
    private Transform _ventBlades;

    private AreaEffector2D _upUnit1;
    private AreaEffector2D _upUnit2;

    private AreaEffector2D _downUnit1;
    private AreaEffector2D _downUnit2;
    // Start is called before the first frame update
    void Start()
    {
        _curSpeed = -_speed;
        _elapsed = 0;
        _triggered = false;
        _upUnit1 = transform.Find("UpUnit1").GetComponent<AreaEffector2D>();
        _upUnit2 = transform.Find("UpUnit2").GetComponent<AreaEffector2D>();
        _downUnit1 = transform.Find("DownUnit1").GetComponent<AreaEffector2D>();
        _downUnit2 = transform.Find("DownUnit2").GetComponent<AreaEffector2D>();
        _ventBlades = transform.Find("ventSp").transform;
    }

    // Update is called once per frame
    void Update()
    {
        _ventBlades.Rotate(0,0,_curSpeed * Time.deltaTime);
        if (_triggered)
        {
            if (_elapsed < _time)
            {
                _curSpeed = -Mathf.Lerp(_speed, _maxSpeed, _elapsed / _time);
                _elapsed += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_triggered)
        {
            _triggered = true;
            Player player = other.transform.GetComponent<Player>();
            player.NotifyPlayerSurvived();
            if (player.GetPlayerNum() < 3)
            {
                _upUnit1.forceMagnitude = _upMagnitude;
                _downUnit2.forceMagnitude = _downMagnitude;
            }
            else
            {
                _upUnit2.forceMagnitude = _upMagnitude;
                _downUnit1.forceMagnitude = _downMagnitude;
            }
        }
    }
}