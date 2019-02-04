using UnityEngine;
using System.Collections.Generic;

namespace Assets {
    public class Building : MonoBehaviour {

        public Building(string name, string desc, int maxHealth, int levelReq, int cost, int maxDamage, float attackRange, float attackSpeed, int buildingType) {
            Name = name;
            Description = desc;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            LevelRequirement = levelReq;
            Cost = cost;
            MaxDamage = maxDamage;
            AttackRange = attackRange;
            AttackSpeed = attackSpeed;
            HealthModifier = 1;
            Resistance = 1;
            DamageModifier = 1;
            AttackSpeedModifier = 0;
            BuildingType = buildingType;
        }

        public void SetValues(string name, string desc, int maxHealth, int levelReq, int cost, int maxDamage, float attackRange, float attackSpeed, int buildingType)        {
            Name = name;
            Description = desc;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            LevelRequirement = levelReq;
            Cost = cost;
            MaxDamage = maxDamage;
            AttackRange = attackRange;
            AttackSpeed = attackSpeed;
            HealthModifier = 1;
            Resistance = 1;
            DamageModifier = 1;
            AttackSpeedModifier = 0;
            BuildingType = buildingType;
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int MaxHealth { get; set; }
        public virtual int CurrentHealth { get; set; }
        public virtual int LevelRequirement { get; set; }
        public virtual int Cost { get; set; }
        public virtual int MaxDamage { get; set; }
        public virtual float AttackRange { get; set; }
        public virtual float AttackSpeed { get; set; }
        public virtual bool Friendly { get; set; }
        public virtual bool UnderAttack { get; set; }   
        public virtual float HealthModifier { get; set; }
        public virtual float Resistance { get; set; }
        public virtual float DamageModifier { get; set; }
        public virtual float AttackSpeedModifier { get; set; }
        public virtual int BuildingType { get; set; }
        public AudioClip attackSFX, explodeSFX;
        public AudioSource audioSource;
        public virtual Vector3 GetPosition() { return transform.position; }
        protected readonly System.Random rng = new System.Random();

        private void Start()
        {
            gameObject.AddComponent<AudioSource>();
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        public virtual void Attack(Unit target) {
            try {
                var damage = Mathf.RoundToInt((rng.Next(0, MaxDamage)) * DamageModifier);
                target.TakeDamage(damage);
            } catch (System.NullReferenceException nre) {
                Debug.LogError(nre);
            }
            
        }

        public virtual void TakeDamage(int damage) {
            float resist = (Resistance / 100) * rng.Next(0, 100);
            //Debug.Log(Name + " - Resistance calculated as " + resist + "%");
            int calculatedDamage = Mathf.RoundToInt(damage - (damage / 100) * resist);

            if (CurrentHealth - calculatedDamage <= 0) {
                Die();
            } else {
                CurrentHealth -= calculatedDamage;
                //Debug.Log("[" + tag + "] Taking " + calculatedDamage + " damage. Health is now " + CurrentHealth + "/" + MaxHealth);
            }
        }

        protected virtual void Die() {
            if (name.ToLower().Contains("tower")) {
                Debug.Log("Tower death.");
                Debug.Log(CurrentHealth + " / " + MaxHealth);
                CurrentHealth = 0;
                gameObject.SetActive(false);
                return;
            }


            var player = tag == "Friendly" ? Player.human : Player.cpu;
            List<Unit> attackers = GetComponent<RocketTurret>() != null ? GetComponent<RocketTurret>().targets : GetComponent<GunTurret>().targets;
            GameObject explosion = GetComponent<RocketTurret>() != null ? GetComponent<RocketTurret>().explosion : GetComponent<GunTurret>().explosion;
            //List<Unit> attackers = GetComponent<GunTurret>().targets;
                foreach (Unit unit in attackers) {
                    UnitAgent agent = unit.GetComponent<UnitAgent>();
                    foreach (Building building in agent.buildingAttackers) {
                        if (building == this) {
                            agent.buildingAttackers.Remove(this);
                            agent.Redirect();
                            break;
                        }
                    }
                }

            
            Instantiate(explosion, transform.position, transform.rotation);
            audioSource.PlayOneShot(explodeSFX);
            Destroy(gameObject);
            player.DestroyBuilding(BuildingType);

        }

    }
}
