using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Text displayMode;
    [SerializeField] GameObject floatingText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        displayMode.text = MainGame.Instance.CurrentTurn.currentAction;
    }

    public void FloatingText(Vector3 pos, string txt)
    {
        Text newText = Instantiate(floatingText, pos, Quaternion.identity, transform).GetComponent<Text>();
        newText.text = txt;
        newText.transform.DOMoveY(pos.y + 200, 3);
    }
}
