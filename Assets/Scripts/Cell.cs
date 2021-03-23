using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Material myMat;
    float value = 1;
    bool highlight;
    bool full;

    private void Awake()
    {
        myMat = GetComponentInChildren<MeshRenderer>().material;
    }

    private void OnMouseEnter()
    {
        value = 1;
        highlight = !highlight;
    }

    private void OnMouseExit()
    {
        highlight = !highlight;
    }

    private void OnMouseDown()
    {
        if (!full)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 position = transform.position;
                position.y = hit.point.y;
                MainGame.Instance.PlaceBuilding(position);
                full = true;
            }
        }
    }

    private void Update()
    {
        value = highlight && !full ? 1 : 0;
        myMat.SetFloat("_Opacity", value);
    }
}
