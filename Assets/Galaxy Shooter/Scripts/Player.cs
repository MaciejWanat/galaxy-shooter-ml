using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public bool canTriple = false;
    public bool shieldOn = false;

    [SerializeField]
    private GameObject explosionAnimation;

    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private GameObject shield;

    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float fireRate = 0.25f;

    [SerializeField]
    private int lifes = 3;

    private Spawn_Manager spawn_Manager;

    private UIManager uiManager;
    private GameManager gameManager;

    private float speedBoost = 1.0f;
    private float canFire = 0.0f;

    private AudioSource audioSource;

    [SerializeField]
    private GameObject[] engines;
    
    // Use this for initialization
	void Start ()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (uiManager)
        {
            uiManager.UpdateLives(lifes);
        }
        /*
        spawn_Manager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();

        if (spawn_Manager)
        {
            spawn_Manager.StartSpawnRoutines();
        }
        */
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Movement();
        ShottityShoot();
    }

    void TripleShoot()
    {
        Instantiate(laserPrefab, transform.position + new Vector3(0, 0.88f, 0), Quaternion.identity);
        Instantiate(laserPrefab, transform.position + new Vector3(0.55f, 0.48f, 0), Quaternion.identity);
        Instantiate(laserPrefab, transform.position + new Vector3(-0.55f, 0.48f, 0), Quaternion.identity);
    }

    private void ShottityShoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            audioSource.Play();
            if (Time.time > canFire)
            {
                if(canTriple)
                    TripleShoot();
                else
                    Instantiate(laserPrefab, transform.position + new Vector3(0, 0.88f, 0), Quaternion.identity);

                canFire = Time.time + fireRate;
            }
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * horizontalInput * speed * speedBoost * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * speed * speedBoost * Time.deltaTime);

        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y < -4.2f)
        {
            transform.position = new Vector3(transform.position.x, -4.2f, 0);
        }

        if (transform.position.x > 7f)
        {
            transform.position = new Vector3(7f, transform.position.y, 0);
            // "scrolling"
            //transform.position = new Vector3(-9.2f, transform.position.y, 0);
        }
        else if (transform.position.x < -7f)
        {
            transform.position = new Vector3(-7f, transform.position.y, 0);
            // "scrolling"
            //transform.position = new Vector3(9.2f, transform.position.y, 0);
        }
    }

    public void OneLifeDown()
    {
        if(!shieldOn)
        {
            //Lets get immortal
            //this.lifes--;

            switch(lifes)
            {
                case 1:
                    engines[0].SetActive(true);
                    break;

                case 2:
                    engines[1].SetActive(true);
                    break;
            }

            uiManager.UpdateLives(lifes);
            
            if (lifes <= 0)
            {
                Instantiate(explosionAnimation, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
                gameManager.GameOver();
                uiManager.ShowTitleScreen();
            }
        }
        else
        {
            shieldOn = false;
            shield.gameObject.SetActive(false);
        }
    }

    public void TurnShieldOn()
    {
        this.shieldOn = true;
        shield.gameObject.SetActive(true);
    }

    public void SpeedBoostPowerUpOn()
    {
        speedBoost = 2.0f;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speedBoost = 1.0f;
    }

    public void TripleShotPowerUpOn()
    {
        canTriple = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        canTriple = false;
    }
}
