using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

[System.Serializable]
public class AudioEntry
{
    public AudioType audioType;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    public bool loop;
}



public class AudioManager : MonoBehaviour, IGameSystem, IPausable, IQuittable
{
    public AudioSource sfxSource;
    public AudioSource backgroungMusic;

    private List<IDisposable> _eventSubscription = new List<IDisposable>();
    public async Task PlayMusic(AudioEntry track)
    {
        StopMusic();
        await Task.Yield();
        if (track == null)
        {
            Debug.LogWarning("PlayMusic: track is null");
            return;
        }

        backgroungMusic.clip = track.clip;
        backgroungMusic.volume = track.volume;
        backgroungMusic.loop = track.loop;
        backgroungMusic.Play();
        Debug.Log($"started {track.clip.ToString()}");
    }
    private void StopMusic()
    {
        backgroungMusic.Stop();
        Debug.Log($"stopped müzik");
    }
    public void PlaySFX(AudioEntry track)
    {
        if (track == null)
        {
            Debug.LogWarning("PlayMusic: track is null");
            return;
        }
        sfxSource.clip = track.clip;
        sfxSource.volume = track.volume;
        sfxSource.loop = track.loop;
        sfxSource.PlayOneShot(track.clip);
        
        Debug.Log($"started sfx");
    }
    private async Task HandlePlaying(PlayAudioEvent e)
    {
        if (e.AudioEntry.audioType == AudioType.Music)
        {
            await PlayMusic(e.AudioEntry);
        }
        else if (e.AudioEntry.audioType == AudioType.SFX)
        {
            PlaySFX(e.AudioEntry);
        }
    }
    // private void HandleStopPlaying(StopPlayingAudioEvent e)
    // {
    //     StopMusic();
    // }
    public async Task Initialize()
    {
        _eventSubscription.Add(EventBus.Subscribe<PlayAudioEvent>(HandlePlaying));

        _eventSubscription.Add(EventBus.Subscribe<ButtonClickedEvent>(HandleButtonClick));
        // _eventSubscription.Add(EventBus.Subscribe<StopPlayingAudioEvent>(HandleStopPlaying));

        await Task.CompletedTask;
    }
    private void HandleButtonClick(ButtonClickedEvent e)
    {
        
    }
    public async Task Shutdown()
    {
        try
        {
            foreach (var sub in _eventSubscription)
            {
                sub?.Dispose();

            }
            _eventSubscription.Clear();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Debug.LogError($"UI Manager shutdown error: {ex}");
            throw;
        }
    }

    public void OnPause()
    {
        StopMusic();
    }
    public void OnResume()
    {

    }

    public void OnQuit()
    {
        StopMusic();
    }

}

