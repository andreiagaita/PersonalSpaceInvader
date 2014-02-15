using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerID
{
    None,
    Game,
    Player1,
    Player2,
    Player3,
    Player4,
    Spectator
}

public class PlayerInfo
{
    public PlayerID ID { get; set; }
    public string Name { get; set; }
    public Vector3 Position { get; set; }
    public Color Color { get; set; }
    public int Score { get; set; }
    public List<PickupObject> ObjectsOwned { get; set; }
    public List<PickupObject> ObjectsPicked { get; set; }

    public PlayerInfo(PlayerID id, string name, Vector3 position, Color color, int score)
    {
        ID = id;
        Name = name;
        Position = position;
        Color = color;
        Score = score;

        ObjectsOwned = new List<PickupObject>();
        ObjectsPicked = new List<PickupObject>();
    }
}
