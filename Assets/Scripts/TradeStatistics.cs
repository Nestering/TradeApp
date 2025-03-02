using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;

public class TradeStatistics : MonoBehaviour
{
    [SerializeField] private Text successfulTradesText;
    [SerializeField] private Text unsuccessfulTradesText;
    [SerializeField] private Text lastSevenDaysNetAmountText;
    [SerializeField] private Text lastSevenDaysRevenueAmountText;
    [SerializeField] private Text totalTradesLastWeekText;
    [SerializeField] private Animator emotionAnimator;

    [SerializeField] private Trades trades; // ������ ������ ������ Instance

    private float pendingEmotionValue = -1f; // ������ ���������� �������� ������, -1 = ��� ���������� �����

    private void Awake()
    {
        if (successfulTradesText == null || unsuccessfulTradesText == null || lastSevenDaysNetAmountText == null ||
            lastSevenDaysRevenueAmountText == null || totalTradesLastWeekText == null)
        {
            Debug.LogWarning("���� ��� ��������� ��������� ����� ���������� �� ���������!");
        }
        if (emotionAnimator == null)
        {
            Debug.LogWarning("Emotion Animator �� ��������!");
        }

        if (trades != null)
        {
            trades.AddTrade += UpdateStatistics;
            trades.DelTrade += UpdateStatistics;
            Debug.Log("�������� �� ������� Trades ������� ���������");
        }
        else
        {
            Debug.LogError("Trades �� �������� � Inspector!");
        }
        gameObject.SetActive(false);
    }
    private void Start()
    {
        UpdateStatistics(trades != null && trades.trades != null && trades.trades.Count > 0 ? trades.trades[0] : null);
    }
    private void OnDestroy()
    {
        if (trades != null)
        {
            trades.AddTrade -= UpdateStatistics;
            trades.DelTrade -= UpdateStatistics;
        }
    }
    private void OnEnable()
    {
        UpdateStatistics(trades != null && trades.trades != null && trades.trades.Count > 0 ? trades.trades[0] : null);
    }
    private void UpdateStatistics(InfoTrade trade)
    {
        if (trades == null || trades.trades == null)
        {
            Debug.LogError("Trades ��� trades ����� null!");
            return;
        }

        Debug.Log($"���������� ����������. ���������� �������: {trades.trades.Count}");
        foreach (var t in trades.trades)
        {
            Debug.Log($"�����: {t.nameTrade}, ������: {t.typeEmotion}, ���: {t.typeOperation}, ����������: {t.successful}, ����: {t.date}");
        }

        // ���������� ���������� ��������/���������� ������
        if (successfulTradesText != null && unsuccessfulTradesText != null)
        {
            int successfulCount = trades.trades.Count(t => t.successful);
            int unsuccessfulCount = trades.trades.Count(t => !t.successful);
            successfulTradesText.text = $"{successfulCount}";
            unsuccessfulTradesText.text = $"{unsuccessfulCount}";
            Debug.Log($"Successful: {successfulCount}, Unsuccessful: {unsuccessfulCount}");
        }

        // ���������� �� ��������� 7 ����
        if (lastSevenDaysNetAmountText != null && lastSevenDaysRevenueAmountText != null && totalTradesLastWeekText != null)
        {
            var lastSevenDaysTrades = trades.trades
                .Where(t => t.date >= DateTime.Now.AddDays(-7) && t.date <= DateTime.Now)
                .ToList();

            double netAmount = lastSevenDaysTrades.Sum(t => t.count * t.price);
            double revenueAmount = 0;

            if (lastSevenDaysTrades.Count > 0)
            {
                var successfulTrades = lastSevenDaysTrades.Where(t => t.successful).ToList();
                if (successfulTrades.Count > 0)
                {
                    var groupedTrades = successfulTrades.GroupBy(t => t.nameTrade);
                    foreach (var group in groupedTrades)
                    {
                        var buys = group.Where(t => t.typeOperation == TypeOperation.buy).ToList();
                        var sells = group.Where(t => t.typeOperation == TypeOperation.sell).ToList();

                        if (buys.Count > 0 && sells.Count > 0)
                        {
                            double totalBuy = buys.Sum(t => t.count * t.price);
                            double totalSell = sells.Sum(t => t.count * t.price);
                            revenueAmount += totalSell - totalBuy;
                        }
                    }
                }
            }

            int totalTradesCount = lastSevenDaysTrades.Count;
            totalTradesLastWeekText.text = $"{totalTradesCount}";
            lastSevenDaysNetAmountText.text = $"{netAmount:F2}";
            lastSevenDaysRevenueAmountText.text = $"{revenueAmount:F2}";
            Debug.Log($"����� ����� �� ��������� 7 ����: {netAmount}, �������: {revenueAmount}, ���-�� ������: {totalTradesCount}");
        }

        // ���������� ������
        List<InfoTrade> emotionTrades = GetFilteredTrades();
        if (emotionAnimator != null && emotionTrades.Count > 0)
        {
            float averageEmotion = CalculateAverageEmotion(emotionTrades);
            Debug.Log($"������ ��� ������: {string.Join(", ", emotionTrades.Select(t => $"{t.nameTrade} ({t.typeEmotion})"))}");
            Debug.Log($"������� �������� ������: {averageEmotion}");
            // ������������� ��������� �������� �����, ���� ���� ������ ��������� ��� ��������� �������������
            UpdateEmotionAnimator(averageEmotion);
            if (!gameObject.activeInHierarchy)
            {
                pendingEmotionValue = averageEmotion; // ��������� ��� ������� ���������
                Debug.Log("������ ���������, �������� �������� � ���������: " + averageEmotion);
            }
        }
        else if (emotionAnimator != null)
        {
            Debug.Log("������� ��� ��� ������ ����, ������������� ����������� ������");
            float neutralValue = 2f;
            UpdateEmotionAnimator(neutralValue); // �������������� ����������
            if (!gameObject.activeInHierarchy)
            {
                pendingEmotionValue = neutralValue;
                Debug.Log("������ ���������, ����������� ������ ��������");
            }
        }
    }

