using UnityEngine;

namespace Assets {
    public class TitleMusic : MonoBehaviour {
        public AudioClip[] titleTracks = new AudioClip[2];
        private AudioSource _audioSource;
        private int _count = 0;
        private float _volume;

        private void Start() {
            _audioSource = GetComponent<AudioSource>();
            _volume = !OptionsMenu.loadedOptions ? .7f : OptionsMenu.MusicVolumeLevel;
            _audioSource.clip = titleTracks[0];
            _audioSource.PlayOneShot(_audioSource.clip, _volume);
            _count++;
            Debug.Log(string.Format("Volume: {0}", _volume));
        }

        private void Update() {
           if (!_audioSource.isPlaying)
               ChangeTrack();           
        }

        private void ChangeTrack() {
            AudioClip clip = _count == 0 ? titleTracks[0] : titleTracks[1];
            _count = _count == 0 ? 1 : 0;
            _audioSource.PlayOneShot(clip, _volume);
        }
    }
}