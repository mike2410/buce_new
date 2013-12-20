using UnityEngine;
using System.Collections;
using System;

public class PowerupEventArgs : EventArgs
{

    public Type typeOfPowerup;

    public PowerupEventArgs(Type typeOfProjectile)
    {
        this.typeOfPowerup = typeOfProjectile;
    }
}
