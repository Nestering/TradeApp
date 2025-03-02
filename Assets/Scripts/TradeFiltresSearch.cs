using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeFiltresSearch : MonoBehaviour
{
    [SerializeField] private InputField tradeFilterInput; // Поле для фильтрации
    public Trades tradesManager; // Ссылка на менеджер трейдов
    public TradeFilter tradefilter; // Ссылка на менеджер трейдов
    private List<bool> initialActiveStates; // Список для хранения начальных состояний объектов

    private void Awake()
    {

        // Сохраняем начальные состояния всех трейдов (активен ли объект)
        SaveInitialActiveStates();

        if (tradeFilterInput != null)
        {
            tradeFilterInput.onValueChanged.AddListener(OnFilterChanged); // Подписка на изменение текста в поле ввода
        }
    }
    private void OnEnable()
    {
        SaveInitialActiveStates();
    }

    // Метод для сохранения начальных состояний активных трейдов
    private void SaveInitialActiveStates()
    {
        initialActiveStates = tradesManager.InstanceTrades
            .Select(tradeUI => tradeUI.gameObject.activeSelf) // Сохраняем, были ли они изначально активны
            .ToList();
    }

    // Метод для фильтрации трейдов по названию
    private void OnFilterChanged(string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            // Если поле пустое, восстанавливаем все трейды в исходное состояние
            RestoreInitialActiveStates();
            return; // Если поле пустое, выходим из метода
        }

        // Если фильтруем, проверяем только те трейды, которые изначально были включены
        for (int i = 0; i < tradesManager.InstanceTrades.Count; i++)
        {
            TradeUI tradeUI = tradesManager.InstanceTrades[i];

            // Если трейд был изначально включён, проверяем на соответствие фильтру
            if (i < initialActiveStates.Count && initialActiveStates[i]) // Проверяем, что индекс в пределах допустимого диапазона
            {
                bool isVisible = tradeUI.currentInfo.nameTrade.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                tradeUI.gameObject.SetActive(isVisible); // Включаем/выключаем в зависимости от фильтра
            }
            else
            {
                // Если объект был выключен, то не учитываем его для фильтрации
                tradeUI.gameObject.SetActive(false);
            }
        }
    }

    // Метод для восстановления начальных состояний активных трейдов
    private void RestoreInitialActiveStates()
    {
        for (int i = 0; i < tradesManager.InstanceTrades.Count; i++)
        {
            bool wasActive = i < initialActiveStates.Count && initialActiveStates[i]; // Получаем начальное состояние
            tradesManager.InstanceTrades[i].gameObject.SetActive(wasActive); // Восстанавливаем активность
        }
    }

    // Метод для обновления фильтра, если добавлены новые трейды
    public void OnTradesUpdated()
    {
        SaveInitialActiveStates(); // Сохраняем состояние для новых трейдов
        OnFilterChanged(tradeFilterInput.text); // Применяем текущий фильтр (если он есть)
    }
}
