using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Text displayMoney;
    [SerializeField] GameObject floatingText;

    [Header("Turn actions")]
    [SerializeField] Text displayMode;
    [SerializeField] Text turnText;
    [SerializeField] Text turnActionText;
    [SerializeField] GameObject turnButtons;
    Vector3 scale;

    [Header("Build mode")]
    [SerializeField] GameObject buildPanel;
    Vector3 originPos;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        EventManager.Instance.onNewAction.AddListener(NewAction);
        EventManager.Instance.onNewTurn.AddListener(NewTurn);
        EventManager.Instance.onCost.AddListener(UpdateUI);
        turnText.text = MainGame.Instance.turnCount + " turns left !";

        scale = turnButtons.transform.localScale;
        turnButtons.transform.localScale = Vector3.one * 0.0001f;

        originPos = buildPanel.transform.localPosition;
    }

    void ActionPanelAnim(bool grow)
    {
        turnButtons.transform.DOComplete();
        float duration = 0.7f;
        Sequence sequence = DOTween.Sequence();
        Vector3 newScale = grow ? scale : Vector3.one * 0.0001f;
        sequence.Append(turnButtons.transform.DOScale(newScale, duration).SetEase(Ease.Linear));
        if (grow)
            sequence.Join(turnButtons.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetDelay(duration - 0.1f));

        sequence.Play();
    }

    public void NewAction()
    {
        ActionPanelAnim(true);
        displayMode.text = "Choose an action !";
        buildPanel.transform.DOLocalMoveX(originPos.x, 2f);
    }

    public void NewTurn()
    {
        displayMode.text = "...";
        buildPanel.transform.DOLocalMoveX(originPos.x, 2f);
    }

    public void UpdateUI()
    {
        displayMoney.text = ResourceManager.Instance.Money.CurrentAmount + " $";
        turnActionText.text = MainGame.Instance.CurrentTurn.actionsCount + " points left";
        turnText.text = MainGame.Instance.turnCount + " turns left !";
    }

    public void NewMode(int type)
    {
        ActionPanelAnim(false);
        ModeType mode = (ModeType)type;
        MainGame.Instance.SetMode(mode);
        turnText.text = MainGame.Instance.turnCount + " turns left !";
        switch (mode)
        {
            case ModeType.Building:
                displayMode.text = "Build mode !";
                buildPanel.transform.DOLocalMoveX(-800, 0.7f);
                break;
            case ModeType.ActionChoose:
                displayMode.text = "Make an action !";
                break;
            case ModeType.Store:
                displayMode.text = "Choose an action to buy !";
                break;
        }
    }

    public void EndTurn()
    {
        EventManager.Instance.onNewTurn.Invoke();
        ActionPanelAnim(false);
    }

    public void FloatingText(string txt)
    {
        Text newText = Instantiate(floatingText, displayMoney.transform.position, Quaternion.identity, transform).GetComponent<Text>();
        newText.text = txt;
        float duration = 3;
        newText.transform.DOMoveY(displayMoney.transform.position.y + 200, duration);
        Destroy(newText.gameObject, duration);
    }
}
