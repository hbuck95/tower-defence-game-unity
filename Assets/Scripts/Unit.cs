using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
namespace Assets {
    public class Unit : MonoBehaviour {
        //Name - Description - LevelReq - Cost - MaxDamage - MovementSpeed - AttackSpeed - Resistance - MaxHP - XPReward - MoneyReward
        public Unit(string name, string desc, int levelReq, int cost, int maxDamage, float damageModifier, float attackRange, float mvmtSpeed, float atkSpeed, int resist, int maxHp, int healthModifier, int xp, int money, int type) {
            Name = name;
            Description = desc;
            LevelReq = levelReq;
            Cost = cost;
            MaxDamage = maxDamage;
            DamageModifier = damageModifier;
            AttackRange = attackRange;
            MovementSpeed = mvmtSpeed;
            AttackSpeed = atkSpeed;
            Resistance = resist;
            BaseMaxHealth = maxHp;
            MaxHealth = BaseMaxHealth;
            CurrentHealth = BaseMaxHealth;
            HealthModifier = 0;
            AttackSpeedModifier = 0;
            ExperienceReward = xp;
            MoneyReward = money;
            unitType = (Type)type;
        }

        public Type unitType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int HealthModifier { get; set; }
        public float Resistance { get; set; }
        public float DamageModifier { get; set; }
        public float MovementSpeed { get; set; }
        public float AttackRange { get; set; }
        public float AttackSpeed { get; set; }
        public float AttackSpeedModifier { get; set; }
        public float MovementSpeedModifier { get; set; }
        public float ResistanceModifier { get; set; }
        public float Accuracy { get; set; }
        public float AccuracyModifier { get; set; }
        public int MaxDamage { get; set; }
        public int MaxHealth { get; set; }
        public int BaseMaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int ExperienceReward { get; set; }
        public int MoneyReward { get; set; }
        public int Cost { get; set; }
        public int LevelReq { get; set; }
        public Upgrade[] Upgrades = new Upgrade[3];
        public bool UnderAttack { get; set; }
        public bool Dead { get; private set; }
        private System.Random rng = new System.Random();
        public bool InsideEnergyShield { get; set; }
        public bool? ShieldIsFriendly { get; set; }
        public GameObject projectile;
        public AudioClip firingSFX;
        public Vector3 openingRoute;
        private AudioSource audioSource;

        private void Start() {
            gameObject.AddComponent<AudioSource>();
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        public int GetUnitType() {
            return (int)unitType;
        }

        public enum Type {
            Infantry,
            Demolition,
            HeavyWeapon };

        public void AddUpgrade(Upgrade upgrade) {
            for (int i = 0; i < Upgrades.Length; i++) {
                if (Upgrades[i] == null) {
                    Upgrades[i] = upgrade;
                    break;
                }
            }
            try {
              //  Debug.Log(upgrade.Name);
                HealthModifier += upgrade.HealthModifier;
                CurrentHealth += upgrade.HealthModifier;
                MaxHealth += upgrade.HealthModifier;
              //  Debug.Log("Max Health: " + BaseMaxHealth);
              //  Debug.Log("Health Modifier: " + HealthModifier);
              //  Debug.Log("This upgrades HP modifier: " + upgrade.HealthModifier);
             //   Debug.Log("Current Health: " + CurrentHealth);
                MaxHealth = (BaseMaxHealth + HealthModifier);
              //  Debug.Log("Max Health Now: " + MaxHealth);
                DamageModifier += upgrade.DamageModifier;
                MovementSpeedModifier += upgrade.SpeedModifier;
                ResistanceModifier += upgrade.ResistanceModifier;
                AttackSpeedModifier += upgrade.AttackSpeedModifier;
                GetComponent<UnityEngine.AI.NavMeshAgent>().speed += upgrade.SpeedModifier;
            } catch (System.NullReferenceException) {
                //ignore
            }
        }

        public void RemoveUpgrade(Upgrade upgrade) {
            HealthModifier -= upgrade.HealthModifier;
            DamageModifier -= upgrade.DamageModifier;
            MovementSpeedModifier -= upgrade.SpeedModifier;
            ResistanceModifier -= upgrade.ResistanceModifier;
            GetComponent<UnityEngine.AI.NavMeshAgent>().speed -= upgrade.SpeedModifier;
        }

        private IEnumerator Die() {
            UnitAgent agent = GetComponent<UnitAgent>();
            agent.weapon.Hide();
            Dead = true;
            Debug.Log(tag + " -- " + Name + " - starting death.");
            var player = tag == "Friendly" ? Player.human : Player.cpu;
            var killer = tag == "Friendly" ? Player.cpu : Player.human;

           // var UpgradeMenu = GameObject.Find("Upgrade Interface").GetComponent<UpgradeMenu>();

           // if(UpgradeMenu.GetSelectedUnit() == this) {
            //    UpgradeMenu.SelectUnit(null);
          //  }       

            //Loop through each attackers targetted units and remove any instances of this unit.
            if (agent.unitAttackers.Count > 0) {
                foreach (Unit unit in agent.unitAttackers) {
                    UnitAgent attackingAgent = unit.GetComponent<UnitAgent>();
                    foreach (Unit attackingUnit in attackingAgent.unitAttackers) {
                        if (attackingUnit == this) {
                            //Debug.Log("Found it!");
                            attackingAgent.unitAttackers.Remove(this);//Maybe removing it during iteration is bad??
                            attackingAgent.Redirect();
                            break;
                        }
                    }
                }
            }

            if(agent.buildingAttackers.Count > 0) {
                foreach (Building building in agent.buildingAttackers.Where(x => !x.name.Contains("Tower"))) {
                    List<Unit> attackers = building.GetComponent<RocketTurret>() ? building.GetComponent<RocketTurret>().targets : building.GetComponent<GunTurret>().targets;
                    foreach (Unit attackingUnit in attackers) {
                        if(attackingUnit == this) {
                            attackers.Remove(this);
                            agent.Redirect();
                            break;
                        }
                    }
                }
            }

            //Loop through the attackers of the targetted building and remove the instance of this object
            if (agent.targetBuilding != null) {
                List<Unit> buildingAttackers = agent.targetBuilding.GetComponent<TowerAgent>().attackers;
                foreach (Unit unit in buildingAttackers) {
                    if (unit == this) {
                        buildingAttackers.Remove(this);
                        break;
                    }
                }
            }

            Animator anim = GetComponent<Animator>();
            anim.SetBool("Dead", true);
            GetComponent<CapsuleCollider>().enabled = false;//disable so ai can walk through the unit whilst it is dieing.
            GetComponent<UnityEngine.AI.NavMeshAgent>().Stop();
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 3f);
            Debug.Log(tag + " -- " + Name + " - death complete.");
            killer.AddXP(ExperienceReward);
            killer.AddCredits(MoneyReward);
            Destroy(gameObject);
            player.KillUnit((int)unitType);
           
        }

