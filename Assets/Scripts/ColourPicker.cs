using UnityEngine;
using UnityEngine.UI;

namespace Assets {
    public class ColourPicker : MonoBehaviour {
        [SerializeField]
        private Slider _r, _g, _b;
        [SerializeField]
        private Text _rValue, _gValue, _bValue;
        [SerializeField]
        private RawImage _selectedColour;
        [SerializeField]
        private OptionsMenu _optionsMenu;

        public void OK() {
            if (_optionsMenu.PickingEnemyColour) {
                OptionsMenu.EnemyUnitColour = _selectedColour.color;
                _optionsMenu.PickingEnemyColour = false;
                _optionsMenu.EnemyColour.color = _selectedColour.color;
                Debug.Log(OptionsMenu.EnemyUnitColour.ToString());
            }

            if (_optionsMenu.PickingFriendlyColour) {
                OptionsMenu.FriendlyUnitColour = _selectedColour.color;
                _optionsMenu.PickingFriendlyColour = false;
                _optionsMenu.FriendlyColour.color = _selectedColour.color;
                Debug.Log(OptionsMenu.FriendlyUnitColour.ToString());
            }


            _optionsMenu.UpdateColours();
            Close();
        }

        public void Cancel() {
            _optionsMenu.PickingEnemyColour = false;
            _optionsMenu.PickingFriendlyColour = false;
            Close();
        }

        public void Open() {
            gameObject.SetActive(true);
            Color32 c = _optionsMenu.PickingEnemyColour ? OptionsMenu.EnemyUnitColour : OptionsMenu.FriendlyUnitColour;

            _r.value = c.r;
            _g.value = c.g;
            _b.value = c.b;
        }

        public void Close() {
            _r.value = 255;
            _g.value = 255;
            _b.value = 255;
            gameObject.SetActive(false);
        }

        public void OnSliderValueChange(int slider) {
            switch (slider) {
                case 0:
                    _rValue.text = _r.value.ToString();
                    break;
                case 1:
                    _gValue.text = _g.value.ToString();
                    break;
                case 2:
                    _bValue.text = _b.value.ToString();
                    break;
            }

            _selectedColour.color = new Color32((byte)_r.value, (byte)_g.value, (byte)_b.value, 255);

        }
    }
}