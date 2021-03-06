#if UNITY_EDITOR
using System.Linq;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [SerializeField] CrowdMember memberPrefab;
    [SerializeField] [Range(0, 1000)] int maxMemberAmount = 50;
    [Tooltip("The position crowd members go to on startup, and when they are not used/inactive.")]
    [SerializeField] Vector3 inactivePos = Vector3.up * -75;

    [Tooltip("How long to wait before \"spawning\" a member at a spawn position")]
    [SerializeField] float delayBetweenMemberSpawns = 1;

    [Tooltip("X = Minimum number of waypoints between the start and end of this member's route. Y = Maximum. Defaults to (1, 3).")]
    [SerializeField] Vector2Int routeMidpointRange = Vector2Int.one * -1;

    [Header("Member Collections")]
    [SerializeField] GameObject memberContainer;
    [SerializeField] CrowdMember[] members;
    [SerializeField] List<Transform> memberSpawnPositions = new List<Transform>();
    Dictionary<Transform, float> spawnCooldowns = new Dictionary<Transform, float>();

#if UNITY_EDITOR
    [Header("! Editor Only !")]
    [SerializeField] bool showCooldowns = false;
    [SerializeField] float[] editor_SpawnCooldowns;
#endif

    private void Start()
    {
        //On startup, init the members array to have the amount of members specified,
        members = new CrowdMember[maxMemberAmount];

        //Sanitize the given route length range; default if less than 0,
        routeMidpointRange = routeMidpointRange.x < 0 || routeMidpointRange.y < 0 ? new Vector2Int(1, 3) : routeMidpointRange;
        //Swap if x is bigger than y
        if (routeMidpointRange.x > routeMidpointRange.y)
        {
            int swap = routeMidpointRange.x;
            routeMidpointRange.x = routeMidpointRange.y;
            routeMidpointRange.y = swap;
        }

        //Create an empty game object to put all the members in if one wasn't designated, for better inspector organization
        if (!memberContainer)
            memberContainer = new GameObject("Crowd Members");

        for (int i = 0; i < maxMemberAmount; i++)
        {
            //Then make a new member for each element of the array.
            members[i] = Instantiate(memberPrefab, inactivePos, Quaternion.identity, memberContainer.transform);
            members[i].name = members[i].name.Replace("(Clone)", $"({i})");
        }

        EventManager.Instance.onBuildingBuilt += AddSpawnPosition;
        EventManager.Instance.onBuildingDestroyed += RemoveSpawnPosition;
        EventManager.Instance.onCrowdMemberReachedEnd += ResetCrowdMember;
    }

    private void Update()
    {
        foreach (Transform spawnPosition in memberSpawnPositions)
            spawnCooldowns[spawnPosition] -= Time.deltaTime;

#if UNITY_EDITOR
        if (showCooldowns) { editor_SpawnCooldowns = spawnCooldowns.Values.ToArray(); }
#endif
    }

    private void AddSpawnPosition(Building posObject)
    {
        if (!memberSpawnPositions.Contains(posObject.CrowdEntryPosition))
            memberSpawnPositions.Add(posObject.CrowdEntryPosition);

        if (!spawnCooldowns.ContainsKey(posObject.CrowdEntryPosition))
            spawnCooldowns.Add(posObject.CrowdEntryPosition, delayBetweenMemberSpawns);
    }

    private void RemoveSpawnPosition(Building posObject)
    {
        if (memberSpawnPositions.Contains(posObject.CrowdEntryPosition))
            memberSpawnPositions.Remove(posObject.CrowdEntryPosition);

        if (spawnCooldowns.ContainsKey(posObject.CrowdEntryPosition))
            spawnCooldowns.Remove(posObject.CrowdEntryPosition);
    }

    private void ResetCrowdMember(CrowdMember member)
    {
        //If there aren't any spawn positions, there's nothing to do. Bail out early.
        if (memberSpawnPositions.Count <= 0) { return; }

        //Get a random position to start the route at.
        Transform startPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];

        //If the spawn pos we've chosen as a start isn't on cooldown, continue. Otherwise, we're done here, just bail out
        if (spawnCooldowns[startPos] <= 0)
        {
            //Generate an array to represent a route. It has a start and end(the +2 at the end), plus a random number of mid points
            Transform[] newRoute = new Transform[Random.Range(routeMidpointRange.x, routeMidpointRange.y) + 2];
            //Set the start and end of the route
            newRoute[0] = startPos;
            newRoute[newRoute.Length - 1] = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];

            int lastIndex = -1;
            for (int i = 1; i < newRoute.Length - 1; i++)
            {
                //Choose a random waypoint to go to for this index of the route.
                //If the waypoint we choose is the same as the last one, choose a different one.
                int waypointIndex = lastIndex;
                while (waypointIndex == lastIndex)
                    waypointIndex = Random.Range(0, MainGame.Instance.crowdWaypoints.Length);

                newRoute[i] = MainGame.Instance.crowdWaypoints[waypointIndex];
                lastIndex = waypointIndex;
            }

            //Start the cooldown at this spawn position and reset the member's route in preparation for them to set out
            spawnCooldowns[newRoute[0]] = delayBetweenMemberSpawns;
            member.ResetRoute(newRoute);
        }

        ////Now that the route is constructed, wait until the cooldown on the start location is up
        //if (spawnCooldowns.ContainsKey(newRoute[0]) && newRoute[0] != null)
        //    yield return new WaitUntil(() => spawnCooldowns[newRoute[0]] <= 0);
        //else
        //    yield return null;
    }
}