using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerpadBehaviour : MonoBehaviour {

    public Text padText;
    public bool BL_padActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerExit();
        }
    }

    private void PlayerEnter()
    {
        BL_padActive = true;
        padText.text = "Activated";
        padText.color = new Color32(0, 255, 0, 255);
    }

    private void PlayerExit()
    {
        BL_padActive = false;
        padText.text = "Stand Here";
        padText.color = new Color32(255, 0, 0, 255);
    }
}
