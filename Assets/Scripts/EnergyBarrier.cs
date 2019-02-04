using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets {

    public class EnergyBarrier : MonoBehaviour {
        private Building _building;

        private void Start() {
            //GetComponent<Renderer>().material.color = Color.blue;
            //Green = RGBA(0.000, 1.000, 0.048, 0.534);
            _building = GetComponent<Building>();
            StartCoroutine(LifeSpan());
        }

        private IEnumerator LifeSpan() {
            yield return new WaitForSeconds(30);
            Destroy(gameObject);
            var player = tag == "Friendly" ? Player.human : Player.cpu;
            player.DestroyBuilding(4);
        }

        private void OnTriggerEnter(Collider c) {
            if (c.GetComponent<Unit>()) {
                Unit unit = c.GetComponent<Unit>();
                unit.InsideEnergyShield = true;

                if (c.tag == tag) {//if friendly
                    Debug.Log(c.name + " is friendly.");
                    unit.ShieldIsFriendly = true;
                } else {
                    Debug.Log(c.name + "isn't friendly! Starting damage over time!");
                    unit.StartCoroutine(unit.DamageOverTime(_building.MaxDamage));
                    unit.ShieldIsFriendly = false;
                    UnityEngine.AI.NavMeshAgent agent = c.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    agent.speed *= 0.5f;
                    c.GetComponent<Animator>().speed = 0.75f;
                }
               // unit.ShieldIsFriendly = unit.tag == tag;
            }
        }

        private void OnTriggerExit(Collider c) {
            if (c.GetComponent<Unit>()) {
                Unit unit = c.GetComponent<Unit>();
                unit.InsideEnergyShield = false;
                unit.ShieldIsFriendly = null;
                Debug.Log(c.name + " has left the energy barrier.");
                if(c.tag != tag) {
                    c.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= 2;
                    c.GetComponent<Animator>().speed = 1;
                }
            }
        }


    }
}
