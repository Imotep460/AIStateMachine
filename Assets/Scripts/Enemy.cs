using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // For the enemy I want a reference for the Statemachine component on the Enemy.
    private StateMachine brain;
    // I also want a reference for the animator component.
    private Animator animator;
    // I also wanna see the current State of the Enemy, so I create a reference for the text object in the UI (Enemy/advancedCharacter@Idle/Rig1/Spine1/Canvas/Text).
    // To be able to assign it in the Inspector I make it a SerialiseField.
    [SerializeField]
    private UnityEngine.UI.Text stateNote;
    // To be able to set new paths for the agent and move around i wanna create a reference to the NavMeshAgent.
    UnityEngine.AI.NavMeshAgent agent;
    // To get the Enemy agent to know what it is looking for (what to chase and/or damage) I want a reference to the Player object.
    private Player player;
    // I want the Enemy to chase my Player if they get near eachother.
    // To do that I want my enemy to have a simple Vector calculation if the enemy gets within a certain distance, and if player is within that distance (ie near) return a boolean and start chasing.
    private bool playerIsNear;
    // Next a bolean for attacking since again (just like "playerIsNear").
    private bool withinAttackRange;
    // changeMind is gonna function as a delay in going from my Idle State to my Wander State
    private float changeMind;
    // Setup AttackSpeed:
    private float attackSpeed;


    // Start is called before the first frame update
    void Start()
    {
        // To find the player in my scene im simply gonna search the game scene for a player object.
        player = FindObjectOfType<Player>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        brain = GetComponent<StateMachine>();
        // when I setup the scene both "playerIsNear" and "withinAttackRange" is initialised as false.
        playerIsNear = false;
        withinAttackRange = false;
        // Since i wanna push a state that requires 3 actions: ActiveAction, OnEnterAction & OnExitAction from the State machine. Active being the Action that is being processed every frame while active.
        brain.PushState(Idle, OnIdleEnter, OnIdleExit);
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame I wanna check if the enemy is close to my player and not just that but how close, as that will help determin what State the Enemy should be in.
        // transform.position referenses the gameobject with Enemy on it as a component.
        playerIsNear = Vector3.Distance(transform.position, player.transform.position) < 5;
        withinAttackRange = Vector3.Distance(transform.position, player.transform.position) < 1;
    }

    // OnIdleEnter is to be called when entering/starting the Idle State.
    void OnIdleEnter()
    {
        // For testing purposes i wanna use the stateNote.Text. The text above the Enemy's head
        stateNote.text = "Idle";
        // When I enter the Idle State I wanna reset the NavMesh/Nav Agent path, so I don't have a preexisting path.
        agent.ResetPath();
    }

    // now i wanna create the initial State the Idle State (NOTE: enemies start with "playerIsNear" & "withinAttackRange" set to false, ie enemies should be loaded into the scene as Idle, thus the initial State)
    void Idle()
    {
        changeMind -= Time.deltaTime;
        // I wanna check if the enemy is close to the player.
        if (playerIsNear)
        {
            brain.PushState(Chase, OnChaseEnter, OnChaseExit);
        }
        // if the Enemy is not close to the Player, I want to transition to Wander from Idle.
        else if (changeMind <= 0)
        {
            // When I have waited a bit in Idle I wanna go back to Wander.
            brain.PushState(Wander, OnWanderEnter, OnWanderExit);
            // Before exiting however I wanna get a new delay time for the next time I enter the Idle State. To give my enemy a bit of individuality I randomise this delay.
            changeMind = Random.Range(4, 10);
        }

    }

    // OnIdleExit is to be called when exiting the Idle State.
    void OnIdleExit()
    {

    }

    // I want my enemy to chase my player whenever my player gets to close.
    // OnEnter for Chase
    void OnChaseEnter()
    {
        // Show "Chase" in the text above the Enemy's head.
        stateNote.text = "Chase";
        animator.SetBool("Chase", true);
    }

    // Active for Chase
    void Chase()
    {
        // When the enemy is chasing the player I simply wanna set the Enemy's destination to be whereever the player is.
        agent.SetDestination(player.transform.position);
        if (Vector3.Distance(transform.position, player.transform.position) > 5.5f )
        {
            brain.PushState(Idle, OnIdleEnter, OnIdleExit);
        }
        if (withinAttackRange)
        {
            brain.PushState(Attack, OnAttackEnter, null);
        }
    }

    // OnExit for Chase
    void OnChaseExit()
    {
        animator.SetBool("Chase", false);
    }

    //Next I want the Wander State for when the player object is far away and thus get the enemy to walk around aimlessly. same setup way as with Idle ie Active, OnEnter & OnExit
    // OnEnter State for Wander:
    void OnWanderEnter()
    {
        // Show Wander in the text above the enemy's head.
        stateNote.text = "Wander";
        // When stating/entering the Wander State I want to start animating for the walk cycle (called "Chase" in the animator)
        animator.SetBool("Chase", true);
        // To Wander I need a destination to walk to or in other words Offset my Enemy's position (transform.poosition) by a random Vector3 calculation.
        // To do that I create a Vector3. To get a random direction from the center point I use Random.insideUnitSphere (default this point is inside a sphere with radius of 1)
        Vector3 wanderDirection = (Random.insideUnitSphere * 4f) + transform.position;
        // next I wanna make sure that the position my Enemy is gonna move to is on the NavMesh, thus im gonna grab a sample position of the NavMesh (called NavMesh Hitpoint).
        // For this im gonna use NavMeshHit as it return the information i get from whatever query i make on the NavMesh, in this case I just want a sample position.
        NavMeshHit navMeshHit;
        // Get the point on the NavMesh closest to the point we just calculated in WanderDirection.
        NavMesh.SamplePosition(wanderDirection, out navMeshHit, 3f, NavMesh.AllAreas);
        Vector3 destination = navMeshHit.position;
        agent.SetDestination(destination);
    }

    // for Wander I want my enemy to look around and see if there is a spot they wanna move to, and if there is I will pick one randomly and they will navigate to the spot using the NavMesh and the Nav Agent pathing.
    // Active State for Wander.
    void Wander()
    {
        // I wanna check if my enemy has reached it's wander destination point. so I first wanna get the remaining distance
        if (agent.remainingDistance <= 0.25f)
        {
            agent.ResetPath();
            brain.PushState(Idle, OnIdleEnter, OnIdleExit);
        }
        if (playerIsNear)
        {
            brain.PushState(Chase, OnChaseEnter, OnChaseExit);
        }
    }

    // OnExit State for Wander.
    void OnWanderExit()
    {
        // When leaving/Exiting the Wander State I want to stop the Chase animetion.
        animator.SetBool("Chase", false);
    }

    // I want my Enemy to be able to attack my Player thus I make a Attack State.
    // OnEnter for Attack State
    void OnAttackEnter()
    {
        // show Attack above the Enemy's head.
        stateNote.text = "Attack";
        // I don't want my Enemy to be able to walk when it is attacking.
        agent.ResetPath();
    }

    // Active for Attack State
    void Attack()
    {
        attackSpeed -= Time.deltaTime;
        // If the Player leaves the AttackRange I want my Enemy to fall back to it's previous State ie; Chase.
        if (!withinAttackRange)
        {
            brain.PopState();
        }
        else if (attackSpeed <= 0 )
        {

            animator.SetTrigger("Attack");
            player.Hurt(2, 1);
            // Reset the AttackSpeed:
            attackSpeed = 2f;
        }
    }

    //// OnExit for Attack State
    //void OnAttackExit()
    //{

    //}
}
