﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [System.Serializable]
    public class StoryUI
    {
        public string storyName;
        public TextMeshProUGUI storyText;
        public Image medalImage;
        public TextMeshProUGUI scoreText;
    }

    [Header("Lista de logros por historia")]
    public List<StoryUI> stories;

    [Header("Sprites de medallas")]
    public Sprite bronzeMedal;
    public Sprite silverMedal;
    public Sprite goldMedal;
    public Sprite noMedal;

    [Header("Perfil de usuario")]
    public GameObject profilePanel;
    public TextMeshProUGUI nameUserText;
    public Image imageUser;


    void Start()
    {
        LoadAchievements();
        LoadUserProfile();
    }

    void LoadAchievements()
    {
        string path = Application.persistentDataPath + "/quiz_scores.txt";

        if (!File.Exists(path))
        {
            Debug.LogWarning("No se encontró el archivo de puntajes. Mostrando logros vacíos.");
            // Muestra "0%" y noMedal para cada historia
            foreach (var storyUI in stories)
            {
                storyUI.scoreText.text = "0%";
                storyUI.medalImage.sprite = noMedal;
            }
            return;
        }

        var lines = File.ReadAllLines(path);

        foreach (var storyUI in stories)
        {
            float bestPercentage = 0f;

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length >= 5 && parts[0] == storyUI.storyName)
                {
                    if (float.TryParse(parts[3], out float percent))
                    {
                        bestPercentage = Mathf.Max(bestPercentage, percent);
                    }
                }
            }

            // Actualiza UI
            storyUI.scoreText.text = $"{bestPercentage:F1}%";

            // Asigna medalla
            if (bestPercentage >= 90)
                storyUI.medalImage.sprite = goldMedal;
            else if (bestPercentage >= 80)
                storyUI.medalImage.sprite = silverMedal;
            else if (bestPercentage >= 70)
                storyUI.medalImage.sprite = bronzeMedal;
            else
                storyUI.medalImage.sprite = noMedal;
        }
    }


    public void GoToStorySelector()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void LoadUserProfile()
    {
        string userName = PlayerPrefs.GetString("PlayerName", "");
        string imageName = PlayerPrefs.GetString("ProfileImageName", "");

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(imageName))
        {
            profilePanel.SetActive(false);
            Debug.Log("No se encontró perfil guardado.");
            return;
        }

        nameUserText.text = userName;

        Sprite userSprite = Resources.Load<Sprite>("Profile/" + imageName);
        if (userSprite != null)
        {
            imageUser.sprite = userSprite;
            profilePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No se encontró la imagen de perfil: " + imageName);
            profilePanel.SetActive(false);
        }
    }


}
