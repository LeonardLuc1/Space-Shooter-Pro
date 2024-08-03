using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserspeed = 8.0f;

    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _laserspeed * Time.deltaTime);

        if(transform.position.y > 8)
        {
            Destroy(this.gameObject);
        }
                      
    }
}
