using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets {
    public class UnitAgent : MonoBehaviour     {
        private UnityEngine.AI.NavMeshAgent _agent;
        private Animator _anim;
        public List<Unit> unitAttackers = new List<Unit>();
        public List<Building> buildingAttackers = new List<Building>();
        public Building targetBuilding;
        public Unit targetUnit;//used to hold the target unit before engaging in combat
        public Unit unit;
        public Unit selectedUnit;//The selected friendly unit being directed.
        public Tower tower;//enemy tower
        public bool attacked;
        public float lastAttack = 0f;
        private float _distance;
        public UnitWeapon weapon;

        private void Start() {
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _anim = GetComponent<Animator>();
            unit = GetComponent<Unit>();
            weapon = GetComponent<UnitWeapon>();
            TowerFactory tf = GameObject.Find("ScriptHolder").GetComponent<TowerFactory>();
            tower = unit.tag == "Friendly" ? tf.enemyTower.GetComponent<Tower>() : tf.friendlyTower.GetComponent<Tower>();
            //SetDestination(tower.GetPosition());

            SetDestination(unit.openingRoute);
            targetBuilding = tower;
            _agent.avoidancePriority = 0;
            _agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }


        public void SetDestination(Vector3 location) {
            _agent.SetDestination(location);
            _anim.SetBool("Moving", true);
        }

        /// <summary>
        /// Redirect is used to automatically redirect the units NavMeshAgent.
        /// If the unit still has attackers then it is automatically redirected to that unit,
        /// If there are no attackers left then it is automatically redirected towards the nearest enemy tower.
        /// </summary>
        public void Redirect() {       
            if (unitAttackers.Count > 0) {
                _agent.SetDestination(unitAttackers[0].transform.position);
                unit.Attack(unitAttackers[0]);
            } else if (buildingAttackers.Count > 0) {
                _agent.SetDestination(buildingAttackers[0].transform.position);
                unit.Attack(buildingAttackers[0]);
            } else {
               // Debug.Log("[" + gameObject.tag + "] " + unit.Name + " -- Attackers list is empty. Redirecting unit towards the enemy tower.)");
                unit.UnderAttack = false;
                attacked = false;
                lastAttack = 0;
                SetDestination(tower.GetPosition());//Get the position of the nearest enemy tower.
                targetBuilding = tower;
            }
            weapon.Hide();
            _anim.SetBool("Firing", false);
            _anim.SetBool("Moving", true);
            _agent.Resume();
        }

        private void Update() {

            if (unit.Dead)
                return;

            if (_agent.remainingDistance < 1)
                Redirect();

            //Handle Unit attacking
			if (unit.UnderAttack) {
				if (unitAttackers.Count > 0) {
					Unit enemy = unitAttackers.Last();
					_distance = Vector3.Distance(transform.position, enemy.transform.position);
					_agent.SetDestination(enemy.transform.position);//Update the navmeshagent target with the enemies new position

					//If enemy is within attack range...
					if (_distance <= unit.AttackRange) {
						_anim.SetBool("Moving", false);
						_agent.Stop();
                        weapon.Show();
						_anim.SetBool("Firing", true);//Change the animation state to attacking.
						transform.LookAt(enemy.transform);//Look at the enemy

						//Attack the enemy
						if (!attacked) {
							unit.Attack(enemy);
							attacked = true;
						} else {
							if (lastAttack >= (unit.AttackSpeed+unit.AttackSpeedModifier)) {
								attacked = false;
								lastAttack = 0;
							} else {
								lastAttack += Time.deltaTime;
							}
						}

					}

				} else if (buildingAttackers.Count > 0) {
					Building enemy = buildingAttackers.Last();
					_distance = Vector3.Distance(transform.position, enemy.transform.position);
					_agent.SetDestination(enemy.transform.position);//Update the navmeshagent target with the enemies new position

					//If enemy is within attack range...
					if (_distance <= unit.AttackRange) {
						_anim.SetBool("Moving", false);
						_agent.Stop();
                        weapon.Show();
                        _anim.SetBool("Firing", true);//Change the animation state to attacking.
						transform.LookAt(enemy.transform);//Look at the enemy

						//Attack the enemy
						if (!attacked) {
							unit.Attack(enemy);
							attacked = true;
						} else {
							if (lastAttack >= (unit.AttackSpeed+unit.AttackSpeedModifier)) {
								attacked = false;
								lastAttack = 0;
							} else {
								lastAttack += Time.deltaTime;
							}
						}

					}
				}

			} else {
                weapon.Hide();
                unit.UnderAttack = false;
			}

			if (targetBuilding != null) {
					float distance = Vector3.Distance(transform.position, targetBuilding.GetPosition());
					if (distance <= 45) {
					buildingAttackers.Add(targetBuilding);
					unit.UnderAttack = true;
					targetBuilding = null;
					}
				}
			

            //Attack the target unit once within range
            if (targetUnit != null) {
                float distance = Vector3.Distance(gameObject.transform.position, targetUnit.transform.position);
                if (distance <= 70) {
                    Debug.Log("Distance is now less than 70!");
                    targetUnit.GetComponent<UnitAgent>().unitAttackers.Add(unit);//Add this unit to the enemy's attacker list.
                    targetUnit.UnderAttack = true;
                    unitAttackers.Add(targetUnit);//Add the enemy to this units attacker list 
                    unit.UnderAttack = true;                
                    targetUnit = null;
                } else {
                    _agent.SetDestination(targetUnit.transform.position);
                }
            }

        }
    }
}