using System.Collections.Generic;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Helpers;
using TMPro;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Managers
{
    public class PlayerManager : Manager<PlayerManager>
    {
        [HideInInspector]public GameObject playerMain;
        [HideInInspector]public GameObject playerObj;
        
        
        
        [HideInInspector] private bool xp;

        [HideInInspector]public bool gameStart;
        
        [HideInInspector][Header("XP / Progress Bar")] public Slider xpBar;
        [HideInInspector]public TextMeshProUGUI levelCount;

        public int playerLevel;

        [HideInInspector]public List<Transform> hitTransforms;
        [HideInInspector] public GameObject smoke;
        [HideInInspector]public int hitCount;

        [Header("** Player Variables **")] public float playerHealth;
        [Space(10)]public float reloadSpeed;
        [Space(10)]public float bulletSpeed;
        [Space(10)]public float vortexScale;
        [HideInInspector]public Slider hpBar;
        
        [HideInInspector]public bool multipleShoot;
        [HideInInspector]public bool explosionChain;
        [HideInInspector]public bool autoTurret;
        [HideInInspector]public GameObject autoTurretObj;
        [HideInInspector]public ParticleSystem levelUp;
        [HideInInspector]public ParticleSystem upgraded;
        [HideInInspector]public ParticleSystem expoGreen;
        [HideInInspector]public Image bar;
        [HideInInspector]private Color mainColor;


        [HideInInspector]public List<GameObject> hitText;
        [HideInInspector]public GameObject movingHand;
        [HideInInspector]public GameObject clickEffect;
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Play,GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Continue,GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Retry,GameStart);
                
                EnemyManager.Instance.Subscribe(CustomManagerEvents.GivePlayerDamage,CheckDamage);
                EnemyManager.Instance.Subscribe(CustomManagerEvents.GainXp,GainXp);
                
                UIManager.Instance.Subscribe(CustomManagerEvents.ReloadSpeedUpgrade,ReloadSpeedUpgrade);
                UIManager.Instance.Subscribe(CustomManagerEvents.BulletSpeedUpgrade,BulletSpeedUpgrade);
                UIManager.Instance.Subscribe(CustomManagerEvents.ExplosionPowerUpgrade,ExplosionPowerUpgrade);
                
                UIManager.Instance.Subscribe(CustomManagerEvents.MultipleShotsUpgrade,MultipleShotsUpgrade);
                UIManager.Instance.Subscribe(CustomManagerEvents.ExplosionChainUpgrade,ExplosionChainUpgrade);
                UIManager.Instance.Subscribe(CustomManagerEvents.AutoTurretUpgrade,AutoTurretUpgrade);
            }
            else
            {
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Play,GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Continue,GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Retry,GameStart);
                
                EnemyManager.Instance.Unsubscribe(CustomManagerEvents.GivePlayerDamage,CheckDamage);
                EnemyManager.Instance.Unsubscribe(CustomManagerEvents.GainXp,GainXp);
                
                UIManager.Instance.Unsubscribe(CustomManagerEvents.ReloadSpeedUpgrade,ReloadSpeedUpgrade);
                UIManager.Instance.Unsubscribe(CustomManagerEvents.BulletSpeedUpgrade,BulletSpeedUpgrade);
                UIManager.Instance.Unsubscribe(CustomManagerEvents.ExplosionPowerUpgrade,ExplosionPowerUpgrade);
                
                UIManager.Instance.Unsubscribe(CustomManagerEvents.MultipleShotsUpgrade,MultipleShotsUpgrade);
                UIManager.Instance.Unsubscribe(CustomManagerEvents.ExplosionChainUpgrade,ExplosionChainUpgrade);
                UIManager.Instance.Unsubscribe(CustomManagerEvents.AutoTurretUpgrade,AutoTurretUpgrade);
            }
        }

        private void GameStart(object[] arguments)
        {
            playerHealth = ((GameLevel) LevelManager.Instance.levelData).playerHealth;
            
            hpBar.maxValue = playerHealth;
            hpBar.value = playerHealth;

            mainColor = bar.color;

            playerLevel = 1;
            levelCount.text = playerLevel.ToString();
            xpBar.value = 0;
            
            xpBar.maxValue = ((GameLevel) LevelManager.Instance.levelData).playerNeedExp;
            
            reloadSpeed = .5f;
            bulletSpeed = .5f;
            vortexScale = .5f;
            
            explosionChain = false;
            multipleShoot = false;
            autoTurret = false;
        }

        private void ReloadSpeedUpgrade(object[] arguments)
        {
            reloadSpeed -= 0.10f;
            upgraded.Play();
            if (reloadSpeed <= 0.10f)
            {
                reloadSpeed = 0.10f;
            }
        }
        
        private void BulletSpeedUpgrade(object[] arguments)
        {
            bulletSpeed -= 0.10f;
            upgraded.Play();
            if (bulletSpeed <= 0.10f)
            {
                bulletSpeed = 0.10f;
            }
        }
        
        private void ExplosionPowerUpgrade(object[] arguments)
        {
            vortexScale += 0.10f;
            upgraded.Play();
            if (vortexScale >= 1)
                vortexScale = 1;
        }

        private void MultipleShotsUpgrade(object[] arguments)
        {
            multipleShoot = true;
            upgraded.Play();
            explosionChain = false;
            autoTurret = false;
            
            autoTurretObj.transform.DOScale(0, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                autoTurretObj.SetActive(false);
            });
        }

        private void ExplosionChainUpgrade(object[] arguments)
        {
            explosionChain = true;
            upgraded.Play();
            multipleShoot = false;
            autoTurret = false;
            
            
            autoTurretObj.transform.DOScale(0, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                autoTurretObj.SetActive(false);
            });
        }

        private void AutoTurretUpgrade(object[] arguments)
        {
            autoTurret = true;
            autoTurretObj.SetActive(true);
            autoTurretObj.transform.DOScale(0.67f, 1f).SetEase(Ease.OutBack);
            
            multipleShoot = false;
            explosionChain = false;
        }

