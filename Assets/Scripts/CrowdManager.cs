using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [SerializeField] private CrowdMember memberPrefab = null;
    [SerializeField] [Range(0, 1000)] private int memberAmount = 50;
    [Tooltip("The position crowd members go to on startup, and when they are not used/inactive.")]
    [SerializeField] private Vector3 inactivePos = Vector3.up * -75;

    [SerializeField] private GameObject memberContainer;
    [SerializeField] private CrowdMember[] members;
    [SerializeField] private List<Vector3> memberSpawnPositions;

    private void Start()
    {
        //On startup, init the members array to have the amount of members specified,
        members = new CrowdMember[memberAmount];
        //Create an empty game object to put all the member's in, for better inspector organization
        memberContainer = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        memberContainer.name = "Crowd Members";
        for (int i = 0; i < memberAmount; i++)
        {
            //Then make a new member for each element of the array.
            members[i] = Instantiate(memberPrefab, inactivePos, Quaternion.identity, memberContainer.transform);
        }

        memberSpawnPositions = new List<Vector3>();

        EventManager.Instance.onBuildingBuilt += AddSpawnPosition;
        EventManager.Instance.onCrowdMemberReachedEnd += StartResetCrowdMember;
    }
    private void OnDestroy()
    {
        EventManager.Instance.onBuildingBuilt -= AddSpawnPosition;
        EventManager.Instance.onCrowdMemberReachedEnd -= StartResetCrowdMember;
    }

    private void AddSpawnPosition(GameObject posObject) { memberSpawnPositions.Add(posObject.transform.position); }

    private void StartResetCrowdMember(CrowdMember member) { StartCoroutine(ResetCrowdMember(member)); }
    private IEnumerator ResetCrowdMember(CrowdMember member)
    {
        //Wait until there is at least one spawn position.
        yield return new WaitUntil(() => memberSpawnPositions.Count > 0);

        Vector3 startPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];
        Vector3 endPos = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];

        member.ResetRoute(startPos, endPos);
    }
}
