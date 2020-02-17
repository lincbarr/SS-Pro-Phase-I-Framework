                                                                                                                                       using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private AudioClip _explosionAudioClip;
    [SerializeField] private GameObject _laserPrefab;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;

    private float _destroyClipLength;
    

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy: Unable to find Player script.");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Enemy: Unable to find Animator");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Enemy: Unable to find AudioSource");
        }

        AnimationClip[] clips = _anim.runtimeAnimatorController.animationClips;
        _destroyClipLength = clips[0].length;

        _audioSource.clip = _explosionAudioClip;

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.0f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7.0f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                
                if (_player != null)
                {
                    _player.Damage();
                }

                _speed = 0;
                _canFire = Time.time + _destroyClipLength + 1.0f;  //Turn off Laser firing until Enemy is destroyed
                _anim.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, _destroyClipLength);
                break;

            case "Laser":
                Destroy(other.gameObject);
                LaserDestroysEnemy();
                break;

            case "Mine":
                GameObject _explosiveMine = GameObject.Find("Explosive_Mine(Clone)");
                if (_explosiveMine == null)
                {
                    Debug.Log("Enemy: Unable to find Explosive_Mine");
                    return;
                }
                if (_player != null)
                {
                    _player.AddScore(10);
                }
                _speed = 0;
                _canFire = Time.time + _destroyClipLength + 1.0f; //Turn off Laser firing until Enemy is destroyed
                _explosiveMine.gameObject.GetComponent<ExplosiveMine>().Explode();
                _anim.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, _destroyClipLength);
                break;

        }
    }

    public void LaserDestroysEnemy()
    {
        if (_player != null)
        {
            _player.AddScore(10);
        }

        _speed = 0;
        _canFire = Time.time + _destroyClipLength + 1.0f; //Turn off Laser firing until Enemy is destroyed
        _anim.SetTrigger("OnEnemyDeath");
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, _destroyClipLength);
    }
}

