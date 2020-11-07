using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersUnit : MonoBehaviour
{
    private Vector2 _startPosition;
    [SerializeField] private float ropeLength;
    [SerializeField] private int numOfNodesToIgnore;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    private List<RopeNode> _nodes;
    private GameObject _ropeNodesContainer;

    private Dictionary<string, KeyCode> _player1Arrows;
    private Dictionary<string, KeyCode> _player2Arrows;

    private bool _survived;

    private Sprite _color;

    private GameManager.GameState _state;

    // Start is called before the first frame update
    void Start()
    {
        if (numOfNodesToIgnore == 0)
        {
            numOfNodesToIgnore = 5;
        }

        _survived = false;
        InitRope();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Vector2 GetUnitCenter()
    {
        Vector2 player1Pos = player1.transform.position;
        Vector2 player2Pos = player2.transform.position;
        Vector2 position = (player1Pos + player2Pos) / 2;
        return position;
    }

    public void Init(Vector2 startPosition, int player1Number, int player2Number, Sprite ropeColor, 
        GameManager gameManager)
    {
        _startPosition = startPosition;
        player1.Init(player1Number, this, gameManager);
        player2.Init(player2Number, this, gameManager);
        player1.transform.position = new Vector2(_startPosition.x - ropeLength / 2, _startPosition.y);
        player2.transform.position = new Vector2(_startPosition.x + ropeLength / 2, _startPosition.y);
        _color = ropeColor;
        if (player1Number == 1)
        {
            transform.Find("Player1").transform.Find("UnitC1").gameObject.layer = LayerMask.NameToLayer("Unit1");
            transform.Find("Player2").transform.Find("UnitC2").gameObject.layer = LayerMask.NameToLayer("Unit1");
        }
        else
        {
            transform.Find("Player1").transform.Find("UnitC1").gameObject.layer  = LayerMask.NameToLayer("Unit2");
            transform.Find("Player2").transform.Find("UnitC2").gameObject.layer  = LayerMask.NameToLayer("Unit2");
        }
    }

    private void InitRope()
    {
        _ropeNodesContainer = new GameObject("RopeNodesContainer");
        _ropeNodesContainer.transform.SetParent(transform);

        _nodes = new List<RopeNode>();

        player2.gameObject.AddComponent<HingeJoint2D>();

        float nodeWidth = Resources.Load<RopeNode>("RopeNode").transform.localScale.x;
        for (float pos = player1.transform.position.x + 0.5f * nodeWidth;
            pos < player2.transform.position.x - 0.5f * nodeWidth;
            pos += nodeWidth)
        {
            RopeNode node = Instantiate(Resources.Load<RopeNode>("RopeNode"),
                _ropeNodesContainer.transform, true);
            node.transform.position = new Vector2(pos, _startPosition.y);
            node.Init(_color);
            node.ConnectBody(_nodes.Count == 0
                ? player1.GetComponent<Rigidbody2D>()
                : _nodes[_nodes.Count - 1].GetComponent<Rigidbody2D>());

            node.GetComponent<HingeJoint2D>().anchor = new Vector2(-0.5f, 0);
            node.GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
            _nodes.Add(node);
        }

        player2.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        player2.GetComponent<HingeJoint2D>().connectedBody =
            _nodes[_nodes.Count - 1].GetComponent<Rigidbody2D>();
        player2.GetComponent<HingeJoint2D>().anchor = Vector2.zero;
        player2.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0.5f, 0);

        for (int i = 0; i < numOfNodesToIgnore; i++)
        {
            Physics2D.IgnoreCollision(player1.GetComponent<PolygonCollider2D>(),
                _nodes[i].GetComponent<BoxCollider2D>());
        }

        for (int i = _nodes.Count - numOfNodesToIgnore; i < _nodes.Count; i++)
        {
            Physics2D.IgnoreCollision(player2.GetComponent<PolygonCollider2D>(),
                _nodes[i].GetComponent<BoxCollider2D>());
        }
        // Arms don't collide with the rope anymore.
        List<Collider2D> colliders = new List<Collider2D>
        {
            player1.GetComponent<CircleCollider2D>(),
            player2.GetComponent<CircleCollider2D>(),
            player1.GetComponent<PolygonCollider2D>(),
            player2.GetComponent<PolygonCollider2D>(),
            player1.transform.Find("ArmL").GetComponent<PolygonCollider2D>(),
            player1.transform.Find("ArmR").GetComponent<PolygonCollider2D>(),
            player2.transform.Find("ArmL").GetComponent<PolygonCollider2D>(),
            player2.transform.Find("ArmR").GetComponent<PolygonCollider2D>()
        };

        foreach (var c in colliders)
        {
            foreach (var node in _nodes)
            {
                Physics2D.IgnoreCollision(c, node.GetComponent<BoxCollider2D>());
            }
        }
    }

    public float GetRopeLength()
    {
        return ropeLength;
    }

    public void NotifyUnitSurvived()
    {
        _survived = true;
    }

    public bool IsSurvived()
    {
        return _survived;
    }

    public void CutRope()
    {
        int middle = _nodes.Count / 2;
        Destroy(_nodes[middle].GetComponent<HingeJoint2D>());
    }

    public Vector2 GetPlayer1Pos()
    {
        return player1.transform.position;
    }
    
    public Vector2 GetPlayer2Pos()
    {
        return player2.transform.position;
    }

    public bool InHelp()
    {
        return player1.InHelp() || player2.InHelp();
    }
}