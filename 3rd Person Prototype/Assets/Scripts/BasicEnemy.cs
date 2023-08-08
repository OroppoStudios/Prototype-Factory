using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    bool canAttack = true;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    GameObject Player;

    [Header("Player Detection")]
    public LayerMask PlayerLayer;
    public Vector3 SightRangeOffset, AttackAreaOffset;
    [Range(0, 100)] public float sightRange = 10;
    [Range(0, 100)] public float projectileSpeed = 5;
    [Range(0, 10)] public float attackResetTimer = 2;
    public HealthSystem ThisHealth;
    private void OnValidate()
    {
        ThisHealth = GetComponent<HealthSystem>();
    }
    private void Awake()
    {
        ThisHealth.OnObjectDeath += HandleObjectDeath;
    }
    private void HandleObjectDeath(GameObject obj)
    {
        Debug.Log("triggered");
        Destroy(obj);
    }
    private void OnCollisionEnter(Collision collision)
    {
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CharacterMovement Character))
            return;

        //TODO: Change this with isDashing bool check when implemented
        if (Character.Dashing == true)
        {
            //disable the indicator on the player
            Character.gameObject.TryGetComponent(out MissileSystem Missle);
            Missle.TargetIndicator.SetActive(false);

            Character.ActivateCharge();
            DestroyEnemy();
        }


        if (!other.TryGetComponent(out HealthSystem collisionHealth))
            return;

        DamagePlayer(collisionHealth);
    }
    private void Update()
    {
        bool playerInRange = Physics.CheckSphere(transform.position + transform.rotation * SightRangeOffset, sightRange, PlayerLayer);
        if (!playerInRange)
            return;
        
        AttackTarget(Player);
    }
    private void AttackTarget(GameObject Target)
    {
        if (!canAttack)
            return;

        transform.LookAt(Target.transform);
        Rigidbody rb = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(Vector3.Normalize((Target.transform.position + Vector3.up * Target.GetComponent<CapsuleCollider>().height / 2) - (transform.position)) * projectileSpeed, ForceMode.Impulse);

        canAttack = false;
        StartCoroutine(ResetAttack());
    }
    private static void DamagePlayer(HealthSystem collisionHealth)
    {
        collisionHealth.ModifyHealth(-1);
        //Sounds Animations and events here
    }
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackResetTimer);
        canAttack = true;
    }
    private void DestroyEnemy()
    {
        GetComponent<HealthSystem>().ModifyHealth(-GetComponent<HealthSystem>().currentHealth);
        //Sounds Animations and events here\
     
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
