using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExitBehaviour : NetworkBehaviour {

    public bool BL_exitActive = false;
    public GameObject exitPad;    
    public Material activeMat;
    public Material inactiveMat;
    private GameManager GM;
    private bool BL_standing = false;
    private bool BL_Complete = false;
    private int IN_playerCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Checks for player and whether the exit has been activated
        if (other.tag == "Player" && BL_exitActive)
        {            
            // If statement lock for code to be called only once
            if (!BL_standing)
            {
                // Increases player count
                CmdPlayerCount(true);
                BL_standing = true;
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Checks for player and whether the exit has been activated
        if (other.tag == "Player" && BL_exitActive)
        {
            // If statement lock for code to be called only once
            if (BL_standing)
            {
                // Decreases player count
                CmdPlayerCount(false);
                BL_standing = false;
            }            
        }
    }

    void Start()
    {
        // Instatiates game manager instance
        GM = GameManager.instance;        
    }

    // Update is called once per frame
    void Update () {

        CheckPlayerCount();
        CmdCheckActiveExit();
	}

    private void CheckPlayerCount()
    {
        // Checks player count
        if (IN_playerCount == 1)
        {
            // If statement lock for level completion
            if (!BL_Complete)
            {
                // Progresses onto next level
                NextLevel();
                BL_Complete = true;
            }
        }
    }

    [Command]
    private void CmdCheckActiveExit()
    {        
        // Command sent from server to all clients
        if (BL_exitActive) RpcActivateExit();
        else RpcDeactivateExit();
    }

    [ClientRpc]
    private void RpcActivateExit()
    {
        // Sets activated texture on exit
        exitPad.GetComponent<Renderer>().material = activeMat;
    }

    [ClientRpc]
    private void RpcDeactivateExit()
    {        
        // Sets deactivated texture on exit
        exitPad.GetComponent<Renderer>().material = inactiveMat;
    }

    [Command]
    private void CmdPlayerCount(bool increase)
    {
        RpcPlayerCount(increase);
    }

    [ClientRpc]
    private void RpcPlayerCount(bool increase)
    {
        // Increases or decreases player count depending on parameter
        if (increase) IN_playerCount++;
        else IN_playerCount--;
    }

    private void NextLevel()
    {
        // Current level check
        if (GM.currentLevel == 1)
        {
            // Continues to next level scene
            GM.nextLevelScreen.SetActive(true);
            StartCoroutine(LoadNextScene());
        }
        else
        {
            // Displays game over screen
            GM.gameOverScreen.SetActive(true);
            if (!GM.BL_MobileMode) Cursor.lockState = CursorLockMode.None;
        }        
    }

    IEnumerator LoadNextScene()
    {        
        // Wait time before spawning players in
        yield return new WaitForSeconds(3);
        GM.currentLevel++;              
        GM.CmdNextLevel();
        GM.nextLevelScreen.SetActive(false);
    }
}
