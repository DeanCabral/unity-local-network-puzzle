using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubepadBehaviour : MonoBehaviour {

    public Text padText;
    public bool BL_padActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object")
        {
            CubePlaced();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Object")
        {
            CubeRemoved();
        }
    }

    private void CubePlaced()
    {        
        BL_padActive = true;
        padText.text = "Cube Placed";
        padText.color = new Color32(0, 255, 0, 255);
    }

    private void CubeRemoved()
    {
        BL_padActive = false;
        padText.text = "Place Cube Here";
        padText.color = new Color32(255, 0, 0, 255);
    }
}
