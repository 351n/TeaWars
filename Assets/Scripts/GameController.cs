using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    #region Singleton

    public static GameController instance;

    void Awake() {
        instance = this;
    }

    #endregion

    Queue<PlayerController> players;

    public PlayerController currentPlayer;
    public GameUI ui;

    void Start() {
        Initialize();
        StartTurn();
    }

    private void Initialize() {
        players = new Queue<PlayerController>(FindObjectsOfType<PlayerController>().ToList());
        currentPlayer = players.Peek();
        //TODO remove this after testing
        currentPlayer.AddGold(50);

        foreach(PlayerController p in GetPlayersList()) {
            for(int i = 0; i < 5; i++) {
                p.hand.Add(CardsController.instance.Draw());
            }

            if(p != currentPlayer) {
                foreach(OtherPlayerUI u in ui.otherPlayersUIs) {
                    if(u.player is null) {
                        u.player = p;
                        break;
                    }
                }
            }
        }
    }

    public void StartTurn() {
        currentPlayer.UpdateRegionUI();
        PlagueCard plague = CardsController.instance.GetPlagueCard();
        plague.Apply(GetPlayersList());
        CardsController.instance.Discard(plague);
        currentPlayer.UpdateRegionUI();

        if(plague.effect.skippedTurns <= 0) {
            currentPlayer.ApplyPlantEffect();
            currentPlayer.UpdateRegionUI();
            currentPlayer.ApplyPlantatorEffect();
            currentPlayer.UpdateRegionUI();
        } else {
            //Skip
        }
    }

    public void EndTurn() {
        //Give control to next player
    }

    public List<PlayerController> GetPlayersList() {
        return players.ToList();
    }

    internal void UpdateRegionUI() {
        currentPlayer.UpdateRegionUI();
    }

    internal void UnlockPlayerSelection() {
        ui.UnlockPlayerSelection();
    }

    internal void LockPlayerSelection() {
        ui.LockPlayerSelection();
    }

    internal void SelectPlayer(PlayerController player) {
        currentPlayer.SelectPlayer(player);
    }
}
