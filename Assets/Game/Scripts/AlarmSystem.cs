using System;
using System.Collections;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeSpeed = 0.5f;

    private Coroutine _currentFadeCoroutine;

    private const float MaxVolume = 1f;
    private const float MinVolume = 0.01f;
    private const float StartVolume = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Criminal criminal))
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.volume = StartVolume;
                _audioSource.Play();
                StartCoroutine(ChangeVolume(MaxVolume));
            }

            CheckAndStopCoroutine();

            _currentFadeCoroutine = StartCoroutine(ChangeVolume(MaxVolume));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Criminal criminal))
        {
            CheckAndStopCoroutine();

            _currentFadeCoroutine = StartCoroutine(ChangeVolume(StartVolume, () => 
            {
                if (_audioSource.volume <= MinVolume)
                {
                    _audioSource.Stop();
                }
            }));
        }
    }

    private void CheckAndStopCoroutine()
    {
        if (_currentFadeCoroutine != null)
        {
            StopCoroutine(_currentFadeCoroutine);
        }
    }

    private IEnumerator ChangeVolume(float targetVolume, Action onComplete = null)
    {
        while (!Mathf.Approximately(_audioSource.volume, targetVolume))
        {
            _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetVolume, _fadeSpeed * Time.deltaTime);
            yield return null;
        }
        onComplete?.Invoke();
    }
}