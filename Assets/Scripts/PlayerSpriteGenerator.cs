using UnityEngine;

public class PlayerSpriteGenerator
{
    private readonly int _playerNumber;

    public PlayerSpriteGenerator(int playerNumber)
    {
        _playerNumber = playerNumber;
    }

    public Sprite GetBodySprite()
    {
        return Resources.Load<Sprite>("character" + _playerNumber + "/character" + _playerNumber + "_body");
    }

    public Sprite GetLeftArmSprite()
    {
        return Resources.Load<Sprite>("character" + _playerNumber + "/character" + _playerNumber + "_right_hand");
    }

    public Sprite GetRightArmSprite()
    {
        return Resources.Load<Sprite>("character" + _playerNumber + "/character" + _playerNumber + "_left_hand");
    }

//    public Sprite GetJetPackSprite()
//    {
//        return Resources.Load<Sprite>("character1/backpack");
//    }
}