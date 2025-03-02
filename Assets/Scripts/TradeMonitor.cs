using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TradeMonitor : MonoBehaviour
{
    [SerializeField] private Trades tradeComponent; // ������ �� Trades � ����� ��� ��������

    void Start()
    {
        if (tradeComponent == null)
        {
            tradeComponent = FindFirstObjectByType<Trades>();
            if (tradeComponent == null)
            {
                Debug.LogError("��������� Trades �� ������ � �����!");
                return;
            }
            else
            {
                Debug.Log("��������� Trades ������: " + tradeComponent.name);
            }
        }
        StartCoroutine(MonitorTrades());
    }

    private IEnumerator MonitorTrades()
    {
        while (true)
        {
            YieldMonitor();
            yield return new WaitForSeconds(5f); // �������� ������ 5 ������
        }
    }

    private void YieldMonitor()
    {
        if (tradeComponent == null || tradeComponent.trades == null)
        {
            Debug.LogWarning("tradeComponent ��� trades ����� null!");
            return;
        }

        Debug.Log("=== ���������� ������ trades (�����: " + Time.time + ") ===");
        Debug.Log("����� ���������� �������: " + tradeComponent.trades.Count);

        for (int i = 0; i < tradeComponent.trades.Count; i++)
        {
            InfoTrade trade = tradeComponent.trades[i];
            Debug.Log($"����� [{i}]:");
            Debug.Log($"  ����: {trade.date:yyyy-MM-dd}");
            Debug.Log($"  ��������: {trade.nameTrade}");
            Debug.Log($"  ��� ��������: {trade.typeOperation}");
            Debug.Log($"  ����: {trade.price}");
            Debug.Log($"  ����������: {trade.count}");
            Debug.Log($"  ������: {trade.typeEmotion}");
            Debug.Log($"  ����������: {trade.note}");
        }

        // �������� InstanceTrades
        Debug.Log("=== ���������� InstanceTrades ===");
        Debug.Log("����� ���������� ��������� � InstanceTrades: " + tradeComponent.InstanceTrades.Count);

        for (int i = 0; i < tradeComponent.InstanceTrades.Count; i++)
        {
            TradeUI tradeUI = tradeComponent.InstanceTrades[i];
            if (tradeUI != null && tradeUI.currentInfo != null)
            {
                Debug.Log($"TradeUI [{i}]:");
                Debug.Log($"  ����: {tradeUI.currentInfo.date:yyyy-MM-dd}");
                Debug.Log($"  ��������: {tradeUI.currentInfo.nameTrade}");
                Debug.Log($"  ��� ��������: {tradeUI.currentInfo.typeOperation}");
                Debug.Log($"  ����: {tradeUI.currentInfo.price}");
                Debug.Log($"  ����������: {tradeUI.currentInfo.count}");
                Debug.Log($"  ������: {tradeUI.currentInfo.typeEmotion}");
                Debug.Log($"  ����������: {tradeUI.currentInfo.note}");
            }
            else
            {
                Debug.LogWarning($"TradeUI [{i}] ����� null ��� currentInfo �� ���������������!");
            }
        }

        // �������� tradeUIPrefab
        if (tradeComponent.tradeUIPrefab != null)
        {
            Debug.Log("=== ���������� � tradeUIPrefab ===");
            TradeUI prefabUI = tradeComponent.tradeUIPrefab.GetComponent<TradeUI>();
            if (prefabUI != null)
            {
                Debug.Log("tradeUIPrefab �������� ��������� TradeUI.");
                // ���� � ������� ���� currentInfo, ������� ��� (���� ������ ��� ������ �� ���������������)
                if (prefabUI.currentInfo != null)
                {
                    Debug.Log($"  ����: {prefabUI.currentInfo.date:yyyy-MM-dd}");
                }
                else
                {
                    Debug.Log("currentInfo � tradeUIPrefab �� ����������.");
                }
            }
            else
            {
                Debug.LogWarning("tradeUIPrefab �� �������� ��������� TradeUI!");
            }
        }
        else
        {
            Debug.LogWarning("tradeUIPrefab �� ��������!");
        }
    }
}