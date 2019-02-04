using UnityEngine;
using UnityEngine.UI;

namespace Assets {
    public class TowerInfo : MonoBehaviour {
        public Tower enemyTower, friendlyTower;
        public Text etHP, ftHP;
        public RawImage etC, ftC;
        public GameOver go;

        private void Start() {
            etC.color = OptionsMenu.EnemyUnitColour;
            ftC.color = OptionsMenu.FriendlyUnitColour;
        }
        
        private void Update() {

            if(enemyTower.CurrentHealth <= 0) {
                enemyTower.Destroyed = true;
                enemyTower.CurrentHealth = 0;
                go.goScreen.SetActive(true);
                go.MatchStats();
            }

            if (friendlyTower.CurrentHealth <= 0) {
                friendlyTower.Destroyed = true;
                enemyTower.CurrentHealth = 0;
                go.goScreen.SetActive(true);
                go.MatchStats();
            }

            etHP.text = string.Format("{0}/{1}", enemyTower.CurrentHealth, enemyTower.MaxHealth);
            ftHP.text = string.Format("{0}/{1}", friendlyTower.CurrentHealth, friendlyTower.MaxHealth);
        }
    }
}
