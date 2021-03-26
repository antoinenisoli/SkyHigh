using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LD/LevelData")]
public class LevelData : ScriptableObject
{
    public Goal MainGoal;

    [Header("Charity Actions")]
    public List<CharityAction> allCharityActions = new List<CharityAction>();
    public List<CharityAction> availableCharityActions = new List<CharityAction>();

    [Header("Generate Grid")]
    public GameObject gridCell;

    [Header("Turns")]
    public float waitTurn = 4f;
    public int turnStartCount = 10;

    [Header("Random events")]
    public List<RandomEvent> allRandomEvents = new List<RandomEvent>();
}
