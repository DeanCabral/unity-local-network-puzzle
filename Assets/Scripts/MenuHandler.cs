using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

    public GameObject menuScreen;
    public GameObject helpScreen;

    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }

    public void HelpGame()
    {
        helpScreen.SetActive(true);
        StartCoroutine(OpenHelp());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        StartCoroutine(CloseHelp());        
    }

    IEnumerator LoadGame()
    {
        Image i = menuScreen.GetComponent<Image>();

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / 1));
            yield return null;
        }

        if (i.color.a <= 0.0f) SceneManager.LoadScene(1);
    }

    IEnumerator OpenHelp()
    {
        Image i = helpScreen.GetComponent<Image>();

        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / 1));
            yield return null;
        }
    }

    IEnumerator CloseHelp()
    {
        Image i = helpScreen.GetComponent<Image>();

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / 1));
            yield return null;
        }

        if (i.color.a <= 0.0f) helpScreen.SetActive(false);
    }

}
