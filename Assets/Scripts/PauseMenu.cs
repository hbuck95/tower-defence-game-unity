using UnityEngine;

namespace Assets {
    public class PauseMenu : MonoBehaviour {
        public GameObject pauseMenu, guiElements;    
        public OptionsMenu optionsMenu;
        public UpgradeMenu upgradeMenu;
        public LoadingScreen loadingScreen;
        public AudioSource audioSource;
        public AudioClip click;

        public void MouseOverButton() {
            audioSource.PlayOneShot(click, OptionsMenu.SFXVolumeLevel);
        }

        public void Show() {
            pauseMenu.SetActive(true);
            guiElements.SetActive(false);
            optionsMenu.Close();
            upgradeMenu.Close();
            Time.timeScale = 0;
        }

        public void Resume() {            
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            optionsMenu.Close();
            guiElements.SetActive(true);
        }

        public void Restart() {
            loadingScreen.LoadLevel(1);
        }

        public void Settings() {
            if (!optionsMenu.gameObject.activeSelf) {
                upgradeMenu.Close();
                optionsMenu.Open();
            } else {
                optionsMenu.Close();
            }
        }

        public void goToMainMenu() {
            loadingScreen.LoadLevel(0);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (pauseMenu.activeSelf) {
                    Resume();
                } else {
                    Show();
                }
            }
        }

    }
}