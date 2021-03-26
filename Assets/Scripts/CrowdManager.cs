#if UNITY_EDITOR
using System.Linq;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [SerializeField] private CrowdMember memberPrefab = null;
    [SerializeField] [Range(0, 1000)] private int maxMemberAmount = 50;
    [Tooltip("The position crowd members go to on startup, and when they are not used/inactive.")]
    [SerializeField] private Vector3 inactivePos = Vector3.up * -75;
    [Tooltip("How long to wait before \"spawning\" a member at a spawn position")]
    [SerializeField] private float delayBetweenMemberSpawns = 1;

    [Header("Member Collections")]
    [SerializeField] private GameObject memberContainer;
    [SerializeField] private CrowdMember[] members;
    [SerializeField] private List<Transform> memberSpawnPositions;
    private Dictionary<Transform, float> spawnCooldowns;

#if UNITY_EDITOR
    [Header("! Editor Only !")]
    [SerializeField] private bool showCooldowns = false;
    [SerializeField] private float[] editor_SpawnCooldowns;
#endif

    private void Start()
    {
        //On startup, init the members array to have the amount of members specified,
        members = new CrowdMember[maxMemberAmount];
        //Create an empty game object to put all the member's in if one wasn't designated, for better inspector organization
        if (!memberContainer)
        {
            memberContainer = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            memberContainer.name = "Crowd Members";
        }

        for (int i = 0; i < maxMemberAmount; i++)
        {
            //Then make a new member for each element of the array.
            members[i] = Instantiate(memberPrefab, inactivePos, Quaternion.identity, memberContainer.transform);
            members[i].name = members[i].name.Replace("(Clone)", $"({i})");
        }

        memberSpawnPositions = new List<Transform>();
        spawnCooldowns = new Dictionary<Transform, float>();

        EventManager.Instance.onBuildingBuilt += AddSpawnPosition;
        EventManager.Instance.onBuildingDestroyed += RemoveSpawnPosition;
        EventManager.Instance.onCrowdMemberReachedEnd += StartResetCrowdMember;
    }
    private void OnDestroy()
    {
        EventManager.Instance.onBuildingBuilt -= AddSpawnPosition;
        EventManager.Instance.onBuildingDestroyed -= RemoveSpawnPosition;
        EventManager.Instance.onCrowdMemberReachedEnd -= StartResetCrowdMember;
    }


    private void Update()
    {
        foreach (Transform spawnPosition in memberSpawnPositions)
        {
            spawnCooldowns[spawnPosition] -= Time.deltaTime;
        }

#if UNITY_EDITOR
        if (showCooldowns) { editor_SpawnCooldowns = spawnCooldowns.Values.ToArray(); }
#endif
    }


    private void AddSpawnPosition(Building posObject)
    {
        memberSpawnPositions.Add(posObject.CrowdEntryPosition);
        spawnCooldowns.Add(posObject.CrowdEntryPosition, delayBetweenMemberSpawns);
    }
    private void RemoveSpawnPosition(Building posObject)
    {
        memberSpawnPositions.Remove(posObject.CrowdEntryPosition);
        spawnCooldowns.Remove(posObject.CrowdEntryPosition);
    }

    private void StartResetCrowdMember(CrowdMember member) { StartCoroutine(ResetCrowdMember(member)); }
    private IEnumerator ResetCrowdMember(CrowdMember member)
    {
        //Wait until there is at least one spawn position.
        yield return new WaitUntil(() => memberSpawnPositions.Count > 0);

        Transform startPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];
        Transform endPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];

        //Now that the route is constructed, wait until the cooldown on this location is up
        yield return new WaitUntil(() => spawnCooldowns[startPos] <= 0);

        //Start the cooldown at this spawn position and reset the member's route in preparation for them to set out
        spawnCooldowns[startPos] = delayBetweenMemberSpawns;
        member.ResetRoute(startPos, endPos);
    }
}
