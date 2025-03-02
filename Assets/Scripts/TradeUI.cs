using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class TradeUI : MonoBehaviour
{
    public Text date;
    public Text nameTrade;
    public Text typeOperation;
    public Text price;
    public Text count;
    public Text priceTotal;
    public Color[] priceColor;
    public Image currentPriceBackgrount;
    public GameObject[] typeEmotion;
    public GameObject[] typeSuccessful;
    public InfoTrade currentInfo;
    public void Initialization(InfoTrade info)
    {
        date.text = info.date.ToString("MM.dd.yyyy");
        nameTrade.text = info.nameTrade;
        typeOperation.text = info.typeOperation == TypeOperation.buy ? "Buy" : "Sell";
        currentPriceBackgrount.color = info.typeOperation == TypeOperation.buy ? priceColor[0] : priceColor[1];
        price.text = "$ " + info.price.ToString("0.00");
        count.text = info.count.ToString("0");
        priceTotal.text = "$ " + (info.price * info.count).ToString("0");
        typeEmotion[(int)info.typeEmotion].SetActive(true);
        if (info.successful == true)
        {
            typeSuccessful[0].SetActive(true);
            typeSuccessful[1].SetActive(false);
        }
        else
        {
            typeSuccessful[1].SetActive(true);
            typeSuccessful[0].SetActive(false);
        }
        currentInfo = info;
    }
    public void FillInfoOpenWindow()
    {
        TradeOpenUI.Instance.Initialization(currentInfo);
        TradeOpenUI.Instance.gameObject.SetActive(true);
    }
}
