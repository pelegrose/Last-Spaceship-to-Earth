using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class RevolvingLight : MonoBehaviour
{
    private Light2D _light;
    [SerializeField] private float _speed;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
//        _speed = 120f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,_speed * Time.deltaTime);
    }
}