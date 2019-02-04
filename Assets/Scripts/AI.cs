using UnityEngine;
using System.Collections;

namespace Assets {
    public class AI : MonoBehaviour {
        public UnitFactory uf;
        
        private void Start() {
            StartCoroutine(SpawnUnits());
        }

        private IEnumerator SpawnUnits() {
            yield return new WaitForSeconds(5f);
            var rng = new System.Random();

            while (!Player.gameOver) {
                yield return new WaitForSeconds(Random.Range(4f, 8f));
                int seed = rng.Next(1, 3);
                if (Player.cpu.DeployedUnits < 12)
                    uf.CreateUnit(seed, false);
            }          
        }
    }
}
