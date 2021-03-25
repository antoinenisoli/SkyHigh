using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanel : MonoBehaviour
{
    [SerializeField] GameObject[] Buildings;
    BuildingButton[] buildButtons;

    private void Awake()
    {
        buildButtons = FindObjectsOfType<BuildingButton>();
        for (int i = 0; i < buildButtons.Length; i++)
        {
            if (i < Buildings.Length && Buildings[i])
                buildButtons[i].buildingPrefab = Buildings[i];
        }
    }
}
