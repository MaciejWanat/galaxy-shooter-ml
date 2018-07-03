using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	[SerializeField]
    private float speed = 5.0f;
    
    // Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if(player != null)
            {
                player.TripleShotPowerUpOn();
            }

            Destroy(this.gameObject);

        }

    }
}
