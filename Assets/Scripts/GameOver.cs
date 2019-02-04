using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets {
    public class GameOver : MonoBehaviour {
        public GameObject goScreen;
        public Text result;
        public TowerInfo tinfo;

        public void MatchStats() {
            Time.timeScale = 0; 
            if (tinfo.friendlyTower.Destroyed) {
                result.text = "You lose!\nYour tower was destroyed!";
            } else {
                result.text = "You win!\nYou destroyed the enemy tower!";
            }
        }

    }
}
