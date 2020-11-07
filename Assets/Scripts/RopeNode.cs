using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{
    private HingeJoint2D _hingeJoint2D;
    private Sprite _color;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = _color;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(Sprite color)
    {
        _hingeJoint2D = GetComponent<HingeJoint2D>();
        _color = color;
    }

    public void ConnectBody(Rigidbody2D body2D)
    {
        _hingeJoint2D.connectedBody = body2D;
    }
}