using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;

public class AddTradeOperation : MonoBehaviour
{
    public static AddTradeOperation Instance;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    public InputField date;
    public InputField nameTrade;
    public Dictionary<TypeOperation, bool> typeOperation = new Dictionary<TypeOperation, bool>
    {
        { TypeOperation.buy, true },
        { TypeOperation.sell, false }
    };
    public GameObject[] typeOperationIcons;
    public InputField price;
    public InputField count;
    public Dictionary<TypeEmotion, bool> typeEmotion = new Dictionary<TypeEmotion, bool>
    {
        { TypeEmotion.VeryAngry, false },
        { TypeEmotion.Angry, false },
        { TypeEmotion.Neutral, true },
        { TypeEmotion.Happy, false },
        { TypeEmotion.VeryHappy, false }
    };
    public Animator emotionAnim;
    private Coroutine emotionCoroutine;
    public InputField note;

    private DateInputHandler dateHandler;
    private DecimalInputHandler priceHandler;
    private DecimalInputHandler countHandler;

    public bool IsFillInput = true;

    Color redColor = new Color(1f, 0.8f, 0.8f);
    Color defaultColor = Color.white;

    void Start()
    {
        // Восстановление инициализации обработчиков
        dateHandler = new DateInputHandler(date);
        priceHandler = new DecimalInputHandler(price);
        countHandler = new DecimalInputHandler(count);

        nameTrade.characterLimit = 10;
    }