//----------------------------------------------------------------------

        private void GainXp(object[] arguments)
        {
            xpBar.value++;
            
            if (xpBar.value >= xpBar.maxValue)
            {
                gameStart = false;
                //Özellik ekleme upgrade cart curt
                GetUpgrade();

                expoGreen.Play();
                levelUp.Play();
                
                playerLevel++;
                levelCount.text = playerLevel.ToString();
                xpBar.value = 0;
            }
        }

        private void GetUpgrade()
        {
            Publish(CustomManagerEvents.GetUpgradeScreen);
        }
        
        protected override void MB_Start()
        {
            hpBar.maxValue = playerHealth;hpBar.value = playerHealth;

            mainColor = bar.color;
        }

        
        
        protected override void MB_Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickEffect.SetActive(true);
                movingHand.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f).OnComplete(() =>
                {
                    movingHand.transform.localScale = new Vector3(8, 8, 8);
                    clickEffect.SetActive(false);
                });
            }
        }

        private void CheckDamage(object[] arguments)
        {
            var checker = (GameObject) arguments[0];

            VibrationManager.Instance.TriggerWarning();            
            
            
            if (checker == playerMain)
            {
                var value = (float) arguments[1];

                playerHealth -= value;
                ShowDamageText(value);
                hpBar.value = playerHealth;
                
                bar.DOColor(Color.white, 0.1f).OnComplete(() =>
                {
                    bar.DOColor(mainColor, 0.1f);
                });
                
                CheckHealth();
                
                // Camera.main.transform.DOShakePosition(0.01f, 0.02f).OnComplete(() =>
                // {
                //     Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                // });
            }

            if (EnemyManager.Instance.spawnedMissile == EnemyManager.Instance.missileCounter)
            {
                if (playerHealth > 0)
                {
                    gameStart = false;
                    Publish(CustomManagerEvents.LevelDone);
                    LevelManager.Instance.levelActor.FinishLevel(true);
                }
            }
            
        }

        private void CheckHealth()
        {
            if (playerHealth <= 0)
            {
                LevelManager.Instance.levelActor.FinishLevel(false,0.5f);
                Publish(CustomManagerEvents.LevelDone);
                Instantiate(EnemyManager.Instance.hitExpo, playerObj.transform.position, Quaternion.identity);
                playerObj.SetActive(false);
                gameStart = false;
                // LevelManager.Instance.Debug_LevelFailed();
            }
        }

        private void ShowDamageText(float value)
        {
            var rndm = Random.Range(0, hitText.Count);
            hitText[rndm].SetActive(true);
            var a = hitText[rndm].transform.position;

            hitText[rndm].GetComponent<TextMeshPro>().text = "-"+ value;
            
            hitText[rndm].transform.DOLocalMoveY(5, .5f).OnComplete(() =>
            {
                hitText[rndm].SetActive(false);
                hitText[rndm].transform.position = a;
            });
        }
    }
}