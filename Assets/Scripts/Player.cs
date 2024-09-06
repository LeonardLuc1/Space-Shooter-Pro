using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;

    [SerializeField]
    private float _boostFactor = 1.5f;
    private float _boostMultiplier = 1f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _maxLives = 3;
    private SpawnManager _spawnManager;

    private Vector3 _laserOffset = new Vector3(0, 1.05f, 0);

    [SerializeField]
    private float _thrusterFuel = 100.0f;
    [SerializeField]
    private float _thrusterFuelUse = 10.0f;
    [SerializeField]
    private Slider _myThrusters;
    [SerializeField]
    private float _myThrusterSliderMaxValue = 100f;

    private bool _isScatterShotActive = false;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private int _shieldStrength = 3;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;

    [SerializeField]
    private int _ammoCount = 15;
    [SerializeField]
    private int _maxAmmoCount = 30;
    [SerializeField]
    private Text _ammoText;

    [SerializeField]
    private int _numberOfProjectiles = 5;
    [SerializeField]
    private float _angleStep = 15f;

    private Coroutine _thrusterUp;

    public bool isTurboBoostActive = true;

    private CameraShake _playerShake;


    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _playerShake = GameObject.Find("Camera").GetComponent<CameraShake>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manger is NULL.");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
        if (_audioSource == null)
        {
            Debug.Log("Audio Source on the Player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
        
        _ammoCount = 15;
        _uiManager.UpdateAmmo(_ammoCount);
        _uiManager.UpdateSlider(_thrusterFuel);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }


    void CalculateMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _boostMultiplier = _boostFactor;
            isTurboBoostActive = true;
        }
            
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _boostMultiplier = 1f;
            isTurboBoostActive = false;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (isTurboBoostActive == true && _thrusterFuel > 0.01f)
        {
            if (_thrusterUp == null)
            {
                _thrusterUp = StartCoroutine(ThrustersDown());
            }                                  
        }
        else
        {
            if (_thrusterUp != null)
            {
                StopCoroutine(_thrusterUp);
                _thrusterUp = null;
            }      
            
            StartCoroutine(ThrustersUp());
        }

        transform.Translate(direction * _speed * _boostMultiplier * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x >= 11.3)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -11.3)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }

    }

    void FireLaser()
    {
        if (_ammoCount <= 0)
        {
            return;
        }

        _ammoCount--;
        _uiManager.UpdateAmmo(_ammoCount);
        
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true && _isScatterShotActive == false)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isTripleShotActive == false && _isScatterShotActive == true)
        {
            FireScatterShot();
        }

        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void FireScatterShot()
    {
        float startAngle = -_angleStep * (_numberOfProjectiles - 1) / 2f;

        for (int i = 0; i < _numberOfProjectiles; i++)
        {
            float currentAngle = startAngle + i * _angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Instantiate(_laserPrefab, transform.position + _laserOffset, rotation);
        }
    }

    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            _shieldStrength--;
            if (_shieldStrength == 2)
            {
                StartCoroutine(ShieldHitVisual());
                return;
            }
            else if (_shieldStrength == 1)
            {
                StartCoroutine(ShieldHitVisual());
                return;
            }
            else if (_shieldStrength < 1)
            {
                _isShieldsActive = false;
                _shieldVisualizer.SetActive(false);
                return;
            }
        }
    
        _lives--;       

        if (_lives == 2)
        {
            _playerShake.ActivateShake();
            _leftEngine.SetActive(true);
        }

        else if (_lives == 1)
        {
            _playerShake.ActivateShake();
            _rightEngine.SetActive(true);
        }
 
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _playerShake.ActivateShake();
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }   

    IEnumerator ShieldHitVisual()
    {
        _shieldRenderer.color = Color.red;
        yield return new WaitForSeconds(.5f);
        _shieldRenderer.color = Color.white;
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
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldStrength = 3;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void AddAmmo()
    {
        _ammoCount = _maxAmmoCount;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void UpdateAmmoCount()
    {
        _ammoCount = _ammoCount + 15;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void UpdateAmmo(int playerammo)
    {
        _ammoText.text = "Ammo: " + playerammo.ToString();
    }


    public void AddHealth()
    {
        if (_lives < _maxLives)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }

        if (_lives == 3)
        {
            _leftEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightEngine.SetActive(false);
        }
    }

    public void ScatterShotActive()
    {
        _isScatterShotActive = true;
        _numberOfProjectiles = 5;
        _angleStep = 15f;
        StartCoroutine(ScatterShotPowerDown());
    }

    IEnumerator ScatterShotPowerDown()
    {
        while (_isScatterShotActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isScatterShotActive = false;
        }
    }

    IEnumerator ThrustersDown()
    {
        while (_thrusterFuel > 0)
        {
            _thrusterFuel -= _thrusterFuelUse * Time.deltaTime;            
            _uiManager.UpdateSlider(_thrusterFuel);
            yield return null;

            if (_thrusterFuel <= 0.0f)
            {
                _thrusterFuel = 0.0f;
                _uiManager.UpdateSlider(_thrusterFuel);
                isTurboBoostActive = false;
            }
        }
    }
    IEnumerator ThrustersUp()
    {
        while (_thrusterFuel < _myThrusterSliderMaxValue && isTurboBoostActive == false)
        {
            _thrusterFuel += 0.15f * Time.deltaTime;
            _uiManager.UpdateSlider(_thrusterFuel);
            yield return null;

            if (_thrusterFuel > _myThrusterSliderMaxValue)
            {
                _thrusterFuel = _myThrusterSliderMaxValue;
                _uiManager.UpdateSlider(_thrusterFuel);
            }
        }
    }
}


