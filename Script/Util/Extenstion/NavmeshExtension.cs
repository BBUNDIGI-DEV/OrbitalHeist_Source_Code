using Febucci.UI.Core.Parsing;
using UnityEngine;
using UnityEngine.AI;

public static class NavmeshExtension
{
    public static bool GetCircularRandomPoint(this NavMeshAgent agent, Vector3 origin, float range, out Vector3 result)
    {
        float randomDistance = Random.Range(0.0f, range);
        return getCircularPoint(origin, randomDistance, out result, agent.areaMask);
    }

    public static bool GetCircularRandomPoint(Vector3 origin, float range, out Vector3 result, int navMeshArea = NavMesh.AllAreas)
    {
        float randomDistance = Random.Range(0.0f, range);
        return getCircularPoint(origin, randomDistance, out result, navMeshArea);
    }

    public static bool GetCircularRandomPoint(Vector3 origin, float minRange, float maxRange, out Vector3 result, int navMeshArea = NavMesh.AllAreas)
    {
        float randomDistance = Random.Range(minRange, maxRange);
        return getCircularPoint(origin, randomDistance, out result, navMeshArea);
    }

    private static bool getCircularPoint(Vector3 origin, float range, out Vector3 result, int navMeshArea = NavMesh.AllAreas)
    {
        Vector2 unitCircle = Random.insideUnitCircle;
        Vector3 unitCircleVec3 = new Vector3(unitCircle.x, 0.0f, unitCircle.y);
        unitCircleVec3.Normalize();
        Vector3 randomPoint = origin + unitCircleVec3 * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, range, navMeshArea))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
}
