using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static float Level2TransitionTime;
    private PlayersUnit _unit1;
    private PlayersUnit _unit2;
    [SerializeField] private float minCameraSize;

    [SerializeField] private float startShakeDuration = 0.2f;
    private float shakeDuration;
    private float _offset;

    public float aspect;

    private Camera _mainCamera;
    private Transform _space;


    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
    private bool startShake;

    private Vector3 originalPos;
    private float timeToShake;

    public float minTimeToShake = 3;
    public float maxTimeToShake = 5;

    [SerializeField] private GameObject pauseText;
    private bool inPause = false;

    private float startCameraAcceleration = 1;
    private float cameraSpeed = 0;

    private bool firstGameFrame = true;

    private bool pauseInUse = false;

    private LightBlink _shipLight;

    private PlayersUnit _survived;
    private PlayersUnit _lost;

    private GameObject _pressAny;

    private float _maxCameraSize;
    private float _killTimer;
    private float _timesUp;
    private bool _killed;
    private float _killTransitionTime;
    private float _elapsed;

    private Vector2 pos1;
    private Vector2 pos2;
    private float _ropeLength;

    [SerializeField] private EscapeShip escapeShip;
    private float escapeShipSpeed = 1;

    private List<GameObject> endLines = new List<GameObject>();

    [SerializeField] private GameObject pressToPlayAgain;
    private bool restartPresed = false;
    private float endAlpha = 1;

    public static bool fedout = false;

    [SerializeField] private Button restartButton;

    private bool firstEndFrame = true;
    [SerializeField] private DoorLevel2 alarmDoors;

    public enum GameState
    {
        MovingToText,
        OnText,
        MovingDown,
        WaitingForKeys,
        Game,
        Level2Transition,
        Level2,
        EndTransition,
        End
    }

    public GameState state;

    // Start is called before the first frame update
    void Start()
    {
        _elapsed = 0;
        _killTransitionTime = 7;
        _killed = false;
        _maxCameraSize = 50;
        _killTimer = 0;
        _timesUp = 10;
        fedout = false;
        Level2TransitionTime = 6;
        _shipLight = transform.Find("ShipLight").GetComponent<LightBlink>();
        _space = GameObject.Find("earthandspace").transform;
        _mainCamera = Camera.main;
        _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);

        pressToPlayAgain.SetActive(false);
        restartButton.gameObject.SetActive(false);

        if (minCameraSize == 0)
        {
            minCameraSize = 10;
        }

        if (aspect == 0)
        {
            aspect = 9f / 16f;
        }

        _unit1 = Instantiate(Resources.Load<PlayersUnit>("PlayersUnit"));
        _unit1.name = "PlayersUnit1";
        _unit2 = Instantiate(Resources.Load<PlayersUnit>("PlayersUnit"));
        _unit2.name = "PlayersUnit2";
        _unit1.Init(
            new Vector2(7, 2), 1, 2,
            Resources.Load<Sprite>("rope-pink_crop"),
            this);

        _unit2.Init(
            new Vector2(-7, 2), 3, 4,
            Resources.Load<Sprite>("rope-blue_crop"),
            this);

        pauseText.SetActive(false);
        _ropeLength = _unit1.GetRopeLength();
        state = GameState.MovingToText;
        _mainCamera.transform.position = new Vector3(0, 565, -10);
        _pressAny = transform.Find("pressAny").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            _unit1.transform.position = new Vector2(_unit1.transform.position.x, 285);
            _unit2.transform.position = new Vector2(_unit2.transform.position.x, 285);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            _unit1.transform.position = new Vector2(-7, 374 + 110);
            _survived = _unit1;
            _lost = _unit2;
            _unit1.CutRope();
            state = GameState.Level2;
        }

        if (state == GameState.MovingToText)
        {
            if (Input.anyKeyDown)
            {
                _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, 531.5f,
                    _mainCamera.transform.position.z);
                state = GameState.OnText;
            }

            cameraSpeed = 5;
            _mainCamera.transform.position += (Vector3) Vector2.down * (Time.deltaTime * cameraSpeed);
            if (_mainCamera.transform.position.y <= 531.5f)
            {
                state = GameState.OnText;
            }

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }

        else if (state == GameState.OnText)
        {
            if (Input.anyKeyDown)
            {
                _pressAny.SetActive(false);
                state = GameState.MovingDown;
            }

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }
        else if (state == GameState.MovingDown)
        {
            if (Input.anyKeyDown)
            {
                state = GameState.WaitingForKeys;
            }

            cameraSpeed += startCameraAcceleration;
            cameraSpeed += startCameraAcceleration;
            if (_mainCamera.transform.position.y <= _unit1.transform.position.y + 20)
            {
                cameraSpeed = 5;

                Vector2 unit1Center = _unit1.GetUnitCenter();
                Vector2 unit2Center = _unit2.GetUnitCenter();

                float sizeX = Mathf.Abs(unit1Center.y - unit2Center.y) / 2 +
                              Mathf.Max(_unit1.GetRopeLength(), _unit2.GetRopeLength());
                float sizeY = (Mathf.Abs(unit1Center.x - unit2Center.x) / 2 +
                               Mathf.Max(_unit1.GetRopeLength(), _unit2.GetRopeLength())) * aspect;
                float size = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minCameraSize, Mathf.Infinity);

                _mainCamera.orthographicSize += (size - _mainCamera.orthographicSize) * Time.deltaTime;
                _mainCamera.transform.position +=
                    (new Vector3((unit1Center.x + unit2Center.x) / 2, (unit1Center.y + unit2Center.y) / 2, -10) -
                     _mainCamera.transform.position) * Time.deltaTime;

                if (Math.Abs(_mainCamera.orthographicSize - size) < 1
                    && Math.Abs(Vector3.Distance(_mainCamera.transform.position,
                        new Vector3((unit1Center.x + unit2Center.x) / 2, (unit1Center.y + unit2Center.y) / 2, -10))) <
                    1)
                {
                    state = GameState.WaitingForKeys;
                }
            }
            else
            {
                _mainCamera.transform.position += (Vector3) Vector2.down * (Time.deltaTime * cameraSpeed);
            }

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }

        else if (state == GameState.WaitingForKeys)
        {
            if (!_unit1.InHelp() && !_unit2.InHelp())
            {
                state = GameState.Game;
            }

            if (firstGameFrame)
            {
                firstGameFrame = false;
                shakeDuration = 0;
                timeToShake = 0;
            }

            if (Input.GetAxisRaw("Pause") != 0)
            {
                if (pauseInUse == false)
                {
                    // Call your event function here.
                    if (inPause)
                    {
                        Continue();
                    }
                    else
                    {
                        Pause();
                    }

                    pauseInUse = true;
                }
            }

            if (Input.GetAxisRaw("Pause") == 0)
            {
                pauseInUse = false;
            }

            if (inPause)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Time.timeScale = 1;
                    new SceneController().LoadGame();
                }
                return;
            }

            Vector2 unit1Center = _unit1.GetUnitCenter();
            Vector2 unit2Center = _unit2.GetUnitCenter();

            timeToShake -= Time.deltaTime;

            if (shakeDuration <= 0)
            {
                _mainCamera.transform.position =
                    new Vector3((unit1Center.x + unit2Center.x) / 2, (unit1Center.y + unit2Center.y) / 2, -10);

                if (timeToShake <= 0)
                {
                    timeToShake = Random.Range(minTimeToShake, maxTimeToShake);
                    shakeDuration = startShakeDuration;
                    originalPos = _mainCamera.transform.position;
                    shakeAmount = 0.7f * minCameraSize / (_mainCamera.orthographicSize);
                }
            }
            else
            {
                if (shakeDuration > 0)
                {
                    _mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
                    shakeDuration -= Time.deltaTime * decreaseFactor;
                }
                else
                {
                    _mainCamera.transform.localPosition = originalPos;
                }
            }

            float sizeX = Mathf.Abs(unit1Center.y - unit2Center.y) / 2 +
                          Mathf.Max(_unit1.GetRopeLength(), _unit2.GetRopeLength());
            float sizeY = (Mathf.Abs(unit1Center.x - unit2Center.x) / 2 +
                           Mathf.Max(_unit1.GetRopeLength(), _unit2.GetRopeLength())) * aspect;
            float size = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minCameraSize, Mathf.Infinity);
            _mainCamera.orthographicSize = size;

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }

        else if (state == GameState.Game)
        {
            if (firstGameFrame)
            {
                firstGameFrame = false;
                shakeDuration = 0;
                timeToShake = 0;
            }

            if (Input.GetAxisRaw("Pause") != 0)
            {
                if (pauseInUse == false)
                {
                    // Call your event function here.
                    if (inPause)
                    {
                        Continue();
                    }
                    else
                    {
                        Pause();
                    }

                    pauseInUse = true;
                }
            }

            if (Input.GetAxisRaw("Pause") == 0)
            {
                pauseInUse = false;
            }

            if (inPause)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Time.timeScale = 1;
                    new SceneController().LoadGame();
                }
                return;
            }

            bool transition = false;
            if (_killed)
            {
                transition = true;
                pos1 = _survived.GetPlayer1Pos();
                pos2 = _survived.GetPlayer2Pos();
            }
            else
            {
                pos1 = _unit1.GetUnitCenter();
                pos2 = _unit2.GetUnitCenter();
            }

            timeToShake -= Time.deltaTime;

            if (shakeDuration <= 0)
            {
                _mainCamera.transform.position =
                    new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, -10);

                if (timeToShake <= 0)
                {
                    timeToShake = Random.Range(minTimeToShake, maxTimeToShake);
                    shakeDuration = startShakeDuration;
                    originalPos = _mainCamera.transform.position;
                    shakeAmount = 0.7f * minCameraSize / (_mainCamera.orthographicSize);
                }
            }
            else
            {
                if (shakeDuration > 0)
                {
                    _mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
                    shakeDuration -= Time.deltaTime * decreaseFactor;
                }
                else
                {
                    _mainCamera.transform.localPosition = originalPos;
                }
            }

            if (transition)
            {
                SmoothTransition(pos1, pos2);
                transition = false;
            }
            else
            {
                CameraTrack(pos1, pos2);
            }

            UpdateKillTimer();

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }


        else if (state == GameState.Level2Transition)
        {
            _shipLight.StartBlink();
            if (_unit1.IsSurvived())
            {
                _survived = _unit1;
                _lost = _unit2;
            }
            else
            {
                _survived = _unit2;
                _lost = _unit1;
            }

            Vector2 player1Pos = _survived.GetPlayer1Pos();
            Vector2 player2Pos = _survived.GetPlayer2Pos();

            CameraMove(player1Pos, player2Pos);

            float sizeX = Mathf.Abs(player1Pos.y - player2Pos.y) / 2 +
                          Mathf.Max(_unit1.GetRopeLength(), _unit2.GetRopeLength());
            float sizeY = (Mathf.Abs(player1Pos.x - player2Pos.x) / 2 +
                           Mathf.Max(_unit1.GetRopeLength(), _unit2.GetRopeLength())) * aspect;
            float size = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minCameraSize, Mathf.Infinity);
            _mainCamera.orthographicSize += (size - _mainCamera.orthographicSize) * Time.deltaTime;
            _mainCamera.transform.position +=
                (new Vector3((player1Pos.x + player2Pos.x) / 2, (player1Pos.y + player2Pos.y) / 2, -10) -
                 _mainCamera.transform.position) * Time.deltaTime;
            cameraSpeed += startCameraAcceleration;

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }


        else if (state == GameState.Level2)
        {
            if (_lost.gameObject.activeSelf)
            {
                _lost.gameObject.SetActive(false);
            }

            if (firstGameFrame)
            {
                firstGameFrame = false;
                shakeDuration = 0;
                timeToShake = 0;
            }

            if (Input.GetAxisRaw("Pause") != 0)
            {
                if (pauseInUse == false)
                {
                    // Call your event function here.
                    if (inPause)
                    {
                        Continue();
                    }
                    else
                    {
                        Pause();
                    }

                    pauseInUse = true;
                }
            }

            if (Input.GetAxisRaw("Pause") == 0)
            {
                pauseInUse = false;
            }

            if (inPause)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Time.timeScale = 1;
                    new SceneController().LoadGame();
                }
                return;
            }

            Vector2 player1Pos = _survived.GetPlayer1Pos();
            Vector2 player2Pos = _survived.GetPlayer2Pos();

            if (player1Pos.y > 520 || player2Pos.y > 520)
            {
                state = GameState.EndTransition;
            }

            timeToShake -= Time.deltaTime;

            if (shakeDuration <= 0)
            {
                _mainCamera.transform.position =
                    new Vector3((player1Pos.x + player2Pos.x) / 2, (player1Pos.y + player2Pos.y) / 2, -10);

                if (timeToShake <= 0)
                {
                    timeToShake = Random.Range(minTimeToShake, maxTimeToShake);
                    shakeDuration = startShakeDuration;
                    originalPos = _mainCamera.transform.position;
                    shakeAmount = 0.7f * minCameraSize / (_mainCamera.orthographicSize);
                }
            }
            else
            {
                if (shakeDuration > 0)
                {
                    _mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
                    shakeDuration -= Time.deltaTime * decreaseFactor;
                }
                else
                {
                    _mainCamera.transform.localPosition = originalPos;
                }
            }

            CameraTrack(player1Pos, player2Pos);

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }

        else if (state == GameState.EndTransition)
        {
            // escapeShip.transform.position += (Vector3) Vector2.up * escapeShipSpeed;
            var escapeShipPosition = escapeShip.transform.position;
            var cameraPosition = _mainCamera.transform.position;

            if (((Vector2) (cameraPosition - (escapeShipPosition - new Vector3(0, 10, 0)))).magnitude <= 1)
            {
                state = GameState.End;
            }

            _mainCamera.transform.position += (Vector3) new Vector2(escapeShipPosition.x - cameraPosition.x,
                                                  (escapeShipPosition.y - 10) - cameraPosition.y) *
                                              (Time.deltaTime);
            _mainCamera.orthographicSize += (40 - _mainCamera.orthographicSize) * Time.deltaTime;

            _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
        }


        else if (state == GameState.End)
        {
            if (firstEndFrame)
            {
                firstEndFrame = false;
                _shipLight.StopBlink();
                transform.Find("ShipLight").GetComponent<Light2D>().color = Color.white;
                transform.Find("ShipLight").GetComponent<Light2D>().intensity = 0.4f;
                Destroy(_survived.gameObject);
                alarmDoors.TurnOffAlarm();
                escapeShip.StartMoving();
            }

            if (escapeShip.transform.position.y >= 650)
            {
                if (_space != null)
                {
                    Destroy(_space.gameObject);
                    _space = null;
                }

                foreach (var t in endLines)
                {
                    Destroy(t.gameObject);
                }

                endLines.Clear();
                for (int i = 0; i < 10; i++)
                {
                    endLines.Add(new GameObject());
                    endLines[endLines.Count - 1].AddComponent<SpriteRenderer>();
                    endLines[endLines.Count - 1].GetComponent<SpriteRenderer>().sprite =
                        Resources.Load<Sprite>("end_line" + (int) (Random.Range(1, 4)));
                    endLines[endLines.Count - 1].transform.position =
                        new Vector2(_mainCamera.transform.position.x + Random.Range(-70, 70),
                            _mainCamera.transform.position.y + Random.Range(-15, 15));
                }

                escapeShip.transform.position =
                    new Vector2(_mainCamera.transform.position.x + Random.Range(-0.2f, 0.2f),
                        escapeShip.transform.position.y);
                pressToPlayAgain.SetActive(true);
                if (Input.anyKeyDown)
                {
                    restartPresed = true;
                    fedout = true;
                }

                if (restartPresed)
                {
                    // var esColor = escapeShip.GetComponent<SpriteRenderer>().color;
                    // escapeShip.GetComponent<SpriteRenderer>().color =
                    //     new Color(esColor.r, esColor.g, esColor.b, endAlpha);
                    // var textColor = pressToPlayAgain.GetComponent<SpriteRenderer>().color;
                    // pressToPlayAgain.GetComponent<SpriteRenderer>().color =
                    //     new Color(textColor.r, textColor.g, textColor.b, endAlpha);
                    // endAlpha -= Time.deltaTime;
                    // if (endAlpha <= 0)
                    // {
                    //     new SceneController().LoadGame();
                    // }
                    new SceneController().LoadGame();
                }
            }
            else
            {
                escapeShip.transform.position += (Vector3) Vector2.up * escapeShipSpeed;
                escapeShip.transform.position = new Vector2(escapeShip.transform.position.x,
                    escapeShip.transform.position.y);
                _mainCamera.transform.position =
                    new Vector3(escapeShip.transform.position.x,
                        escapeShip.transform.position.y - 10,
                        _mainCamera.transform.position.z);
                _space.position = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.y);
            }
        }
    }

    private void Pause()
    {
        inPause = true;
        Time.timeScale = 0;
        pauseText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    private void Continue()
    {
        inPause = false;
        Time.timeScale = 1;
        pauseText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    public void ResetGame()
    {
        Time.timeScale = 1;
        new SceneController().LoadGame();
    }

    private void KillUnit()
    {
        if (_unit1.GetPlayer1Pos().y < _unit2.GetPlayer1Pos().y)
        {
            _unit2.IsSurvived();
            _survived = _unit2;
            _lost = _unit1;
            StartCoroutine(Kill(_unit1));
        }
        else
        {
            _unit1.IsSurvived();
            _survived = _unit1;
            _lost = _unit2;
            StartCoroutine(Kill(_unit2));
        }
    }

    private void UpdateKillTimer()
    {
        if (_mainCamera.orthographicSize > _maxCameraSize)
        {
            _killTimer += Time.deltaTime;
            if (_killTimer > _timesUp && !_killed)
            {
                _killed = true;
                KillUnit();
            }
        }
        else
        {
            _killTimer = 0;
        }
    }

    private void CameraMove(Vector2 pos1, Vector2 pos2)
    {
        float sizeX = Mathf.Abs(pos1.y - pos2.y) / 2 +
                      Mathf.Max(_ropeLength, _ropeLength);
        float sizeY = (Mathf.Abs(pos1.x - pos2.x) / 2 +
                       Mathf.Max(_ropeLength, _ropeLength)) * aspect;
        float size = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minCameraSize, Mathf.Infinity);
        _mainCamera.orthographicSize += (size - _mainCamera.orthographicSize) * Time.deltaTime;
        _mainCamera.transform.position +=
            (new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, -10) - _mainCamera.transform.position) *
            Time.deltaTime;
    }

    private void CameraTrack(Vector2 pos1, Vector2 pos2)
    {
        float sizeX = Mathf.Abs(pos1.y - pos2.y) / 2 +
                      Mathf.Max(_ropeLength, _ropeLength);
        float sizeY = (Mathf.Abs(pos1.x - pos2.x) / 2 +
                       Mathf.Max(_ropeLength, _ropeLength)) * aspect;
        float size = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minCameraSize, Mathf.Infinity);
        _mainCamera.orthographicSize = size;
    }

    IEnumerator Kill(PlayersUnit unit)
    {
        yield return new WaitForSeconds(2);
        unit.transform.gameObject.SetActive(false);
    }

    private void SmoothTransition(Vector2 pos1, Vector2 pos2)
    {
        if (_elapsed < _killTransitionTime)
        {
            CameraMove(pos1, pos2);
        }
        else
        {
            _elapsed += Time.deltaTime;
        }
    }
}