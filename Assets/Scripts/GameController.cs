using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return _instance; } }
    public int Score;
    public float SecondsSinceStart;
    public bool IsGamePaused;
    [HideInInspector]
    public bool IsGameOver;
    [HideInInspector]
    public GameObject[] StoreShips;

    private static GameController _instance;
    private Text scoreText;
    private ParticleSystem[] particleSystems;
    private ES2Settings _fileSettings;
    private CanvasGroup _headerPanel;
    private CanvasGroup _gameOverWindow;
    private Store _store;
    private bool _isGameStarting;
    private List<EnemyKillStat> _enemyKillStats; //used to populate Game Over stats screen
    private const string _enemyKillStatsKey = "EnemyKillStats";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
       
    // Use this for initialization
	void Start ()
    {
        _isGameStarting = true;
        IsGameOver = false;

        _store = FindObjectOfType<Store>();

        _fileSettings = new ES2Settings
        {
            encrypt = true,
            format = ES2Settings.Format.Binary,
            name = "gamedata",
            optimizeMode = ES2Settings.OptimizeMode.Fast,
            saveLocation = ES2Settings.SaveLocation.PlayerPrefs,
            encryptionType = ES2Settings.EncryptionType.AES128,
            encryptionPassword = "1z90lol4kk209f"
        };
        
        LoadStoreShips();

        var scrapTotal = GetPlayerSalvagePartsTotal();

        _gameOverWindow = GameObject.FindGameObjectWithTag("GameOver").GetComponent<CanvasGroup>();
        HideGameOverWindow();

        _headerPanel = GameObject.Find("HeaderPanel").GetComponent<CanvasGroup>();
        ShowHeaderPanel();

        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        Score = scrapTotal;

        //Always make sure the player owns the starting ship
        SetShipAsOwned("starter ship");

        //Restore the player's ship to the last one they used
        SetNewShip(GetLastShipUsed());

        LoadEnemyKillStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGamePaused)
        {
            return;
        }

        if (_isGameStarting)
        {
            _isGameStarting = false;
            _store.ShowStore();
        }

        if (scoreText != null)
        {
            scoreText.text = Score.ToString();
        }

        SecondsSinceStart += Time.deltaTime;
    }

    private void LoadEnemyKillStats()
    {
        _enemyKillStats = new List<EnemyKillStat>();

        if (ES2.Exists(_enemyKillStatsKey, _fileSettings))
        {
            _enemyKillStats = ES2.LoadList<EnemyKillStat>(_enemyKillStatsKey, _fileSettings);
        }

        //For a new session, we want to take the session kills and store them into last session
        _enemyKillStats.ForEach(ks => ks.KillsLastSession = ks.KillsThisSession);
        //then we want to start the kills for this session at 0.
        _enemyKillStats.ForEach(ks => ks.KillsThisSession = 0);
    }

    public void SaveEnemyKillStats()
    {
        ES2.Save(_enemyKillStats, _enemyKillStatsKey, _fileSettings);
    }

    public void AddKill(string enemyName)
    {
        var enemyStats = _enemyKillStats.FirstOrDefault(e => e.ScoreName == enemyName);

        if (enemyStats == null)
        {
            enemyStats = new EnemyKillStat{ ScoreName = enemyName };

            _enemyKillStats.Add(enemyStats);
        }

        enemyStats.KillsLifetime++;
        enemyStats.KillsThisSession++;

        SaveEnemyKillStats();
    }

    private void LoadStoreShips()
    {
        StoreShips = Resources.LoadAll<GameObject>("PlayerShips");
    }

    public int GetPlayerSalvagePartsTotal()
    {
        var result = 0;

        if (ES2.Exists("ScrapTotal", _fileSettings))
        {
            result = ES2.Load<int>("ScrapTotal", _fileSettings);
        }

        return result;
    }

    private void SetPlayerSalvagePartsTotal(int partsTotal)
    {
        ES2.Save<int>(partsTotal, "ScrapTotal", _fileSettings);
    }

    public string GetLastShipUsed()
    {
        var lastShip = "starter ship";

        if (ES2.Exists("LastShipUsed", _fileSettings))
        {
            lastShip = ES2.Load<string>("LastShipUsed", _fileSettings);
        }

        if (IsShipAlreadyOwned(lastShip) != true)
        {
            return "starter ship";
        }

        return lastShip;
    }

    public void SetLastShipUsed(string shipName)
    {
        ES2.Save<string>(shipName.ToLowerInvariant(), "LastShipUsed", _fileSettings);
    }

    public bool IsShipAlreadyOwned(string shipName)
    {
        var isOwned = false;

        if (ES2.Exists(shipName.ToLowerInvariant(), _fileSettings))
        {
            isOwned = ES2.Load<bool>(shipName.ToLowerInvariant(), _fileSettings);
        }

        return isOwned;
    }

    public void SetShipAsOwned(string shipName)
    {
        ES2.Save<bool>(true, shipName.ToLowerInvariant(), _fileSettings);
    }

    public void AddScore(int amount)
    {
        Score += amount;

        SetPlayerSalvagePartsTotal(Score);
    }

    public void RemoveScore(int amount)
    {
        Score = Math.Max(Score - amount, 0);

        SetPlayerSalvagePartsTotal(Score);
    }

    public void SetNewShip(string shipName)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        
        var newShip = StoreShips.FirstOrDefault(s => s.name.ToLowerInvariant() == shipName.ToLowerInvariant());        
        if (!newShip)
        {
            Debug.Log("Ship not found in game controller list");
            return;
        }
        var newPlayer = Instantiate(newShip, player.transform.position, player.transform.rotation);
        newPlayer.tag = "Player";
                
        // Kill ourselves
        Destroy(player);

        SetLastShipUsed(shipName);
    }

    public void PauseGame(bool pause)
    {
        IsGamePaused = pause;        
        particleSystems = FindObjectsOfType<ParticleSystem>();

        if (pause && !_store.IsOpen && !IsGameOver)
        {
            foreach (var ps in particleSystems.Where(p => p.isPlaying))
            {
                ps.Pause();
            }
        }
        else
        {
            foreach (var ps in particleSystems.Where(p => p.isPaused))
            {
                ps.Play();
            }
        }
    }

    public void HideHeaderPanel()
    {
        _headerPanel.alpha = 0;
        _headerPanel.interactable = false;
        _headerPanel.blocksRaycasts = false;
    }

    public void ShowHeaderPanel()
    {
        _headerPanel.alpha = 1;
        _headerPanel.interactable = true;
        _headerPanel.blocksRaycasts = true;
    }

    public void HideGameOverWindow()
    {
        _gameOverWindow.alpha = 0;
        _gameOverWindow.interactable = false;
        _gameOverWindow.blocksRaycasts = false;
    }

    public void ShowGameOverWindow()
    {
        _gameOverWindow.alpha = 1;
        _gameOverWindow.interactable = true;
        _gameOverWindow.blocksRaycasts = true;
    }

    public void GameOver()
    {
        IsGameOver = true;
        PauseGame(true);
        HideHeaderPanel();
        PopulateGameOverStats();
        ShowGameOverWindow();
    }

    private void PopulateGameOverStats()
    {
        var rowContainer = GameObject.FindWithTag("GameOverContents");
        var rowTemplate = Resources.Load("Game Over/Table Row");

        _enemyKillStats.OrderByDescending(s => s.KillsThisSession)
            .ThenByDescending(ss => ss.KillsLastSession)
            .ThenByDescending(ss => ss.KillsLifetime)
            .ToList()
            .ForEach(e =>
        {
            var newRow = (GameObject)Instantiate(rowTemplate, rowContainer.transform);

            var enemyName = newRow.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "EnemyName");
            enemyName.text = e.ScoreName;

            var killsThisSession = newRow.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "KillsThisSession");
            killsThisSession.text = e.KillsThisSession.ToString();

            var killsLastSession = newRow.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "KillsLastSession");
            killsLastSession.text = e.KillsLastSession.ToString();

            var killsLifetime = newRow.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "KillsLifetime");
            killsLifetime.text = e.KillsLifetime.ToString();

            //var killsPercentChange = newRow.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "KillsPercentChange");
            //double percentChange = (e.KillsThisSession / e.KillsLastSession) * 100;
            //killsPercentChange.text = percentChange.ToString();
        });
    }
}
