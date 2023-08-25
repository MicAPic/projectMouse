using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    public int EnemyCount { get; private set; }
    
    [SerializeField] private Transform playerTransfrom;

    [Header("Enemies Prefs")]
    [SerializeField] private GameObject _simpleEnemyPrefab;
    [SerializeField] private GameObject _shotGunEnemyPrefab;
    [SerializeField] private GameObject _magicEnemyPrefab;

    [Header("Balance Curves")]
    [SerializeField] private AnimationCurve _maxEnemiesOnLevel;
    [SerializeField] private AnimationCurve _levelToHealth;
    [SerializeField] private AnimationCurve _levelToDamage;

    [Header("Spawn Settings")]
    [SerializeField]
    private float initialDelay = 3.5f;
    [SerializeField] private float _timeBetweenSpawns = 2;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private LayerMask _obstacleToSpawn;
    private Vector3 _currentSpawnDirection;
    private bool _isSpawning;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _currentSpawnDirection = playerTransfrom.up * _spawnRadius;
    }

    private float _spawnRotationAngel = 76f;
    private float _timeFromLastSpawn = 0;

    private float _firstChanceBound = 0.5f;
    private float _secondChanceBound = 0.75f;

    private BoxCollider2D[] _spawnCheckArray = new BoxCollider2D[1];
    private bool _stableLevel = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(initialDelay);
        _isSpawning = true;
    }
    
    private void Update()
    {
        if (!_stableLevel)
        {
            if (ExperienceManager.Instance.GetCurrentLevel() == 1)
            {
                _firstChanceBound = 1;
                _secondChanceBound = 1;
            }
            
            if (ExperienceManager.Instance.GetCurrentLevel() == 3)
            {
                _firstChanceBound = 0.5f;
                _secondChanceBound = 0.5f;
            }
            if (ExperienceManager.Instance.GetCurrentLevel() == 6)
            {
                _firstChanceBound = 0.5f;
                _secondChanceBound = 0.75f;
                _stableLevel = true;
            }
        }
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (!_isSpawning) return;
        
        if (EnemyCount < GetEnemiesMaxNumber() && Time.time - _timeFromLastSpawn > _timeBetweenSpawns)
        {
            _currentSpawnDirection = Quaternion.AngleAxis(_spawnRotationAngel, playerTransfrom.forward) * _currentSpawnDirection;
            Vector3 _spawnPosition = playerTransfrom.position + _currentSpawnDirection;
            float chance = Random.value;
            if (chance <= _firstChanceBound)
            {
                if (Physics2D.OverlapBoxNonAlloc(_spawnPosition, _simpleEnemyPrefab.GetComponent<BoxCollider2D>().size, 0f, _spawnCheckArray, _obstacleToSpawn.value) > 0)
                    return;
                Instantiate(_simpleEnemyPrefab, playerTransfrom.position + _currentSpawnDirection, Quaternion.identity);
                _firstChanceBound -= 0.01f;
                _secondChanceBound -= 0.005f;
            }
            else if (chance > _secondChanceBound)
            {
                if (Physics2D.OverlapBoxNonAlloc(_spawnPosition, _magicEnemyPrefab.GetComponent<BoxCollider2D>().size, 0f, _spawnCheckArray, _obstacleToSpawn.value) > 0)
                    return;
                Instantiate(_magicEnemyPrefab, playerTransfrom.position + _currentSpawnDirection, Quaternion.identity);
                _secondChanceBound += 0.01f;
                _firstChanceBound += 0.005f;
            }
            else
            {
                if (Physics2D.OverlapBoxNonAlloc(_spawnPosition, _shotGunEnemyPrefab.GetComponent<BoxCollider2D>().size, 0f, _spawnCheckArray, _obstacleToSpawn.value) > 0)
                    return;
                Instantiate(_shotGunEnemyPrefab, playerTransfrom.position + _currentSpawnDirection, Quaternion.identity);
                _firstChanceBound += 0.005f;
                _secondChanceBound -= 0.005f;
            }
            ++EnemyCount;
            _timeFromLastSpawn = Time.time;
        }
    }

    public void EnemyDeath()
    {
        --EnemyCount;
    }

    public float GetEnemyDamage()
    {
        return _levelToDamage.Evaluate(ExperienceManager.Instance.GetCurrentLevel());
    }

    public float GetEnemyHealth()
    {
        return _levelToHealth.Evaluate(ExperienceManager.Instance.GetCurrentLevel());
    }

    private float GetEnemiesMaxNumber()
    {
        return _maxEnemiesOnLevel.Evaluate(ExperienceManager.Instance.GetCurrentLevel());
    }


    







}
