using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPull : MonoBehaviour
{
    [SerializeField] private float _downMagnitude;
    [SerializeField] private float _upMagnitude;

    private bool _triggered;
    private AreaEffector2D _downPlayer1;
    private AreaEffector2D _downPlayer2;
    private AreaEffector2D _upPlayer1;
    private AreaEffector2D _upPlayer2;
    // Start is called before the first frame update
    void Start()
    {
        _triggered = false;
        _downPlayer1 = transform.Find("DownPlayer1").GetComponent<AreaEffector2D>();
        _downPlayer2 = transform.Find("DownPlayer2").GetComponent<AreaEffector2D>();
        _upPlayer1 = transform.Find("UpPlayer1").GetComponent<AreaEffector2D>();
        _upPlayer2 = transform.Find("UpPlayer2").GetComponent<AreaEffector2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && !_triggered)
        {
            _triggered = true;
            int playerNum = other.transform.GetComponent<Player>().GetPlayerNum();
            if (playerNum == 1 || playerNum == 3)
            {
                _upPlayer1.forceMagnitude = _upMagnitude;
                _downPlayer2.forceMagnitude = _downMagnitude;
            }
            else
            {
                _upPlayer2.forceMagnitude = _upMagnitude;
                _downPlayer1.forceMagnitude = _downMagnitude;
            }
        }
    }
}
