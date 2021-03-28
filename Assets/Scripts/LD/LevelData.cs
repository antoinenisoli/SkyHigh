using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LD/LevelData")]
public class LevelData : ScriptableObject
{
    public Goal MainGoal;
    public GameObject gridCell;
    public int turnStartCount = 10;
    public List<RandomEvent> allRandomEvents = new List<RandomEvent>();
    public List<CharityAction> allCharityActions = new List<CharityAction>();
}
