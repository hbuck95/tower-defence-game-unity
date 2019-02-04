using UnityEngine;
using System.Collections.Generic;

namespace Assets {
    public class RocketTurret : MonoBehaviour {
        public GameObject projectile;
        public float turnSpeed = 5f;
        public Transform[] muzzlePositions;
        public Transform gameTilt, gamePivot;
        public Transform aimTilt, aimPivot;
        public List<Unit> targets = new List<Unit>();
        private float _nextFireTime, _lastAttack = 0;
        private string _tagCheck;
        private Building _building;
        private bool _attacked;
        public GameObject explosion;

        private void Start() {
            _tagCheck = tag == "Friendly" ? "Enemy" : "Friendly";
            _building = GetComponent<Building>();
        }

        private void Update() {
            //Rotates the turret and fires the missiles
            if (targets.Count > 0) {
                aimPivot.LookAt(targets[0].transform);
                Vector3 newRot = aimPivot.eulerAngles;
                newRot.x = newRot.z = 0;
                aimPivot.rotation = Quaternion.Euler(newRot);

                aimTilt.LookAt(targets[0].transform);
                Vector3 rot = aimTilt.localEulerAngles;
                rot.y = rot.z = 0;
                aimTilt.localRotation = Quaternion.Euler(rot);

                gamePivot.rotation = Quaternion.Lerp(gamePivot.rotation, aimPivot.rotation, Time.deltaTime * turnSpeed);
                gameTilt.localRotation = Quaternion.Lerp(gameTilt.localRotation, aimTilt.localRotation, Time.deltaTime * turnSpeed);

                if (_lastAttack >= (_building.AttackSpeed - _building.AttackSpeedModifier)) {
                    FireProjectile();
                    _lastAttack = 0;
                } else {
                    _lastAttack += Time.deltaTime;
                }

                //if (Time.time >= _nextFireTime)
                //    FireProjectile();//Replace with attack     
            }
        }

        private void OnTriggerEnter(Collider c) {
            if (c.tag == _tagCheck) {
                Debug.Log("Rocket turret has detected an enemy " + c.name);
                //_nextFireTime = Time.time + ((_building.AttackSpeed - _building.AttackSpeedModifier) * 0.5f);
                Unit unit = c.GetComponent<Unit>();
                targets.Add(unit);
                unit.UnderAttack = true;
                c.GetComponent<UnitAgent>().buildingAttackers.Add(_building);
            }
        }

        private void OnTriggerExit(Collider c) {
            if (c.tag == _tagCheck) {
                Unit unit = c.GetComponent<Unit>();
                if (targets.Contains(unit))
                    targets.Remove(unit);                                                    
            }
        }

        private void FireProjectile() {
            //_nextFireTime = Time.time + (_building.AttackSpeed-_building.AttackSpeedModifier);
            _building.audioSource.PlayOneShot(_building.attackSFX);
            int missilePosition = Random.Range(0, muzzlePositions.Length);
            GameObject missileObject = Instantiate(projectile, muzzlePositions[missilePosition].position, muzzlePositions[missilePosition].localRotation) as GameObject;
            Missile missile = missileObject.GetComponent<Missile>();
            missile.target = targets[0];
            missile.turret = this;
        }

    }
}