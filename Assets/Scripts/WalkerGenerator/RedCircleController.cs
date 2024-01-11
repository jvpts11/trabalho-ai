using System.Collections;
using UnityEngine;

public class RedCircleController : MonoBehaviour
{
    public enum RedCircleState
    {
        Patrolling,
        ChasingBlueCircle
    }

    public RedCircleState currentState = RedCircleState.Patrolling;

    public float patrolSpeed = 1.0f;
    public float chaseSpeed = 2.0f;
    public float detectionDistance = 3.0f;

    private WalkerGenerator walkerGenerator;

    void Start()
    {
        walkerGenerator = GameObject.FindGameObjectWithTag("WalkerGenerator").GetComponent<WalkerGenerator>();

        StartCoroutine(StateMachine());
    }

    void Update()
    {
        float distanceToBlueCircle = Vector2.Distance(transform.position, walkerGenerator.blueCirclePosition);

        if (distanceToBlueCircle < detectionDistance)
        {
            currentState = RedCircleState.ChasingBlueCircle;
        }
        else
        {
            currentState = RedCircleState.Patrolling;
        }

        switch (currentState)
        {
            case RedCircleState.Patrolling:
                Patrol();
                break;

            case RedCircleState.ChasingBlueCircle:
                ChaseBlueCircle();
                break;
        }
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case RedCircleState.Patrolling:
                    Patrol();
                    break;

                case RedCircleState.ChasingBlueCircle:
                    ChaseBlueCircle();
                    break;
            }

            yield return null;
        }
    }

    void Patrol()
    {
        Debug.Log("Patrolling");

        Vector2 patrolStart = new Vector2(0, 0);
        Vector2 patrolEnd = new Vector2(5, 0);

        transform.position = Vector2.Lerp(patrolStart, patrolEnd, Mathf.PingPong(Time.time * patrolSpeed, 1.0f));
    }


    void ChaseBlueCircle()
    {
        Debug.Log("Chasing");

        Vector2 targetPosition = walkerGenerator.blueCirclePosition;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
    }
}