    private List<InfoTrade> GetFilteredTrades()
    {
        if (DynamicBarChart.Instance == null)
        {
            Debug.LogWarning("DynamicBarChart.Instance is null, using all trades");
            return trades.trades;
        }

        string startDateText = DynamicBarChart.Instance.GetStartDateText();
        string endDateText = DynamicBarChart.Instance.GetEndDateText();

        if (string.IsNullOrEmpty(startDateText) || string.IsNullOrEmpty(endDateText) ||
            !IsDateValid(startDateText) || !IsDateValid(endDateText))
        {
            Debug.Log("Invalid dates, using all trades");
            return trades.trades;
        }

        DateTime startDate = DateTime.ParseExact(startDateText, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact(endDateText, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);

        return trades.trades
            .Where(t => t.date >= startDate && t.date <= endDate)
            .ToList();
    }

    private bool IsDateValid(string dateText)
    {
        if (string.IsNullOrEmpty(dateText) || dateText.Length != 10 || dateText[2] != '.' || dateText[5] != '.')
            return false;

        return DateTime.TryParseExact(dateText, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out _);
    }

    private float CalculateAverageEmotion(List<InfoTrade> trades)
    {
        if (trades == null || trades.Count == 0) return 2f; // Neutral �� ���������

        float totalEmotion = 0f;
        foreach (var trade in trades)
        {
            totalEmotion += trade.typeEmotion switch
            {
                TypeEmotion.VeryAngry => 0f,
                TypeEmotion.Angry => 1f,
                TypeEmotion.Neutral => 2f,
                TypeEmotion.Happy => 3f,
                TypeEmotion.VeryHappy => 4f,
                _ => 2f
            };
        }
        return totalEmotion / trades.Count;
    }

    private void UpdateEmotionAnimator(float value)
    {
        if (emotionAnimator != null)
        {
            emotionAnimator.SetFloat("line", value);
            Debug.Log($"�������� ��������� �� ��������: {value}");
        }
        else
        {
            Debug.LogWarning("Animator is null, cannot update animation!");
        }
    }
}