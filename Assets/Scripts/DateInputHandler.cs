using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DateInputHandler
{
    private InputField dateInput;

    public DateInputHandler(InputField inputField)
    {
        dateInput = inputField;
        dateInput.onValueChanged.AddListener(FormatDate);
    }

    private void FormatDate(string input)
    {
        string digitsOnly = Regex.Replace(input, @"\D", "");
        if (digitsOnly.Length > 8) digitsOnly = digitsOnly.Substring(0, 8);

        if (digitsOnly.Length >= 2)
        {
            string month = digitsOnly.Substring(0, 2);
            int monthInt = int.Parse(month);
            monthInt = Mathf.Clamp(monthInt, 1, 12);
            digitsOnly = monthInt.ToString("D2") + digitsOnly.Substring(2);
        }

        if (digitsOnly.Length >= 4)
        {
            string day = digitsOnly.Substring(2, 2);
            int dayInt = int.Parse(day);
            dayInt = Mathf.Clamp(dayInt, 1, 31);
            digitsOnly = digitsOnly.Substring(0, 2) + dayInt.ToString("D2") + (digitsOnly.Length > 4 ? digitsOnly.Substring(4) : "");
        }

        if (digitsOnly.Length >= 8)
        {
            string year = digitsOnly.Substring(4, 4);
            int yearInt = int.Parse(year);
            yearInt = Mathf.Clamp(yearInt, 1900, 2999);
            digitsOnly = digitsOnly.Substring(0, 4) + yearInt.ToString("D4");
        }

        string formattedDate = FormatWithDots(digitsOnly);
        dateInput.text = formattedDate;
        dateInput.caretPosition = formattedDate.Length;
    }

    private string FormatWithDots(string digits)
    {
        if (digits.Length <= 2) return digits;
        if (digits.Length <= 4) return digits.Insert(2, ".");
        if (digits.Length <= 8) return digits.Insert(2, ".").Insert(5, ".");
        return digits;
    }
}