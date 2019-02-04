using UnityEngine;
using System.Collections.Generic;

namespace Assets {

    public class Initialise : MonoBehaviour {

        public GameObject c;
        public List<Material> friendlyMaterial, enemyMaterial;
        public Light friendlyTowerLight, enemyTowerLight;

        private void Start() {
            Time.timeScale = 1;
            c.SetActive(true);
            UpdateColours();
        }

        public void UpdateColours() {
            string colourMap = "";

            friendlyTowerLight.color = OptionsMenu.FriendlyUnitColour;
            enemyTowerLight.color = OptionsMenu.EnemyUnitColour;
 
           foreach (var m in friendlyMaterial) {
                if (m.name.Contains("_friendly") || m.name.ToLower().Contains("turret") || m.name.ToLower().Contains("energy")) {
                    colourMap = "_Color";
                } else {
                    colourMap = "_EmissionColor";
                }

                if (m.name.ToLower().Contains("energy")) {
                    Color32 energyBarrierColour = new Color32(OptionsMenu.FriendlyUnitColour.r, OptionsMenu.FriendlyUnitColour.g, OptionsMenu.FriendlyUnitColour.b, 90);
                    m.SetColor(colourMap, energyBarrierColour);
                }                else {
                    m.SetColor(colourMap, OptionsMenu.FriendlyUnitColour);
                }
            }

            foreach (var m in enemyMaterial) {
                if (m.name.Contains("_enemy") || m.name.ToLower().Contains("turret") || m.name.ToLower().Contains("energy")) {
                    colourMap = "_Color";
                } else {
                    colourMap = "_EmissionColor";
                }

                if (m.name.ToLower().Contains("energy")) {
                    Color32 energyBarrierColour = new Color32(OptionsMenu.EnemyUnitColour.r, OptionsMenu.EnemyUnitColour.g, OptionsMenu.EnemyUnitColour.b, 90);
                    m.SetColor(colourMap, energyBarrierColour);
                }
                else {
                    m.SetColor(colourMap, OptionsMenu.EnemyUnitColour);       
                }

            }

            friendlyTowerLight.enabled = true;
            enemyTowerLight.enabled = true;
        }
    }
}