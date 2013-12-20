//UnityEditorInternal.InternalEditorUtility.tags <-- accesses available tags in unity
using UnityEngine;
using System;
using System.Collections;

public class Projectile : MonoBehaviour
{
    /*
     the general missile class which utilised to realize both missiles and powerups
     */


    public float speedModifier = 2.0f; //determins how fast the missile moves by multiplying this value with a forward vector and deltaTime
    public GameObject projectileMesh;

    protected Vector3 spinningVector;

    public int locationSpawnedAt;

    public EventManager _eventManager;

    // Use this for initialization
    protected void Start()
    {
        _eventManager = EventManager.getInstance();
        setUpOrientation();
    }

    // Update is called once per frame
    void Update()
    {

        move();
    }

    private void setUpOrientation()
    {
        //orient missile so that they move toward the middle when +z is their forward direction
        transform.LookAt(new Vector3(0, transform.position.y, 0));
    }

    protected virtual void move()
    {
        /*translate the object using a vector-multiplication with a forward-vector (0,0,1) - multiplied with the speedModifier to make it faster/slower
        translation occurs in object space (Space.Self) */
        transform.Translate(Vector3.forward * speedModifier * Time.deltaTime, Space.Self);
        spinAround(spinningVector);

    }

    private void spinAround(Vector3 vector)
    {
        projectileMesh.transform.Rotate(vector, 80.0f * (speedModifier * speedModifier * speedModifier) * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider collidedWith)
    {
        handleCollisions(collidedWith);
        
        //_eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnProjectileCollision);

    }

    protected virtual void handleCollisions(Collider collidedWith)
    {


    }


    public void disableProjectile()
    {
        //quirk so that audio can still be played while object visually and functionally removed
        
        projectileMesh.renderer.enabled = false;
        this.collider.enabled = false;

    }



}
