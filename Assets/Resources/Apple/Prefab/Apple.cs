using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public Vector3 direction;
    public float velocity;
    public float birth_time;
    public GameObject birth_turret;

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - birth_time > 10.0f)  // apples live for 10 sec
        {
            Destroy(transform.gameObject);
        }
        transform.position = transform.position + velocity * direction * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ////////////////////////////////////////////////
        // WRITE CODE HERE:
        // (a) if the object collides with Claire, subtract one life from her, and destroy the apple
        // (b) if the object collides with another apple, or its own turret that launched it (birth_turret), don't do anything
        // (c) if the object collides with anything else (e.g., terrain, a different turret), destroy the apple
        ////////////////////////////////////////////////
        ///

        if (other.CompareTag("Player")) // Assuming Claire has the "Player" tag
        {
            // (a) if the object collides with Claire, subtract one life from her, and destroy the apple
            Claire claire = other.GetComponent<Claire>();
            if (claire != null && !claire.isDead)
            {
                claire.num_lives = claire.num_lives - 1;
                if (claire.num_lives == 0)
                {
                    claire.isDead = true;
                }
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Apple"))
        {
            // (b) if the object collides with another apple, don't do anything
            // You may add more conditions here if needed
        }
        else if (other.CompareTag("Turret") && other.gameObject == birth_turret)
        {
            // (b) if the object collides with its own turret that launched it (birth_turret), don't do anything
        }
        else
        {
            // (c) if the object collides with anything else (e.g., terrain, a different turret), destroy the apple
            Destroy(gameObject);
        }


    }
}
