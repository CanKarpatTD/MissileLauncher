using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Managers;
using TMPro;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Actors
{
    public class ButtonActor : Actor<UIManager>
    {
        public enum TypeForButton
        {
            None,
            BulletSpeed,
            ReloadSpeed,
            ExplosionPower,
            IIIIIIIIIIIIII,
            MultipleShots,
            ExplosionChain,
            AutoTurret
        }
        public TypeForButton typeForButton;

        public bool skill, speciality;

        public int level;
        public bool equipped;

        public List<GameObject> stars;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Play,GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Continue,GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Retry,GameStart);
            }
            else
            {
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Play,GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Continue,GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Retry,GameStart);
            }
        }

        private void GameStart(object[] arguments)
        {
            level = 0;
            equipped = false;
            
            stars[0].gameObject.SetActive(false);
            stars[0].gameObject.transform.localScale = Vector3.zero;

            if (!speciality)
            {
                if (stars[1] != null)
                {
                    stars[1].gameObject.SetActive(false);
                    stars[1].gameObject.transform.localScale = Vector3.zero;
                }

                if (stars[2] != null)
                {
                    stars[2].gameObject.SetActive(false);
                    stars[2].gameObject.transform.localScale = Vector3.zero;
                }

                if (stars[3] != null)
                {
                    stars[3].gameObject.SetActive(false);
                    stars[3].gameObject.transform.localScale = Vector3.zero;
                }

                if (stars[4] != null)
                {
                    stars[4].gameObject.SetActive(false);
                    stars[4].gameObject.transform.localScale = Vector3.zero;
                }
            }
        }

        public void SetUpgrade()
        {
            if (Manager.canClick)
            {
                if (skill)
                {
                    level++;

                    if (level == 1)
                    {
                        stars[0].gameObject.SetActive(true);
                        stars[0].gameObject.transform.DOScale(0.7783926f, .1f).SetEase(Ease.OutBack);
                    }

                    if (level == 2)
                    {
                        stars[1].gameObject.SetActive(true);
                        stars[1].gameObject.transform.DOScale(0.7783926f, .1f).SetEase(Ease.OutBack);
                    }

                    if (level == 3)
                    {
                        stars[2].gameObject.SetActive(true);
                        stars[2].gameObject.transform.DOScale(0.7783926f, .1f).SetEase(Ease.OutBack);
                    }

                    if (level == 4)
                    {
                        stars[3].gameObject.SetActive(true);
                        stars[3].gameObject.transform.DOScale(0.7783926f, .1f).SetEase(Ease.OutBack);
                    }

                    if (level == 5)
                    {
                        stars[4].gameObject.SetActive(true);
                        stars[4].gameObject.transform.DOScale(0.7783926f, .1f).SetEase(Ease.OutBack);

                        Manager.buttons.Remove(gameObject);
                        Manager.fullSkills.Add(gameObject);
                    }
                }

                if (speciality)
                {

                    foreach (var obj in Manager.specialityButtons)
                    {
                        if (obj.GetComponent<ButtonActor>().equipped)
                        {
                            obj.GetComponent<ButtonActor>().equipped = false;
                            Manager.buttons.Add(gameObject);
                            Manager.equippedSpeciality.Clear();
                        }
                    }

                    equipped = true;
                    Manager.buttons.Remove(gameObject);
                    Manager.equippedSpeciality.Add(gameObject);

                    stars[0].gameObject.SetActive(true);
                    stars[0].gameObject.transform.DOScale(1, .1f).SetEase(Ease.OutBack);
                }

                PushUpgradeToPlayer();
            }
        }

        private void PushUpgradeToPlayer()
        {
            if (typeForButton == TypeForButton.BulletSpeed)
            {
                Push(CustomManagerEvents.BulletSpeedUpgrade);
            }
            
            if (typeForButton == TypeForButton.ReloadSpeed)
            {
                Push(CustomManagerEvents.ReloadSpeedUpgrade);
            }
            
            if (typeForButton == TypeForButton.ExplosionPower)
            {
                Push(CustomManagerEvents.ExplosionPowerUpgrade);
            }
            
            if (typeForButton == TypeForButton.MultipleShots)
            {
                Push(CustomManagerEvents.MultipleShotsUpgrade);
            }
            
            if (typeForButton == TypeForButton.ExplosionChain)
            {
                Push(CustomManagerEvents.ExplosionChainUpgrade);
            }
            
            if (typeForButton == TypeForButton.AutoTurret)
            {
                Push(CustomManagerEvents.AutoTurretUpgrade);
            }


            Manager.canClick = false;
            
            StartCoroutine(CloseUI());
        }

        IEnumerator CloseUI()
        {
            yield return new WaitForSeconds(.5f);
            
            Manager.upgradeUI.transform.DOScale(0, .5f).OnComplete(() =>
            {
                Manager.upgradeUI.SetActive(false);
                PlayerManager.Instance.gameStart = true;
            });
            
            Manager.SetList();
        }
    }
}