#region Header

// GameUiActor.cs
// Developed by Taygun Savaş
// taygun.savas@triflesgames.com

#endregion

#region Libs

using Game.Managers;
using Sirenix.OdinInspector;
using TMPro;
using TriflesGames.Actors;
using TriflesGames.ManagerFramework;

#endregion

public class GameUiActor : UiActor
{
    #region Serialized Fields

    // Custom Ui fields, methods
    [Title("# Currency Settings #")]
    public TextMeshProUGUI txtCurrency;

    #endregion

    protected override void MB_Listen(bool status)
    {
        base.MB_Listen(status);
        if (status)
        {
            CurrencyManager.Instance.Subscribe(ManagerEvents.ShowCurrency, OnUiCurrencyShow);
        }
        else
        {
            CurrencyManager.Instance.Unsubscribe(ManagerEvents.ShowCurrency, OnUiCurrencyShow);
        }
    }

    private void OnUiCurrencyShow(object[] arguments) => txtCurrency.text = ((int)arguments[0]).ToString();
}