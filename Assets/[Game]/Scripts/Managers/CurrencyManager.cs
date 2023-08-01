#region Header

// CurrencyManager.cs
// Developed by Taygun Savaş
// taygun.savas@triflesgames.com

#endregion

#region Libs

using Assets._Game_.Scripts.Entities;
using Sirenix.OdinInspector;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

#endregion

namespace Game.Managers
{
    public class CurrencyManager : Manager<CurrencyManager>
    {
        #region Serialized Fields

        [Title("# Currency Settings #")]
        public int defaultCurrencyAmount;

        #endregion

        public CurrencyData CurrencyData { get; private set; }

        protected override void MB_Start()
        {
            CurrencyData = CurrencyData.Get();
            if (CurrencyData == null)
            {
                CurrencyData = new();
                bool isSuccess = CurrencyData.Register();
                if (!isSuccess) Debug.LogError("Currency Data Entity register error!");
            }

            // Load currency data
            CurrencyData.Load();
            if (CurrencyData.Gold < defaultCurrencyAmount)
                CurrencyData.Gold = defaultCurrencyAmount;
        }

        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Init, OnGameInit);
            }
            else
            {
                GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Init, OnGameInit);
            }
        }

        private void OnGameInit(object[] arguments) => Publish(ManagerEvents.ShowCurrency, CurrencyData.Gold);

        #region Currency Manager API

        public void AddCurrency(int amount) => CurrencyData.AddCurrency(amount);
        public void SubstractCurrency(int amount) => CurrencyData.SubstractCurrency(amount);

        #endregion
    }
}