using System;
using System.Collections;
using UnityEngine;

public class PickupObject
{
    public Guid GUID { get; set; }
    public string Name { get; set; }
    public int Owner { get; set; }
    public int Holder { get; set; }

    public PickupObject(string name, int owner, int holder = 0)
    {
        Name = name;
        Owner = owner;
        Holder = holder;
        GUID = Guid.NewGuid();
    }
}
