using System;
using System.Collections;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeSpeed = 0.5f;

    private Coroutine _currentFadeCoroutine;
    private const float MaxVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Criminal _))
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.volume = 0f;
                _audioSource.Play();
            }

            CheckAndStopCoroutine();
            _currentFadeCoroutine = StartCoroutine(ChangeVolume(MaxVolume));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Criminal _))
        {
            CheckAndStopCoroutine();
            _currentFadeCoroutine = StartCoroutine(ChangeVolume(0f, StopAudio));
        }
    }

    private void CheckAndStopCoroutine()
    {
        if (_currentFadeCoroutine != null)
        {
            StopCoroutine(_currentFadeCoroutine);
        }
    }

    private void StopAudio()
    {
        _audioSource.Stop();
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