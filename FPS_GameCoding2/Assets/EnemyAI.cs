using UnityEngine;
using UnityEngine.AI;
using System.IO;


public class EnemyAI : MonoBehaviour
{
    //define different states and switches between them
    public enum EnemyState { Idle, Patrol, Chase, Attack, Death}
    public EnemyState currentState;

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
        LoadEnemyData(enemyType);

        currentState = EnemyState.Patrol; //start with patrolling
        MoveToNextPatrolPoint();

        //find and assign our player
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //switch statement determines what behavior the enemyt should perform based on its current state
        //switch statenment checks current state of enemy and decides which bevhavior to execute
        switch(currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                //break makes sure program doesnt check other cases once a match is found
                break;
            case EnemyState.Patrol:
                PatrolBehavior();
                if (distanceToPlayer <= detectionRange) ChangeState(EnemyState.Chase);
                break;
            //moves toward player if close enough and switches to attack
            case EnemyState.Chase:
                ChaseBehavior();
                if (distanceToPlayer <= attackRange) ChangeState(EnemyState.Attack);
                else if (distanceToPlayer > detectionRange) ChangeState(EnemyState.Patrol);
                break;
            case EnemyState.Attack:
                AttackBehavior();
                if(distanceToPlayer > attackRange) ChangeState(EnemyState.Chase);
                break;
            case EnemyState.Death:
                break;
        }
    }

    void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    void IdleBehavior()
    {
        //you can add an animation here
    }

    void PatrolBehavior()
    {
        //ensures the enemy only switches patrol points after reaching the targed
        //if enemy is close enough to patrol point, .5 moves to the next one
        if(!agent.pathPending && agent.remainingDistance < .5f)
        {
            MoveToNextPatrolPoint();
        }
    }

    void MoveToNextPatrolPoint()
    {
        //if we have no patrol points, exit the function
        if (patrolPoints.Length == 0) return;

        //set destination moves to the next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        //update our index so it moves by 1
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void ChaseBehavior()
    {
        //set the destination to follow the player
        agent.SetDestination(player.position);
    }

    void AttackBehavior()
    {
        if(Time.time >= lastAttackTime + attackCoolDown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Enemy Attacked player");
        }
    }

    private void LoadEnemyData(string enemyName)
    {
        //path to our json file
        string path = Application.dataPath + "/Data/enemies.json";
        //if the file exists
        if(File.Exists(path))
        {
            //read json file as text and store it into a string
            string json = File.ReadAllText(path);
            //convert json to c# objects
            //stores the result
            EnemyDataBase enemyDB = JsonUtility.FromJson<EnemyDataBase>(json);

            //find the correct enemy in json
            //loop through all of our enemies
            foreach(EnemyStats enemy in enemyDB.enemiesList)
            {
                Debug.Log($"Checking enemy: {enemy.name}");

                //if the enemy that matches the requested name
                if(enemy.name == enemyName)
                {
                    Debug.Log($"Enemy: {enemy.name} found! Assigning Stats...");
                    health = enemy.health;
                    speed = enemy.speed;
                    detectionRange = enemy.detectionRange;
                    attackRange = enemy.attackRange;
                    attackCoolDown = enemy.attackCoolDown;
                    Debug.Log($"Loaded enemy: {enemy.name}");
                }
            }
        }
        else
        {
            Debug.Log("enemy json file not found");
        }

    }
}
