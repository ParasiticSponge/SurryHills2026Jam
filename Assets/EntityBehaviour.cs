using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    public float speed = 1;
    public string serial = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, -1 * speed * Time.deltaTime, 0);
        if (speed > 0)
        {

        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        speed = 0;
    }
}
