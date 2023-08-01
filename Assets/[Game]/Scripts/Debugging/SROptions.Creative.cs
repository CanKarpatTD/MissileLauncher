#region Header

// SROptions.Creative.cs
// Developed by Taygun Savaş
// taygun.savas@triflesgames.com

#endregion

#if TRIFLES_DEBUG

#region Libs

using System.ComponentModel;
using TMPro;
using TriflesGames.Managers;
using UnityEngine;

#endregion

/// <summary>
///     To add change a variable in runtime, you can add them here as a property.
///     They will be shown in SROptions > Options
///     There are attributes to distinguish your variables properly:
///     NumberRange: Limits a number property to the range provided.
///     Increment:   Changes how much a number property is changed by pressing the up/down buttons on the options tab.
///     DisplayName: Manually control what name the option will use on the options tab.
///     Sort:        Provide a sorting index that is used to control the order in which options appear.
///     Category:    Group options by category name. (Note: The System.ComponentModel namespace must be imported in your
///     file to use this attribute.
/// </summary>
public partial class SROptions
{
    private GameObject _headerPanel;
    private TextMeshProUGUI _headerText;
    private GameObject _mainCanvas;

    [Category("Creative")]
    [DisplayName("Toggle UI")]
    public bool ToggleUi
    {
        get
        {
            _mainCanvas ??= GameManager.Instance.GetActors<GameUiActor>()[0].pnl_Header.transform.root.gameObject;

            return _mainCanvas.activeSelf;
        }
        set => _mainCanvas.SetActive(value);
    }

    [Category("Creative")]
    [DisplayName("Change Level Text")]
    public int SetLevelText
    {
        get
        {
            _headerText ??= GameManager.Instance.GetActors<GameUiActor>()[0].txt_Level;

            return GameManager.Instance.gameData.Level + 1;
        }
        set => _headerText.text = $"LEVEL {value}";
    }

    [Category("Creative")]
    [DisplayName("Toggle Level Text")]
    public bool ToggleLevelText
    {
        get
        {
            _headerPanel ??= GameManager.Instance.GetActors<GameUiActor>()[0].pnl_Header.gameObject;

            return _headerPanel.activeSelf;
        }
        set => _headerPanel.SetActive(value);
    }
}
#endif