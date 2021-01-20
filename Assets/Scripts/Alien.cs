using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    private Camera cam;
    private bool alienDisabled;
    private Gameplay gameplay;
    private GameObject alienProjectile;
    private Vector3 startDirection;
    private Rigidbody2D rigid;
    private Vector2 moveDirection;
    private float lastTimeShoot;
    private float alienSpawnTime;
    private GameObject ship;

    public float moveSpeed;
    public float projForce;
    public float shootDelay;
    public Transform firePoint;
    public int alienPoints;

    //---------POOLS---------//
    private List<GameObject> pooledProj;
    public GameObject projToPool;
    public int amountToPool;
    public bool shouldExpand;
    //---------END_POOLS----//

    
    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        lastTimeShoot = 0;
        if (rigid == null)
        {
            Debug.LogError("Can't find Rigidbody2D for Alien");
        }
        ship = GameObject.FindWithTag("Player");
        gameplay = GameObject.FindObjectOfType<Gameplay>();

        pooledProj = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(projToPool);
            obj.SetActive(false);
            pooledProj.Add(obj);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (alienDisabled)
        {
            return;
        }
        if (Time.time > lastTimeShoot + shootDelay)
        {
            if (ship.GetComponent<Collider2D>().enabled)
            {
                Shoot();
            }
            lastTimeShoot = Time.time;
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
        float shotAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion q = Quaternion.AngleAxis(shotAngle, Vector3.forward);

        alienProjectile = GetPooledProjectile();

        if (alienProjectile != null)
        {
            alienProjectile.transform.position = firePoint.position;
            alienProjectile.transform.rotation = q;
            alienProjectile.SetActive(true);
            alienProjectile.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, projForce));
        }
    }

    private void FixedUpdate()
    {
        if (alienDisabled)
        {
            return;
        }
        MoveToPlayer();
    }

    public void NewLevel()
    {
        HideAlienShip();
        alienSpawnTime = Random.Range(5.0f, 15.0f);
        Invoke("ShowAlienShip", alienSpawnTime);
    }

    private void HideAlienShip()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        alienDisabled = true;
    }

    private void ShowAlienShip()
    {
        Vector2 newAlienPosition = gameplay.CalcAlienPosition();
        gameObject.transform.position = newAlienPosition;
        startDirection = (ship.transform.position - transform.position).normalized;
        gameObject.GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        alienDisabled = false;
    }

    private void MoveToPlayer()
    {
        if (!ship.GetComponent<Collider2D>().enabled)
        {
            rigid.AddForce(new Vector2(startDirection.x, 0f) * moveSpeed * 5);
        }
        else
        {
            moveDirection = (ship.transform.position - transform.position).normalized;
            rigid.MovePosition(rigid.position + moveDirection * moveSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("ShipProjectile"))
        {
            ship.SendMessage("ScorePoints", alienPoints);
            HideAlienShip();
            coll.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.CompareTag("Player"))
        {
            HideAlienShip();
        }
    }
}
