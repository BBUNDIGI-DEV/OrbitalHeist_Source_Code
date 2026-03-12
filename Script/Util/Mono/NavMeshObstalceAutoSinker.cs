using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode, RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(NavMeshObstacle))]
public class NavMeshObstalceAutoSinker : MonoBehaviour
{

    private void Update()
    {
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        BoxCollider collider = GetComponent<BoxCollider>();
        obstacle.size = collider.size;
        obstacle.center = collider.center;
    }
}