        private void OnCollisionEnter(Collision c) {
            if(c.gameObject.tag.Contains("Projectile"))
                if (tag == "Enemy") {
                    if (c.gameObject.tag == "FriendlyProjectile")
                        Destroy(c.gameObject);
                } else if (tag == "Friendly") {
                    if (c.gameObject.tag == "EnemyProjectile")
                        Destroy(c.gameObject);
                } //else {
                    //Debug.Log(c.gameObject.name);
                //}

        }

        private IEnumerator FireProjectile() {                 
            if ((int)unitType == 0) {
                for (int i = 0; i < 5; i++) {
                    audioSource.PlayOneShot(firingSFX, 0.8f);
                    projectile.transform.localScale = new Vector3(0.2f, 0.6f, 0.2f);
                    GameObject p = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
                    p.GetComponent<UnitProjectile>().unit = this;
                    p.transform.Rotate(new Vector3(90f, p.transform.rotation.y, p.transform.rotation.z));
                    p.transform.position = (new Vector3(p.transform.position.x, 5.65f, p.transform.position.z));
                    yield return new WaitForSeconds(0.15f);
                }
                yield break;
            }


            if((int)unitType == 1) {
                yield return new WaitForSeconds(1.75f);
                audioSource.PlayOneShot(firingSFX, 2f);
                projectile.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);
                GameObject p = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
                p.GetComponent<UnitProjectile>().unit = this;
                p.transform.Rotate(new Vector3(90f, p.transform.rotation.y, p.transform.rotation.z));
                p.transform.position = (new Vector3(p.transform.position.x, 5.65f, p.transform.position.z));             
            }

            if((int)unitType == 2) {
                yield return new WaitForSeconds(0.8f);
                for (int i = 0; i < 3 ; i++) {
                    audioSource.PlayOneShot(firingSFX, 2f);
                    projectile.transform.localScale = new Vector3(0.1f, 0.6f, 0.1f);
                    GameObject p = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
                    p.GetComponent<UnitProjectile>().unit = this;
                    p.transform.Rotate(new Vector3(90f, p.transform.rotation.y, p.transform.rotation.z));
                    p.transform.position = (new Vector3(p.transform.position.x, 5.65f, p.transform.position.z));
                    yield return new WaitForSeconds(0.1f);
                }            
            }
        }
       
        public void Attack(Unit target) {
            var damage = Mathf.RoundToInt((rng.Next(0, MaxDamage)) * (100 + DamageModifier)/100);
            StartCoroutine(FireProjectile());
            target.TakeDamage(damage);
            var player = tag == "Friendly" ? Player.human : Player.cpu;
            player.AddXP(Mathf.RoundToInt(damage / 2));
            player.AddCredits(Mathf.RoundToInt(damage / 2));
        }

        public void Attack(Building target) {
            var damage = Mathf.RoundToInt((rng.Next(0, MaxDamage)) * (100 + DamageModifier) / 100);
            target.TakeDamage(damage);
        }

        public void TakeDamage(int damage) {
            float resist = (100 - (Resistance + ResistanceModifier))/100;
            float calculatedDamage = damage * resist;

            if(InsideEnergyShield && ShieldIsFriendly.Value)
                calculatedDamage *= 0.75f;          

            if (CurrentHealth - calculatedDamage <= 0) {
                if(!Dead)
                    StartCoroutine(Die());
            } else {
                CurrentHealth -= Mathf.RoundToInt(calculatedDamage);
            }      
        }

        public IEnumerator DamageOverTime(int damage) {
            while (InsideEnergyShield) {
                //Debug.Log("Dealing 7 damage to " + Name);
                if(CurrentHealth - damage <= 0) {
                    //Debug.Log(Name+" is about to die!");
                    InsideEnergyShield = false;
                    ShieldIsFriendly = null;
                    StartCoroutine(Die());
                } else {
                    CurrentHealth -= damage;
                    //Debug.Log(Name + " Health: " + CurrentHealth + " / " + (BaseMaxHealth + HealthModifier));
                    yield return new WaitForSeconds(1);
                }
            }
        }

    }
}