using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] public PlayerManager playerReference;
    private int petCount = 0;
    private int waveLevel =1;
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar xpBar;
    [SerializeField] ProgressBar towerBar;
    [SerializeField] ProgressBar bossBar;
    public TMP_Text petText;
    public TMP_Text levelText;
    public TMP_Text waveText;
    public TMP_Text bossText;
    public GameObject TowerUI;
    public GameObject BossUI;
    public Tower tower;
    public bool IsInWave;
    public Camera mainCamera;
    public CinemachineBrain cineBrain;
    public GameObject pauseUI;
    public EnemyDragon dragon;

    public void CheckDragonHp()
    {
        if (dragon.Health <= 0) SceneManager.LoadScene("Win");
    }

    [SerializeField] float progressBarUpdateSpeed = 0.1f;

    [SerializeField] private List<Spawner> Spawners;

    private List<EnemyWildlife> pets = new List<EnemyWildlife>();
    public List<GameObject> towerEnemyList = new List<GameObject>();

    public void AddToTowerEnemyList(GameObject towerEnemy)
    {
        towerEnemyList.Add(towerEnemy);
    }

    public int WaveLevel
    {
        get { return waveLevel; }
        set { waveLevel = value; }
    }
    public int PetCount
    {
        get { return petCount; }
        set { petCount = value; }
    }
    public List<EnemyWildlife> Pets
    {
        get { return pets; }
    }

    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    public void AddPet(EnemyWildlife pet)
    {
        pets.Add(pet);
    }

    public void NotifyPetsTarget(Enemy target)
    {
        foreach(var pet in pets)
        {
            pet.Target = target.transform;
            pet.Tagged = target.gameObject;
        }
    }

    public Transform GetPlayerPosition()
    {
        return playerReference.GetPosition();
    }

    private IEnumerator CountdownBeforeSpawn(int seconds)
    {
        while (seconds > 0)
        {
            waveText.text = $"Wave starting in {seconds}...";
            yield return new WaitForSeconds(1);
            seconds--;
        }

        SetWaveText();
        StartCoroutine(BroTolongSpawnMusuhnyaDong());
    }

    public void InvokeSpawnTowerEnemy()
    {
        StartCoroutine(CountdownBeforeSpawn(20));
    }
    private IEnumerator BroTolongSpawnMusuhnyaDong()
    {
        int quadrantCount = 0;
        for (int i = 0; i < waveLevel; i++)
        {
            quadrantCount = (quadrantCount + 1) % 4;
            Spawners[quadrantCount].SpawnFox((Node.Quadrant)quadrantCount);
            yield return new WaitForSeconds(5);
        }
    }



    #region progressBars
    public void InitHealthBar(float value)
    {
        //jan lupa init status dulu
        //SmoothBar(healthBar, value);
        healthBar.SetMaxValue(value);
    }
    public void UpdateHealthBar(float value)
    {
        SmoothBar(healthBar, value);
        healthBar.SetValue(value);
    }

    public void InitXpBar(float value)
    {
        //jan lupa init status dulu
        //SmoothBar(xpBar, value);
        xpBar.SetMaxValue(value);

    }
    public void UpdateXpBar(float value)
    {
        SmoothBar(xpBar, value);
        xpBar.SetValue(value);
    }

    public void InitTowerBar(int value)
    {
        //jan lupa init status dulu
        //SmoothBar(xpBar, value);
        towerBar.SetMaxValue(value);

    }
    public void UpdateTowerBar(float value)
    {
        SmoothBar(towerBar, value);
        towerBar.SetValue(value);
    }

    public void InitBossBar(float value)
    {
        //jan lupa init status dulu
        //SmoothBar(xpBar, value);
        bossBar.SetMaxValue(value);

    }
    public void UpdateBossBar(float value)
    {
        SmoothBar(towerBar, value);
        bossBar.SetValue(value);
    }


    void SmoothBar(ProgressBar progressBar, float newValue)
    {
        StartCoroutine(DecreaseBarSmoothly(progressBar, newValue));
    }

    #endregion

    public void SetPetText()
    {
        petText.text = petCount + " Pets";
    }
    public void SetLevelText()
    {
        levelText.text = playerReference.Level + "";
    }
    public void SetWaveText()
    {
        waveText.text = (waveLevel - destroyCount) + "/" + waveLevel + " Enemies Left";
    }
    public void OnKill(float xpPoints)
    {
        UpdateXpBar(playerReference.Xp += xpPoints);
    }

    IEnumerator DecreaseBarSmoothly(ProgressBar bar, float targetValue)
    {

        float startValue = bar.slider.value;
        float timeElapsed = 0;

        while (timeElapsed < progressBarUpdateSpeed)
        {
            bar.slider.value = Mathf.Lerp(startValue, targetValue, timeElapsed / progressBarUpdateSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        bar.slider.value = targetValue;
    }

    public float fadeDuration = 4.0f;

    public IEnumerator FadeIn(GameObject ui)
    {
        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogError(";)");
            yield break;
        }

        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            cg.alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / fadeDuration);
            yield return null;
        }

        cg.alpha = 1;
    }

    public IEnumerator FadeOut(GameObject ui)
    {
        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogError(":)");
            yield break;
        }

        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        { 
            //Debug.Log("fading out");
            cg.alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeDuration);
            yield return null;
        }

        cg.alpha = 0;
    }

    public int destroyCount = 0;
    public void DestroyObject(GameObject gameObject)
    {
        if (towerEnemyList.Contains(gameObject)) towerEnemyList.Remove(gameObject);
        destroyCount++;
        Destroy(gameObject);
        SetWaveText();
    }

    public bool IsWaveSuccessful;
    public void CheckWaveEnd()
    {
        if (IsInWave && destroyCount >= waveLevel)
        {
            IsInWave = false;
            destroyCount = 0;
            Invoke(nameof(StartTowerUIFadeOut), 3f);
            IsWaveSuccessful = true;
            waveText.text = "";
        }
        CheckIsWaveSuccessful();
    }

    public void CheckIsWaveSuccessful()
    {
        if (IsWaveSuccessful)
        {
            waveLevel++;
            IsWaveSuccessful = false;
        }
    }

    public void StartTowerUIFadeIn()
    {
        TowerUI.SetActive(true);
        StartCoroutine(FadeIn(TowerUI));
    }

    public void StartTowerUIFadeOut()
    {
        StartCoroutine(FadeOut(TowerUI));
        
    }

    public void StartBossUIFadeIn()
    {
        BossUI.SetActive(true);
        StartCoroutine(FadeIn(BossUI));
    }

    public void StartBossUIFadeOut()
    {
        StartCoroutine(FadeOut(BossUI));
    }

    private void InitCamera()
    {
        mainCamera = Camera.main;
        cineBrain = mainCamera.transform.GetComponent<CinemachineBrain>();
        Debug.Log(cineBrain);
    }
    private void Start()
    {
        TowerUI.SetActive(false);
        BossUI.SetActive(false);
        pauseUI.SetActive(false);
        InitCamera();
    }
    private void Update()
    {
        CheckDragonHp();
        CheckWaveEnd();
        CheckIsPaused();

        if(playerReference.Health <=0 || tower.TowerHp <= 0)
        {
            SceneManager.LoadScene("Lose");

        }
        if (isPaused)
        {
            OnPause();
        }
    }


    private bool pauseKey;
    public bool isPaused;

    private void CheckIsPaused()
    {
        pauseKey = Input.GetKeyDown(KeyCode.P);
        if (pauseKey)
        {
            isPaused = !isPaused;
            cineBrain.enabled = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            pauseUI.SetActive(isPaused);
        }

    }

    private float mouseSensitivity = 100f;
    private float fpsXrotate = 0;
    private float fpsYrotate = 0;
    private float rotationSmoothness = 0.2f;
    private Quaternion rotation;
    private void OnPause()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

        fpsYrotate += mouseX;
        fpsXrotate = Mathf.Clamp(fpsXrotate - mouseY, -90f, 90f);


        Quaternion cameraTargetRotation = Quaternion.Euler(fpsXrotate, fpsYrotate, 0);

        cameraTargetRotation = Quaternion.Lerp(cameraTargetRotation, cameraTargetRotation, rotationSmoothness);

        transform.rotation = cameraTargetRotation;
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        Camera.main.transform.rotation = cameraTargetRotation;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");

    }

}
