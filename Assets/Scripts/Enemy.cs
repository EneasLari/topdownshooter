using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent pathFinder;
    Transform target;
    //we override the (virtual) start method of LivingEntity and we call it here with base.Start 
    protected override void Start()
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator UpdatePath() {
        float refreshrate = 0.25f;
        while (target!=null) {
            Vector3 targetPosition=new Vector3 (target.position.x, 0, target.position.z);
            //if this coroutine tries to run after the object is destroyed
            if (!dead) {
                pathFinder.SetDestination(targetPosition);
            }
            
            yield return new WaitForSeconds(refreshrate);
        }
    }
}
