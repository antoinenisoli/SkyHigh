using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class Cell : MonoBehaviour
{
    [SerializeField] LayerMask surfaceLayer;
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
        if (EventSystem.current.IsPointerOverGameObject() || full || !MainGame.Instance.BuildingPrefab)
            return;

        value = 1;
        highlight = !highlight;
    }

    private void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject() || full || !MainGame.Instance.BuildingPrefab)
            return;

        highlight = !highlight;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || full || !MainGame.Instance.BuildingPrefab)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, surfaceLayer))
            Build(hit.point);
    }

    private void Build(Vector3 pos)
    {
        Vector3 position = transform.position;
        position.y = pos.y;
        MainGame.Instance.PlaceBuilding(position);
        full = true;
        SoundManager.Instance.PlayAudio("click-casualbuilding");
    }

    private void Update()
    {
        value = highlight && !full && !EventSystem.current.IsPointerOverGameObject() ? 1 : 0;
        myMat.SetFloat("_Opacity", value);
    }
}
