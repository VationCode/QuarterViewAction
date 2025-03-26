using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip[] audioClips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int clipNum)
    {
        audioSource.clip = audioClips[clipNum];
    }
}
