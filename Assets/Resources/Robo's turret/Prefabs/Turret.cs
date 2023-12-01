using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private float shooting_delay; 
    private GameObject projectile_template;
    private Vector3 direction_from_turret_to_claire;
    private Vector3 shooting_direction;
    private Vector3 projectile_starting_pos;
    private float projectile_velocity;
    private bool claire_is_accessible;

    // Start is called before the first frame update
    void Start()
    {
        projectile_template = (GameObject)Resources.Load("Apple/Prefab/Apple", typeof(GameObject));  // projectile prefab
        if (projectile_template == null)
            Debug.LogError("Error: could not find the apple prefab in the project! Did you delete/move the prefab from your project?");
        shooting_delay = 0.5f;  
        projectile_velocity = 5.0f;
        direction_from_turret_to_claire = new Vector3(0.0f, 0.0f, 0.0f);
        projectile_starting_pos = new Vector3(0.0f, 0.0f, 0.0f);
        claire_is_accessible = false;
        StartCoroutine("Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        GameObject claire = GameObject.Find("Claire");
        if (claire == null)
            Debug.LogError("Error: could not find the game character 'Claire' in the scene. Did you delete the model Claire from your scene?");
        Vector3 claire_centroid = claire.GetComponent<CapsuleCollider>().bounds.center;
        Vector3 turret_centroid = GetComponent<Collider>().bounds.center;

        Vector3 claire_position = claire.transform.position;
        Vector3 claire_velocity = claire.GetComponent<Claire>().currentVelocity * claire.GetComponent<Claire>().movement_direction;

        direction_from_turret_to_claire = claire_centroid - turret_centroid;
        direction_from_turret_to_claire.Normalize();

        float epsilon = 0.1f; // Adjust as needed
        float delta_pos = float.MaxValue;

        RaycastHit hit;
        if (Physics.Raycast( turret_centroid, direction_from_turret_to_claire, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == claire)
            {
                ////////////////////////////////////////////////
                // WRITE CODE HERE:
                // implement deflection shooting
                float look_ahead_time = direction_from_turret_to_claire.magnitude / projectile_velocity;
                Vector3 future_claire_position = claire_position + claire_velocity * look_ahead_time;

                while (delta_pos > epsilon)
                {
                    // Update variables for the next iteration
                    Vector3 last_future_claire_position = future_claire_position;

                    // Recalculate look-ahead time and future position
                    look_ahead_time = (future_claire_position - turret_centroid).magnitude / projectile_velocity;
                    future_claire_position = claire_position + claire_velocity * look_ahead_time;

                    // Update delta_pos for the next iteration
                    delta_pos = (future_claire_position - last_future_claire_position).magnitude;
                }


                shooting_direction = (future_claire_position - turret_centroid).normalized; // this is a very simple heuristic for shooting, replace it
                ////////////////////////////////////////////////

                float angle_to_rotate_turret = Mathf.Rad2Deg * Mathf.Atan2(shooting_direction.x, shooting_direction.z);
                transform.eulerAngles = new Vector3(0.0f, angle_to_rotate_turret, 0.0f);
                Vector3 current_turret_direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y), 1.1f, Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y));
                projectile_starting_pos = transform.position + 1.1f * current_turret_direction;  // estimated position of the turret's front of the cannon
                claire_is_accessible = true;
            }
            else
                claire_is_accessible = false;            
        }
    }

    private IEnumerator Spawn()
    {
        while (true)
        {            
            if (claire_is_accessible)
            {
                GameObject new_object = Instantiate(projectile_template, projectile_starting_pos, Quaternion.identity);
                new_object.GetComponent<Apple>().direction = shooting_direction;
                new_object.GetComponent<Apple>().velocity = projectile_velocity;
                new_object.GetComponent<Apple>().birth_time = Time.time;
                new_object.GetComponent<Apple>().birth_turret = transform.gameObject;
            }
            yield return new WaitForSeconds(shooting_delay); // next shot will be shot after this delay
        }
    }
}
