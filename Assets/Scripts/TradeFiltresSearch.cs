using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeFiltresSearch : MonoBehaviour
{
    [SerializeField] private InputField tradeFilterInput; // ���� ��� ����������
    public Trades tradesManager; // ������ �� �������� �������
    public TradeFilter tradefilter; // ������ �� �������� �������
    private List<bool> initialActiveStates; // ������ ��� �������� ��������� ��������� ��������

    private void Awake()
    {

        // ��������� ��������� ��������� ���� ������� (������� �� ������)
        SaveInitialActiveStates();

        if (tradeFilterInput != null)
        {
            tradeFilterInput.onValueChanged.AddListener(OnFilterChanged); // �������� �� ��������� ������ � ���� �����
        }
    }
    private void OnEnable()
    {
        SaveInitialActiveStates();
    }

    // ����� ��� ���������� ��������� ��������� �������� �������
    private void SaveInitialActiveStates()
    {
        initialActiveStates = tradesManager.InstanceTrades
            .Select(tradeUI => tradeUI.gameObject.activeSelf) // ���������, ���� �� ��� ���������� �������
            .ToList();
    }

    // ����� ��� ���������� ������� �� ��������
    private void OnFilterChanged(string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            // ���� ���� ������, ��������������� ��� ������ � �������� ���������
            RestoreInitialActiveStates();
            return; // ���� ���� ������, ������� �� ������
        }

        // ���� ���������, ��������� ������ �� ������, ������� ���������� ���� ��������
        for (int i = 0; i < tradesManager.InstanceTrades.Count; i++)
        {
            TradeUI tradeUI = tradesManager.InstanceTrades[i];

            // ���� ����� ��� ���������� �������, ��������� �� ������������ �������
            if (i < initialActiveStates.Count && initialActiveStates[i]) // ���������, ��� ������ � �������� ����������� ���������
            {
                bool isVisible = tradeUI.currentInfo.nameTrade.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                tradeUI.gameObject.SetActive(isVisible); // ��������/��������� � ����������� �� �������
            }
            else
            {
                // ���� ������ ��� ��������, �� �� ��������� ��� ��� ����������
                tradeUI.gameObject.SetActive(false);
            }
        }
    }

    // ����� ��� �������������� ��������� ��������� �������� �������
    private void RestoreInitialActiveStates()
    {
        for (int i = 0; i < tradesManager.InstanceTrades.Count; i++)
        {
            bool wasActive = i < initialActiveStates.Count && initialActiveStates[i]; // �������� ��������� ���������
            tradesManager.InstanceTrades[i].gameObject.SetActive(wasActive); // ��������������� ����������
        }
    }

    // ����� ��� ���������� �������, ���� ��������� ����� ������
    public void OnTradesUpdated()
    {
        SaveInitialActiveStates(); // ��������� ��������� ��� ����� �������
        OnFilterChanged(tradeFilterInput.text); // ��������� ������� ������ (���� �� ����)
    }
}
