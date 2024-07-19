using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotTowardsEnemy : MonoBehaviour
{
    public float WeaponAimOffset;
    
    public GameObject Troop;
    TroopAutoAttack AttackScript;
    public GameObject Enemy;

    private void Start()
    {
        AttackScript = Troop.GetComponent<TroopAutoAttack>();
    }

    Vector2 EnemyScreenpoint;

    private void Update()
    {
        Enemy = AttackScript.targetEnemy;

        Vector2 TroopScreenpoint = Camera.main.WorldToScreenPoint(transform.position);
        

        if (Enemy != null)
        {
            EnemyScreenpoint = Camera.main.WorldToScreenPoint(Enemy.transform.position);
        }

        Vector2 offset = new Vector2(EnemyScreenpoint.x - TroopScreenpoint.x, EnemyScreenpoint.y + WeaponAimOffset - TroopScreenpoint.y);

        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;


        if (AttackScript.GetComponent<TroopClass>().GoingLeft == true)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x + 180, 0, -angle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}

