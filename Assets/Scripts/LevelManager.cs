using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkBehaviour {

    // Essential game objects
    public GameObject cubePad1, cubePad2, cubePad3;
    public GameObject playerPad1, playerPad2;
    public GameObject platform1, platform2, platform3;
    public GameObject levelExit1, levelExit2;    

    // Scripts which are linked to the gameobjects above
    private CubepadBehaviour cubePadBehaviour1, cubePadBehaviour2, cubePadBehaviour3;
    private PlayerpadBehaviour playerPadBehaviour1, playerPadBehaviour2;    
    private Vector3 platform1Pos, platform2Pos;
    private ExitBehaviour exitBehaviour1, exitBehaviour2;

    void Awake () {		
        
        AssignBehaviours();
    }

    void Update () {

        // Constant update depedning on the current level
        SceneHandler(GameManager.instance.currentLevel);
	}

    private void AssignBehaviours()
    {
        // Assigns all cubepad behaviours in the scene
        cubePadBehaviour1 = cubePad1.GetComponentInChildren<CubepadBehaviour>();
        cubePadBehaviour2 = cubePad2.GetComponentInChildren<CubepadBehaviour>();
        cubePadBehaviour3 = cubePad3.GetComponentInChildren<CubepadBehaviour>();

        // Assigns all playerpad behaviours in the scene
        playerPadBehaviour1 = playerPad1.GetComponentInChildren<PlayerpadBehaviour>();
        playerPadBehaviour2 = playerPad2.GetComponentInChildren<PlayerpadBehaviour>();

        // Assigns all exit behaviours in the scene
        exitBehaviour1 = levelExit1.GetComponentInChildren<ExitBehaviour>();
        exitBehaviour2 = levelExit2.GetComponentInChildren<ExitBehaviour>();

        // Sets the original platform positions
        platform1Pos = platform1.transform.position;
        platform2Pos = platform2.transform.position;
    }

    private void SceneHandler(int level)
    {
        if (level == 1) LevelOne();
        else if (level == 2) LevelTwo();
    }

    private void LevelOne()
    {
        // Enables exit if a cube is placed on the cubepad
        if (cubePadBehaviour1.BL_padActive) exitBehaviour1.BL_exitActive = true;        

        if (playerPadBehaviour1.BL_padActive)
        {
            // Moves the platform down a specific amount
            if (platform1.transform.position.y >= platform1Pos.y - 2.5f)
            {
                platform1.transform.Translate(Vector3.down * Time.deltaTime / 0.5f, Space.World);
            }            
        }
        else
        {
            // Stops movement once the original limit height has been reached
            if (platform1.transform.position == platform1Pos) return;

            // Automatically moves the platform back to the original position
            if (platform1.transform.position.y <= platform1Pos.y)
            {
                platform1.transform.Translate(Vector3.up * Time.deltaTime / 0.5f, Space.World);
            }
        }        
    }

    private void LevelTwo()
    {
        // Enables exit if both cubes are placed on the 2 cubepads
        if (cubePadBehaviour2.BL_padActive && cubePadBehaviour3.BL_padActive) exitBehaviour2.BL_exitActive = true;       

        if (playerPadBehaviour2.BL_padActive)
        {
            // Moves the platform down a specific amount
            if (platform2.transform.position.y >= platform2Pos.y - 5f)
            {
                platform2.transform.Translate(Vector3.down * Time.deltaTime / 0.5f, Space.World);
                platform3.transform.Translate(Vector3.down * Time.deltaTime / 0.5f, Space.World);
            }
        }
        else
        {
            // Stops movement once the original limit height has been reached
            if (platform2.transform.position == platform2Pos) return;

            // Automatically moves the platform back to the original position
            if (platform2.transform.position.y <= platform2Pos.y)
            {
                platform2.transform.Translate(Vector3.up * Time.deltaTime / 0.5f, Space.World);
                platform3.transform.Translate(Vector3.up * Time.deltaTime / 0.5f, Space.World);
            }
        }
    }
}
