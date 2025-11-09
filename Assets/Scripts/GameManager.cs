using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject asteroidPrefab;
    public GameObject starPrefab;

    [Header("Spawn Range")]
    public float minInstantiateValue;
    public float maxInstantiateValue;

    [Header("Spawn Timings")]
    public float enemySpawnDelay = 2.5f;
    public float asteroidSpawnDelay = 3.5f;
    public float starSpawnDelay = 4.5f;

    [Header("Auto Destroy Timings")]
    public float enemyDestroyTime = 8f;
    public float asteroidDestroyTime = 10f;
    public float starDestroyTime = 10f;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;

    [Header("Panels")]
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;
    public GameObject scoreUI;

    [Header("Background Options")]
    public GameObject[] backgrounds = new GameObject[4];
    const string PREF_BG = "BGIndex";

    [Header("Gameplay UI")]
    public Transform livesPanel;  
    public Image[] heartImages;        
    public TMP_Text scoreText;

    [Header("Player Stats")]
    public Transform playerSpawn;
    public int maxLives = 3;
    [HideInInspector] public int currentLives;
    public int currentScore = 0;

    [Header("Game Over UI")]
    public TMP_Text finalScoreText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    private void Start()
    {
        SafeSetActive(startMenu, true);
        SafeSetActive(howToPlayPanel, false);
        SafeSetActive(settingsPanel, false);
        SafeSetActive(pauseMenu, false);
        SafeSetActive(gameOverMenu, false);
        SafeSetActive(scoreUI, false);


        int saved = PlayerPrefs.GetInt(PREF_BG, 0);
        ApplyBackground(saved);

        Time.timeScale = 0f;
        CancelInvoke();

        InitializeLives();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)
            && startMenu != null && !startMenu.activeSelf
            && gameOverMenu != null && !gameOverMenu.activeSelf
            && howToPlayPanel != null && !howToPlayPanel.activeSelf
            && settingsPanel != null && !settingsPanel.activeSelf)
        {
            bool showPause = pauseMenu != null && !pauseMenu.activeSelf;
            PauseGame(showPause);
        }
    }

    public void InitializeLives()
    {
        if ((heartImages == null || heartImages.Length == 0) && livesPanel != null)
        heartImages = livesPanel.GetComponentsInChildren<Image>(true);

        currentLives = Mathf.Min(maxLives, heartImages.Length);
        UpdateLivesUI();
    }

    public void StartGame()
    {
        SafeSetActive(startMenu, false);
        SafeSetActive(howToPlayPanel, false);
        SafeSetActive(settingsPanel,  false);
        SafeSetActive(pauseMenu, false);
        SafeSetActive(gameOverMenu, false);
        SafeSetActive(scoreUI, true);

        ApplyBackground(PlayerPrefs.GetInt(PREF_BG, 0));

        InitializeLives();
        currentScore = 0;
        UpdateScoreUI();

        Time.timeScale = 1f;

        CancelInvoke();
        SpawnFreshPlayer();

        if (enemyPrefab) 
            InvokeRepeating(nameof(InstantiateEnemy), 1.0f, enemySpawnDelay);
        if (asteroidPrefab) 
            InvokeRepeating(nameof(InstantiateAsteroid), 1.5f, asteroidSpawnDelay);
        if (starPrefab) 
            InvokeRepeating(nameof(InstantiateStar), 2.0f, starSpawnDelay);
    }

    public void PauseGame(bool isPaused)
    {
        if (isPaused == true)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        SafeSetActive(startMenu, false);
        SafeSetActive(howToPlayPanel, false);
        SafeSetActive(settingsPanel, false);
        SafeSetActive(pauseMenu, false);
        SafeSetActive(gameOverMenu, false);
        SafeSetActive(scoreUI, false);
        SafeSetActive(startMenu, true);

        Time.timeScale = 0f;
        CancelInvoke();

        DestroyAllWithTag("Enemy");
        DestroyAllWithTag("Asteroid");
        DestroyAllWithTag("Star");

        currentScore = 0;
        InitializeLives();
        UpdateScoreUI();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        SafeSetActive(scoreUI, false);
        SafeSetActive(pauseMenu, false);

        if (finalScoreText != null)
            finalScoreText.text = $"Your Score: {currentScore}";
        
        SafeSetActive(gameOverMenu, true);

        Time.timeScale = 0f;
        CancelInvoke();
    }

    public void Retry()
    {
        SafeSetActive(gameOverMenu, false);
        SafeSetActive(scoreUI, true);
        SafeSetActive(pauseMenu, false);

        CancelInvoke();
        DestroyAllWithTag("Enemy");
        DestroyAllWithTag("Asteroid");
        DestroyAllWithTag("Star");

        SpawnFreshPlayer();
        InitializeLives();
        currentScore = 0;
        UpdateScoreUI();

        // Chạy lại
        Time.timeScale = 1f;
        if (enemyPrefab)    InvokeRepeating(nameof(InstantiateEnemy),   1.0f, enemySpawnDelay);
        if (asteroidPrefab) InvokeRepeating(nameof(InstantiateAsteroid),1.5f, asteroidSpawnDelay);
        if (starPrefab)     InvokeRepeating(nameof(InstantiateStar),    2.0f, starSpawnDelay);
}

    public void ShowHowTo(bool show)
    {
        SafeSetActive(startMenu, !show);
        SafeSetActive(howToPlayPanel, show);
    }

    public void ShowSettings(bool show)
    {
        SafeSetActive(startMenu, !show);
        SafeSetActive(settingsPanel, show);
    }

    public void SetBackground(int index)
    {
        ApplyBackground(index);
        PlayerPrefs.SetInt(PREF_BG, index);
        PlayerPrefs.Save();
    }

    void ApplyBackground(int index)
    {
        if (backgrounds == null || backgrounds.Length == 0) return;
        for (int i = 0; i < backgrounds.Length; i++)
            SafeSetActive(backgrounds[i], i == index);
    }

    void InstantiateEnemy()
    {
        Vector3 enemypos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 8f);
        GameObject enemy = Instantiate(enemyPrefab, enemypos, Quaternion.Euler(0f, 0f, 180f));
        Destroy(enemy, enemyDestroyTime);
    }

    void InstantiateAsteroid()
    {
        Vector3 asteroidpos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 8f);
        GameObject asteroid = Instantiate(asteroidPrefab, asteroidpos, Quaternion.identity);
        Destroy(asteroid, asteroidDestroyTime);
    }

    void InstantiateStar()
    {
        Vector3 starpos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 8f);
        GameObject star = Instantiate(starPrefab, starpos, Quaternion.identity);
        Destroy(star, starDestroyTime);
    }

    private GameObject SpawnFreshPlayer()
    {
        // xoá Player cũ
        var old = GameObject.FindGameObjectWithTag("Player");
        if (old != null) Destroy(old);

        Vector3 spawnPos = (playerSpawn ? playerSpawn.position : Vector3.zero);
        return Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    public void OnStarCollected()
    {
        AddScore(10);
    }

    public void OnAsteroidHitPlayer()
    {
        TakeHit(1);
    }

    void SafeSetActive(GameObject go, bool isActive)
    {
        if (go != null)
            go.SetActive(isActive);
    }

    public void TakeHit(int dmg = 1)
    {
        currentLives = Mathf.Max(0, currentLives - dmg);
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) Destroy(player);   // sẽ spawn mới ở Start/Retry
            GameOver();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    void UpdateLivesUI()
    {
        if (heartImages == null || heartImages.Length == 0) return;
        for (int i = 0; i < heartImages.Length; i++)
        heartImages[i].enabled = (i < currentLives);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }

    void DestroyAllWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

}
