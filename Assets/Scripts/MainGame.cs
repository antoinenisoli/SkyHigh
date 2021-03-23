using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] GameObject cell;
    [SerializeField] Vector2 gridSize;

    private void Awake()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                GameObject newCell = Instantiate(cell, transform);
                newCell.transform.localPosition = new Vector3(i, 0, j);
            }
        }
    }
}
