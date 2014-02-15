using System;
using System.Collections;
using UnityEngine;

public class PickupObject
{
    public Guid GUID { get; set; }
    public string Name { get; set; }
    public PlayerID Owner { get; set; }
    public PlayerID Holder { get; set; }

    public PickupObject(string name, PlayerID owner, PlayerID holder = PlayerID.None)
    {
        Name = name;
        Owner = owner;
        Holder = holder;
        GUID = Guid.NewGuid();
    }
}
