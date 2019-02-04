using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Assets {
    public class GunTurret : MonoBehaviour {

        public GameObject[] muzzleFire = new GameObject[2];
        public GameObject mf;
        public GameObject projectile;
        public float turnSpeed = 5f;
        public float firePauseTime = 0.05f;
        public Transform[] muzzlePositions;
        public Transform gameTilt, gamePan;
        public Transform aimTilt, aimPan;
        public List<Unit> targets = new List<Unit>();
        protected float nextFireTime;
        protected float nextMoveTime;
        protected Quaternion aimPanStart;
        protected Quaternion aimTiltStart;
        protected string _tagCheck;
        public Building building;
        protected float lastAttack = 0;
        public GameObject explosion;

        private List<GameObject> muzzlefirestuff = new List<GameObject>();


        protected virtual void Awake() {
            aimPanStart = aimPan.rotation;
            aimTiltStart = aimTilt.localRotation;          
        }

        protected virtual void Start() {
            building = GetComponent<Building>();
            _tagCheck = tag == "Friendly" ? "Enemy" : "Friendly";
            foreach (GameObject mf in muzzleFire) { mf.SetActive(false); }
        }

        protected virtual void Update() {
            if (targets.Count > 0) {
                if (Time.time >= nextMoveTime) {
                    aimPan.LookAt(targets[0].transform);
                    Vector3 newRot = aimPan.eulerAngles;
                    newRot.x = newRot.z = 0;
                    aimPan.rotation = Quaternion.Euler(newRot);

                    aimTilt.LookAt(targets[0].transform);
                    Vector3 rot = aimTilt.localEulerAngles;
                    rot.y = rot.z = 0;
                    aimTilt.localRotation = Quaternion.Euler(rot);

                    gamePan.rotation = Quaternion.Lerp(gamePan.rotation, aimPan.rotation, Time.deltaTime * turnSpeed);
                    gameTilt.localRotation = Quaternion.Lerp(gameTilt.localRotation, aimTilt.localRotation, Time.deltaTime * turnSpeed);

                }

                if (lastAttack >= (building.AttackSpeed - building.AttackSpeedModifier)) {
                    StartCoroutine(FireProjectile());
                    lastAttack = 0;
                } else {
                    lastAttack += Time.deltaTime;
                }


            } else {
                gamePan.rotation = Quaternion.Lerp(gamePan.rotation, aimPanStart, Time.deltaTime * turnSpeed);
                gameTilt.localRotation = Quaternion.Lerp(gameTilt.localRotation, aimTiltStart, Time.deltaTime * turnSpeed);
            }
        }

        /*private void OnTriggerStay(Collider col) {
            if (target == null) {
                if (col.gameObject.tag == "Enemy") {
                    nextFireTime = Time.time + (reloadTime * 0.5f);
                    target = col.gameObject.transform;
                }
            }
        }*/

        protected virtual void OnTriggerEnter(Collider c) {
            if (c.tag == _tagCheck) {
                //nextFireTime = Time.time + (reloadTime * 0.5f);
                Unit unit = c.GetComponent<Unit>();
                targets.Add(unit);
                unit.UnderAttack = true;
                c.GetComponent<UnitAgent>().buildingAttackers.Add(building);
            }
        }

        protected virtual void OnTriggerExit(Collider c) {
            if(c.tag == _tagCheck) {
                Unit unit = c.GetComponent<Unit>();
                if (targets.Contains(unit))
                    targets.Remove(unit);
            }
        }

        protected virtual IEnumerator FireProjectile() {
            //nextFireTime = Time.time + reloadTime;
            nextMoveTime = Time.time + firePauseTime;
            building.Attack(targets[0]);

            foreach (GameObject mf in muzzleFire) {
                mf.SetActive(true);
            }

            foreach (Transform t in muzzlePositions) {
                building.audioSource.PlayOneShot(building.attackSFX);
                for (int i = 0; i < 3; i++) {
                    GameObject p = Instantiate(projectile, t.position, t.rotation) as GameObject;
                    p.GetComponent<Projectile>().tagCheck = _tagCheck;
                    yield return new WaitForSeconds(0.05f);
                }
            }

            foreach (GameObject mf in muzzleFire) {
                mf.SetActive(false);
            }
        }
    }
}