using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TradeFilter : MonoBehaviour
{
    [SerializeField] private InputField dateInput;
    [SerializeField] private InputField assetNameInput;
    [SerializeField] private GameObject[] typeOperationIcons;
    [SerializeField]
    private Dictionary<TypeOperation, bool> typeOperation = new Dictionary<TypeOperation, bool>
    {
        { TypeOperation.buy, false },
        { TypeOperation.sell, false }
    };
    [SerializeField] private InputField currencyAmountInput;
    [SerializeField] private InputField pricePerUnitInput;
    [SerializeField] private Animator emotionAnim;
    [SerializeField]
    private Dictionary<TypeEmotion, bool> typeEmotion = new Dictionary<TypeEmotion, bool>
    {
        { TypeEmotion.VeryAngry, false },
        { TypeEmotion.Angry, false },
        { TypeEmotion.Neutral, false },
        { TypeEmotion.Happy, false },
        { TypeEmotion.VeryHappy, false }
    };
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Transform tradeParent;

    public Trades tradesManager;
    public List<TradeUI> filteredTrades = new List<TradeUI>();
    private List<TradeUI> allTradeInstances = new List<TradeUI>();
    private Coroutine emotionCoroutine;
    private DateInputHandler dateHandler;

    public event Action OnFilterRebuild;

    void Awake()
    {
        dateHandler = new DateInputHandler(dateInput);
        dateInput.onValueChanged.AddListener(OnDateChanged);
        if (tradesManager != null)
        {
            tradesManager.AddTrade += RebuildFilter;
            tradesManager.DelTrade += RebuildFilter;
            allTradeInstances.AddRange(tradesManager.InstanceTrades.Where(t => t != null)); // Инициализация из текущих данных
            Debug.Log($"Initialized with {allTradeInstances.Count} trade instances");
        }

        saveButton.onClick.AddListener(ApplyFullFilter);
        resetButton.onClick.AddListener(ResetFilter);

        ResetFilter();
        emotionAnim.SetFloat("line", 0f);
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        RebuildFilter(null);
    }

    private void OnDateChanged(string input)
    {
        string digitsOnly = Regex.Replace(input, @"\D", "");
        if (digitsOnly.Length == 8)
        {
            ApplyDateFilter();
        }
    }

    void ApplyDateFilter()
    {
        foreach (TradeUI tradeUI in allTradeInstances)
        {
            if (tradeUI != null && tradeUI.gameObject != null)
            {
                tradeUI.gameObject.SetActive(false);
            }
        }

        filteredTrades.Clear();
        DateTime? filterDate = ParseDate(dateInput.text);

        foreach (TradeUI tradeUI in allTradeInstances)
        {
            if (tradeUI == null || tradeUI.currentInfo == null) continue;

            InfoTrade trade = tradeUI.currentInfo;
            if (!filterDate.HasValue || trade.date == filterDate.Value)
            {
                filteredTrades.Add(tradeUI);
                tradeUI.gameObject.SetActive(true);
            }
        }

        filteredTrades.Sort((a, b) => b.currentInfo.date.CompareTo(a.currentInfo.date));
        ReorderTradeUI();
    }

    void ApplyFullFilter()
    {
        DateTime? filterDate = ParseDate(dateInput.text);
        string filterAssetName = assetNameInput.text.ToLower();
        var selectedOperations = typeOperation.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        double? filterAmount = string.IsNullOrEmpty(currencyAmountInput.text) ? (double?)null : double.Parse(currencyAmountInput.text);
        double? filterPrice = string.IsNullOrEmpty(pricePerUnitInput.text) ? (double?)null : double.Parse(pricePerUnitInput.text);
        var selectedEmotions = typeEmotion.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        // Скрываем все трейды сначала
        foreach (TradeUI tradeUI in allTradeInstances)
        {
            if (tradeUI != null && tradeUI.gameObject != null)
            {
                tradeUI.gameObject.SetActive(false);
            }
        }

        filteredTrades.Clear();

        // Перебираем все трейды и применяем фильтры
        foreach (TradeUI tradeUI in allTradeInstances)
        {
            if (tradeUI == null || tradeUI.currentInfo == null) continue;

            InfoTrade trade = tradeUI.currentInfo;
            bool matches = true;

            // Проверка на дату
            if (filterDate.HasValue && trade.date != filterDate.Value) matches = false;

            // Проверка на название актива
            if (!string.IsNullOrEmpty(filterAssetName) && !trade.nameTrade.ToLower().Contains(filterAssetName)) matches = false;

            // Проверка на тип операции
            if (selectedOperations.Count > 0 && !selectedOperations.Contains(trade.typeOperation)) matches = false;

            // Проверка на количество валюты
            if (filterAmount.HasValue && trade.count != filterAmount.Value) matches = false;

            // Проверка на цену за единицу
            if (filterPrice.HasValue && trade.price != filterPrice.Value) matches = false;

            // Проверка на эмоцию
            if (selectedEmotions.Count > 0 && !selectedEmotions.Contains(trade.typeEmotion)) matches = false;

            // Если все фильтры пройдены, добавляем трейд в список и показываем его
            if (matches)
            {
                filteredTrades.Add(tradeUI);
                tradeUI.gameObject.SetActive(true);
            }
        }

        // Сортируем трейды по дате
        filteredTrades.Sort((a, b) => b.currentInfo.date.CompareTo(a.currentInfo.date));

        // Переставляем объекты в UI (по порядку)
        ReorderTradeUI();
        
    }

    void ResetFilter()
    {
        dateInput.text = "";
        assetNameInput.text = "";
        foreach (var operation in typeOperation.Keys.ToList()) typeOperation[operation] = false;
        UpdateOperationIcons();
        currencyAmountInput.text = "";
        pricePerUnitInput.text = "";
        foreach (var emotion in typeEmotion.Keys.ToList()) typeEmotion[emotion] = false;

        if (emotionAnim != null) emotionAnim.SetFloat("line", 0f);

        filteredTrades.Clear();
        foreach (TradeUI tradeUI in allTradeInstances)
        {
            if (tradeUI != null && tradeUI.gameObject != null)
            {
                tradeUI.gameObject.SetActive(true);
                filteredTrades.Add(tradeUI);
            }
        }

        filteredTrades.Sort((a, b) => b.currentInfo.date.CompareTo(a.currentInfo.date));
        ReorderTradeUI();
    }

    void ReorderTradeUI()
    {
        for (int i = 0; i < filteredTrades.Count; i++)
        {
            if (filteredTrades[i] != null && filteredTrades[i].transform != null)
            {
                filteredTrades[i].transform.SetSiblingIndex(i);
            }
        }
    }

    void RebuildFilter(InfoTrade trade)
    {
        allTradeInstances.Clear();
        allTradeInstances.AddRange(tradesManager.InstanceTrades.Where(t => t != null));
        Debug.Log($"RebuildFilter: Updated {allTradeInstances.Count} trade instances, trade: {trade?.nameTrade ?? "null"}");

        ApplyFullFilter();
        OnFilterRebuild?.Invoke();
    }

    DateTime? ParseDate(string dateStr)
    {
        if (string.IsNullOrEmpty(dateStr) || dateStr.Length < 8) return null;
        string digitsOnly = Regex.Replace(dateStr, @"\D", "");
        if (digitsOnly.Length != 8) return null;

        if (DateTime.TryParseExact(digitsOnly, "MMddyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
        {
            return date;
        }
        return null;
    }

    public void SelectTypeOperation(int selectedType)
    {
        var operations = new List<TypeOperation>(typeOperation.Keys);
        foreach (var operation in operations) typeOperation[operation] = false;
        typeOperation[(TypeOperation)selectedType] = true;
        UpdateOperationIcons();
        Debug.Log("SelectTypeOperation: Эмоции не должны меняться, текущее состояние: " + GetSelectedEmotion());
    }

    public void SelectTypeEmotion(int selectedEmotion)
    {
        var emotions = new List<TypeEmotion>(typeEmotion.Keys);
        foreach (var emotion in emotions) typeEmotion[emotion] = false;
        TypeEmotion selected = (TypeEmotion)selectedEmotion;
        typeEmotion[selected] = true;

        if (emotionCoroutine != null) StopCoroutine(emotionCoroutine);
        emotionCoroutine = StartCoroutine(UpdateEmotionAnimation(selected));
        Debug.Log("SelectTypeEmotion: Выбрана эмоция: " + selected);
    }

    private IEnumerator UpdateEmotionAnimation(TypeEmotion selectedEmotion)
    {
        float targetValue = selectedEmotion switch
        {
            TypeEmotion.VeryAngry => 0f,
            TypeEmotion.Angry => 1f,
            TypeEmotion.Neutral => 2f,
            TypeEmotion.Happy => 3f,
            TypeEmotion.VeryHappy => 4f,
            _ => 2f
        };

        float startValue = emotionAnim.GetFloat("line");
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentValue = Mathf.Lerp(startValue, targetValue, t);
            emotionAnim.SetFloat("line", currentValue);
            yield return null;
        }

        emotionAnim.SetFloat("line", targetValue);
    }

    private void UpdateOperationIcons()
    {
        bool anySelected = typeOperation.Any(kvp => kvp.Value);
        for (int i = 0; i < typeOperationIcons.Length; i++)
        {
            typeOperationIcons[i].SetActive(anySelected && (i == (typeOperation.FirstOrDefault(kvp => kvp.Value).Key == TypeOperation.buy ? 0 : 1)));
        }
    }

    private TypeOperation GetSelectedOperation()
    {
        var selected = typeOperation.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        return selected.Count > 0 ? selected[0] : TypeOperation.buy;
    }

    private TypeEmotion GetSelectedEmotion()
    {
        var selected = typeEmotion.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        return selected.Count > 0 ? selected[0] : TypeEmotion.Neutral;
    }

    public void TriggerRebuild()
    {
        RebuildFilter(null);
    }
}