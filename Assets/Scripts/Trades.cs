using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trades : MonoBehaviour
{
    public static Trades Instance;
    [SerializeField] public List<InfoTrade> trades = new List<InfoTrade>();
    [SerializeField] public List<TradeUI> InstanceTrades;
    [SerializeField] public TradeUI tradeUIPrefab;
    [SerializeField] public Transform tradeParent;

    [SerializeField] private Text successfulTradesText;
    [SerializeField] private Text unsuccessfulTradesText;

    public event Action<InfoTrade> AddTrade;
    public event Action<InfoTrade> DelTrade;

    private TradeDataManager dataManager = new TradeDataManager();

    // ������ �� TradeFiltresSearch
    [SerializeField] private TradeFiltresSearch tradeFilterSearch;

    void Awake()
    {
        Instance = this;
        Debug.Log("Trades Instance initialized");

        if (tradeParent == null)
            tradeParent = GameObject.Find("ContentTradeParent")?.GetComponent<RectTransform>();

        if (trades == null)
            trades = new List<InfoTrade>();

        trades = dataManager.LoadTrades();
        DisplayTrades();
        Debug.Log(transform);
        gameObject.SetActive(false);
    }

    void DisplayTrades()
    {
        if (tradeParent == null) return;

        if (trades.Count > 0)
        {
            foreach (InfoTrade trade in trades.ToArray())
            {
                bool tradeExists = InstanceTrades.Any(existingTrade => existingTrade != null && existingTrade.currentInfo == trade);

                if (!tradeExists)
                {
                    TradeUI tradeUIObject = Instantiate(tradeUIPrefab, tradeParent);
                    if (tradeUIObject != null)
                    {
                        InstanceTrades.Add(tradeUIObject);
                        tradeUIObject.Initialization(trade);
                    }
                }
            }
        }
        else
        {
            foreach (TradeUI ui in InstanceTrades.ToArray())
            {
                if (ui != null)
                {
                    Destroy(ui.gameObject);
                    InstanceTrades.Remove(ui);
                }
            }
        }

        // ��������� ���������� ����� ����������� �������
        tradeFilterSearch?.OnTradesUpdated();
    }

    public void OnAddTrade(InfoTrade addTrade)
    {
        trades.Add(addTrade);
        Debug.Log($"�������� �����: {addTrade.nameTrade}, ������: {addTrade.typeEmotion}");
        DisplayTrades();
        AddTrade?.Invoke(addTrade);
        dataManager.SaveTrades(trades);
    }

    public void OnDelTrade(InfoTrade addTrade)
    {
        if (tradeParent == null || trades == null) return;

        foreach (TradeUI ui in InstanceTrades.ToArray())
        {
            if (ui != null && ui.currentInfo == addTrade)
            {
                Destroy(ui.gameObject);
                InstanceTrades.Remove(ui);
            }
        }
        trades.Remove(addTrade);
        Debug.Log($"����� �����: {addTrade.nameTrade}");
        DisplayTrades();
        DelTrade?.Invoke(addTrade);
        dataManager.SaveTrades(trades);
    }
}


// ����� ������ ������
// ����������� ����� InfoTrade � �������������� ����� ��� ������ ����
[System.Serializable]
public class InfoTrade
{
    public DateTime date; // ������������ ����, ������������ ������ ����
    public string dateString; // ��������������� ���� ��� ������������
    public string nameTrade;
    public TypeOperation typeOperation;
    public double price;
    public double count;
    public TypeEmotion typeEmotion;
    public string note;
    public bool successful;
}

// ������������ ��� ������ � ��������
public enum TypeEmotion
{
    VeryAngry,
    Angry,
    Neutral,
    Happy,
    VeryHappy
}

public enum TypeOperation
{
    buy,
    sell
}
