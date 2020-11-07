using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static bool triggered;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationFactor;
    [SerializeField] private float maxSpeed;

    private Rigidbody2D _body2D;
    private SpriteRenderer _body;
    private SpriteRenderer _leftHand;
    private SpriteRenderer _rightHand;
    private SpriteRenderer _jetPack;
    private AudioSource _source;

    private int _playerNumber;

    private PlayersUnit _unit;

    private PlayerSpriteGenerator _generator;

    private GameManager _gameManager;

    private Animator _animator;
    private Light2D _right;
    private Light2D _left;

    private Light2D rightLight;
    private Light2D leftLight;

    private bool keysHelp = true;

    private float _blinkTimer;
    private bool blink = true;

    public enum FireAnimationState
    {
        Left,
        Right,
        Both,
        None
    }

    private FireAnimationState _animationState;

    private bool fadeoutKeysHelp = false;

    // Start is called before the first frame update
    void Start()
    {
        _animationState = FireAnimationState.None;
        triggered = false;
        _source = GetComponent<AudioSource>();
        _body2D = GetComponent<Rigidbody2D>();
        _animator = transform.Find("backpack").GetComponent<Animator>();
        _right = transform.Find("right").GetComponent<Light2D>();
        _left = transform.Find("left").GetComponent<Light2D>();
        _body = GetComponent<SpriteRenderer>();
        _leftHand = transform.Find("ArmL").GetComponent<SpriteRenderer>();
        _rightHand = transform.Find("ArmR").GetComponent<SpriteRenderer>();
//        jetPack = transform.Find("JetPack").GetComponent<SpriteRenderer>();

        _body.sprite = _generator.GetBodySprite();
        _leftHand.sprite = _generator.GetLeftArmSprite();
        _rightHand.sprite = _generator.GetRightArmSprite();
//        jetPack.sprite = _generator.GetJetPackSprite();

        transform.Find("keys").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("newkeys1");

        rightLight = transform.Find("RightLight").GetComponent<Light2D>();
        leftLight = transform.Find("LeftLight").GetComponent<Light2D>();
        leftLight.gameObject.SetActive(false);
        rightLight.gameObject.SetActive(false);

        if (acceleration == 0)
        {
            acceleration = 1;
        }

        if (rotationFactor == 0)
        {
            rotationFactor = 1;
        }

        if (maxSpeed == 0)
        {
            maxSpeed = 300;
        }

        _blinkTimer = Random.Range(3, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.state == GameManager.GameState.Game || _gameManager.state == GameManager.GameState.Level2)
        {
            if (fadeoutKeysHelp)
            {
                leftLight.intensity -= Time.deltaTime;
                rightLight.intensity -= Time.deltaTime;
                var kColor = transform.Find("keys").GetComponent<SpriteRenderer>().color;
                transform.Find("keys").GetComponent<SpriteRenderer>().color =
                    new Color(kColor.r, kColor.g, kColor.b, kColor.a - Time.deltaTime);
                transform.Find("KeysLight").GetComponent<Light2D>().intensity -= Time.deltaTime;
                if (kColor.a <= 0)
                {
                    leftLight.gameObject.SetActive(false);
                    rightLight.gameObject.SetActive(false);
                    transform.Find("keys").gameObject.SetActive(false);
                    transform.Find("KeysLight").gameObject.SetActive(false);
                    fadeoutKeysHelp = false;
                }
            }

            _blinkTimer -= Time.deltaTime;
            if (_blinkTimer < 0 && blink)
            {
                StartCoroutine(ChangeFace("blink"));
            }

            float left = Input.GetAxis("Left" + _playerNumber);
            float right = Input.GetAxis("Right" + _playerNumber);

            _body2D.AddRelativeForce(Vector2.up * (acceleration * Time.deltaTime * Mathf.Min(left, right)),
                ForceMode2D.Impulse);
            _body2D.AddTorque(-rotationFactor * Time.deltaTime * (right - left));


            if (right > 0 && left > 0)
            {
                if (_animationState != FireAnimationState.Both)
                {
                    _animator.SetTrigger("Both");
                }

                _animationState = FireAnimationState.Both;

                _right.intensity = 1.5f;
                _left.intensity = 1.5f;
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }
            else if (right > 0)
            {
                if (_animationState != FireAnimationState.Right)
                {
                    _animator.SetTrigger("Right");
                }

                _animationState = FireAnimationState.Right;

                _left.intensity = 0;
                _right.intensity = 1.5f;
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }
            else if (left > 0)
            {
                if (_animationState != FireAnimationState.Left)
                {
                    _animator.SetTrigger("Left");
                }

                _animationState = FireAnimationState.Left;

                _right.intensity = 0;
                _left.intensity = 1.5f;
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }
            else
            {
                if (_animationState != FireAnimationState.None)
                {
                    _animator.SetTrigger("None");
                }

                _animationState = FireAnimationState.None;

                _right.intensity = 0;
                _left.intensity = 0;
                if (_source.isPlaying)
                {
                    _source.Stop();
                }
            }


            Vector2 currentVelocity = _body2D.velocity;
            float speed = Mathf.Clamp(currentVelocity.magnitude, 0, maxSpeed);
            Vector2 newVelocity = currentVelocity.normalized * speed;
            _body2D.velocity = newVelocity;
        }

        if (_gameManager.state == GameManager.GameState.WaitingForKeys)
        {
            float left = Input.GetAxis("Left" + _playerNumber);
            float right = Input.GetAxis("Right" + _playerNumber);

            if (keysHelp)
            {
                leftLight.gameObject.SetActive(left > 0);
                rightLight.gameObject.SetActive(right > 0);
            }

            if (right > 0 && left > 0)
            {
                keysHelp = false;
                StartCoroutine(TurnOffKeysHelp());
                if (_animationState != FireAnimationState.Both)
                {
                    _animator.SetTrigger("Both");
                }

                _animationState = FireAnimationState.Both;

                _right.intensity = 1.5f;
                _left.intensity = 1.5f;
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }
            else if (right > 0)
            {
                if (_animationState != FireAnimationState.Right)
                {
                    _animator.SetTrigger("Right");
                }

                _animationState = FireAnimationState.Right;

                _left.intensity = 0;
                _right.intensity = 1.5f;
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }
            else if (left > 0)
            {
                if (_animationState != FireAnimationState.Left)
                {
                    _animator.SetTrigger("Left");
                }

                _animationState = FireAnimationState.Left;

                _right.intensity = 0;
                _left.intensity = 1.5f;
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }
            else
            {
                if (_animationState != FireAnimationState.None)
                {
                    _animator.SetTrigger("None");
                }

                _animationState = FireAnimationState.None;

                _right.intensity = 0;
                _left.intensity = 0;
                if (_source.isPlaying)
                {
                    _source.Stop();
                }
            }
        }
    }

    public void Init(int playerNumber, PlayersUnit unit, GameManager gameManager)
    {
        _playerNumber = playerNumber;
        _unit = unit;
        _generator = new PlayerSpriteGenerator(playerNumber);
        _gameManager = gameManager;
    }

    public void NotifyPlayerSurvived()
    {
        _unit.NotifyUnitSurvived();
    }

    public int GetPlayerNum()
    {
        return _playerNumber;
    }

    public void BeginLevel2()
    {
        _unit.CutRope();
        _gameManager.state = GameManager.GameState.Level2Transition;
        triggered = true;
        StartCoroutine(ChangeGameState());
    }

    IEnumerator ChangeGameState()
    {
        yield return new WaitForSeconds(GameManager.Level2TransitionTime);
        _gameManager.state = GameManager.GameState.Level2;
    }

    public bool InHelp()
    {
        return keysHelp;
    }

    IEnumerator TurnOffKeysHelp()
    {
        yield return new WaitForSeconds(1);
        fadeoutKeysHelp = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Hand") && blink)
        {
            StartCoroutine(ChangeFace("face1"));
        }
    }

    IEnumerator ChangeFace(string face)
    {
        blink = false;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("faces/character" + _playerNumber + "/" + face + "");
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("character" + _playerNumber + "/character" + _playerNumber + "_body");
        _blinkTimer = Random.Range(3, 5);
        blink = true;
    }
}