using UnityEngine;
using System;
using System.Collections;

public class ProjectilePowerup : Projectile
{




    void Start()
    {
        base.Start();
        spinningVector = Vector3.up;
    }

    protected override void handleCollisions(Collider collidedWith)
    {

        //check out with what kind of object the missile collided

        if (collidedWith.CompareTag("bound")) //if collided with bound remove missile from the game, since out of bounds
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnProjectilePowerupToBoundCollision);
        }


        if (collidedWith.CompareTag("segment")) //if the missile collided with a (cube-) segment
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnProjectilePowerupPickedUp);
        }


        if (collidedWith.CompareTag("missile")) //remove the missile once it collided with another missile
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnProjectilePowerupToProjectileCollision);
        }

    }


}
