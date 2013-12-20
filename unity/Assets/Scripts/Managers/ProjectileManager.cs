using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ProjectileManager : MonoBehaviour
{
    /*
	
    can be launched
        "A Projectile can be launched at random times at 4 different locations."
    can be traveling
        "A Projectile travels along one axis, heading towards the cube."
    can die
        "A Projectile dies, if it didn't hit either one of the Cube Segments on its (the Proectiles) axis OR if it hits another Projectile.
    can hit a [Cube Segment]
        ...
		
    */

    public bool spawnProjectiles = true;
    public List<GameObject> activeMissiles = new List<GameObject>(); //a list that registers which projectiles are currently active	
    public List<GameObject> activePowerups = new List<GameObject>(); //a list that registers which projectiles are currently active	

    public Transform[] startPositions; //the positions from where projectiles are launched from
    public GameObject missile; //the type of missile/missile (= prefab in this case) that gets launced
    public GameObject[] PowerupProjectiles; //the type of powerup possible to spawn from this object


    public int[] SpawningSchedule;

    public int[] SpawningScheduleSpeeds;

    public int[] SpawningScheduleIntervals;

    public bool scheduledSpawning = true;

    public int schedulePosition = 0;


    public int probabilityOfPowerupSpawning = 20;

    public float spawnInterval = 5.0f; //the interval within which projectiles are spawned; "spawn a missile every spawnInterval seconds".
    public float rndSpeedLower = 1.0f; //lower amount of random speed of to the launched missile
    public float rndSpeedUpper = 5.0f; //upper amount of random speed of the launched missile

    public float rndSpawnIntervalLower = 0.5f;
    public float rndSpawnIntervalUpper = 3.0f;

    // "launch a missile every 5 (=spawnInterval) seconds, with a speed between 1.0f (=rndSpeedLower) and 5.0f (=rndSppedUpper)



    private uint projectilesCounter = 0; //keeps track of how many projectiles have been instantiated

    private EventManager _eventManager;


    void Start()
    {
        _eventManager = EventManager.getInstance();

        _eventManager.addListener(ProjectileManager_OnProjectileToBoundCollision, EventManager.eventName.OnMissileToBoundCollision);
        _eventManager.addListener(ProjectileManager_OnProjectileToBoundCollision, EventManager.eventName.OnProjectilePowerupToBoundCollision);
        _eventManager.addListener(ProjectileManager_OnMissileToSegmentCollision, EventManager.eventName.OnMissileToSegmentCollision);
        _eventManager.addListener(ProjectileManager_OnProjectileToProjectile, EventManager.eventName.OnMissileToProjectileCollision);
        _eventManager.addListener(ProjectileManager_OnProjectileTrapped, EventManager.eventName.OnProjectileTrapped);
        _eventManager.addListener(ProjectileManager_OnProjectilePowerupPickedUp, EventManager.eventName.OnProjectilePowerupPickedUp);
    }

    private void ProjectileManager_OnProjectilePowerupPickedUp(GameObject g, EventArgs e)
    {
        removeProjectileFromGame(g);
    }




    private void ProjectileManager_OnProjectileTrapped(GameObject g, EventArgs e)
    {
        if (g.GetComponent<Projectile>().GetType() == typeof(ProjectilePowerup))
        {
            Debug.Log("powerup trapped.");
            _eventManager.dispatchEvent(g, e, EventManager.eventName.OnProjectilePowerupPickedUp);

        }
        else {
            removeProjectileFromGame(g);
        }

       
    }

    private void ProjectileManager_OnProjectileToBoundCollision(GameObject g, EventArgs e)
    {
        removeProjectileFromGame(g);
    }


    private void ProjectileManager_OnMissileToSegmentCollision(GameObject g, EventArgs e)
    {



        if (g.GetComponent<Projectile>().GetType() == typeof(ProjectileMissile))
        {
            activeMissiles.Remove(g);
            disableAllMissiles();
            Destroy(g);   
        }

           
        
        
        
        //disableAllMissiles();
        //removeProjectileFromGame(g);
    }

    private void ProjectileManager_OnProjectileToProjectile(GameObject g, EventArgs e)
    {
        removeProjectileFromGame(g);
    }

    private void removeProjectileFromGame(GameObject g)
    {
        if (g.GetComponent<Projectile>().GetType() == typeof(ProjectilePowerup))
        {
            activePowerups.Remove(g);
        }

        if (g.GetComponent<Projectile>().GetType() == typeof(ProjectileMissile))
        { 
            activeMissiles.Remove(g);    
        }

        Destroy(g);
    }


    public IEnumerator spawnProjectile()
    {

        //if (schedulePosition >= SpawningSchedule.Length)
        //{
        //    scheduledSpawning = false;
        //}
        //else
        //{
        //    scheduledSpawning = true;
        //}

        while (spawnProjectiles) //TODO why do i actually need this loop?
        {
            //select a position to launch missile from (at random)
            if (!scheduledSpawning || schedulePosition >= SpawningSchedule.Length)
            {
                spawnRandomProjectile();
            }
            else
            {
                spawnScheduledProjectile();
            }





            yield return new WaitForSeconds(spawnInterval); //launch another missile when interval-time has passed
        }
    }

    private void spawnRandomProjectile()
    {
        if (!shouldSpawnPowerup(probabilityOfPowerupSpawning))
        {
            spawnRandomMissile();
            
        }
        else
        {
            spawnRandomPowerup();
        }


    }

    public void spawnRandomPowerup() //only public for debugging reasons
    {
        int randomPowerupIndex = (int)UnityEngine.Random.Range(0, PowerupProjectiles.Length);

        GameObject instantiatedProjectile = spawnProjectileAtRandomLocation(PowerupProjectiles[randomPowerupIndex]);
        instantiatedProjectile.GetComponent<Projectile>().speedModifier = UnityEngine.Random.Range(rndSpeedLower, rndSpeedUpper);
        projectilesCounter++;

        //register instantiated missile
        activePowerups.Add(instantiatedProjectile); //TODO: create pendant to registerNewInstnace() or modify that function based on missile-type?


    }

    private GameObject spawnProjectileAtRandomLocation(GameObject typeOfProjectile)
    {
        int location = (int)UnityEngine.Random.Range(0, 4);

        GameObject instantiatedProjectile = Instantiate(typeOfProjectile, startPositions[location].transform.position, startPositions[location].transform.rotation) as GameObject;
        instantiatedProjectile.GetComponent<Projectile>().locationSpawnedAt = location;
        instantiatedProjectile.name = "msh_" + typeOfProjectile.GetType().Name;
        return instantiatedProjectile;

    }


    public void spawnRandomMissile() //only public for debugging reasons
    {
        // spawnInterval = UnityEngine.Random.Range(rndSpawnIntervalLower, rndSpawnIntervalUpper);

        GameObject instantiatedProjectile = spawnProjectileAtRandomLocation(missile);

        _eventManager.dispatchEvent(this.gameObject, new SpawningEventArgs(instantiatedProjectile.GetComponent<Projectile>().locationSpawnedAt, instantiatedProjectile.GetComponent<Projectile>().speedModifier), EventManager.eventName.OnMissileSpawned);
        instantiatedProjectile.GetComponent<Projectile>().speedModifier = UnityEngine.Random.Range(rndSpeedLower, rndSpeedUpper);
        projectilesCounter++;

        //register instantiated missile
        activeMissiles.Add(instantiatedProjectile);
    }

    private void spawnScheduledProjectile()
    {
        int location = SpawningSchedule[schedulePosition];
        spawnInterval = SpawningScheduleIntervals[schedulePosition];

        GameObject instantiatedProjectile = Instantiate(missile, startPositions[location].transform.position, startPositions[location].transform.rotation) as GameObject;
        instantiatedProjectile.name = "msh_instantiatedProjectile_" + projectilesCounter.ToString();
        instantiatedProjectile.GetComponent<Projectile>().speedModifier = SpawningScheduleSpeeds[schedulePosition];
        projectilesCounter++;

        //register instantiated missile
        activeMissiles.Add(instantiatedProjectile);
        schedulePosition++;

    }

    private bool shouldSpawnPowerup(int probability)
    {
        int randomNumber = (int)UnityEngine.Random.Range(0, 100);
        //Debug.Log("Random number: " + randomNumber);

        if (randomNumber > probability)
        {
            //Debug.Log("should spawn missile");
            return false;
        }
        else
        {
            //Debug.Log("should spawn powerup");
            return true;
        }
    }



    public List<GameObject> getActiveProjectiles()
    {
        return activeMissiles;
    }


    public void removeAllProjectiles()
    {

        removeAllProjectilesOfList(activeMissiles);
        removeAllProjectilesOfList(activePowerups);

    }

    public void removeAllMissiles()
    {
        removeAllProjectilesOfList(activeMissiles);

    }

    public void removeAllPowerups()
    {
        removeAllProjectilesOfList(activePowerups);

    }


    private void removeAllProjectilesOfList(List<GameObject> projectileList)
    {
        
        if (projectileList.Count > 0)
        {

            //destroy every registered game object within the activeMissiles list
            foreach (GameObject projectile in projectileList)
            {
                Destroy(projectile.GetComponent<Projectile>().gameObject);


            }

            //clear the list
            projectileList.Clear();
        }
    }



    public void disableAllMissiles()
    {
        //disable every registered game object within the activeMissiles list, this is a quirk that is needed because of delayed removal made  necessary so that sounds can finish playing
        foreach (GameObject projectile in activeMissiles)
        {
            projectile.GetComponent<Projectile>().disableProjectile();
        }
    }


}
