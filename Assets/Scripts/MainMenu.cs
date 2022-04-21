using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject firstCanvas;
    [SerializeField] private GameObject secondCanvas;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button survivalButton;
    private void Start()
    {
        firstCanvas.SetActive(true);
        secondCanvas.SetActive(false);

        if (PlayerPrefs.GetInt("Played", 0) == 1)
        {
            normalButton.interactable = true;
            survivalButton.interactable = true;
        }
        else
        {
            normalButton.interactable = false;
            survivalButton.interactable = false;
        }
    }
    public void PlayGame()
    {
        firstCanvas.SetActive(false);
        secondCanvas.SetActive(true);
    }
    public void Quit()
    {
        audioSource.PlayOneShot(buttonClick, 1);
        Application.Quit();
    }
    public void StartTutorial()
    {
        audioSource.PlayOneShot(buttonClick, 1);
        PlayerPrefs.SetInt("Played", 1);
        SceneManager.LoadScene(1);
    }
    public void StartNormal()
    {
        audioSource.PlayOneShot(buttonClick, 1);
        SceneManager.LoadScene(2);
    }
    public void StartSurvival()
    {
        audioSource.PlayOneShot(buttonClick, 1);
        SceneManager.LoadScene(3);
    }
}
