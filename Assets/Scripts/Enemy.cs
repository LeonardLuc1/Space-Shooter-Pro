using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("Player is NULL");
        }

        _anim = gameObject.GetComponent<Animator>();
        
        if(_anim == null)
        {
            Debug.Log("Animatior is NULL");
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);        
        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-8, 8);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
                
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.gameObject.CompareTag("Player"))
        {            
            Player player = other.transform.GetComponent<Player>();
            
            if (player != null)
            {
                player.Damage();
            }
            
           _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;                                   
            Destroy(this.gameObject, 2.5f);
        }

        if(other.gameObject.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            } 
                       
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(this.gameObject, 2.5f);
        }       
               
    }

}
