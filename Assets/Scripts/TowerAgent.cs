using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Assets {
    public class TowerAgent : MonoBehaviour {
        public List<Unit> attackers = new List<Unit>();
        private float _lastAttack;
        private bool _attacked;
        private Tower _tower;

        private void Start() {
            _tower = GetComponent<Tower>();
        }

        private void Update() {

            if(attackers.Count > 0) {
                if (!_attacked) {
                    _tower.Attack(attackers.Last());
                    _attacked = true;
                } else {
                    if (_lastAttack >= (_tower.AttackSpeed - _tower.AttackSpeedModifier)) {
                        _attacked = false;
                        _lastAttack = 0;
                    } else {
                        _lastAttack += Time.deltaTime;
                    }
                }
            }
        }


        private void OnTriggerEnter(Collider c) {
            if (tag == "FriendlyTowerGun" || tag == "EnemyTowerGun")
                return;

            string tagCheck = tag == "EnemyTower" ? "Friendly" : "Enemy";//if this games objects tag is EnemyTower then we will look for Friendly units (vice-versa)
            if(c.tag == tagCheck && c.GetComponent<Unit>()) {
                Unit unit = c.GetComponent<Unit>();
               // Debug.Log("[" + tag + "] Tower has detected a " + c.tag + " " + unit.Name + "!");
                if (!attackers.Contains(unit)) {
                    attackers.Add(unit);
                    _tower.UnderAttack = true;                  
                }
            }
        }

        private void OnTriggerExit(Collider c) {
            string tagCheck = tag == "EnemyTower" ? "Friendly" : "Enemy";//if this games objects tag is EnemyTower then we will look for Friendly units (vice-versa)
            if(c.tag == tagCheck) {
                Unit triggerUnit = c.GetComponent<Unit>();
                foreach (Unit unit in attackers) {
                    if(unit == triggerUnit) {
                        attackers.Remove(triggerUnit);
                        _tower.UnderAttack = false;
                        break;
                    }
                }             
            }
        }

    }
}
