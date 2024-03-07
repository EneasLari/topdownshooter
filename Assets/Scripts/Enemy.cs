using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum StateOfAttck { Idle, Chasing, Attacking }
    StateOfAttck currentState;
    public ParticleSystem deathEffect;

    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Color originalColour;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;

    float myCollisionRadius;
    float targetCollisionradius;

    bool hasTarget;

    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionradius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    //we override the (virtual) start method of LivingEntity and we call it here with base.Start 
    protected override void Start()
    {
        base.Start();


        if (hasTarget)
        {
            currentState = StateOfAttck.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }

    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColour)
    {
        pathFinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        skinMaterial = GetComponent<Renderer>().sharedMaterial;
        skinMaterial.color = skinColour;
        originalColour = skinMaterial.color;
    }


    public override void TakeHit(float damage, Vector3 hitpoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health && !dead)
        {
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            //insrtantiate and destroy after start lifetime expires
            Destroy(Instantiate(deathEffect.gameObject, hitpoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitpoint, hitDirection);
    }


    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = StateOfAttck.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if hasTarget is true and target is not null before proceeding
        if (hasTarget && target != null)
        {
            if (Time.time > nextAttackTime)
            {
                // Calculate the squared distance to the target and check it against the threshold
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionradius, 2))
                {
                    // Schedule the next attack time and play attack sound
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }


    IEnumerator Attack()
    {

        currentState = StateOfAttck.Attacking;
        //in order pathfinder donty inerfier with attack animation above we disable it
        pathFinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        while (percent <= 1)
        {
            if (percent >= 0.5 && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        skinMaterial.color = originalColour;
        //enable again pathfinder after attack is done
        currentState = StateOfAttck.Chasing;
        pathFinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshrate = 0.25f;
        while (hasTarget)
        {
            if (currentState == StateOfAttck.Chasing)
            {
                Vector3 dirTotarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirTotarget * (myCollisionRadius + targetCollisionradius + attackDistanceThreshold / 2);
                //if this coroutine tries to run after the object is destroyed
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }

            }
            yield return new WaitForSeconds(refreshrate);
        }
    }
}
