using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public bool canTriple = false;

    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float fireRate = 0.25f;

    private float canFire = 0.0f;
    
    // Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Movement();
        ShottiyShoot();
    }

    void TripleShoot()
    {
        Instantiate(laserPrefab, transform.position + new Vector3(0, 0.88f, 0), Quaternion.identity);
        Instantiate(laserPrefab, transform.position + new Vector3(0.55f, 0.48f, 0), Quaternion.identity);
        Instantiate(laserPrefab, transform.position + new Vector3(-0.55f, 0.48f, 0), Quaternion.identity);
    }

    private void ShottiyShoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time > canFire)
            {
                if(canTriple)
                    TripleShoot();
                else
                    Instantiate(laserPrefab, transform.position + new Vector3(0, 0.88f, 0), Quaternion.identity);

                Debug.Log("xy");
                canFire = Time.time + fireRate;
            }
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * speed * Time.deltaTime);

        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y < -4.2f)
        {
            transform.position = new Vector3(transform.position.x, -4.2f, 0);
        }

        if (transform.position.x > 9.2f)
        {
            transform.position = new Vector3(-9.2f, transform.position.y, 0);
        }
        else if (transform.position.x < -9.2f)
        {
            transform.position = new Vector3(9.2f, transform.position.y, 0);
        }
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
