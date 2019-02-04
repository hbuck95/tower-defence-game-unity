using UnityEngine;
using UnityEngine.UI;

namespace Assets {
    public class UIOverlay : MonoBehaviour {

        [SerializeField]
        private Image[] _playerUpgrades = new Image[6];
        [SerializeField]
        private Text _playerLevel, _playerCredits, _playerExperience, _playerExperienceToNextLevel, _buildingCount, _unitCount,_GTCount, _LTCount, _EBCount, _MLCount, _IUCount, _DUCount, _HWUCount;
        [SerializeField]
        private UpgradeMenu _upgradeMenu;
        [SerializeField]
        private GameObject _playerUpgradesMenu, _totalUnitcount, _totalBuildingCount, _upgradeMenuObject;

        public void RefreshUpgradeView() {
            int i = 0;
            foreach (var upgrade in Player.human.playerUpgrades) {
                _playerUpgrades[i].enabled = true;
                _playerUpgrades[i].sprite = _upgradeMenu.upgradeSprites[upgrade.Id];
                i++;
            }
        }

        public void OpenPlayerUpgrades() {
            if (_playerUpgradesMenu.activeSelf) {
                _playerUpgradesMenu.SetActive(false);
            } else {
                _playerUpgradesMenu.SetActive(true);
                RefreshUpgradeView();
            }
        }

        public void ToggleUnitCount() {
            _totalUnitcount.SetActive(!_totalUnitcount.activeSelf);
        }

        public void ToggleBuildingCount() {
            _totalBuildingCount.SetActive(!_totalBuildingCount.activeSelf);
        }

        public void ToggleUpgradeMenu() {
            if (!_upgradeMenuObject.activeSelf) {
                _upgradeMenu.Open();
            } else {
                _upgradeMenu.Close();
            }
        }

        private void Update() {
            _unitCount.text = Player.human.DeployedUnits.ToString();
            _buildingCount.text = Player.human.DeployedBuildings.ToString();
            _playerLevel.text = Player.human.Level.ToString();
            _playerCredits.text = Player.human.Credits.ToString();

            if (_totalUnitcount.activeSelf) {
                _IUCount.text = Player.human.infantryUnitCount.ToString();
                _DUCount.text = Player.human.demoUnitCount.ToString();
                _HWUCount.text = Player.human.hwUnitcount.ToString();
            }

            if (_totalBuildingCount.activeSelf) {
                _GTCount.text = Player.human.gunTurretCount.ToString();
                _LTCount.text = Player.human.laserTurretCount.ToString();
                _EBCount.text = Player.human.energyBarrierCount.ToString();
                _MLCount.text = Player.human.missileLauncherCount.ToString();
            }
        }
    }
}
