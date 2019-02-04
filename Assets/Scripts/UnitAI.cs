using UnityEngine;

namespace Assets {
    public class UnitAI : MonoBehaviour {
        private UnitAgent _agent;
        private Unit _unit;
        private SphereCollider _spC;
        private int _enemiesInRange, _friendliesInRange;//in this instance enemies are the tagged 'Friendlies' (vice-versa) as they are enemies to this unit.

        private void Start() {
            _agent = GetComponent<UnitAgent>();
            _unit = GetComponent<Unit>();
            _spC = gameObject.AddComponent<SphereCollider>();
            _spC.isTrigger = true;
            _spC.radius = 4f;
            _spC.center += new Vector3(0, 0, 2);
        }

        private void OnTriggerEnter(Collider c) {
            if (c.tag == "Friendly") {
                _enemiesInRange++;
                DecideToAttack(c.GetComponent<Unit>());
            } else if (c.tag == "Enemy") {
                _friendliesInRange++;
            }
        }

        private void OnTriggerExit(Collider c) {
            if (c.tag == "Friendly") {
                _enemiesInRange--;
            } else if (c.tag == "Enemy") {
                _friendliesInRange--;
            }
        }

        //Debug.Log(string.Format("R is {0} and {1}. Returning.", r, _unit.UnderAttack));
        //Debug.Log(string.Format("EIR: {0} -- FIR: {1} -- M: {2} -- R: {3}", _enemiesInRange, _friendliesInRange, (_enemiesInRange - _friendliesInRange), r));
        private void DecideToAttack(Unit unit) {
            var r = new System.Random().Next(0, 5);

            if (r <= 2 || _unit.UnderAttack)
                return;
            
            if ((_enemiesInRange - _friendliesInRange >= 3) && r == 3)
                return;

            _agent.targetUnit = unit;
        }
    }
}
