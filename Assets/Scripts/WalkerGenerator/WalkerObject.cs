using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerObject
{
    public Vector2 position;

    public Vector2 direction;

    public float chanceToChange;


    public WalkerObject(Vector2 position, Vector2 direction, float chanceToChange)
    {
        this.position = position;
        this.direction = direction;
        this.chanceToChange = chanceToChange;
    }
}
