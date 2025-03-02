using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class DynamicBarChart : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private float maxHeight = 100f;
    [SerializeField] private float maxSpacing = 50f;
    [SerializeField] private int manualMaxColumns = 15;

    [SerializeField] private List<BarData> barValues = new();
    [SerializeField] private Text[] valueLabels = new Text[4];
    [SerializeField] private InputField startDateInput;
    [SerializeField] private InputField endDateInput;
    [SerializeField] private Text[] monthLabels = new Text[5];

    private Vector3[] originalPositions;
    private float barWidth;

    private DateInputHandler startDateHandler;
    private DateInputHandler endDateHandler;

    [System.Serializable]
    public class BarData
    {
        public DateTime date;
        public float value;
    }
    public static DynamicBarChart Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Получение текста дат
    public string GetStartDateText()
    {
        return startDateInput != null ? startDateInput.text?.Trim() ?? "" : "";
    }

    public string GetEndDateText()
    {
        return endDateInput != null ? endDateInput.text?.Trim() ?? "" : "";
    }

    private void Start()
    {
        gameObject.SetActive(true);
        // Инициализация ширины столбца
        barWidth = barPrefab?.GetComponent<RectTransform>()?.rect.width ?? 10f;

        // Сохранение исходных позиций меток
        originalPositions = new Vector3[monthLabels.Length];
        for (int i = 0; i < monthLabels.Length; i++)
        {
            if (monthLabels[i] != null)
            {
                originalPositions[i] = monthLabels[i].transform.localPosition;
            }
        }

        startDateHandler = new DateInputHandler(startDateInput);
        endDateHandler = new DateInputHandler(endDateInput);

        startDateInput.onValueChanged.AddListener(OnDateInputChanged);
        endDateInput.onValueChanged.AddListener(OnDateInputChanged);

        InitializeFromTrades();
        UpdateDatePlaceholders();

        if (Trades.Instance != null)
        {
            Trades.Instance.AddTrade += OnTradeChanged;
            Trades.Instance.DelTrade += OnTradeChanged;
        }

        CreateBars();
        container.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Trades.Instance != null)
        {
            Trades.Instance.AddTrade -= OnTradeChanged;
            Trades.Instance.DelTrade -= OnTradeChanged;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        CreateBars();
    }

    private void OnDateInputChanged(string input)
    {
        CreateBars();
    }

    // Проверка валидности даты
    private bool IsDateValid(string dateText)
    {
        container.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(dateText) || dateText.Length != 10 || dateText[2] != '.' || dateText[5] != '.')
            return false;
        return DateTime.TryParseExact(dateText, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out _);
    }

    // Инициализация данных из сделок
    private void InitializeFromTrades()
    {
        container.gameObject.SetActive(true);
        if (Trades.Instance == null) return;

        barValues.Clear();
        foreach (var trade in Trades.Instance.trades)
        {
            barValues.Add(new BarData { date = trade.date, value = (float)trade.price });
        }
        UpdateDatePlaceholders();
        container.gameObject.SetActive(true);
    }

    // Обновление placeholder дат
    private void UpdateDatePlaceholders()
    {
        container.gameObject.SetActive(true);
        if (barValues.Count == 0) return;

        DateTime minDate = barValues.Min(b => b.date);
        DateTime maxDate = barValues.Max(b => b.date);

        if (startDateInput != null && string.IsNullOrEmpty(startDateInput.text))
            startDateInput.placeholder.GetComponent<Text>().text = minDate.ToString("MM.dd.yyyy");

        if (endDateInput != null && string.IsNullOrEmpty(endDateInput.text))
            endDateInput.placeholder.GetComponent<Text>().text = maxDate.ToString("MM.dd.yyyy");
        container.gameObject.SetActive(true);
    }

    private void OnTradeChanged(InfoTrade trade)
    {
        InitializeFromTrades();
        CreateBars();
        container.gameObject.SetActive(true);
    }

    // Создание столбцов диаграммы
    private void CreateBars()
    {
        container.gameObject.SetActive(true);
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (barValues.Count == 0 || container == null)
            return;

        List<BarData> filteredBars = FilterBarsByDateRange();
        if (filteredBars.Count == 0)
            filteredBars = new List<BarData>(barValues);

        UpdateDatePlaceholders();

        float globalMax = filteredBars.Max(data => data.value);
        int originalCount = filteredBars.Count;
        float containerWidth = container.rect.width;
        float containerHeight = container.rect.height;

        List<BarData> displayBars;
        float spacing;

        if (originalCount <= manualMaxColumns)
        {
            displayBars = new List<BarData>(filteredBars);
            spacing = (displayBars.Count > 1) ? (containerWidth - displayBars.Count * barWidth) / (displayBars.Count - 1) : 0f;
        }
        else
        {
            displayBars = new List<BarData>();
            int groups = manualMaxColumns;
            int baseGroupSize = originalCount / groups;
            int remainder = originalCount % groups;

            int index = 0;
            for (int i = 0; i < groups; i++)
            {
                int groupSize = baseGroupSize + (i < remainder ? 1 : 0);
                float groupMax = 0f;
                DateTime groupDate = filteredBars[index].date;
                for (int j = 0; j < groupSize; j++)
                {
                    float currentValue = filteredBars[index].value;
                    if (j == 0 || currentValue > groupMax)
                        groupMax = currentValue;
                    index++;
                }
                displayBars.Add(new BarData { date = groupDate, value = groupMax });
            }
            spacing = maxSpacing;
        }

        int finalCount = displayBars.Count;
        float totalWidth = (finalCount > 1) ? (finalCount * barWidth + (finalCount - 1) * spacing) : barWidth;
        float startX = -containerWidth / 2 + barWidth / 2;

        for (int i = 0; i < finalCount; i++)
        {
            GameObject bar = Instantiate(barPrefab, transform);
            RectTransform barRect = bar.GetComponent<RectTransform>();
            float height = (globalMax > 0) ? (displayBars[i].value / globalMax) * maxHeight : 0f;
            barRect.sizeDelta = new Vector2(barWidth, height);
            float xPos = startX + i * (barWidth + spacing);
            barRect.localPosition = new Vector2(xPos, -containerHeight / 2);
        }

        SetupValueLabels(globalMax);
        UpdateMonthLabels(filteredBars);
    }

    // Фильтрация столбцов по диапазону дат
    private List<BarData> FilterBarsByDateRange()
    {
        List<BarData> filtered = new();

        if (string.IsNullOrEmpty(startDateInput.text) || string.IsNullOrEmpty(endDateInput.text) ||
            !IsDateValid(startDateInput.text) || !IsDateValid(endDateInput.text))
            return new List<BarData>(barValues);

        DateTime startDate = DateTime.ParseExact(startDateInput.text, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact(endDateInput.text, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);

        if (startDate > endDate)
            return filtered;

        foreach (BarData data in barValues)
        {
            if (data.date >= startDate && data.date <= endDate)
                filtered.Add(data);
        }

        return filtered;
    }

    // Настройка меток значений
    private void SetupValueLabels(float maxVal)
    {
        if (valueLabels.Length != 4) return;

        float valueStep = maxVal / 3f;
        for (int i = 0; i < 4; i++)
        {
            if (valueLabels[i] != null)
                valueLabels[i].text = (maxVal - i * valueStep).ToString("F1");
        }
    }
    private void Update()
    {
        if (!container.gameObject.activeInHierarchy)
        {
            container.gameObject.SetActive(true);
        }
    }
    // Обновление меток месяцев
    private void UpdateMonthLabels(List<BarData> filteredBars)
    {
        if (monthLabels == null || monthLabels.Length == 0) return;
        originalPositions = new Vector3[monthLabels.Length];
        // Сброс позиций меток
        for (int i = 0; i < monthLabels.Length; i++)
        {
            if (monthLabels[i] != null)
            {
                monthLabels[i].gameObject.SetActive(true);
                monthLabels[i].rectTransform.localPosition = originalPositions[i];
                monthLabels[i].gameObject.SetActive(false);
            }
        }

        if (filteredBars == null || filteredBars.Count == 0)
            return;

        var monthGroups = filteredBars
            .GroupBy(data => new DateTime(data.date.Year, data.date.Month, 1))
            .Select(g => new { Date = g.Key, TotalValue = g.Sum(x => x.value) })
            .OrderBy(g => g.Date)
            .ToList();

        if (monthGroups == null || monthGroups.Count == 0)
            return;

        int monthCount = monthGroups.Count;
        float containerWidth = container != null ? container.rect.width : 0f;
        if (containerWidth <= 0)
            return;

        float startX = -containerWidth / 2;
        string endDateText = endDateInput != null ? endDateInput.text?.Trim() ?? "" : "";
        if (string.IsNullOrEmpty(endDateText))
            endDateText = filteredBars.Max(b => b.date).ToString("MM.dd.yyyy");

        if (!IsDateValid(endDateText))
            return;

        DateTime endDate = DateTime.ParseExact(endDateText, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);

        // Обработка диапазона больше года
        DateTime minDate = filteredBars.Min(b => b.date);
        DateTime maxDate = filteredBars.Max(b => b.date);
        if ((maxDate.Year - minDate.Year) > 0 || (maxDate.Year - minDate.Year == 0 && maxDate.Month - minDate.Month >= 12))
        {
            var yearGroups = monthGroups.Select(g => g.Date.Year).Distinct().OrderBy(y => y).ToList();
            int maxYearsToShow = Math.Min(yearGroups.Count, 5);
            float yearStep = containerWidth / (maxYearsToShow > 0 ? maxYearsToShow - 1 : 1);

            for (int i = 0; i < Math.Min(maxYearsToShow, monthLabels.Length); i++)
            {
                if (monthLabels[i] != null)
                {
                    monthLabels[i].gameObject.SetActive(true);
                    Vector3 newPosition = monthLabels[i].transform.localPosition;
                    newPosition.x = startX + i * yearStep;
                    if (float.IsNaN(newPosition.x))
                        newPosition.x = startX;
                    monthLabels[i].transform.localPosition = newPosition;
                    monthLabels[i].text = yearGroups[i].ToString() + " y";
                }
            }
            return;
        }

        int labelsToShow = 0;
        List<int> indicesToShow = new();

        // Определение отображаемых месяцев
        switch (monthCount)
        {
            case 2:
                indicesToShow.AddRange(new[] { 0, 1 });
                labelsToShow = 2;
                break;
            case 3:
                indicesToShow.AddRange(new[] { 0, 1, 2 });
                labelsToShow = 3;
                break;
            case 4:
                indicesToShow.AddRange(new[] { 0, 1, 2 });
                labelsToShow = 3;
                break;
            case 5:
                indicesToShow.AddRange(new[] { 0, 1, 2, 3, 4 });
                labelsToShow = 5;
                break;
            default:
                indicesToShow.Add(0);
                labelsToShow = 1;
                break;
        }

        float step = labelsToShow > 1 ? containerWidth / (labelsToShow - 1) : containerWidth / 2f;

        for (int i = 0; i < Math.Min(labelsToShow, monthLabels.Length); i++)
        {
            int index = indicesToShow[i];
            if (monthLabels[i] != null && index < monthGroups.Count)
            {
                monthLabels[i].gameObject.SetActive(true);
                Vector3 newPosition = monthLabels[i].transform.localPosition;
                newPosition.x = startX + i * step;
                if (float.IsNaN(newPosition.x))
                    newPosition.x = startX;
                monthLabels[i].transform.localPosition = newPosition;
                monthLabels[i].text = monthGroups[index].Date.ToString("MMM", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}