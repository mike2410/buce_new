using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SpawningEventArgs : EventArgs
{

    public int spawningPosition;
    public float projectileSpeed;

    public SpawningEventArgs(int spawningPosition, float projectileSpeed)
    {
        this.spawningPosition = spawningPosition;
        this.projectileSpeed = projectileSpeed;
    }
}