using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    [Tooltip("The path this crowd member is following.\n" +
        "Elements are connected by a straight line.\n" +
        "[0] = start, [length - 1] = end.")]
    [SerializeField] private Vector3[] route;
    [Tooltip("How fast this crowd member should move, in units per second.")]
    [SerializeField] private float speed;

    private int nextRouteIndex = 1;
    private bool endActionInvoked = false;

    private void Update()
    {
        //If the route is valid and we're not at the end,
        if (route.Length > 1 && nextRouteIndex < route.Length - 1)
        {
            //Move towards the next waypoint in the route at `speed` units per second.
            transform.position = Vector3.MoveTowards(transform.position, route[nextRouteIndex], speed * Time.deltaTime);
        }
        else if (!endActionInvoked)
        {
            EventManager.Instance.onCrowdMemberReachedEnd?.Invoke(this);
            endActionInvoked = true;
        }
    }

    /// <summary>
    /// Gives this crowd member a new route, places this member at the start of that route, and resets relevant internal vars.
    /// </summary>
    /// <param name="newRoute">This crowd member's new route.</param>
    public void ResetRoute(params Vector3[] newRoute)
    {
        //If the route is invalid, bail out and cry about it in the console.
        if (newRoute.Length < 2)
        {
            Debug.LogError($"Invalid route assigned to {gameObject.name}: {newRoute}");
            return;
        }

        nextRouteIndex = 1;
        route = newRoute;
        transform.position = newRoute[0];
        endActionInvoked = false;
    }
}
