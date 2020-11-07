using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationFactor;

    public Rigidbody2D obstacle;

    private AudioSource _source;

    private int _hitCounter;
    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _hitCounter = 0;
        float angle = Random.Range(0, 359.9f);
        obstacle.AddRelativeForce(acceleration * Time.deltaTime * DegreeToVector2(angle));
        obstacle.AddTorque(rotationFactor * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }

    private Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private float VectorToDegree(Vector2 vector)
    {
        var degree = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        return Mathf.Repeat(degree, 360f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _hitCounter += 1;
        if (_hitCounter == 4)
        {
            _source.Play();
            _hitCounter = 0;
        }
//        if (transform.CompareTag("SmallOb"))
//        {
//            _hitCounter += 1;
//            if (_hitCounter == 4)
//            {
//                _source.Play();
//                _hitCounter = 0;
//            }
//        }
//        else
//        {
//            _source.Play();
//        }
    }
}