    private bool ValidateInputs()
    {
        IsFillInput = true;

        // Проверка даты
        string inputDate = date.text?.Trim() ?? "";
        if (string.IsNullOrEmpty(inputDate) || inputDate.Length < 10)
        {
            Debug.LogError("Пожалуйста, введите корректную дату в формате MM.dd.yyyy");
            HighlightField(date, redColor);
            IsFillInput = false;
        }
        else
        {
            try
            {
                char[] dateChars = inputDate.ToCharArray();
                if (dateChars.Length < 10)
                {
                    Debug.LogError("Некорректная длина даты. Ожидается формат: MM.dd.yyyy");
                    HighlightField(date, redColor);
                    IsFillInput = false;
                }
                else
                {
                    (dateChars[0], dateChars[3]) = (dateChars[3], dateChars[0]);
                    (dateChars[1], dateChars[4]) = (dateChars[4], dateChars[1]);
                    string swappedDate = new string(dateChars);

                    if (!DateTime.TryParseExact(swappedDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        Debug.LogError("Некорректный формат даты. Используйте MM.dd.yyyy");
                        HighlightField(date, redColor);
                        IsFillInput = false;
                    }
                    else
                    {
                        HighlightField(date, defaultColor);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при обработке даты: {ex.Message}");
                HighlightField(date, redColor);
                IsFillInput = false;
            }
        }

        // Проверка имени сделки
        string tradeName = nameTrade.text?.Trim() ?? "";
        if (string.IsNullOrEmpty(tradeName))
        {
            Debug.LogError("Пожалуйста, введите название сделки");
            HighlightField(nameTrade, redColor);
            IsFillInput = false;
        }
        else
        {
            HighlightField(nameTrade, defaultColor);
        }

        // Проверка цены
        string priceText = price.text?.Trim() ?? "";
        if (string.IsNullOrEmpty(priceText) || !double.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out double priceValue) || priceValue <= 0)
        {
            Debug.LogError("Пожалуйста, введите корректную цену (положительное число)");
            HighlightField(price, redColor);
            IsFillInput = false;
        }
        else
        {
            HighlightField(price, defaultColor);
        }

        // Проверка количества
        string countText = count.text?.Trim() ?? "";
        if (string.IsNullOrEmpty(countText) || !double.TryParse(countText, NumberStyles.Any, CultureInfo.InvariantCulture, out double countValue) || countValue <= 0)
        {
            Debug.LogError("Пожалуйста, введите корректное количество (положительное число)");
            HighlightField(count, redColor);
            IsFillInput = false;
        }
        else
        {
            HighlightField(count, defaultColor);
        }

        // Проверка эмоции (опционально)
        if (!typeEmotion.Values.Any(value => value))
        {
            Debug.LogWarning("Эмоция не выбрана, будет использована нейтральная");
            typeEmotion[TypeEmotion.Neutral] = true;
        }

        return IsFillInput;
    }

    public void OnDisable()
    {
        HighlightField(date, defaultColor);
        HighlightField(nameTrade, defaultColor);
        HighlightField(price, defaultColor);
        HighlightField(count, defaultColor);
        date.text = "";
        nameTrade.text = "";
        price.text = "";
        count.text = "";
        note.text = "";
    }

    private void HighlightField(InputField field, Color color)
    {
        if (field.TryGetComponent(out Image background))
        {
            background.color = color;
        }
    }

    public void AddTrade()
    {
        if (!ValidateInputs())
        {
            return;
        }

        string inputDate = date.text;
        char[] dateChars = inputDate.ToCharArray();
        (dateChars[0], dateChars[3]) = (dateChars[3], dateChars[0]);
        (dateChars[1], dateChars[4]) = (dateChars[4], dateChars[1]);
        string swappedDate = new string(dateChars);

        DateTime parsedDate = DateTime.ParseExact(swappedDate, "dd.MM.yyyy", null);
        TypeOperation selectedOperation = GetSelectedOperation();
        TypeEmotion selectedEmotion = GetSelectedEmotion();

        InfoTrade newTrade = new InfoTrade
        {
            date = parsedDate,
            nameTrade = nameTrade.text,
            typeOperation = selectedOperation,
            price = double.Parse(price.text, CultureInfo.InvariantCulture.NumberFormat),
            count = double.Parse(count.text, CultureInfo.InvariantCulture.NumberFormat),
            typeEmotion = selectedEmotion,
            note = note.text,
            successful = true
        };
        Trades.Instance.OnAddTrade(newTrade);
        Debug.Log("Новый трейд добавлен: " + newTrade.date.ToString("yyyy-MM-dd") + ", Цена: " + newTrade.price);
    }

    private TypeOperation GetSelectedOperation()
    {
        foreach (var operation in typeOperation)
        {
            if (operation.Value)
                return operation.Key;
        }
        return TypeOperation.sell;
    }

    private TypeEmotion GetSelectedEmotion()
    {
        foreach (var emotion in typeEmotion)
        {
            if (emotion.Value)
                return emotion.Key;
        }
        return TypeEmotion.Neutral;
    }

    public void SelectTypeOperation(int selectedType)
    {
        var operations = new List<TypeOperation>(typeOperation.Keys);

        foreach (var operation in operations)
            typeOperation[operation] = false;

        typeOperation[(TypeOperation)selectedType] = true;
    }

    public void SelectTypeEmotion(int selectedEmotion)
    {
        var emotions = new List<TypeEmotion>(typeEmotion.Keys);

        foreach (var emotion in emotions)
            typeEmotion[emotion] = false;

        TypeEmotion selected = (TypeEmotion)selectedEmotion;
        typeEmotion[selected] = true;

        if (emotionCoroutine != null)
            StopCoroutine(emotionCoroutine);

        emotionCoroutine = StartCoroutine(UpdateEmotionParameter(selected));
    }

    private IEnumerator UpdateEmotionParameter(TypeEmotion selectedEmotion)
    {
        float targetValue = selectedEmotion switch
        {
            TypeEmotion.VeryAngry => 0f,
            TypeEmotion.Angry => 1f,
            TypeEmotion.Neutral => 2f,
            TypeEmotion.Happy => 3f,
            TypeEmotion.VeryHappy => 4f,
            _ => 2f
        };

        float startValue = emotionAnim.GetFloat("line");
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentValue = Mathf.Lerp(startValue, targetValue, t);
            emotionAnim.SetFloat("line", currentValue);
            yield return null;
        }

        emotionAnim.SetFloat("line", targetValue);
    }

    public class DecimalInputHandler
    {
        private InputField inputField;

        public DecimalInputHandler(InputField field)
        {
            inputField = field;
            inputField.onValueChanged.AddListener(AllowNumbersAndDecimal);
        }

        private void AllowNumbersAndDecimal(string input)
        {
            string validInput = Regex.Replace(input, @"[^0-9.]", ""); // Удаляем недопустимые символы
            string[] parts = validInput.Split('.');

            if (parts.Length > 2) // Более одной точки - удаляем лишние
            {
                validInput = parts[0] + "." + parts[1]; // Оставляем только первую точку
            }
            else if (parts.Length == 2)
            {
                // Ограничиваем количество цифр после точки
                if (parts[1].Length > 10)
                {
                    parts[1] = parts[1].Substring(0, 10);
                }
                validInput = parts[0] + "." + parts[1];
            }

            inputField.text = validInput;
            inputField.caretPosition = validInput.Length;
        }
    }
}