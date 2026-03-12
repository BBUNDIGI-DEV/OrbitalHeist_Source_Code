using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CheatManager : MonoBehaviour
{
    private bool mIsDamagedCheatActivated;

    private void Start()
    {
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat1.ToString(), damageCheatToggle);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat2.ToString(), addCharacter);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat3.ToString(), fullUltimateGuage);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat4.ToString(), makePlayerCriticalDamage);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat5.ToString(), reviveAllCharacter);

    }



    private void OnDestroy()
    {
        if (InputManager.IsExist)
        {
            InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat1.ToString(), damageCheatToggle);
            InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat2.ToString(), addCharacter);
            InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat3.ToString(), fullUltimateGuage);
            InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat4.ToString(), makePlayerCriticalDamage);
            InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Cheat5.ToString(), reviveAllCharacter);
        }

    }


    private void damageCheatToggle(CallbackContext context)
    {
        if(mIsDamagedCheatActivated)
        {
            PlayerManager.Instance.GlobalPlayerStatus.GlobalDamage -= 100;
            mIsDamagedCheatActivated = false;
        }
        else
        {
            PlayerManager.Instance.GlobalPlayerStatus.GlobalDamage += 100;
            mIsDamagedCheatActivated = true;
        }
    }


    private void addCharacter(CallbackContext context)
    {
        if(PlayerManager.Instance.ActivatedCharacters.Count != 3)
        {
            PlayerManager.Instance.AddActivatedCharacter(eCharacterName.Shiv);
            PlayerManager.Instance.AddActivatedCharacter(eCharacterName.Hypo);
        }
    }

    private void fullUltimateGuage(CallbackContext context)
    {
        PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.UltimateGuage.Value =
            PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.UltimateGuage.Value.MaximizeGauge();
    }

    private void makePlayerCriticalDamage(CallbackContext context)
    {
        PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.DecreaseHP(100000000.0f, out bool isShield);

    }

    private void reviveAllCharacter(CallbackContext context)
    {
        PlayerManager.Instance.ReviveAllCharacter(1000.0f);
    }
}
