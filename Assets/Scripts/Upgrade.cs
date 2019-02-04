using UnityEngine;
using System.Linq;

namespace Assets {
    public class Upgrade {

        public Upgrade() {//int id, string name, int levelReq, int cost, float[] statChanges, string description) {
            /*Id = id;
            Name = name;
            LevelReq = levelReq;
            Cost = cost;
            DamageModifier = statChanges[0];
            HealthModifier = statChanges[1];
            SpeedModifier = statChanges[2];
            DefenceModifier = statChanges[3];
            AttackSpeedModifier = statChanges[4];
            AccuracyModifier = statChanges[5];
            Description = description;
            Unlocked = false;*/
        }

        public enum Type {
            Unit,
            Building,
            Gameplay,
            Multi
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int LevelReq { get; set; }
        public int Cost { get; set; }
        public float DamageModifier { get; set; }
        public int HealthModifier { get; set; }
        public float SpeedModifier { get; set; }
        public float ResistanceModifier { get; set; }
        public float AccuracyModifier { get; set; }
        public float AttackSpeedModifier { get; set; }
        public string Description { get; set; }
        public bool Unlocked { get; set; }
        public Sprite icon { get; set; }
        public Type type { get; set; }

        public void Unlock() {
            var upgrades = GameObject.Find("Upgrade Interface").GetComponent<UpgradeMenu>().GetUpgradeItems();
            upgrades[Id].transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);//navigate the hierarchy to get the padlock image
            Unlocked = true;
            //Debug.Log("The " + Name + " upgrade is now available.");

        }
    }
}