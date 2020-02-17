using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;

    [SerializeField] private Text _laserInventoryText;

    [SerializeField] private Image _livesImg;

    [SerializeField] private Text _gameOverText;

    [SerializeField] private Text _restartGameText;

    [SerializeField] private Sprite[] _liveSprites;

    [SerializeField] private Image _shieldsImg;

    [SerializeField] private Sprite[] _shieldSprites;

    [SerializeField] private AudioClip _explosionSoundClip;

    [SerializeField] private Slider _thrustSlider;

    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private GameObject _player;
    private Animator _anim;
    private float _destroyClipLength;
    private bool _gameOver = false;

    void Start()
    {
        _scoreText.text = "Score: 0";
        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("UIManager: Unable to find Game_Manager");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) {
            Debug.LogError("UIManager: Unable to find SpawnManager script");
        }

        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("UIManager: Unable to find Player game object.");
        }

        _anim = _player.GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("UIManager: Unable to find Animator");
        }

        AnimationClip[] clips = _anim.runtimeAnimatorController.animationClips;
        _destroyClipLength = clips[0].length;
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives >= 0) // Checks for case when Player hit twice by two Lasers
        {
            
             _livesImg.sprite = _liveSprites[currentLives];
            

            if (currentLives < 1)
            {
                GameOverSequence();
            }
        }
        
    }

    public void UpdateShields(int shieldsLeft)
    {
       if (shieldsLeft >=0)
        {
            _shieldsImg.sprite = _shieldSprites[shieldsLeft];
        }
    }

    private void GameOverSequence()
    {
        _gameOver = true;
        UpdateThrusterHUD(0.0f);
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        if (_spawnManager != null)
        {
            _spawnManager.OnPlayerDeath();
        }
        _gameManager.GameOver();
        StartCoroutine(GameOverFlickerRoutine());
        GameObject em = GameObject.Find("Explosive_Mine(Clone)");
        if (em != null)
        {
            Destroy(em);
        }
        if (_player != null)
        {
            _anim.SetTrigger("OnEnemyDeath");
            AudioSource _audioSource = _player.GetComponent<AudioSource>();
            _audioSource.clip = _explosionSoundClip;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            _player.GetComponent<Player>().SetEngines(0);
            _player.GetComponent<Player>().RemoveMine();
            Destroy(_player, _destroyClipLength);
        }
    }

    private IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateLaserInventory(int laserInventory)
    {
        if (_gameOver == true)
        {
            _laserInventoryText.text = "Laser Inv: 0";
            _laserInventoryText.color = new Color(1.0f, 1.0f, 1.0f);
            return;
        }
        if (_player.GetComponent<Player>().IsMineLoaded() == true)
        {
            _laserInventoryText.text = "Mine Loaded";
            _laserInventoryText.color = new Color(1.0f, 0.65f, 0.0f);
        } else
        {
            _laserInventoryText.text = "Laser Inv: " + laserInventory;
            if (laserInventory < 1)
            {
                _laserInventoryText.color = new Color(1.0f, 0.0f, 0.0f);
            }
            else if (laserInventory < 5)
            {
                _laserInventoryText.color = new Color(1.0f, 1.0f, 0.0f);
            }
            else
            {
                _laserInventoryText.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }  
    }

    public void SetMaxThrusterHUD(float maxValue)
    {
        _thrustSlider.maxValue = maxValue;
    }

    public void UpdateThrusterHUD(float value)
    {
        if (_gameOver == false)
        {
            _thrustSlider.value = value;
        } else
        {
            _thrustSlider.value = 0.0f;
        }
    }
}
