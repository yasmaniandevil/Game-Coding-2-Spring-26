using System;
using UnityEngine;
using System.IO;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //define different states and switches between them
    public enum EnemyState{ Idle, Patrol, Chase, Attack, Death}
    private EnemyState currentState;

    private Transform player;
    private NavMeshAgent agent;
    
    //patrol settings
    public Transform[] patrolPoints; //also called waypoints
    private int currentPatrolIndex;
    
    //enemy states loaded from json
    public string enemyType;
    private int health;
    private float speed;
    private float detectionRange;
    private float attackRange;
    private float attackCoolDown;

    private float lastAttackTime;
    private int collisionCount = 0;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        //load enemy data from json
        LoadEnemyData(enemyType);

        currentState = EnemyState.Patrol; //start with patrolling
        MoveToNextPatrolPoint();
        
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //switch statement is like multiple choice descion maker in programming, instead of a bunch if else statements
        //checks a variable and decides what code to run based off of its value

        //switch statement determines what behavior the enemy should perform based on its currentState
        //switch statement checks current state of enemy and decides which behavior to execute
        switch(currentState)
        { 
            case EnemyState.Idle:
                IdleBehaviour();
                //break makes sure program doesnt check other cases once a match is found
                break;

            //moves between waypoints if player is detected it switches to chase
            case EnemyState.Patrol:
                PatrolBehaviour();
                //if enemy within detection will switch to chase
                if (distanceToPlayer <= detectionRange) ChangeState(EnemyState.Chase);
                break;

            //moves toward player if close enough switches to attack
            //if player escapes switches back to patrol
            case EnemyState.Chase:
                ChaseBehaviour();
                if(distanceToPlayer <= attackRange) ChangeState(EnemyState.Attack);
                else if(distanceToPlayer > detectionRange) ChangeState(EnemyState.Patrol);
                break;

            //attacks player if player moves away switches back to chase
            case EnemyState.Attack:
                AttackBehavior();
                if(distanceToPlayer > attackRange) ChangeState(EnemyState.Chase);
                break;
            case EnemyState.Death:
                break;
        }
    }

    //updates the enemies current state
    void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    void IdleBehaviour()
    {
        //you can add animation if you want
    }

    void PatrolBehaviour()
    {
        //enemy follows fath to target (patrol point)
        //it waits until it reaches patrol point
        //once it reaches the point it moves to next location

        //pathPending is true if unity is still calculating path
        //if false means path has been fully calculated and enemy is actually moving towards target
        //ensures enemy only switches patrol points after reaching the target
        //if enmy is close enough to patrol point, .5 it moves to next one
        if(!agent.pathPending && agent.remainingDistance < .5f)
        {
            MoveToNextPatrolPoint();
        }
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        
        //.set destination moves to next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        //updates the index
        //if currentpatrolindex is 0 it moves it to 1
        //loops back around
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        
    }

    void ChaseBehaviour()
    {
        agent.SetDestination(player.position);
        //Debug.Log("chase called");
    }

    void AttackBehavior()
    {
        if(Time.time >= lastAttackTime + attackCoolDown)
        {
            lastAttackTime = Time.time;
            //Debug.Log("Enemy attacked player");
            //logic to reduce players health call from game manager
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            
            //why couldnt i declare collisioncount locally?
            collisionCount++;
            Debug.Log("collision counts: " + collisionCount);
            Debug.Log("Enemy hit");

            if(collisionCount == 3)
            {
                agent.enabled = false;
                //ChangeState(EnemyState.);
                Destroy(gameObject);
                FPSGameManager.Instance.Score++;
            }
        }
    }

    private void LoadEnemyData(string enemyName)
    {
        //path to json file
        string path = Application.dataPath + "/Data/enemies.json";
        //check if file exists
        if (File.Exists(path))
        {
            //read json file as text and store it in a string
            string json = File.ReadAllText(path);
            //convert json to c# objects
            //stores result
            EnemyDataBase enemyDB = JsonUtility.FromJson<EnemyDataBase>(json);
            
            //find the correct enemy in json
            //loops through all enemies
            foreach (EnemyStats enemy in enemyDB.enemiesList)
            {
                Debug.Log($"Checking enemy: {enemy.name}");
                
                //find the enemy that matches the requested name
                if (enemy.name == enemyName)
                {
                    Debug.Log($"Enemy {enemy.name} found! Assigning stats...");
                    health = enemy.health;
                    speed = enemy.speed;
                    detectionRange = enemy.detectionRange;
                    attackRange = enemy.attackRange;
                    attackCoolDown = enemy.attackCoolDown;
                    Debug.Log($"Loaded enemy: {enemy.name}");
                    return;
                }
            }

        }
        else
        {
            Debug.Log("enemy json file not found");
        }
    }
}
