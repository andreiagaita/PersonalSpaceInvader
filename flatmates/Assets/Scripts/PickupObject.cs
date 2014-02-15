using UnityEngine;
using System.Collections;

public class PickupObject
{
    public string Name { get; set; }
    public PlayerID Owner { get; set; }
    public PlayerID Holder { get; set; }

    public PickupObject(string name, PlayerID owner, PlayerID holder = PlayerID.None)
    {
        Name = name;
        Owner = owner;
        Holder = holder;
    }
}
