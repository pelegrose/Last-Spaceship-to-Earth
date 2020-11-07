using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class LightBlink : MonoBehaviour
{
    private Light2D _light;
    private bool _off;
    private float _elapsed;
    [SerializeField] private float _time;
    [SerializeField] private float _minIntensity;
    [SerializeField] private float _maxIntensity;
    private bool _blink;
    [SerializeField] private bool _global = false;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
        _off = true;
        _elapsed = 0;
        if (_global)
        {
            _blink = false;
        }
        else
        {
            _blink = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_blink)
        {
            if (_elapsed < _time)
            {
                if (_off)
                {
                    _light.intensity = Mathf.Lerp(_maxIntensity, _minIntensity, (_elapsed / _time));
                    _elapsed += Time.deltaTime;    
                }
                else
                {
                    _light.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, (_elapsed / _time));
                    _elapsed += Time.deltaTime;   
                }
            }
            else
            {
                _elapsed = 0;
                _off = !_off;
            }    
        }
    }

    public void StartBlink()
    {
        _light.color = Color.red;
        _blink = true;
    }

    public void StopBlink()
    {
        _blink = false;
    }
}
