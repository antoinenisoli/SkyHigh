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
    [SerializeField] float xPan = -800;
    Vector3 originPos;

    [Header("Charity Action mode")]
    [SerializeField] GameObject charityPanel;
    Vector3 charityPanelScale;
    CharityActionButton[] charityActionButtons;
    Slider[] sliders;

    [Header("Random Events")]
    [SerializeField] GameObject eventPanel;
    Vector3 eventPanelScale;
    [SerializeField] Image blackScreen;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (blackScreen)
        {
            blackScreen.DOFade(0, 0);
            blackScreen.gameObject.SetActive(false);
        }

        turnText.text = MainGame.Instance.turnCount + " turns left !";
        charityActionButtons = GetComponentsInChildren<CharityActionButton>();
        SetupEvents();
        NewDeck();
        SetupScales();
        UpdateUI();
    }

    void SetupScales()
    {
        originPos = buildPanel.transform.localPosition;

        turnPanelScale = turnButtons.transform.localScale;
        turnButtons.transform.localScale = Vector3.one * 0.0001f;

        charityPanelScale = charityPanel.transform.localScale;
        charityPanel.transform.localScale = Vector3.one * 0.0001f;

        eventPanelScale = eventPanel.transform.localScale;
        eventPanel.transform.localScale = Vector3.one * 0.0001f;
    }

    void SetupEvents()
    {
        EventManager.Instance.onNewAction.AddListener(NewAction);
        EventManager.Instance.onNewTurn.AddListener(NewTurn);
        EventManager.Instance.onCost.AddListener(UpdateUI);
        EventManager.Instance.onNewTurn.AddListener(UpdateUI);
        EventManager.Instance.onNewRandomEvent.AddListener(DisplayEvent);
    }

    void DisplayEvent(RandomEvent randomEvent)
    {
        blackScreen.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackScreen.DOFade(0.7f, 0.5f));
        float duration = 0.7f;
        sequence.Join(eventPanel.transform.DOScale(eventPanelScale, duration));
        sequence.Join(eventPanel.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetDelay(duration - 0.1f));
        sequence.Play();
    }

    public void ExitEventPanel()
    {
        Sequence sequence = DOTween.Sequence();
        float duration = 0.3f;
        sequence.Append(blackScreen.DOFade(0, duration));
        sequence.Join(eventPanel.transform.DOScale(Vector3.one * 0.0001f, duration));
        sequence.Play().SetDelay(2f).OnComplete(NewAction);
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
        blackScreen.gameObject.SetActive(false);
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
        displayGoal.text = "Goals : \n" + MainGame.Instance.MainGoal.ToString();

        sliders = statPanel.GetComponentsInChildren<Slider>();
        for (int i = 0; i < sliders.Length; i++)
        {
            Statistic stat = ResourceManager.Instance.stats[i];
            sliders[i].maxValue = stat.MaxAmount;
            sliders[i].DOValue(stat.CurrentAmount, 1);
            sliders[i].GetComponentInChildren<Text>().text = stat.CurrentAmount + "\n / " + "\n" + stat.MaxAmount;
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
                displayMode.text = "Pick a building !";
                buildPanel.transform.DOLocalMoveX(xPan, 0.7f);
                EventManager.Instance.onCost?.Invoke();
                break;
            case ModeType.ActionChoose:
                displayMode.text = "Make an action !";
                PanelAnim(charityPanel, true, charityPanelScale);
                NewDeck();
                break;
            case ModeType.Store:
                displayMode.text = "Choose an action to buy !";
                break;
        }
    }

    void NewDeck()
    {
        foreach (var item in charityActionButtons)
        {
            int random = Random.Range(0, MainGame.Instance.availableCharityActions.Count);
            item.Action = MainGame.Instance.availableCharityActions[random];
        }
    }

    public void EndTurn()
    {
        EventManager.Instance.onNewTurn.Invoke();
        PanelAnim(turnButtons, false, turnPanelScale);
    }

    public void FloatingText(int amount)
    {
        Text newText = Instantiate(floatingText, displayMoney.transform.position, Quaternion.identity, transform).GetComponent<Text>();
        displayMoney.transform.DOComplete();
        if (amount > 0)
        {
            newText.text = "+" + amount + " $";
            displayMoney.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        }
        else if (amount < 0)
        {
            newText.text = amount + " $";
            displayMoney.transform.DOPunchScale(Vector3.one * -0.2f, 0.3f);
        }

        float duration = 1.5f;
        newText.transform.DOMoveY(displayMoney.transform.position.y + 200, duration);
        newText.DOFade(0, duration).SetEase(Ease.Linear);
        Destroy(newText.gameObject, duration);
    }
}
