using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Camera cam;
    private Gameplay gameplay;
    private GameObject ship;

    public GameObject asteroidLarge;
    public GameObject asteroidMedium;
    public GameObject asteroidSmall; 
    public float maxRotation;
    public float maxSpeed;
    public int asteroidSize;
    public int asteroidPoints;

    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            Debug.LogError("Can't find Rigidbody2D for Asteroid");
        }
        ship = GameObject.FindWithTag("Player");
        gameplay = GameObject.FindObjectOfType<Gameplay>();
        AsteroidSpawn();
    }

    private void AsteroidSpawn()
    {
        float moveSpeed = Random.Range(0, maxSpeed);
        float rotation = Random.Range(-maxRotation, maxRotation);
        rigid.AddForce(transform.right * moveSpeed);
        rigid.AddForce(transform.up * rotation);
    }

    private void FixedUpdate()
    {
        AsteroidPosition();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("ShipProjectile"))
        {
            AsteroidDivision(asteroidSize, 2);
            coll.gameObject.SetActive(false);
        }
    }

    private void AsteroidDivision(int asteroidSize, int asteroidsCount)
    {
        for (int i = 1; i <= asteroidsCount; i++)
        {
            if (asteroidSize == 3)
            {
                Instantiate(asteroidMedium, new Vector2(transform.position.x, transform.position.y), transform.rotation);
                gameplay.UpdateNumberOfAsteroids(1.0f / asteroidsCount);
            }
            else if (asteroidSize == 2)
            {
                Instantiate(asteroidSmall, new Vector2(transform.position.x, transform.position.y), transform.rotation);
                gameplay.UpdateNumberOfAsteroids(1.0f / asteroidsCount);
            }
            else if (asteroidSize == 1)
            {
                gameplay.UpdateNumberOfAsteroids(-1.0f / asteroidsCount);
            }
        }
        ship.SendMessage("ScorePoints", asteroidPoints);
        Destroy(gameObject);
    }

    private void AsteroidPosition()
    {
        float sceneWidth = cam.orthographicSize * 2 * cam.aspect;
        float sceneHeight = cam.orthographicSize * 2;

        float sceneRightEdge = sceneWidth / 2;
        float sceneLeftEdge = sceneRightEdge * -1;
        float sceneTopEdge = sceneHeight / 2;
        float sceneBotEdge = sceneTopEdge * -1;

        if (transform.position.x > sceneRightEdge)
        {
            transform.position = new Vector2(sceneLeftEdge, transform.position.y);
        }
        if (transform.position.x < sceneLeftEdge)
        {
            transform.position = new Vector2(sceneRightEdge, transform.position.y);
        }
        if (transform.position.y > sceneTopEdge)
        {
            transform.position = new Vector2(transform.position.x, sceneBotEdge);
        }
        if (transform.position.y < sceneBotEdge)
        {
            transform.position = new Vector2(transform.position.x, sceneTopEdge);
        }
    }
}
