using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PowerUpEffectsManager : MonoBehaviour
{

    private EventManager _eventManager;
    private GameManager _gameManager;


    public int TransmutePowerUp_PointsPerProjectile = 5;
    public int MultiplicatorPowerUp_MultplicationFactor = 3;

    List<Projectile> slowedProjectiles = new List<Projectile>();
    public static List<Type> currentlyMultipleActivePowerups = new List<Type>(); //stores which powerups are currently active twice TODO: is name realy accurate?!

    void Start()
    {

        _eventManager = EventManager.getInstance();
        _gameManager = GameManager.getInstance();


        //listen to all powerup-events
        _eventManager.addListener(PowerUpEffectsManager_onPowerUpMultiplicatorActivated, EventManager.eventName.OnPowerUpMultiplicatorActivated);
        _eventManager.addListener(PowerUpEffectsManager_onPowerUpMultiplicatorDepleted, EventManager.eventName.OnPowerUpMultiplicatorDepleted);
        _eventManager.addListener(PowerUpEffectsManager_onPowerUpTransmute, EventManager.eventName.OnPowerUpTransmute);
        _eventManager.addListener(PowerUpEffectsManager_onPowerUpSlowMissilesActivated, EventManager.eventName.OnPowerUpSlowMissilesActivated);
        _eventManager.addListener(PowerUpEffectsManager_onPowerUpSlowMissilesDeactivated, EventManager.eventName.OnPowerUpSlowMissilesDeactivated);
        _eventManager.addListener(PowerUpEffectsManager_OnRestartLoss, EventManager.eventName.OnRestartLoss);


    }

    private void PowerUpEffectsManager_OnRestartLoss(GameObject g, EventArgs e)
    {
        resetPointsMultiplicator();
        currentlyMultipleActivePowerups.Clear();
    }

    private void resetPointsMultiplicator()
    {
        _gameManager.pointsMultiplier = 1; //set back  to initial value; attention: 1 = hardcoded value, which is assumed to be the normal value for point calculation
    }




    void PowerUpEffectsManager_onPowerUpMultiplicatorActivated(GameObject g, EventArgs e)
    {
        //have to pass type of powerup in eventArgs since g gets destroyed
        PowerupEventArgs t = e as PowerupEventArgs;
        getNumberOfSamePowerupsCurrentlyActive(t.typeOfPowerup);
        if (getNumberOfSamePowerupsCurrentlyActive(t.typeOfPowerup) >= 1 || currentlyMultipleActivePowerups.Count == 0)
        {
            currentlyMultipleActivePowerups.Add(t.typeOfPowerup);

        }
        _gameManager.pointsMultiplier = MultiplicatorPowerUp_MultplicationFactor;



        //Debug.Log("PEM carrying out PowerUpEffectsManager_onPowerUpMultiplicatorActivated");
    }


    void PowerUpEffectsManager_onPowerUpMultiplicatorDepleted(GameObject g, EventArgs e)
    {
        PowerupEventArgs t = e as PowerupEventArgs;
        Type poweruptype = t.typeOfPowerup;

        currentlyMultipleActivePowerups.Remove(poweruptype);

       // Debug.Log("getNumberOfSamePowerupsCurrentlyActive: " + getNumberOfSamePowerupsCurrentlyActive(poweruptype));
        printActivePowerups();
        if (getNumberOfSamePowerupsCurrentlyActive(poweruptype) < 1)
        {
            resetPointsMultiplicator();
        }

        

       // Debug.Log("PEM carrying out _onPowerUpMultiplicatorDepleted");
    }

    void PowerUpEffectsManager_onPowerUpSlowMissilesActivated(GameObject g, EventArgs e)
    {

        PowerupEventArgs t = e as PowerupEventArgs;
       // Debug.Log(t.typeOfPowerup);
        getNumberOfSamePowerupsCurrentlyActive(t.typeOfPowerup);
        currentlyMultipleActivePowerups.Add(t.typeOfPowerup);
        //Debug.Log("PEM carrying out PowerUpEffectsManager_onPowerUpSlowMissilesActivated");

        List<GameObject> allProjectiles = _gameManager.projectileManager.getActiveProjectiles();

        foreach (GameObject missile in allProjectiles)
        {

            Projectile missileComponent = missile.GetComponent<Projectile>();
            missileComponent.speedModifier *= 0.3f; //attention: 0.3 = hardcoded value; figure out in balancing and store in variable
            slowedProjectiles.Add(missileComponent);
        }


    }


    void PowerUpEffectsManager_onPowerUpSlowMissilesDeactivated(GameObject g, EventArgs e)
    {

        PowerupEventArgs t = e as PowerupEventArgs;
        Type poweruptype = t.typeOfPowerup;

        foreach (Projectile missileComponent in slowedProjectiles)
        {
            missileComponent.speedModifier = 1;
        }
        slowedProjectiles.Clear();
        currentlyMultipleActivePowerups.Remove(poweruptype);


        Debug.Log("PEM carrying out PowerUpEffectsManager_onPowerUpSlowMissilesDeactivated");
    }



    void PowerUpEffectsManager_onPowerUpTransmute(GameObject g, EventArgs e)
    {

        int pointsForAllActiveMissiles = _gameManager.projectileManager.getActiveProjectiles().Count * TransmutePowerUp_PointsPerProjectile;
        _gameManager.projectileManager.removeAllMissiles();
        _gameManager.increasePoints(pointsForAllActiveMissiles);
    }

    private int getNumberOfSamePowerupsCurrentlyActive(Type pu)
    {

        int isMultiplyActivated = 0;



        foreach (Type currentlyActivePowerup in currentlyMultipleActivePowerups)
        {

            if (currentlyActivePowerup == pu)
            {
                isMultiplyActivated++;

            }

        }

        return isMultiplyActivated;
    }

    public static void printActivePowerups()
    {
        int i = 0;

        foreach (Type currentlyActivePowerup in currentlyMultipleActivePowerups)
        {
            Debug.Log(i);
            Debug.Log("================");
            Debug.Log("active: " + currentlyActivePowerup);
            Debug.Log("================");
            i++;


        }

    }




}
