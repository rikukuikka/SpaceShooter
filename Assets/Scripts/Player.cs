using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shields;
    [SerializeField]
    private int _score;
    private UIManager _canvas;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private AudioClip _laserSound;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    [SerializeField]
    private bool _isPlayer1 = false;
    [SerializeField]
    private bool _isPlayer2 = false;
    private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _canvas = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_canvas == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio source on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSound;
        }

        if (_gameManager == null)
        {
            Debug.LogError("Game manager is NULL");
        }

        if (_animator == null)
        {
            Debug.LogError("Player animator is NULL.");
        }

        if (_gameManager.IsCoopMode() == false)
        {
            transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            if (_isPlayer1)
            {
                transform.position = new Vector3(1.5f, 0, 0);
            }
            else
            {
                transform.position = new Vector3(-1.5f, 0, 0);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (_isPlayer1)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > _canFire)
            {
                fireLaser();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire2") && Time.time > _canFire)
            {
                fireLaser();
            }

        }
    }

    void CalculateMovement()
    {
        float horizontalInput;
        float verticalInput;
        if (_isPlayer1)
        {
            horizontalInput = Input.GetAxis("Horizontal1");
            verticalInput = Input.GetAxis("Vertical1");
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal2");
            verticalInput = Input.GetAxis("Vertical2");
        }
        if (horizontalInput > 0)
        {
            _animator.SetBool("Turn_Right", true);

        }
        else if (horizontalInput < 0)
        {
            _animator.SetBool("Turn_Left", true);
        }
        else
        {
            _animator.SetBool("Turn_Right", false);
            _animator.SetBool("Turn_Left", false);
        }

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void fireLaser()
    {
        _canFire = Time.time + _fireRate;
        Vector3 offset = new Vector3(0, 1.05f, 0);
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
        }
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _shields.SetActive(false);
            _isShieldActive = false;
            return;
        }
        _lives--;
        if (_lives == 2)
        {
            int engine = Random.Range(0, 2);
            if (engine == 0)
            {
                _rightEngine.SetActive(true);
            }
            else
            {
                _leftEngine.SetActive(true);
            }

        }
        else if (_lives == 1)
        {
            if (_leftEngine.activeSelf)
            {
                _rightEngine.SetActive(true);
            }
            else
            {
                _leftEngine.SetActive(true);
            }
        }
        _canvas.UpdateLives(_lives);
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedActive()
    {
        _speed = _speed * _speedMultiplier;
        StartCoroutine(SpeedPowerDownRoutine());
    }

    IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed = _speed / _speedMultiplier;
    }

    public void ShieldActive()
    {
        _shields.SetActive(true);
        _isShieldActive = true;
    }

    public void AddScore(int points)
    {
        _score += points;
        _canvas.UpdateScore(_score);
    }
}
