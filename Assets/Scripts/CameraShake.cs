using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam;
    private bool _cameraShakeEnabled;
    private float _shakeAmount = 0.01f;

    private void Update()
    {
        BeginShakeCheck();
    }

    public void ActivateShake()
    {
        _cameraShakeEnabled = true;
    }


    void BeginShakeCheck()
    {
        if (_cameraShakeEnabled == true)
        {
            Vector3 camPos = _mainCam.transform.position;

            float offSetX = Random.value * _shakeAmount * 2 - _shakeAmount;
            float offSetY = Random.value * _shakeAmount * 2 - _shakeAmount;
            camPos.x += offSetX;
            camPos.y += offSetY;

            _mainCam.transform.position = camPos;

            StartCoroutine(StopShake());
        }
    }

    IEnumerator StopShake()
    {
        yield return new WaitForSeconds(1f);
        _cameraShakeEnabled = false;
    }
}