using System;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;

public class KasaSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private string correctCode = "5732";
    private string input = "";
    private bool isOpened = false;

    public void PressButton(string digit)
    {
        if (isOpened)
            return;

        if (input.Length < 4)
        {
            input += digit;
            displayText.text = input;
        }

        if (input.Length == 4)
        {
            CheckCode();
        }
    }

    void CheckCode()
    {
        if (input == correctCode)
        {
            displayText.color = Color.green;
            displayText.text = "AÇILDI";
            isOpened = true;
            // Animasyon Burada Çalışıcak
            // Açılma Sesi Burda Çalacak
        }
        else
        {
            displayText.color = Color.red;
            displayText.text = "HATALI";
            isOpened = false;
            // Hata Sesi Burada Çalacak
        }

        Invoke(nameof(ClearInput), 2f);
    }

    void ClearInput()
    {
        displayText.color = Color.white;
        input = "";
        displayText.text = "";
    }
}