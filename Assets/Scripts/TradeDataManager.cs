using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TradeDataManager
{
    private const string SaveFileName = "trades.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public List<InfoTrade> LoadTrades()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            Debug.Log($"Loading JSON: {json}"); // �������� ���������� ����� ��� �������
            TradeData data = JsonUtility.FromJson<TradeData>(json);
            if (data?.trades != null)
            {
                foreach (var trade in data.trades)
                {
                    // ��������� ����
                    if (string.IsNullOrEmpty(trade.dateString))
                    {
                        trade.date = DateTime.Now; // ������������� ������� ����, ���� ������ �����
                        trade.dateString = trade.date.ToString("o");
                        Debug.LogWarning($"dateString was empty for trade {trade.nameTrade}, set to {trade.date}");
                    }
                    else if (!DateTime.TryParse(trade.dateString, out DateTime parsedDate))
                    {
                        Debug.LogWarning($"Failed to parse dateString {trade.dateString} for trade {trade.nameTrade}, using default");
                        trade.date = DateTime.Now; // ������������� ������� ���� ��� ������
                        trade.dateString = trade.date.ToString("o");
                    }
                    else
                    {
                        trade.date = parsedDate;
                    }

                    Debug.Log($"Loaded trade: {trade.nameTrade}, Date: {trade.date}, Emotion: {trade.typeEmotion}");
                }
                return data.trades;
            }
            else
            {
                Debug.LogWarning("Deserialized data.trades is null, returning empty list");
            }
        }
        else
        {
            Debug.Log("No save file found at " + SavePath);
        }
        return new List<InfoTrade>();
    }

    public void SaveTrades(List<InfoTrade> trades)
    {
        foreach (var trade in trades)
        {
            if (trade.dateString == null || trade.dateString != trade.date.ToString("o"))
            {
                trade.dateString = trade.date.ToString("o"); // ��������� ������ ����
            }
        }

        TradeData data = new TradeData { trades = trades };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Trades saved to {SavePath} with JSON: {json}"); // �������� ����������� ������
    }
}

[System.Serializable]
public class TradeData
{
    public List<InfoTrade> trades;
}
