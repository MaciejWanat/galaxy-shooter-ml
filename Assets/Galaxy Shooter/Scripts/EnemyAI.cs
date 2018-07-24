using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    [SerializeField]
    private GameObject enemyExplosionPrefab;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float speed = 10.0f;
    private UIManager uiManager;
    [SerializeField]
    private AudioClip clip;
    public bool collideWithPlayer = false;
    private GameObject Player;

    // Use this for initialization
    void Start ()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if(transform.position.y < -7)
        {
            //Destroy(this.gameObject);
            int inYourFace = Random.Range(0, 3);

            if (inYourFace == 0)
            {
                transform.position = new Vector3(Player.transform.position.x, 7, 0);
            }
            else
            {
                transform.position =  new Vector3(Random.Range(-3.5f, 3.5f), 7, 0);
            }

             /*
                float randomX = Random.Range(-7f, 7f);
                transform.position = new Vector3(randomX, 7, 0);
             */
        }
	}

    private void Explode(GameObject prefab)
    {
        GameObject Anim = Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(Anim, 3f);
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    public void PlayExplode()
    {
        Explode(enemyExplosionPrefab);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            
            if(!uiManager)
            {
                uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
            }

            uiManager.UpdateScore();
            Explode(enemyExplosionPrefab);            
        }
        */
        //Enemy is destroyed when training fucntion decects collision with player
        /*
        else
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            collideWithPlayer = true;

            if (player != null)
            {
                player.OneLifeDown();
            }

            //Disable destroy for testing
            //Destroy(this.gameObject);
            Explode(explosionPrefab);
        }
        */
    }
}
