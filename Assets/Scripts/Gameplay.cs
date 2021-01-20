using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour
{
    private int startAsteroidsCount;
    private Camera cam;
    private int asteroidHP;
    private GameObject alienClone;
    private float totalAsteroidsCount;
    private int lvlNumber;

    public GameObject asteroid;
    public GameObject ship;
    public GameObject alienShip; 
    public GameObject gameOverPanel;

    // Start is called before the first frame update
    private void Start()
    {
        lvlNumber = 0;
        ship.SetActive(true);
        cam = Camera.main;
        StartNewLevel();
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOverPanel.activeSelf)
        {
            if (Input.GetKey("space"))
            {
                gameOverPanel.SetActive(false);
                RestartGame();
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                GoToMenu();
            }
        }
    }

    public Vector2 CalcAlienPosition()
    {
        float sceneWidth = cam.orthographicSize * 2 * cam.aspect;
        float sceneHeight = cam.orthographicSize * 2;
        float randomY = Random.Range(-sceneHeight/2, sceneHeight/2);
        Vector2 alienSize = alienShip.GetComponent<SpriteRenderer>().bounds.size;
        Vector2 initAlienPosition = new Vector2(Random.Range(-sceneWidth / 2, sceneWidth / 2), randomY);
        Vector2 alienPosition = (ship.transform.position.x - initAlienPosition.x >= 0) ? new Vector2(-sceneWidth/2+alienSize.x, randomY) : new Vector2(sceneWidth/2 - alienSize.x, randomY);
        return alienPosition;
    }
    
    private void SpawnAlien()
    {
        if (lvlNumber >= 1) {
            if (lvlNumber == 1)
            {
                alienClone = Instantiate(alienShip, Vector2.zero, transform.rotation);
            }
            alienClone.GetComponent<Alien>().NewLevel();
        }
    }

    private void CreateAsteroids(int asteroidsCount)
    {
        float sceneWidth = cam.orthographicSize * 2 * cam.aspect;
        float sceneHeight = cam.orthographicSize * 2;
        bool asteroidSpawned;

        for (int i = 1; i <= asteroidsCount; i++)
        {
           asteroidSpawned = false;
            while (!asteroidSpawned)
            {
                Vector2 asteroidPosition = new Vector2(Random.Range(-sceneWidth / 2, sceneWidth / 2), Random.Range(-sceneHeight / 2, sceneHeight / 2));
                if (((Vector3)asteroidPosition - ship.transform.position).magnitude < 5)
                {
                    continue;
                }
                else
                {
                    GameObject asteroidClone = Instantiate(asteroid, asteroidPosition, transform.rotation);
                    asteroidSpawned = true;
                }
            }
        }
    }

    public void GameOver()
    {
        ship.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("StartPanel");
    }

    public void UpdateNumberOfAsteroids(float change)
    {
        totalAsteroidsCount += change;

        if (totalAsteroidsCount <= 0)
        {
            Invoke("StartNewLevel", 2.0f);
        }
    }

    private void StartNewLevel()
    {
        lvlNumber++;
        startAsteroidsCount = (lvlNumber == 1) ? Random.Range(3, 4) : startAsteroidsCount+1;
        totalAsteroidsCount = startAsteroidsCount;
        CreateAsteroids(startAsteroidsCount);
        SpawnAlien();
    }
}
