using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //move down at 4 meters per second        
    //if bottom of screen
    //respawn at top with a new random position
    
    [SerializeField]
    private float _speed = 4.0f; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        float randomX = Random.Range(-8, 8);
        if (transform.position.y < -5.5f)
        {
            transform.position = new Vector3(randomX, 7.5f, 0);
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }

        if(other.gameObject.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }          
        
    }
}
