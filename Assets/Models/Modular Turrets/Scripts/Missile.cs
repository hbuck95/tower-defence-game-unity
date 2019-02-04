using UnityEngine;

namespace Assets {
    public class Missile : MonoBehaviour {
        public GameObject explosion;
        public RocketTurret turret;
        public Unit target;
        private float range = 100;
        private float speed = 40;
        private float dist;

        private void Update() {
            if (target) {
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                dist += Time.deltaTime * speed;
                if (dist >= range) {
                    Debug.Log("Distance is " + dist + ". Destroy missile.");
                    Destroy(gameObject);
                    Instantiate(explosion, transform.position, transform.rotation);
                }
            }

            if (target) {
                transform.LookAt(target.transform);
            } else {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider c) {
            if (c.tag == "Enemy") {
                Instantiate(explosion, target.transform.position, target.transform.rotation);
                turret.GetComponent<Building>().Attack(target);
                //UnitAgent targetAgent = target.GetComponent<UnitAgent>();
                //targetAgent.
                Debug.Log("Attacking " + c.name);
                Destroy(gameObject);
            }
        }

    }
}