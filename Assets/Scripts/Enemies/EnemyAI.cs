using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;

    private enum State
    {
        Roaming
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private SpriteRenderer enemyRenderer;
    private GameObject[] objects;

    private const int behindAmbienceSortingOrder = -6;
    private const int enemySortingOrder = 0;

    void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(RoamingRoutine());    
    }

    void Update()
    {
        objects = GameObject.FindGameObjectsWithTag("Objects");

        if (objects.Length > 0)
        {
            GameObject closestObject = FindClosestObjectBelowEnemy(4f);

            if (transform.position.y > closestObject.transform.position.y)
            {
                enemyRenderer.sortingOrder = behindAmbienceSortingOrder;
            }
            else
            {
                enemyRenderer.sortingOrder = enemySortingOrder;
            }
        }
    }

    private IEnumerator RoamingRoutine()
    {
        while ( state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            enemyPathfinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(roamChangeDirFloat);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private GameObject FindClosestObjectBelowEnemy(float maxDistance)
    {
        List<GameObject> closestObjects = objects
            .Where(ob => Vector3.Distance(transform.position, ob.transform.position) <= maxDistance)
            .OrderBy(ob => Vector3.Distance(transform.position, ob.transform.position))
            .Take(7)
            .ToList();

        GameObject lowestObj = null;

        foreach (GameObject obj in closestObjects)
        {
            if (obj.CompareTag("Objects") && obj.transform.position.y < transform.position.y)
            {
                lowestObj = obj;
            }
        }

        return lowestObj != null ? lowestObj : closestObjects.FirstOrDefault();
    }
}
