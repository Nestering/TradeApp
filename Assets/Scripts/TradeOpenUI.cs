using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TradeOpenUI : MonoBehaviour
{
    public static TradeOpenUI Instance;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    public Text date;
    public Text nameTrade;
    public Text typeOperation;
    public Text price;
    public Text count;
    public Text priceTotal;
    public GameObject[] typeEmotion;
    public GameObject[] typeSuccessful;
    public Text note;

    public InfoTrade currentInfo;
    public void Initialization(InfoTrade info)
    {
        currentInfo = info;
        date.text = info.date.ToString("MM.dd.yyyy");
        
        nameTrade.text = info.nameTrade;
        typeOperation.text = info.typeOperation == TypeOperation.buy ? "Buy" : "Sell";
        price.text = "$ " + info.price.ToString("0.00");
        count.text = info.count.ToString("0");
        priceTotal.text = "$ " + (info.price * info.count).ToString("0");
        typeEmotion[(int)info.typeEmotion].SetActive(true);
        note.text = info.note;
        typeSuccessful[0].SetActive(info.successful);
        typeSuccessful[1].SetActive(!info.successful);
    }
    public void SelectSuccessful(bool active )
    {
        List<InfoTrade> trades = Trades.Instance.trades;
        for (int i = 0; i < trades.Count; i++)
        {
            if(trades[i] == currentInfo)
            {
                typeSuccessful[0].SetActive(active);
                typeSuccessful[1].SetActive(!active);
                currentInfo.successful = active;
                Trades.Instance.InstanceTrades[i].typeSuccessful[0].SetActive(currentInfo.successful);
                Trades.Instance.InstanceTrades[i].typeSuccessful[1].SetActive(!currentInfo.successful);
                Trades.Instance.trades[i] = currentInfo;
            }
            else { Debug.Log("не найден"); }
        }
    }
    public void DeleteTrade()
    {
        Trades.Instance.OnDelTrade(currentInfo); // Обновленный вызов метода
    }
}