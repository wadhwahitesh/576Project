using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Claire : MonoBehaviour {

    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    public Text text;    
    public float velocity;
    public int num_lives;
    public bool has_won;
    public double idleY;
    public bool isJumping;
    public double jumpTimer;
    public float currentVelocity;
    public bool isDead;
    public float accelerationTime;
    public Canvas canvas;
    public TMPro.TextMeshProUGUI finalText;
    

	// Use this for initialization
	void Start ()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 1.5f;
        velocity = 0.0f;
        num_lives = 5;
        has_won = false;
        idleY = transform.position.y;
        isJumping = false;
        jumpTimer = 0.0;
        currentVelocity = 0;
        isDead = false;
        accelerationTime = 2f;
        //GameObject uiCanvasObject = GameObject.Find("UiCanvas");
        //canvas = uiCanvasObject.GetComponent<Canvas>();
        //finalText = canvas.GetComponentInChildren<TMPro.TextMeshProUGUI>();

 
       
        //canvas.gameObject.SetActive(false);
        
        

        //Transform textTransform = canvas.transform.FindWithTag("StatusText");
        //finalText = textTransform.GetComponent<Text>();
        //finalText = uiCanvasObject.GetComponentInChildren<StatusText> ();
    }

    // Update is called once per frame

    void setParameterTrue(string param)
    {
        animation_controller.SetBool("IsIdle", param == "IsIdle");
        animation_controller.SetBool("IsDead", param == "IsDead");
        animation_controller.SetBool("IsWalkForward", param == "IsWalkForward");
        animation_controller.SetBool("IsWalkBackward", param == "IsWalkBackward");
        animation_controller.SetBool("IsRunForward", param == "IsRunForward");
        animation_controller.SetBool("IsCrouchBackward", param == "IsCrouchBackward");
        animation_controller.SetBool("IsCrouchForward", param == "IsCrouchForward");
        animation_controller.SetBool("IsJump", param == "IsJump");
    }
    void Update()
    {

        //if (!isDead)
        //    text.text = "Lives left: " + num_lives;
        //else
        //    text.text = "Dead!";
        



        ////////////////////////////////////////////////
        // WRITE CODE HERE:
        // (a) control the animation controller (animator) based on the keyboard input. Adjust also its velocity and moving direction. 
        // (b) orient (i.e., rotate) your character with left/right arrow [do not change the character's orientation while jumping]
        // (c) check if the character is out of lives, call the "death" state, let the animation play, and restart the game
        // (d) check if the character reached the target (display the message "you won", freeze the character (idle state), provide an option to restart the game
        // feel free to add more fields in the class        
        ////////////////////////////////////////////////
        ///

        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    // Set the bool parameter to true
        //    animation_controller.
        //}

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (isDead)
        {
            animation_controller.SetBool("IsDead", true);

            StartCoroutine(WaitAndDoSomething(3.5f));
            
        }
        IEnumerator WaitAndDoSomething(float time)
        {
            // Wait for 3 seconds
            yield return new WaitForSeconds(time);
            finalText.text = "You are Dead!";
            finalText.color = Color.red;
            canvas.gameObject.SetActive(true);

            // Code to execute after waiting for 3 seconds
            //Debug.Log("Waited for 3 seconds. Now do something!");

            // Additional code related to handling death...
        }
        IEnumerator WaitBeforeFinish()
        {
            yield return new WaitForSeconds(1f);
            finalText.text = "You Won the Game!";
            canvas.gameObject.SetActive(true);
        }


        if (!isDead && !has_won)
        {
            

            if (!isJumping)
            {
                float rotationAmount = 180f * Time.deltaTime * horizontalInput;
                transform.Rotate(Vector3.up, rotationAmount);



                if (Input.GetKey(KeyCode.UpArrow) && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    currentVelocity = Mathf.Lerp(currentVelocity, walking_velocity, Time.deltaTime * accelerationTime);
                    //currentVelocity = walking_velocity;
                    setParameterTrue("IsWalkForward");
                    //animation_controller.SetBool("IsWalkForward", true);
                    
                    Vector3 moveDirection = transform.forward * verticalInput * currentVelocity;
                    character_controller.Move(moveDirection * Time.deltaTime);

                }
                //else
                //{
                    
                //    animation_controller.SetBool("IsWalkForward", false);
                //}

                else if (Input.GetKey(KeyCode.DownArrow) && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)))
                {
                    //currentVelocity = walking_velocity/ 1.5f;
                    currentVelocity = Mathf.Lerp(currentVelocity, walking_velocity/1.5f, Time.deltaTime * accelerationTime);
                    //animation_controller.SetBool("IsWalkBackward", true);
                    setParameterTrue("IsWalkBackward");
                    Vector3 moveDirection = transform.forward * verticalInput * currentVelocity;
                    character_controller.Move(moveDirection * Time.deltaTime);
                }
                //else
                //{
                //    animation_controller.SetBool("IsWalkBackward", false);
                //}

                else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKey(KeyCode.UpArrow))
                {
                    //currentVelocity = walking_velocity / 2.0f;
                    currentVelocity = Mathf.Lerp(currentVelocity, walking_velocity/2.0f, Time.deltaTime * accelerationTime);
                    //animation_controller.SetBool("IsCrouchForward", true);
                    setParameterTrue("IsCrouchForward");
                    Vector3 moveDirection = transform.forward * verticalInput * currentVelocity;
                    character_controller.Move(moveDirection * Time.deltaTime);
                }
                //else
                //{
                //    animation_controller.SetBool("IsCrouchForward", false);
                //}

                else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKey(KeyCode.DownArrow))
                {
                    //currentVelocity = walking_velocity / 2.0f;
                    currentVelocity = Mathf.Lerp(currentVelocity, walking_velocity/2.0f, Time.deltaTime * accelerationTime);
                    //animation_controller.SetBool("IsCrouchBackward", true);
                    setParameterTrue("IsCrouchBackward");
                    Vector3 moveDirection = transform.forward * verticalInput * currentVelocity;
                    character_controller.Move(moveDirection * Time.deltaTime);
                }
                //else
                //{
                //    animation_controller.SetBool("IsCrouchBackward", false);
                //}

                else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.UpArrow))
                {
                    //currentVelocity = walking_velocity * 2.0f;
                    currentVelocity = Mathf.Lerp(currentVelocity, walking_velocity * 2, Time.deltaTime * accelerationTime);
                    // animation_controller.SetBool("IsRunForward", true);
                    setParameterTrue("IsRunForward");
                    Vector3 moveDirection = transform.forward * verticalInput * currentVelocity;
                    character_controller.Move(moveDirection * Time.deltaTime);
                }

                else
                {

                    setParameterTrue("IsIdle");
                    
                    currentVelocity = Mathf.Lerp(currentVelocity, 0, Time.deltaTime * accelerationTime);
                    Vector3 moveDirection = transform.forward * verticalInput * currentVelocity;
                    character_controller.Move(moveDirection * Time.deltaTime);
                }
                
                //else
                //{
                //    animation_controller.SetBool("IsRunForward", false);
                //}



                if (Input.GetKey(KeyCode.Space) && !isJumping && animation_controller.GetBool("IsRunForward"))
                {
                    animation_controller.SetBool("IsJump", true);


                    isJumping = true;
                    jumpTimer = 0;



                }
            }


            if (isJumping)
            {
                jumpTimer += Time.deltaTime;
                currentVelocity = Mathf.Lerp(currentVelocity, walking_velocity * 3, Time.deltaTime * accelerationTime);
                float jumpDistance = currentVelocity * Time.deltaTime;
                Vector3 jumpVector = transform.forward * jumpDistance;
                character_controller.Move(jumpVector);

                jumpTimer += Time.deltaTime;

                // Check if the jump duration has reached its maximum
                if (jumpTimer >= 3.5)
                {
                    isJumping = false;
                    animation_controller.SetBool("IsJump", false);
                    jumpTimer = 0;
                }
            }
        }

        if (has_won)
        {
            setParameterTrue("IsIdle");
            StartCoroutine(WaitBeforeFinish());

        }
        





            // you don't need to change the code below (yet, it's better if you understand it). Name your FSM states according to the names below (or change both).
            // do not delete this. It's useful to shift the capsule (used for collision detection) downwards. 
            // The capsule is also used from turrets to observe, aim and shoot (see Turret.cs)
            // If the character is crouching, then she evades detection. 
            bool is_crouching = false;
        if ( (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("Crouch Forward"))
         ||  (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("Crouch Backward")) )
        {
            is_crouching = true;
        }

        if (is_crouching)
        {
            GetComponent<CapsuleCollider>().center = new Vector3(GetComponent<CapsuleCollider>().center.x, 0.0f, GetComponent<CapsuleCollider>().center.z);
        }
        else
        {
            GetComponent<CapsuleCollider>().center = new Vector3(GetComponent<CapsuleCollider>().center.x, 0.9f, GetComponent<CapsuleCollider>().center.z);
        }

        // you will use the movement direction and velocity in Turret.cs for deflection shooting 
        float xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        float zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, 0.0f, zdirection);

        if(!isJumping && !isDead && Input.GetKey(KeyCode.DownArrow)){
            movement_direction = -movement_direction;
        }
        
      

        // character controller's move function is useful to prevent the character passing through the terrain
        // (changing transform's position does not make these checks)
        if (transform.position.y > 0.0f) // if the character starts "climbing" the terrain, drop her down
        {
            Vector3 lower_character = movement_direction * velocity * Time.deltaTime;
            lower_character.y = -100f; // hack to force her down
            character_controller.Move(lower_character);
        }
        else
        {
            character_controller.Move(movement_direction * velocity * Time.deltaTime);
        }
    }                    
}
