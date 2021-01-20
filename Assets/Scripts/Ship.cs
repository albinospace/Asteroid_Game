using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    private Camera cam;
    private float moveHorizontal;
    private float moveVertical;
    private GameObject projectile;
    private Rigidbody2D rigid;
    private bool shipDisabled;
    private int shipScore;

    public Image[] hearts;
    public Gameplay gameplay; 
    public float MaxMoveSpeed;
    public float rotateSpeed;
    public Transform firePoint;
    public float projForce;
    public int shipLives;
    public Color invulColor;
    public Color normalColor;
    public Text scoreText;

    //---------POOLS---------//
    private List<GameObject> pooledProj;
    public GameObject projToPool;
    public int amountToPool;
    public bool shouldExpand;
    //---------END_POOLS----//


    // Start is called before the first frame update
    private void Start()
    {
        shipScore = 0;
        cam = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            Debug.LogError("Can't find Rigidbody2D for Ship");
        }
        scoreText.text = shipScore.ToString();
        checkHearts();
        shipDisabled = false;

        pooledProj = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(projToPool);
            obj.SetActive(false);
            pooledProj.Add(obj);
        }
    }

    private void checkHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < shipLives)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        ShipMovement();
        ShipPosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && !shipDisabled)
        {
            Shoot();
        }
    }

    private GameObject GetPooledProjectile()
    {
        for (int i = 0; i < pooledProj.Count; i++)
        {
            if (!pooledProj[i].activeInHierarchy)
            {
                return pooledProj[i];
            }
        }
        if (shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(projToPool);
            obj.SetActive(false);
            pooledProj.Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

    private void Shoot()
    {
        projectile = GetPooledProjectile();
        if (projectile != null)
        {
            projectile.transform.position = firePoint.position;
            projectile.transform.rotation = firePoint.rotation;
            projectile.SetActive(true);
            projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.up * projForce);
        }
    }

    private void ShipMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        { 
            rigid.AddRelativeForce(Vector2.up * moveVertical * MaxMoveSpeed);
            transform.Rotate(0, 0, -moveHorizontal * rotateSpeed);
        }
    }

    private void ShipPosition()
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

    private void RespawnShip()
    {
        shipDisabled = false;
        transform.position = Vector2.zero;
        rigid.velocity = Vector2.zero;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = true;
        sprite.color = invulColor;
        Invoke("Invulnerability", 4.0f);
    }

    private void Invulnerability()
    {
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = normalColor;
    }

    public void ScorePoints(int additionalPoints)
    {
        shipScore += additionalPoints;
        scoreText.text = shipScore.ToString();
    }

    private void LoseLife()
    {
        shipLives--;
        checkHearts();
        shipDisabled = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Invoke("RespawnShip", 3.0f);
        if (shipLives <= 0)
        {
            CancelInvoke("RespawnShip");
            gameplay.GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        LoseLife();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("AlienProjectile"))
        {
            LoseLife();
            coll.gameObject.SetActive(false);
        }
    }
}
