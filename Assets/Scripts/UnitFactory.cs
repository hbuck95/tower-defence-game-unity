using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Assets {
    public class UnitFactory : MonoBehaviour {
        //Name - Description - LevelReq - Cost - MaxDamage - DamageModifier - MovementSpeed - AttackSpeed - Resistance - MaxHP - HealthModifier - XPReward - MoneyReward
        private readonly Unit _infantry = new Unit("Infantry", "A standard infantry unit.", 0, 150, 22, 1, 50f, 1.5f, 1.1f, 1, 135, 1, 150, 150, 0);
        private readonly Unit _demolitions = new Unit("Demolitions", "A unit which specialises in destroying buildings.", 5, 200, 25, 1, 50f, 1.9f, 2.56f, 1, 180, 1, 250, 250, 1);
        private readonly Unit _heavyWeapons = new Unit("Heavy Weapons", "A slow moving, high-damage unit.", 10, 250, 38, 1, 50f, 2.1f, 0.85f, 1, 350, 1, 500, 500, 2);
        public GameObject[] friendlyPrefabs = new GameObject[3];
        public GameObject[] enemyPrefabs = new GameObject[3];
        public GameObject fProjectile, eProjectile;
        public Dictionary<Transform, bool> enemySpawns = new Dictionary<Transform, bool>();
        public Dictionary<Transform, bool> friendlySpawns = new Dictionary<Transform, bool>();
        private bool _initialSpawn = false;
        public AudioClip[] firingSounds;
        public Transform[] friendlyOpeningRoutes, enemyOpeningRoutes;
        private System.Random _rng = new System.Random();

        private void Awake() {//Gather spawn points.
            foreach (var spawn in GameObject.FindGameObjectsWithTag("EnemySpawn")) { enemySpawns.Add(spawn.transform, true); }
            foreach (var spawn in GameObject.FindGameObjectsWithTag("FriendlySpawn")) { friendlySpawns.Add(spawn.transform, true); }
        }

        private void Start() {//Create the initial units
            _initialSpawn = true;
            CreateUnit(3, true);
            CreateUnit(3, false);
            CreateUnit(2, true);
            CreateUnit(2, false);
            CreateUnit(1, true);
            CreateUnit(1, false);
            _initialSpawn = false;
            Player.human.AddCredits(150);
        }    

        private IEnumerator SpawnCooldown(Transform spawnKey, bool isFriendly) {
            var spawns = isFriendly ? friendlySpawns : enemySpawns;
            var player = isFriendly ? Player.human : Player.cpu;
            yield return new WaitForSeconds(player.SpawnCooldown);
            spawns[spawnKey] = true;
        }

        private Unit GetUnit(int type) {
            switch (type) {
                case 1:
                    return _infantry;
                case 2:
                    return _demolitions;
                case 3:
                    return _heavyWeapons;
                default:
                    return _infantry;
            }
        }

        private AudioClip GetFiringSFX(int type) {
            return firingSounds[type - 1];
        }

        private GameObject GetPrefab(int type, bool friendly) {
            return friendly ? friendlyPrefabs[type - 1] : enemyPrefabs[type - 1];
        }

        private Vector3? GetSpawn(bool friendly) {
            var spawns = friendly ? friendlySpawns : enemySpawns;
            var spawnValues = spawns.Values;

            for (int i = 0; i < spawnValues.Count; i++) {
                if(spawnValues.ElementAt(i)) {
                    Transform key = spawns.ElementAt(i).Key;
                    spawns[key] = false;
                    StartCoroutine(SpawnCooldown(key, friendly));
                    return key.position;
                }
            }
            return null;
        }

        private Vector3 GetOpeningRoute(bool friendly) {
            var r = _rng.Next(0, 4);
            Debug.Log(r);
            return friendly ? friendlyOpeningRoutes[r].position : enemyOpeningRoutes[r].position;
        }

        public void CreateUnit(int type, bool isFriendly) {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Unit unit = GetUnit(type);
            Vector3? spawn = GetSpawn(isFriendly);
            Player player = isFriendly ? Player.human : Player.cpu;

            if (player.PlayerId == 1) {

                if(player.Credits - unit.Cost < 0) {
                    Debug.Log("You don't have enough credits to purchase this unit!");
                    return;
                }

                if (player.DeployedUnits >= player.MaxUnits) {
                    Debug.Log(string.Format("The maximum amount of units you can deploy is {0}.\nYou currently have {1} units deployed and cannot spawn any more.", player.MaxUnits, player.DeployedUnits));
                    return;
                }

                if (spawn == null) {
                    Debug.Log("You can't create another unit yet!");
                    return;
                }
            }

            GameObject obj = Instantiate(GetPrefab(type, isFriendly), spawn.Value, Quaternion.identity) as GameObject;
            obj.tag = isFriendly ? "Friendly" : "Enemy";
            Unit destUnit = obj.AddComponent<Unit>();
            //Unit destUnit = obj.GetComponent<Unit>();
            FieldInfo[] fieldInfo = unit.GetType().GetFields(flags);
            FieldInfo[] dest = destUnit.GetType().GetFields(flags);

            foreach (FieldInfo info in fieldInfo) {
                FieldInfo destField = dest.FirstOrDefault(field => field.Name == info.Name);
                destField.SetValue(destUnit, info.GetValue(unit));
            }

            destUnit.projectile = isFriendly ? fProjectile : eProjectile;
            destUnit.firingSFX = GetFiringSFX(type);
            obj.AddComponent<UnitAgent>();
            player.DeployUnit();
            unit.openingRoute = GetOpeningRoute(isFriendly);
           
            if(!_initialSpawn)
                player.SpendCredits(unit.Cost);

            if (!isFriendly)
                obj.AddComponent<UnitAI>();

            switch (type) {
                case 1:
                    player.infantryUnitCount++;
                    break;
                case 2:
                    player.demoUnitCount++;
                    break;
                case 3:
                    player.hwUnitcount++;
                    break;
            }
        }
    }
}