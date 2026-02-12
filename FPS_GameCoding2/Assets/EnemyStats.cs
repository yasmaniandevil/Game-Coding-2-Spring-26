using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    //template for enemy stats 
    //will be used inside json file
    public string name;
    public int health;
    public float speed;
    public float detectionRange;
    public float attackRange;
    public float attackCoolDown;
}

[System.Serializable]
public class EnemyDataBase
{
    //container for multiple enemies
    //unity will read our json file into this scructure
    //must match the name in our json file
    public List<EnemyStats> enemiesList = new List<EnemyStats>();
}
