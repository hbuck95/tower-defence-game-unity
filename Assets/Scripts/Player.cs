using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Assets {
    public class Player : MonoBehaviour    {
        public static Player human, cpu;
        private const int BASE_XP = 200;
      private int _totalXP, _xpToNextLevel;
     /*     private int _level = 0;
        public int credits = 500;
        private int _playerId;
        private float _spawnCooldown = 5f;
        private int _maxUnits = 10;
        private int _maxBuildings = 5;
        private int _deployedUnits, _deployedBuildings;
        private bool _unlockedExtraUpgradeSlot;*/

    
        public int Credits { get; set; }
        public int Level { get; private set; }
        public int PlayerId { get; private set; }
        public float SpawnCooldown { get; private set; }
        public int MaxUnits { get; private set; }
        public int MaxBuildings { get; private set; }
        public int DeployedBuildings { get; private set; }
        public int DeployedUnits { get; private set; }
        public int energyBarrierCount = 0;
        public int gunTurretCount = 0;
        public int laserTurretCount = 0;
        public int missileLauncherCount = 0;
        public int infantryUnitCount = 0;
        public int demoUnitCount = 0;
        public int hwUnitcount = 0;
        public bool UnlockedExtraUpgradeSlot { get; private set; }
        public List<Upgrade> playerUpgrades = new List<Upgrade>();//contains upgrades for the player (e.g additional slots);
        public static bool gameOver;

        private void Start() {
            human = new Player(1);//Human Player
            cpu = new Player(2);//Computer Player
        }

        public Player(int playerId) {
            PlayerId = playerId;
            Level = 0;
            Credits = 1000;
            SpawnCooldown = 5f;
            MaxUnits = 10;
            MaxBuildings = 10;
            DeployedBuildings = 0;
            DeployedUnits = 0;
            UnlockedExtraUpgradeSlot = false;
            LevelUp();
        }

        public bool HasUpgrade(Upgrade upgrade) {
            return playerUpgrades.Contains(upgrade);
        }

        public void AddUpgrade(Upgrade upgrade) {
            playerUpgrades.Add(upgrade);


            //Using if => return for readability
            if(upgrade.Id == 4) {//Fast Production
                ReduceCooldown(0.5f);
                Debug.Log("'Fast Production' upgrade detected. Decreasing cooldown by 0.5 seconds.");
                return;
            }

            if(upgrade.Id == 5) {//Increased Production
                IncreaseUnitSlots(2);
                Debug.Log("'Increased Production' upgrade detected. Increasing unit slots by 2.");
                return;
            }

            if(upgrade.Id == 6) {//Extra Building Slot
                IncreaseBuildingSlots(1);
                Debug.Log("'Extra Building Slot' upgrade detected. Increasing building slots by 1.");
                return;
            }

            if(upgrade.Id == 8) {
                UnlockedExtraUpgradeSlot = true;
                Debug.Log("'Extra Upgrade Slot' detected. Increasing maximum upgrade slots by 1.");
                return;
            }

            if(upgrade.Id == 12) {
                Debug.Log("'Advanced Production' upgrade detected. Increasing unit slots by 4.");
                IncreaseUnitSlots(4);
                return;
            }

            if(upgrade.Id == 13) {
                Debug.Log("Extra Building Slots' upgrade detected. Increasing building slots by 2.");
                IncreaseBuildingSlots(2);
                return;
            }

        }

        public void AddCredits(int amount) {
            Credits += amount;
        }

        public void DeployUnit() {
            DeployedUnits++;
        }

        public void DeployBuilding() {
            DeployedBuildings++;
        }

        public void KillUnit(int type) {
            switch (type) {
                case 0:
                    infantryUnitCount--;
                    break;
                case 1:
                    demoUnitCount--;
                    break;
                case 2:
                    hwUnitcount--;
                    break;
            }

            DeployedUnits--;
        }

        public void DestroyBuilding(int type) {
            switch (type) {
                case 1:
                    gunTurretCount--;
                    break;
                case 2:
                    laserTurretCount--;
                    break;
                case 3:
                    missileLauncherCount--;
                    break;
                case 4:
                    energyBarrierCount--;
                    break;
                default:
                    break;
            }
            DeployedBuildings--;
        }

        public void SpendCredits(int amount) {
            Credits -= amount;
        }

        public void LevelUp() {
            Level++;
            _xpToNextLevel = Mathf.FloorToInt((float)(BASE_XP * Math.Pow(double.Parse(Level.ToString()), 0.8)));//base_xp * (level_to_get ^ factor)

            if(PlayerId == 1)
                Debug.Log(string.Format("You levelled up!\nYou are now level {0}", Level));            
        }

        public void AddXP(int XP) {
            _totalXP += XP;
            if (_xpToNextLevel - XP <= 0) {
                LevelUp();
            } else {
                _xpToNextLevel -= XP;
            }
        }

        public void ReduceCooldown(float time) {
            SpawnCooldown -= time;
        }

        public void IncreaseUnitSlots(int slots) {
            MaxUnits += slots;
        }

        public void IncreaseBuildingSlots(int slots) {
            MaxBuildings += slots;
        }
    }
}