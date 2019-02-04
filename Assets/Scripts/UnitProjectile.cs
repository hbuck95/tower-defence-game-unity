using UnityEngine;
using System.Collections;


namespace Assets {
    public class UnitProjectile : MonoBehaviour {
        private float _speed = 150f;
        private string _tagCheck;
        public Unit unit;

        private void Start() {
            _tagCheck = tag == "FriendlyProjectile" ? "Enemy" : "Friendly";
            StartCoroutine(LifeSpan());
            Physics.IgnoreLayerCollision(12, 13, true);
            Physics.IgnoreLayerCollision(13, 12, true);
        }

        private IEnumerator LifeSpan() {
            yield return new WaitForSeconds(4);
            Destroy(gameObject);
        }
        
        private void OnCollisionEnter(Collision c) {
            if (c.gameObject.tag == _tagCheck)
                Destroy(gameObject);
            
        }

        private void Update() {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
            if (!unit.UnderAttack || unit.Dead)
                Destroy(gameObject);
        }
    }
}