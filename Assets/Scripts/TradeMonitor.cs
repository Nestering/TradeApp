using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TradeMonitor : MonoBehaviour
{
    [SerializeField] private Trades tradeComponent; // Ссылка на Trades в сцене для проверки

    void Start()
    {
        if (tradeComponent == null)
        {
            tradeComponent = FindFirstObjectByType<Trades>();
            if (tradeComponent == null)
            {
                Debug.LogError("Компонент Trades не найден в сцене!");
                return;
            }
            else
            {
                Debug.Log("Компонент Trades найден: " + tradeComponent.name);
            }
        }
        StartCoroutine(MonitorTrades());
    }

    private IEnumerator MonitorTrades()
    {
        while (true)
        {
            YieldMonitor();
            yield return new WaitForSeconds(5f); // Проверка каждые 5 секунд
        }
    }

    private void YieldMonitor()
    {
        if (tradeComponent == null || tradeComponent.trades == null)
        {
            Debug.LogWarning("tradeComponent или trades равны null!");
            return;
        }

        Debug.Log("=== Мониторинг списка trades (время: " + Time.time + ") ===");
        Debug.Log("Общее количество трейдов: " + tradeComponent.trades.Count);

        for (int i = 0; i < tradeComponent.trades.Count; i++)
        {
            InfoTrade trade = tradeComponent.trades[i];
            Debug.Log($"Трейд [{i}]:");
            Debug.Log($"  Дата: {trade.date:yyyy-MM-dd}");
            Debug.Log($"  Название: {trade.nameTrade}");
            Debug.Log($"  Тип операции: {trade.typeOperation}");
            Debug.Log($"  Цена: {trade.price}");
            Debug.Log($"  Количество: {trade.count}");
            Debug.Log($"  Эмоция: {trade.typeEmotion}");
            Debug.Log($"  Примечание: {trade.note}");
        }

        // Проверка InstanceTrades
        Debug.Log("=== Мониторинг InstanceTrades ===");
        Debug.Log("Общее количество элементов в InstanceTrades: " + tradeComponent.InstanceTrades.Count);

        for (int i = 0; i < tradeComponent.InstanceTrades.Count; i++)
        {
            TradeUI tradeUI = tradeComponent.InstanceTrades[i];
            if (tradeUI != null && tradeUI.currentInfo != null)
            {
                Debug.Log($"TradeUI [{i}]:");
                Debug.Log($"  Дата: {tradeUI.currentInfo.date:yyyy-MM-dd}");
                Debug.Log($"  Название: {tradeUI.currentInfo.nameTrade}");
                Debug.Log($"  Тип операции: {tradeUI.currentInfo.typeOperation}");
                Debug.Log($"  Цена: {tradeUI.currentInfo.price}");
                Debug.Log($"  Количество: {tradeUI.currentInfo.count}");
                Debug.Log($"  Эмоция: {tradeUI.currentInfo.typeEmotion}");
                Debug.Log($"  Примечание: {tradeUI.currentInfo.note}");
            }
            else
            {
                Debug.LogWarning($"TradeUI [{i}] равен null или currentInfo не инициализирован!");
            }
        }

        // Проверка tradeUIPrefab
        if (tradeComponent.tradeUIPrefab != null)
        {
            Debug.Log("=== Информация о tradeUIPrefab ===");
            TradeUI prefabUI = tradeComponent.tradeUIPrefab.GetComponent<TradeUI>();
            if (prefabUI != null)
            {
                Debug.Log("tradeUIPrefab содержит компонент TradeUI.");
                // Если в префабе есть currentInfo, выведем его (хотя обычно оно пустое до инстанцирования)
                if (prefabUI.currentInfo != null)
                {
                    Debug.Log($"  Дата: {prefabUI.currentInfo.date:yyyy-MM-dd}");
                }
                else
                {
                    Debug.Log("currentInfo в tradeUIPrefab не установлен.");
                }
            }
            else
            {
                Debug.LogWarning("tradeUIPrefab не содержит компонент TradeUI!");
            }
        }
        else
        {
            Debug.LogWarning("tradeUIPrefab не назначен!");
        }
    }
}