using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player movement vector field. movement direction.
    private Vector3 movementVector;
    // I want my player to move so I want my player to have some kind of speed value. I even want to be able to multiply my speed value on my players velocity.
    private float speed;
    // Make sure the player has a Rigidbody component on it.
    private Rigidbody body;
    private Animator animator;
    private Health health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        // note that the Animator object is on the child of the player object, ie the model which is the child object. since this is an exspensive call, first find the child then find the component of Animator.
        // I call the reference to animator in Start so as to not having to call it in Update ie once per frame.
        animator = transform.GetChild(0).GetComponent<Animator>();
        // Adds Rigidbody to the player on start.
        body = GetComponent<Rigidbody>();
        speed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        // I wanna rotate to face the direction im moving. First check if some movement IS happening.
        if (movementVector != Vector3.zero)
        {
            // Using Unity's built-in function i use SLERP (Spherical Linear Interpolation) instead of LERP (Linear Interpolation).
            // The method takes in the starting rotation (transform.rotation) and then a target position (movementVector) in the form of a Quaternion.LookRotation.
            // Finally it takes in the timestep, ie how much closer the target it moves for every calculation (giving the rotation a smoothing effect rather than a snap effect)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), 0.25f);
        }
        // Set a animator parameter that is on the animator component. I want it to be true ie do animation is im walking and false/don't move.
        animator.SetBool("Walking", movementVector != Vector3.zero);
        if (Input.GetKeyDown(KeyCode.K))
        {
            Hurt(2, 2);
        }

    }

    // Get direction Player is moving in, which can then be multiplied with a speed.
    void CalculateMovement()
    {
        // Get the directions the player is moving in. ie what directionto look in.
        movementVector = new Vector3(
            Input.GetAxis("Horizontal"),
            //since im not jumping I don't need a direction for the y axis, thus the y-value for the movementVector is just gonna be whatever the y-value is set as in the Inspector.
            body.velocity.y,
            Input.GetAxis("Vertical")
            );
        // Get the speed the player in moving, if the speed is 0 the player does not move.
        body.velocity = new Vector3(
            movementVector.x * speed,
            // NOTE y does not need a speed value bacause that is being handled by the physics engine ie gravity.
            movementVector.y,
            movementVector.z * speed
            );
    }


    public void Hurt(int amount, float delay = 0)
    {
        StartCoroutine(health.TakeDamageDelayed(amount, 2f));
    }
}
