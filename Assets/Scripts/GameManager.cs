using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Round Settings")]
    public int currentRound = 0;
    public int baseActiveEnemies = 5;
    public int activeIncrement = 2;

    [Header("Runtime Tracking")]
    public int enemiesAlive = 0;
    public bool captureFilled = false;

    [Header("Capture Goal Scaling")]
    public int baseCaptureGoal = 100;
    public int captureGoalIncrement = 25;

    [Header("Spawn Settings")]
    public Transform[] spawnPoint;
    public GameObject enemyPrefab;

    private CapturePointController captureController;
    private float spawnTimer = 0f; // prevents 60 calls per second

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        captureController = GetComponent<CapturePointController>();
        StartRound();
    }

    // =====================================================
    // START NEW ROUND
    // =====================================================
    public void StartRound()
    {
        currentRound++;
        Debug.Log($"--- ROUND {currentRound} STARTED ---");

        captureFilled = false;
        enemiesAlive = 0;

        // Update player capture goal
        var player = FindObjectOfType<PlayerScore>();
        if (player != null)
        {
            player.captureGoal = baseCaptureGoal + (currentRound - 1) * captureGoalIncrement;
            player.ResetCaptureProgress();
        }

        // Activate NEW capture point
        if (captureController != null)
            captureController.ActivatePointForNewRound();

        // Initial wave
        SpawnEnemies();
    }

    // =====================================================
    // SPAWN ENEMY
    // =====================================================
    private void SpawnEnemies()
    {
        if (captureFilled) return;

        int maxActiveEnemies = baseActiveEnemies + (currentRound - 1) * activeIncrement;
        if (enemiesAlive >= maxActiveEnemies) return;

        int index = Random.Range(0, spawnPoint.Length);
        Instantiate(enemyPrefab, spawnPoint[index].position, spawnPoint[index].rotation);

        enemiesAlive++;
    }

    // =====================================================
    // ENEMY DIED
    // =====================================================
    public void EnemyDead()
    {
        enemiesAlive--;

        if (captureFilled && enemiesAlive <= 0)
        {
            NextRound();
            return;
        }

        SpawnEnemies();
    }

    // =====================================================
    // CAPTURE POINT FILLED
    // =====================================================
    public void CaptureComplete()
    {
        captureFilled = true;

        if (enemiesAlive <= 0)
            NextRound();
    }

    // =====================================================
    // NEXT ROUND
    // =====================================================
    private void NextRound()
    {
        Debug.Log($"--- ROUND {currentRound} COMPLETE ---");

        if (captureController != null)
            captureController.DeactivateCurrentPoint();

        StartRound();
    }

    private void Update()
    {
        if (captureFilled) return;

        // Spawn enemies every 0.5 seconds instead of every frame
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemies();
            spawnTimer = 0.5f;
        }
    }
}