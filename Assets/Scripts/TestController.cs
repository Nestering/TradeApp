using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestController : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;         // Текст вопроса
        public Button downButton;          // Кнопка "Нет"
        public Button upButton;            // Кнопка "Да"
        public bool correctAnswer;         // Правильный ответ (true - Да, false - Нет)
        [HideInInspector]
        public bool userAnswer;            // Ответ пользователя
        [HideInInspector]
        public bool isAnswered;           // Был ли дан ответ
    }

    public List<Question> questions;       // Список вопросов
    public GameObject correctMenu;         // Меню для правильного ответа
    public GameObject wrongMenu;           // Меню для неправильного ответа  
    public int currentQuestionIndex = 0;  // Текущий индекс вопроса
    public int correctAnswers = 0;        // Счетчик правильных ответов

    public MenuManage menuManage;
    public void StartTest()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
    }
    void Start()
    {
        // Скрываем все меню при старте
        correctMenu.SetActive(false);
        wrongMenu.SetActive(false);

        // Настраиваем обработчики кнопок для каждого вопроса
        foreach (Question q in questions)
        {
            // Создаем локальные копии для замыкания
            Question currentQ = q;

            // Обработчик для кнопки "Да"
            q.upButton.onClick.AddListener(() =>
            {
                ProcessAnswer(currentQ, true);
            });

            // Обработчик для кнопки "Нет"
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

            // Проверяем правильность ответа и показываем соответствующее меню
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
                Debug.Log("овтет неправльный");
                ShowFeedbackMenu(wrongMenu);
            }
        }
    }

    void ShowFeedbackMenu(GameObject menu)
    {
        menu.SetActive(true);
    }
}