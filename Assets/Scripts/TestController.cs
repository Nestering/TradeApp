using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestController : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;         // ����� �������
        public Button downButton;          // ������ "���"
        public Button upButton;            // ������ "��"
        public bool correctAnswer;         // ���������� ����� (true - ��, false - ���)
        [HideInInspector]
        public bool userAnswer;            // ����� ������������
        [HideInInspector]
        public bool isAnswered;           // ��� �� ��� �����
    }

    public List<Question> questions;       // ������ ��������
    public GameObject correctMenu;         // ���� ��� ����������� ������
    public GameObject wrongMenu;           // ���� ��� ������������� ������  
    public int currentQuestionIndex = 0;  // ������� ������ �������
    public int correctAnswers = 0;        // ������� ���������� �������

    public MenuManage menuManage;
    public void StartTest()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
    }
    void Start()
    {
        // �������� ��� ���� ��� ������
        correctMenu.SetActive(false);
        wrongMenu.SetActive(false);

        // ����������� ����������� ������ ��� ������� �������
        foreach (Question q in questions)
        {
            // ������� ��������� ����� ��� ���������
            Question currentQ = q;

            // ���������� ��� ������ "��"
            q.upButton.onClick.AddListener(() =>
            {
                ProcessAnswer(currentQ, true);
            });

            // ���������� ��� ������ "���"
            q.downButton.onClick.AddListener(() =>
            {
                ProcessAnswer(currentQ, false);
            });
        }

    }
    public void NextAnimation()
    {
        if (currentQuestionIndex != 0 )
        {
           menuManage.SelectNumberAddMenu(currentQuestionIndex + 9);
        }
    }
    void ProcessAnswer(Question question, bool answer)
    {
        if (!question.isAnswered)
        {
            question.userAnswer = answer;

            // ��������� ������������ ������ � ���������� ��������������� ����
            if (question.userAnswer == question.correctAnswer)
            {
                correctAnswers++;
                ShowFeedbackMenu(correctMenu);
                currentQuestionIndex++;
                if (currentQuestionIndex >= questions.Count)
                {
                    correctAnswers = 0;
                    currentQuestionIndex = 0;
                    menuManage.SelectNumberBaseMenu(2);
                }
            }
            else
            {
                Debug.Log("����� �����������");
                ShowFeedbackMenu(wrongMenu);
            }
        }
    }

    void ShowFeedbackMenu(GameObject menu)
    {
        menu.SetActive(true);
    }
}