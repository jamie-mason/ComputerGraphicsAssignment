using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hitbottom : MonoBehaviour
{

    public void WinScene()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void loseScene()
    {
        SceneManager.LoadScene("LoseScreen");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            loseScene();
        }

        if (other.gameObject.tag == "Push")
        {
            WinScene();
        }
    }

}
