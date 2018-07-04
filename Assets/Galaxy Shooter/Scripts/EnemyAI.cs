using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    [SerializeField]
    private GameObject enemyExplosionPrefab;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float speed = 5.0f;
    private UIManager uiManager;

    // Use this for initialization
    void Start ()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if(transform.position.y < -7)
        {
            Destroy(this.gameObject);
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            uiManager.UpdateScore();
            Explode(enemyExplosionPrefab);            
        }
        else
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.OneLifeDown();
            }

            Destroy(this.gameObject);
            Explode(explosionPrefab);
        }
    }
}
