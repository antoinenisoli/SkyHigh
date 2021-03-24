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
    [SerializeField] GameObject statPanel;
    [SerializeField] Text displayGoal;

    [Header("Turn actions")]
    [SerializeField] Text displayMode;
    [SerializeField] Text turnText;
    [SerializeField] Text turnActionText;
    [SerializeField] GameObject turnButtons;
    Vector3 turnPanelScale;

    [Header("Build mode")]
    [SerializeField] GameObject buildPanel;
    Vector3 originPos;

    [Header("Charity Action mode")]
    [SerializeField] GameObject charityPanel;
    Vector3 charityPanelScale;

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

        turnPanelScale = turnButtons.transform.localScale;
        turnButtons.transform.localScale = Vector3.one * 0.0001f;

        originPos = buildPanel.transform.localPosition;
        charityPanelScale = charityPanel.transform.localScale;
        charityPanel.transform.localScale = Vector3.one * 0.0001f;

        UpdateUI();
    }

    void PanelAnim(GameObject obj, bool grow, Vector3 baseScale, float delay = default(float))
    {
        obj.transform.DOComplete();
        float duration = 0.7f;
        Sequence sequence = DOTween.Sequence();
        Vector3 newScale = grow ? baseScale : Vector3.one * 0.0001f;
        sequence.Append(obj.transform.DOScale(newScale, duration).SetEase(Ease.Linear));
        if (grow)
            sequence.Join(obj.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetDelay(duration - 0.1f));

        sequence.Play().SetDelay(delay);
    }

    public void NewAction()
    {
        PanelAnim(turnButtons, true, turnPanelScale);
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
        displayGoal.text = 
            "Goal : " + MainGame.Instance.MainGoal.stat.ToString() + 
            " needs to be " + MainGame.Instance.MainGoal.condition.ToString().ToLower() + 
            " than " + MainGame.Instance.MainGoal.compare.ToString().ToLower();

        Slider[] sliders = statPanel.GetComponentsInChildren<Slider>();
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maxValue = ResourceManager.Instance.stats[i].MaxAmount;
            sliders[i].value = ResourceManager.Instance.stats[i].CurrentAmount;
        }
    }

    public void ExitCharityPanel()
    {
        foreach (var item in charityPanel.GetComponentsInChildren<Button>())
        {
            item.interactable = false;
        }

        PanelAnim(charityPanel, false, charityPanelScale, 0.8f);
        NewAction();
    }

    public void NewMode(int type)
    {
        PanelAnim(turnButtons, false, turnPanelScale);
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
                PanelAnim(charityPanel, true, charityPanelScale);
                break;
            case ModeType.Store:
                displayMode.text = "Choose an action to buy !";
                break;
        }
    }

    public void EndTurn()
    {
        EventManager.Instance.onNewTurn.Invoke();
        PanelAnim(turnButtons, false, turnPanelScale);
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
