using UnityEngine;
using System.Collections;
using System;

public class KeyboardInput : MonoBehaviour {

    GameObject segmentA;
    GameObject segmentB;
    GameObject segmentC;
    GameObject segmentD;

    GameManager gm;
    EventManager ev;
	
	
	
//    // Use this for initialization
    void Start()
    {
        segmentA = GameObject.Find("pivot_cube_segment_a");
        segmentB = GameObject.Find("pivot_cube_segment_b");
        segmentC = GameObject.Find("pivot_cube_segment_c");
        segmentD = GameObject.Find("pivot_cube_segment_d");

        gm = GameManager.getInstance();
        ev = EventManager.getInstance();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                segmentA.GetComponent<CubeSegment>().chooseFoldingOperation(true);
            }
            else
            {
                segmentA.GetComponent<CubeSegment>().chooseFoldingOperation(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                segmentC.GetComponent<CubeSegment>().chooseFoldingOperation(true);
            }
            else
            {
                segmentC.GetComponent<CubeSegment>().chooseFoldingOperation(false);
            }
            
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                segmentD.GetComponent<CubeSegment>().chooseFoldingOperation(true);
            }
            else
            {
                segmentD.GetComponent<CubeSegment>().chooseFoldingOperation(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("bla");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                segmentB.GetComponent<CubeSegment>().chooseFoldingOperation(true);
            }
            else
            {
                segmentB.GetComponent<CubeSegment>().chooseFoldingOperation(false);
            }
        }



        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
  


            ev.dispatchEvent(GameObject.Find("msh_gui_powerup_slot0"), EventArgs.Empty, EventManager.eventName.OnInventorySlotClicked);
        }



        if (Input.GetKeyDown(KeyCode.Alpha2))
        {


            ev.dispatchEvent(GameObject.Find("msh_gui_powerup_slot1"), EventArgs.Empty, EventManager.eventName.OnInventorySlotClicked);
        }



        if (Input.GetKeyDown(KeyCode.Alpha3))
        {



            ev.dispatchEvent(GameObject.Find("msh_gui_powerup_slot2"), EventArgs.Empty, EventManager.eventName.OnInventorySlotClicked);
        }



        if (Input.GetKeyDown(KeyCode.F1))
        {
            gm.pointsLeftToWin -= 5;
        }


        if (Input.GetKeyDown(KeyCode.F2))
        {
            gm.projectileManager.rndSpeedLower += 0.5f;
            Debug.Log("random speed lower increased  - harder");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            gm.projectileManager.rndSpeedLower -= 0.5f;
            Debug.Log("random speed lower decreased - easier");
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            gm.projectileManager.rndSpeedUpper += 0.5f;
            Debug.Log("random speed upperincreased  - harder");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            gm.projectileManager.rndSpeedUpper -= 0.5f;
            Debug.Log("random speed upper decreased - easier");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            gm.projectileManager.spawnInterval -= 0.25f;
            Debug.Log("spawnInterval decreased - harder");
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            gm.projectileManager.spawnInterval += 0.25f;
            Debug.Log("spawnInterval increased - easier");
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            gm.projectileManager.spawnRandomPowerup();
            Debug.Log("spawning random powerup");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            gm.projectileManager.spawnRandomMissile();
            Debug.Log("spawning random powerup");
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            PowerUpEffectsManager.printActivePowerups();
        }

    }
}
