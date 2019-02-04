using UnityEngine;

namespace Assets {
    public class GUIDockMenus : MonoBehaviour {
        public GameObject addBuildingMenu, addUnitMenu;
        public GameObject addBuildingButton, addUnitButton;
        public UpgradeMenu upgradeMenu;
        private UnitFactory _unitFactory;
        private SelectionHandler _selectionHandler;

        private void Start() {
            _selectionHandler = GameObject.Find("ScriptHolder").GetComponent<SelectionHandler>();
            _unitFactory = GameObject.Find("ScriptHolder").GetComponent<UnitFactory>();
        }

        public void SpawnUnit(int unit) {
            _unitFactory.CreateUnit(unit, true);
        }

        public void SpawnBuilding(int building) {
            _selectionHandler.chosenBuilding = building;
            _selectionHandler.selectingBuildingLocation = true;
        }

        public void ToggleUnitMenu() {
            addUnitMenu.SetActive(!addUnitMenu.activeSelf);
        }

        public void ToggleBuildingMenu() {
            addBuildingMenu.SetActive(!addBuildingMenu.activeSelf);
        }

        public void ToggleUpgradeMenu() {
            if (upgradeMenu.gameObject.activeSelf) {
                upgradeMenu.Close();
            } else {
                upgradeMenu.Open();
            }

        }

    }
}
