using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Assets {
    public class OptionsMenu : MonoBehaviour {
        private float _masterVolume, _sfxVolume, _musicVolume;
        private int _language, _antiAliasLevel, _textureLevel;
        private Resolution _selectedResolution;
        private bool _enableFullscreen, _enableVsync;
        public static Color32 FriendlyUnitColour, EnemyUnitColour;
        public bool PickingFriendlyColour, PickingEnemyColour;
        public static bool loadedOptions = false;
        public static float MusicVolumeLevel, SFXVolumeLevel;

        public Material[] friendlyMaterial;

        public RawImage FriendlyColour, EnemyColour;

        [SerializeField]
        private Dropdown _languageDD, _antiAliasDD, _textureDD, _resolutionsDD;
        [SerializeField]
        private Toggle _vsyncTG, _drawFPSTG;
        [SerializeField]
        private Slider _masterS, _sfxS, _musicS;
        [SerializeField]
        private Text _masterVolValue, _sfxVolValue, _musicVolValue;
        [SerializeField]
        private ColourPicker _cp;

        private void Start() {
            _resolutionsDD.options.Clear();

            foreach (Resolution res in Screen.resolutions.Reverse()) {
                _resolutionsDD.options.Add(new Dropdown.OptionData { text = string.Format("{0} x {1}", res.width, res.height) });
            }

            for (var i = 0; i < _resolutionsDD.options.Count; i++) {
                string res = _resolutionsDD.options[i].text;
                int w = int.Parse(res.Substring(0, res.IndexOf('x') - 1));
                int h = int.Parse(res.Substring(res.IndexOf('x') + 1));
                if (w == Screen.currentResolution.width && h == Screen.currentResolution.height) {
                    _resolutionsDD.value = i;
                    break;
                }
            }

            int temp = _resolutionsDD.value;
            _resolutionsDD.value += 1;
            _resolutionsDD.value = temp;

        }

        public void Open() {
            gameObject.SetActive(true);

            /*   for (var i = 0; i < ResolutionsDD.options.Count; i++) {
                   string res = ResolutionsDD.options[i].text;
                   int w = int.Parse(res.Substring(0, res.IndexOf('x') - 1));
                   int h = int.Parse(res.Substring(res.IndexOf('x') + 1));
                   if (w == Screen.currentResolution.width && h == Screen.currentResolution.height) {
                       ResolutionsDD.value = i;
                       break;
                   }
               }

               int temp = ResolutionsDD.value;
               ResolutionsDD.value += 1;
               ResolutionsDD.value = temp;*/

            //ResetToDefault();
            /*
            MasterS.value = MasterVolume * 100;
            SFXS.value = SFXVolume * 100;
            MusicS.value = MusicVolume * 100;
            FriendlyColour.color = FriendlyUnitColour;
            EnemyColour.color = EnemyUnitColour;
            DrawFPSTG.isOn = EnableDrawFPS;
            VsyncTG.isOn = EnableVsync;
            LanguageDD.value = Language;
            TextureDD.value = TextureLevel;
            AntiAliasDD.value = AntiAliasingLevel;   */
            StartCoroutine(RetrieveSettings());

        }

        public void SliderValueChange(int slider) {
            switch (slider) {
                case 0:
                    _masterVolValue.text = string.Format("{0}%", _masterS.value);
                    break;
                case 1:
                    _sfxVolValue.text = string.Format("{0}%", _sfxS.value);
                    break;
                case 2:
                    _musicVolValue.text = string.Format("{0}%", _musicS.value);
                    break;
            }
        }

        public void PickFriendlyColour() {
            if(Application.loadedLevelName == "Main") {
                Debug.Log("You cannot change faction colours in the middle of a game!");
                return;
            }

            PickingFriendlyColour = true;
            _cp.Open();
        }

        public void PickEnemyColour() {
            if (Application.loadedLevelName == "Main") {
                Debug.Log("You cannot change faction colours in the middle of a game!");
                return;
            }

            PickingEnemyColour = true;
            _cp.Open();
        }

        public void Close() {
            gameObject.SetActive(false);
            PickingEnemyColour = false;
            PickingFriendlyColour = false;

            if(Application.loadedLevelName != "Main")
            _cp.Close();
        }

        public void ResetToDefault() {
            _masterVolume = 100f;
            _sfxVolume = 100f;
            _musicVolume = 100f;
            _language = 0;
            _antiAliasLevel = 0;
            _textureLevel = 0;
            _selectedResolution = Screen.currentResolution;
            _enableFullscreen = true;
            _enableVsync = false;
            EnemyColour.color = Color.red;
            FriendlyColour.color = Color.green;
            _masterS.value = 100f;
            _sfxS.value = 100f;
            _musicS.value = 100f;
            _vsyncTG.isOn = false;
            _languageDD.value = 0;
            _antiAliasDD.value = 0;
            _textureDD.value = 0;
            _resolutionsDD.value = 0;
            ApplySettings(0);
        }

        public void UpdateColours() {
            foreach (var m in friendlyMaterial) {
                float emission = Mathf.PingPong(Time.time, 1.0f);
                m.SetColor("_EmissionColor", (Color)FriendlyUnitColour * Mathf.LinearToGammaSpace(emission));
            }
        }

        public void ApplySettings(int sync) {

            if (PickingEnemyColour || PickingFriendlyColour)
                return;

            if (Application.loadedLevelName != "Main") {
                EnemyUnitColour = EnemyColour.color;
                FriendlyUnitColour = FriendlyColour.color;
            }
            _enableVsync = _vsyncTG.isOn;
            _textureLevel = _textureDD.value;
            _antiAliasLevel = _antiAliasDD.value;
            _language = _languageDD.value;
            _masterVolume = _masterS.value;
            _sfxVolume = _sfxS.value;
            _musicVolume = _musicS.value;
            Localisation.Localise.SetLanguage(_language);
            QualitySettings.vSyncCount = _enableVsync ? 2 : 0;

            string res = _resolutionsDD.options[_resolutionsDD.value].text;
            _selectedResolution.width = int.Parse(res.Substring(0, res.IndexOf('x') - 1));
            _selectedResolution.height = int.Parse(res.Substring(res.IndexOf('x') + 1));

            SetScreenResolution();
            if(sync == 1)
                StartCoroutine(SyncWithServer());
        }

        private void SetScreenResolution() {

            if (_selectedResolution.width != Screen.resolutions.Last().width && _selectedResolution.height != Screen.resolutions.Last().height) {
                _enableFullscreen = false;
            } else {
                _enableFullscreen = true;
            }

            Screen.SetResolution(_selectedResolution.width, _selectedResolution.height, _enableFullscreen, Screen.currentResolution.refreshRate);
            Camera.main.ResetAspect();
            Canvas.ForceUpdateCanvases();
        }

        private int SelectedAntiAliasLevel() {
            switch (_antiAliasDD.value) {
                case 0:
                    return 8;
                case 1:
                    return 4;
                case 2:
                    return 2;
                case 3:
                default:
                    return 0;
            }
        }

        private IEnumerator<WWW> SyncWithServer() {
            var request = new WWW(string.Format("http://localhost:3034/api/player/syncsettings?username={0}&masterVol={1}&sfxVol={2}&musicVol={3}&lang={4}&textureQuality={5}&antialiasing={6}&enableVsync={7}&drawFps={8}&enemycolourR={9}&enemyColourG={10}&enemyColourB={11}&friendlyColourR={12}&friendlyColourG={13}&friendlyColourB={14}&resHeight={15}&resWidth={16}", MenuSystem.loggedInUser, _masterVolume, _sfxVolume, _musicVolume, _language, _textureLevel, _antiAliasDD.value, _enableVsync, false, EnemyUnitColour.r, EnemyUnitColour.g, EnemyUnitColour.b, FriendlyUnitColour.r, FriendlyUnitColour.g, FriendlyUnitColour.b, _selectedResolution.height, _selectedResolution.width));
            yield return request;

            if (request.text == null || request.text == "") {
                Debug.Log("Error connecting to server.");
                yield break;
            }

            Debug.Log(request.text);

            if (int.Parse(request.text) == 1) {
                Debug.Log("Synced.");
            } else if (int.Parse(request.text) == 2) {
                Debug.Log("Error");
            } else {
                Debug.Log(request.text);
            }
        }

        private IEnumerator<WWW> RetrieveSettings() {
            var request = new WWW(string.Format("http://localhost:3034/api/player/getsettings?username={0}", MenuSystem.loggedInUser));
            yield return request;

            if (request.text == null || request.text == "") {
                Debug.Log("Error Connecting to server.");
                yield break;
            }

            if (request.text == "null") {
                Debug.Log("No settings saved.");
                ResetToDefault();
                yield break;
            }

            var json = request.text.Replace("\\", "");
            json = json.Substring(1, json.Length - 1);
            var settings = new JSONObject(json).ToDictionary();

            _masterVolume = float.Parse(settings["MasterVolume"]);
            _musicVolume = float.Parse(settings["MusicVolume"]);
            _sfxVolume = float.Parse(settings["SFXVolume"]);
            _language = int.Parse(settings["Language"]);
            _antiAliasLevel = int.Parse(settings["AntialiasingLevel"]);
            _textureLevel = int.Parse(settings["TextureLevel"]);
            _enableVsync = bool.Parse(settings["Vsync"]);           
            _selectedResolution.width = int.Parse(settings["ResWidth"]);
            _selectedResolution.height = int.Parse(settings["ResHeight"]);
            FriendlyUnitColour.r = byte.Parse(settings["FriendlyColourR"]);
            FriendlyUnitColour.g = byte.Parse(settings["FriendlyColourG"]);
            FriendlyUnitColour.b = byte.Parse(settings["FriendlyColourB"]);
            FriendlyUnitColour.a = 255;
            EnemyUnitColour.r = byte.Parse(settings["EnemyColourR"]);
            EnemyUnitColour.g = byte.Parse(settings["EnemyColourG"]);
            EnemyUnitColour.b = byte.Parse(settings["EnemyColourB"]);
            EnemyUnitColour.a = 255;
            EnemyColour.color = EnemyUnitColour;
            FriendlyColour.color = FriendlyUnitColour;
            _vsyncTG.isOn = _enableVsync;
            _textureDD.value = _textureLevel;
            _antiAliasDD.value = _antiAliasLevel;
            _languageDD.value = _language;
            _masterS.value = _masterVolume;
            _sfxS.value = _sfxVolume;
            _musicS.value = _musicVolume;

            MusicVolumeLevel = (_masterVolume / 100) * (_musicVolume / 100);
            SFXVolumeLevel = (_masterVolume / 100) * (_sfxVolume / 100);

            Localisation.Localise.SetLanguage(_language);
            QualitySettings.vSyncCount = _enableVsync ? Application.targetFrameRate : 0;
            QualitySettings.antiAliasing = SelectedAntiAliasLevel();

            for (var i = 0; i < _resolutionsDD.options.Count; i++)
            {
                string res = _resolutionsDD.options[i].text;
                int w = int.Parse(res.Substring(0, res.IndexOf('x') - 1));
                int h = int.Parse(res.Substring(res.IndexOf('x') + 1));
                if (w == _selectedResolution.width && h == _selectedResolution.height)
                {
                    _resolutionsDD.value = i;
                    Debug.Log("Found resolution at index" + i);
                    break;
                }
            }
            int temp = _resolutionsDD.value;
            _resolutionsDD.value += 1;
            _resolutionsDD.value = temp;
            SetScreenResolution();
            UpdateColours();

            Debug.Log("Settings Retrieved.");
            loadedOptions = true;
        }
    }
}