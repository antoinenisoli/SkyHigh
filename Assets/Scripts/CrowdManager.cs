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

    private void AddSpawnPosition(Building posObject) { memberSpawnPositions.Add(posObject.CrowdEntryPosition); }
    private void RemoveSpawnPosition(Building posObject) { memberSpawnPositions.Remove(posObject.CrowdEntryPosition); }

    private void StartResetCrowdMember(CrowdMember member) { StartCoroutine(ResetCrowdMember(member)); }
    private IEnumerator ResetCrowdMember(CrowdMember member)
    {
        //Wait until there is at least one spawn position.
        yield return new WaitUntil(() => memberSpawnPositions.Count > 0);

        Transform startPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];
        Transform endPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];

        member.ResetRoute(startPos, endPos);
    }
}
