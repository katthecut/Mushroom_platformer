using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemySlug : MonoBehaviour
{
    ///Slug movement speed
    [SerializeField] private float patrolSpeed = 3f;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        //only patrols left/right on platforms
        enemy.enableMovement = true;
        enemy.verticalMovement = false;
        enemy.movementSpeed = patrolSpeed;

        enemy.hasRaycast = false;

    }

    //to aallow speed changes from other scripts
    public void SetSpeed(float newSpeed)
    {
        patrolSpeed = newSpeed;
        if (enemy != null) enemy.movementSpeed = patrolSpeed;
    }
}