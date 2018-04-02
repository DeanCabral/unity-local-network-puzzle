using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    public static GameManager instance = null;
    public bool BL_MobileMode = true;
    public GameObject chamber1, chamber2;
    public GameObject L1Spawn1, L1Spawn2, L2Spawn1, L2Spawn2;
    public GameObject cube1, cube2, cube3;
    public GameObject cubePad1, cubePad2, cubePad3;
    public GameObject levelExit1, levelExit2;
    // UI for the player interface and pause screen
    public GameObject playerUI;
    public Toggle cheatToggle;
    public GameObject nextLevelScreen;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public NetworkManager NM;
    public PlayerBehaviour PB;
    // UI Text object for the information taskbar
    public Text informationText;
    // Boolean for games pause state
    public bool BL_Paused;
    // Integer for current scene
    public int currentLevel = 1;
    // String for information text
    public string ST_info = "";

    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
            // If not, set instance to this
            instance = this;
        // If instance already exists and it's not this:
        else if (instance != this)
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        // Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
       
        // States and object assignments
        BL_Paused = false;
        nextLevelScreen = GameObject.Find("LevelComplete");
        gameOverScreen = GameObject.Find("GameComplete");
        nextLevelScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public override void OnStartClient()
    {
        // Connection feedback message
        StartCoroutine(ShowMessage("Player has connected . . .", 2));
        SetMobileControls();        
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        // Disconnection feedback messagge
        StartCoroutine(ShowMessage("Player has disconnected . . .", 2));
    }

    // Update is called once per frame
    void Update () {

        UpdateUI();
        CheckPause();
	}

    private void UpdateUI()
    {
        // Updates UI text on screen
        informationText.text = ST_info;
    }

    private void CheckPause()
    {
        // Constant input check for pausing the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggles the pause state
            TogglePause();
        }
    }


    private void TogglePause()
    {
        // Switches pause boolean whenever method is called
        BL_Paused = !BL_Paused;

        // Checks if the game is paused
        if (BL_Paused)
        {
            pauseScreen.gameObject.SetActive(true);
        }
        else
        {           
            pauseScreen.gameObject.SetActive(false);
        }
    }

    public void ToggleCheatmode(bool value)
    {
        // Checks if toggle state is true
        if (value)
        {
            // Move cubes at a designated position depending on current level
            if (currentLevel == 1)
            {
                if (BL_MobileMode) levelExit1.GetComponentInChildren<ExitBehaviour>().BL_exitActive = true;
                cube1.transform.position = cubePad1.transform.position + (Vector3.up * 2);
                
            }
            else if (currentLevel == 2)
            {
                if (BL_MobileMode) levelExit2.GetComponentInChildren<ExitBehaviour>().BL_exitActive = true;
                cube2.transform.position = cubePad2.transform.position + (Vector3.up * 2);
                cube3.transform.position = cubePad3.transform.position + (Vector3.up * 2);                
            }

            cheatToggle.enabled = false;
        }
    }

    private void SwitchScene(int index)
    {
        // Resets the timescale
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    public void SetMobileControls()
    {
        // Activates the mobile controls UI
        playerUI.gameObject.SetActive(true);
    }

    public void PauseGame()
    {        
        TogglePause();
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void DisconnectGame()
    {
        // Disconnects connection from server/host and shutsdown connection
        Network.Disconnect();
        MasterServer.UnregisterHost();
        NetworkServer.Shutdown();
        // Returns to main menu
        SwitchScene(0);
    }

    [Command]
    public void CmdNextLevel()
    {
        RpcNextLevel();
    }

    [ClientRpc]
    public void RpcNextLevel()
    {
        // Activates / Deactivates chambers which help with overall gameplay performance
        switch (currentLevel)
        {
            case 1:
                chamber1.SetActive(true);
                chamber2.SetActive(false);
                break;
            case 2:
                chamber1.SetActive(false);
                chamber2.SetActive(true);
                break;           
        }

        // Locates all players in game and assigns them to an array
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // Resets the cheat toggle in the pause menu
        cheatToggle.enabled = true;
        cheatToggle.isOn = false;        
        SpawnPlayers(players);
        
    }

    private void SpawnPlayers(GameObject[] players)
    {
        // Checks current level and spawns the players accordingly
        switch (currentLevel)
        {
            case 1:
                players[0].transform.position = L1Spawn1.transform.position;
                if (players[1]!= null) players[1].transform.position = L1Spawn2.transform.position;
                break;
            case 2:
                players[0].transform.position = L2Spawn1.transform.position;
                if (players[1] != null) players[1].transform.position = L2Spawn2.transform.position;
                break;
        }
    }

    public IEnumerator ShowMessage(string message, float delay)
    {
        // Displays message for a delayed amount of time
        ST_info = message;
        yield return new WaitForSeconds(delay);
        ST_info = ". . .";
    }
}
