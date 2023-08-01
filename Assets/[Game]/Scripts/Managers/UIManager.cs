using System.Collections.Generic;
using DG.Tweening;
using Game.Actors;
using Game.GlobalVariables;
using TMPro;
using TriflesGames.ManagerFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Managers
{
    public class UIManager : Manager<UIManager>
    {
        //TODO: 6 farklı buton sahnede bulunsun. butonlar liste içerisinde bulunsun. Listeden random seçilsin. Buton etkileşimlerinde içerisinde bulunan buton aktörüne geçsin.
        [HideInInspector]public GameObject upgradeUI;


        [HideInInspector]public List<GameObject> buttons,openedButtons;

        [HideInInspector]public List<GameObject> equippedSpeciality,fullSkills;

        [HideInInspector]public List<GameObject> specialityButtons;
        
        [HideInInspector]public Transform btn1, btn2, btn3;

        public bool canClick;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                PlayerManager.Instance.Subscribe(CustomManagerEvents.GetUpgradeScreen,SetScene);
            }
            else
            {
                PlayerManager.Instance.Unsubscribe(CustomManagerEvents.GetUpgradeScreen,SetScene);
            }
        }

        private void SetScene(object[] arguments)
        {
            canClick = true;
            
            upgradeUI.SetActive(true);

            upgradeUI.transform.DOScale(1, .5f);
            
            for (int i = 0; i < 3; i++)
            {
                var listPicker = Random.Range(0, buttons.Count);
            
                buttons[listPicker].SetActive(true);
                if (i == 0)
                    buttons[listPicker].transform.localPosition = btn1.localPosition;
                if (i == 1)
                    buttons[listPicker].transform.localPosition = btn2.localPosition;
                if (i == 2)
                    buttons[listPicker].transform.localPosition = btn3.localPosition;
                
                openedButtons.Add(buttons[listPicker]);
                buttons.Remove(buttons[listPicker]);
                
            }
        }

        public void SetList()
        {
            foreach (var obj in openedButtons)
            {
                if (!buttons.Contains(obj))
                    buttons.Add(obj);

                if (obj.GetComponent<ButtonActor>().equipped)
                {
                    buttons.Remove(obj);
                }
                if (obj.GetComponent<ButtonActor>().level == 5)
                {
                    buttons.Remove(obj);
                }
            }
            
            openedButtons.Clear();

            foreach (var obj in buttons)
            {
                obj.SetActive(false);
            }

            foreach (var obj in equippedSpeciality)
            {
                obj.SetActive(false);
                
            }

            foreach (var obj in specialityButtons)
            {
                if (!obj.GetComponent<ButtonActor>().equipped)
                {
                    if (!buttons.Contains(obj))
                        buttons.Add(obj);
                    obj.GetComponent<ButtonActor>().stars[0].gameObject.transform.localScale = Vector3.zero;
                }
            }
        }
    }
}