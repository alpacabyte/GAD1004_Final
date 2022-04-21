using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioClip gameStart;
    [SerializeField] private AudioClip passedDay;
    [SerializeField] private GameObject pausedCanvas;
    [SerializeField] private CanvasGroup winCanvas;
    [SerializeField] private GameObject nextDayButton;
    [SerializeField] private ShopManager shop;
    [SerializeField] private TMP_Text dayTimeZombieCounterText;
    [SerializeField] private TMP_Text dayCounterText;
    [SerializeField] [Range(1, 300)] private float timeToNextRound = 15f;
    [SerializeField] [Range(1, 50)] private int _zombieNeedForFirstDay = 30;
    [SerializeField] [Range(1, 2)] private float _zombieMultiplierEachDay = 1.5f;
    private static GameController gm;
    private int _zombiesNeedForNextDay, _zombiesNeedForCurrentDay;
    private AudioSource[] audioSources;
    private int dayCounter = 1;
    private float timeCounter = 0;
    private bool isFinished = true;
    private bool isPaused = false;
    public bool IsFinished => isFinished;
    public static GameController Instance => gm;
    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        _zombiesNeedForNextDay = _zombieNeedForFirstDay;
        _zombiesNeedForCurrentDay = _zombiesNeedForNextDay;

        timeCounter = timeToNextRound;

        dayCounterText.text = "DAY " + dayCounter;

        volumeSlider.value = AudioListener.volume;

        audioSources[1].PlayOneShot(gameStart, 0.4f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Continue();
            }
            else
            {
                PauseGame();
            }
        }

        if (isFinished)
        {
            dayTimeZombieCounterText.text = "Time Left: " + timeCounter.ToString("F1");
            PrepareNextDay();
        }

        else
        {
            dayTimeZombieCounterText.text = "Zombie Left: " + _zombiesNeedForCurrentDay + "/" + _zombiesNeedForNextDay;
        }
    }
    public void changeVolume(float value)
    {
        AudioListener.volume = value;
    }
    private void PauseGame()
    {
        isPaused = true;
        pausedCanvas.SetActive(true);
        Time.timeScale = 0;
        AudioListener.pause = true;
    }
    public void Continue()
    {
        isPaused = false;
        pausedCanvas.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
    private IEnumerator Win()
    {
        winCanvas.alpha = 0;
        Time.timeScale = 0.3f;
        winCanvas.gameObject.SetActive(true);
        winCanvas.interactable = false;

        while (winCanvas.alpha < 1)
        {
            winCanvas.alpha += Time.deltaTime / Time.timeScale;
            yield return new WaitForSeconds(Time.deltaTime * Time.timeScale * Time.timeScale);
        }

        winCanvas.interactable = true;
        Time.timeScale = 0;
    }
    private void PrepareNextDay()
    {
        audioSources[0].Pause();
        nextDayButton.SetActive(true);
        timeCounter -= Time.deltaTime;

        if (timeCounter < 0)
        {
            StartNextDay();
        }
    }

    public void StartNextDay()
    {
        audioSources[0].Play();
        isFinished = false;
        nextDayButton.SetActive(false);
        shop.CloseMenu();
        timeCounter = timeToNextRound;
        EnemySpawner.Instance.SendSpawnCommand(_zombiesNeedForCurrentDay);
    }

    public void ZombieKilled(bool canCountAsZombie, int moneyDropped)
    {
        if (canCountAsZombie)
        {
            _zombiesNeedForCurrentDay--;

            if (_zombiesNeedForCurrentDay <= 0)
            {
                dayCounterText.text = "DAY " + ++dayCounter;
                if (dayCounter == 7)
                {
                    StartCoroutine("Win");
                }
                audioSources[1].PlayOneShot(passedDay);
                _zombiesNeedForNextDay = Mathf.CeilToInt(_zombiesNeedForNextDay * _zombieMultiplierEachDay);
                _zombiesNeedForCurrentDay = _zombiesNeedForNextDay;
                isFinished = true;
            }
        }

        PlayerInventory.Instance.GainMoney(moneyDropped);
    }

    public bool canFire()
    {
        return !((isFinished && RectTransformUtility.RectangleContainsScreenPoint(nextDayButton.GetComponent<RectTransform>(), Input.mousePosition)) || isPaused);
    }
    public void Restart()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(0);
    }
}