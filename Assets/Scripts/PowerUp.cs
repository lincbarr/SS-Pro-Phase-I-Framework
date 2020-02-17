using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _downSpeed = 3.0f;
    [SerializeField] private int _powerUpID; // 0 = TripleShot, 1 = PowerUp, 2 = Shields, 3 = Ammo, 4 = Health
    
    private GameManager _gameManager;
    private AudioSource _audioSource;
    
    private void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("PowerUp: Unable to find GameManager script");
        }

        _audioSource = GameObject.Find("Audio_Manager").transform.GetChild(1).GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("PowerUp: Unable to find AudioSource component of PowerUp object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _downSpeed * Time.deltaTime);

        if (transform.position.y < -5.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                _audioSource.Play();

                switch (_powerUpID)
                {
                    case 0:
                        if (player.GetLaserInventory() >=3 )
                        {
                            player.TripleShotActive();
                        }
                        break;

                    case 1:
                        PlayerScript_InputAction inputAction = _gameManager.GetComponent<PlayerScript_InputAction>();
                        inputAction.SpeedPowerUp();
                        break;

                    case 2:
                        player.ActivateShields();
                        break;

                    case 3: // Ammo
                        player.ReloadLaserInventory();
                        break;

                    case 4: // Health
                        player.AddHealth();
                        break;

                    case 5: // Space Mine
                        player.LoadMine();
                        break;

                    default:
                        Debug.LogError("Unknown PowerUpID");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
