using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey("space"))
        {
            StartGame();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Quit();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void Quit()
    {
        Application.Quit();
    }
}
