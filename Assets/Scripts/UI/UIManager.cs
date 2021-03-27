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
    [SerializeField] Button backButton;

    [Header("Stats")]
    [SerializeField] Text hapinessDescription;
    [SerializeField] Text educationDescription;
    Slider[] statSliders;

    [Header("Turn actions")]
    [SerializeField] Text displayMode;
    [SerializeField] Text turnText;
    [SerializeField] Text turnActionText;
    [SerializeField] GameObject turnPanel;
    Vector3 turnPanelScale;

    [Header("Build mode")]
    [SerializeField] GameObject buildPanel;
    [SerializeField] float xPan = -800;
    Vector3 originPos;

    [Header("Charity Action mode")]
    [SerializeField] GameObject charityPanel;
    Vector3 charityPanelScale;
    CharityActionButton[] charityActionButtons;

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

        hapinessDescription.text =
            "<b>Hapiness</b> \n \n"
            + "Increase all money incomes by " 
            + ResourceManager.Instance.hapinessPercent 
            + "% of this stat value.";

        educationDescription.text =
            "<b>Education</b> \n \n "
            + "Defines the base money income per turn, which is "
            + ResourceManager.Instance.educationPercent + "% of this stat value. ";

        turnText.text = MainGame.Instance.turnCount + " turns left !";
        charityActionButtons = GetComponentsInChildren<CharityActionButton>();
        SetupEvents();
        NewDeck();
        SetupScales();
        UpdateUI();
    }

    void SetBlackScreen(bool b)
    {
        if (blackScreen)
        {
            blackScreen.DOFade(0.7f, 1f);
            blackScreen.gameObject.SetActive(true);
        }
    }

    void SetupScales()
    {
        originPos = buildPanel.transform.localPosition;

        turnPanelScale = turnPanel.transform.localScale;
        turnPanel.transform.localScale = Vector3.one * 0.0001f;

        charityPanelScale = charityPanel.transform.localScale;
        charityPanel.transform.localScale = Vector3.one * 0.0001f;

        eventPanelScale = eventPanel.transform.localScale;
        eventPanel.transform.localScale = Vector3.one * 0.0001f;
    }

    void SetupEvents()
    {
        EventManager.Instance.onEndGame.AddListener(SetBlackScreen);
        EventManager.Instance.onNewAction.AddListener(NewAction);
        EventManager.Instance.onNewTurn.AddListener(DoTurn);
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
        sequence.Join(eventPanel.transform.DOScale(eventPanelScale, duration).SetEase(Ease.InOutCubic));
        sequence.Play();
    }

    public void ExitEventPanel()
    {
        Sequence sequence = DOTween.Sequence();
        float duration = 0.3f;
        sequence.Append(blackScreen.DOFade(0, duration));
        sequence.Join(eventPanel.transform.DOScale(Vector3.one * 0.0001f, duration).SetEase(Ease.InOutCubic));
        sequence.Play().SetDelay(2f).OnComplete(NewAction);
    }

    void PanelAnim(GameObject obj, Vector3 targetScale, float delay = default)
    {
        obj.transform.DOComplete();
        float duration = 0.7f;
        obj.transform.DOScale(targetScale, duration).SetEase(Ease.InOutCubic).SetDelay(delay);
    }

    public void NewAction()
    {
        blackScreen.gameObject.SetActive(false);
        PanelAnim(charityPanel, Vector3.one * 0.0001f);
        buildPanel.transform.DOLocalMoveX(originPos.x, 0.5f).SetEase(Ease.InBack);
        NewMode(ModeType.ChooseBasicAction);
    }

    public void DoTurn()
    {
        NewMode(ModeType.ExecuteTurn);
        buildPanel.transform.DOLocalMoveX(originPos.x, 0.5f).SetEase(Ease.InBack);
    }

    public void UpdateUI()
    {
        displayMoney.text = ResourceManager.Instance.Money.CurrentAmount + "";
        turnActionText.text = MainGame.Instance.CurrentTurn.actionsCount + " points left";
        turnText.text = MainGame.Instance.turnCount + " turns left !";
        displayGoal.text = "Goals : \n" + MainGame.Instance.MainGoal.ToString();

        statSliders = statPanel.GetComponentsInChildren<Slider>();
        for (int i = 0; i < statSliders.Length; i++)
        {
            Statistic stat = ResourceManager.Instance.stats[i];
            statSliders[i].maxValue = stat.MaxAmount;
            if (statSliders[i].value != stat.CurrentAmount)
            {
                statSliders[i].DOValue(stat.CurrentAmount, 1);
                statSliders[i].transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            }
            
            statSliders[i].GetComponentInChildren<Text>().text = stat.CurrentAmount + "\n / " + "\n" + stat.MaxAmount;
        }
    }

    public void ExitCharityPanel()
    {
        foreach (var item in charityPanel.GetComponentsInChildren<Button>())
        {
            item.interactable = false;
        }

        NewAction();
    }

    public void NewMode(ModeType type)
    {
        if (type != ModeType.ChooseBasicAction)
            PanelAnim(turnPanel, Vector3.one * 0.0001f);
        else
            PanelAnim(turnPanel, turnPanelScale);

        MainGame.Instance.SetMode(type);
        turnText.text = MainGame.Instance.turnCount + " turns left !";
        backButton.interactable = true;
        switch (type)
        {
            case ModeType.Building:
                displayMode.text = "Pick a building !";
                buildPanel.transform.DOLocalMoveX(xPan, 0.7f).SetEase(Ease.OutExpo);
                EventManager.Instance.onCost?.Invoke();
                break;
            case ModeType.Charity:
                displayMode.text = "Make a Charity Action !";
                PanelAnim(charityPanel, charityPanelScale);
                break;
            case ModeType.Store:
                displayMode.text = "Pick an action to buy !";
                break;
            case ModeType.ChooseBasicAction:
                displayMode.text = "Choose an action!";
                break;
            case ModeType.ExecuteTurn:
                displayMode.text = "Execute turn...";
                backButton.interactable = false;
                break;
        }
    }

    void NewDeck()
    {
        List<CharityAction> allActions = MainGame.Instance.allCharityActions;
        for (int i = 0; i < charityActionButtons.Length; i++)
        {
            bool good = false;
            while (!good)
            {
                int random = Random.Range(0, allActions.Count);
                CharityAction randomAction = allActions[random];
                if (allActions.Contains(randomAction))
                {
                    good = true;
                    charityActionButtons[i].Action = randomAction;
                    allActions.Remove(randomAction);
                }
            }
        }
    }

    public void EndTurn()
    {
        EventManager.Instance.onNewTurn.Invoke();
        NewDeck();
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

        float duration = 3f;
        newText.transform.DOMoveY(displayMoney.transform.position.y + 200, duration);
        newText.DOFade(0, duration).SetEase(Ease.Linear);
        Destroy(newText.gameObject, duration);
    }
}
