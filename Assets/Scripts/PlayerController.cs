using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string nickname;
    public uint Gold { get => gold; private set => gold = value; }
    public List<Card> hand = new List<Card>();
    public Region region;

    public RegionGameObject regionGameObject;
    public PlayerUI ui;

    private uint gold;
    private Card selectedCard;
    private Zone selectedZone;
    private PlayerController selectedTarget;

    bool playedAssetCard = false;
    bool playedWorkerCard = false;
    bool playedTrickeryCard = false;

    bool canPlayTwoCards = true;

    void Start() {
        regionGameObject.UpdateUI();
        ui.UpdateHand(hand);
    }

    internal void ApplyPlantEffect() {
        region.ApplyPlantEffect();
    }

    internal void ApplyPlantatorEffect() {
        region.ApplyPlantatorEffect();
        ui.UpdateHand(hand);
    }

    public void SelectCard(Card card) {
        if(selectedCard != null) {
            region.LockAssetZonesSelection();
            region.LockWorkersZonesSelection();
        }

        selectedCard = card;

        if(card is AssetCard) {
            region.UnlockAssetZonesSelection();
        } else if(card is WorkerCard) {
            region.UnlockWorkerZonesSelection();
        } else if(card is TrickeryCard) {
            TrickeryCard t = card as TrickeryCard;
            if(t.Target == Target.Player) {
                //selection of player
            } else if (t.Target == Target.Building) {
                //selection of player and building
            }
        }
    }

    public void SelectAssetZone(Zone zone) {
        selectedZone = zone;

        if(ui) {
            ui.ShowConfirmButton();
        }
    }

    public void SelectWorkerZone(Zone zone) {
        selectedZone = zone;

        if(ui) {
            ui.ShowConfirmButton();
        }
    }

    private void ResetSelection() {
        selectedCard = null;
        selectedZone = Zone.None;
    }

    public void PlaySelectedCard() {
        if(selectedCard is WorkerCard && (!playedWorkerCard || canPlayTwoCards)) {
            PlayCard(selectedCard as WorkerCard, selectedZone);

            if(!playedWorkerCard) {
                playedWorkerCard = true;
            } else {
                canPlayTwoCards = false;
            }
        } else if(selectedCard is AssetCard && (!playedAssetCard || canPlayTwoCards)) {
            PlayCard(selectedCard as AssetCard, selectedZone);

            if(!playedAssetCard) {
                playedAssetCard = true;
            } else {
                canPlayTwoCards = false;
            }
        } else if(selectedCard is TrickeryCard && (!playedTrickeryCard || canPlayTwoCards)) {
            PlayCard(selectedCard as TrickeryCard, selectedTarget);

            if(!playedAssetCard) {
                playedTrickeryCard = true;
            } else {
                canPlayTwoCards = false;
            }
        }

        UpdateCardsPermissions();
    }

    public bool UpdateCardsPermissions() {
        if(canPlayTwoCards) {
            bool a = playedAssetCard;
            bool b = playedWorkerCard;
            bool c = playedTrickeryCard;

            //check if zero or only one card of any type was played
            canPlayTwoCards = ((!a && !b && !c) || (!a && !b && c) || (!a && b && !c) || (a && !b && !c));
        }

        return canPlayTwoCards;
    }

    internal void PlayCard(Card card, Zone zone) {
        if(card is AssetCard || card is WorkerCard) {
            region.PlayCard(card, zone);
        }

        hand.Remove(selectedCard);

        if(ui) {
            ui.UpdateHand(hand);
            ui.HideConfirmButton();
        }

        region.LockAssetZonesSelection();
        region.LockWorkersZonesSelection();
        ResetSelection();
    }

    internal void PlayCard(TrickeryCard card, PlayerController target) {
        throw new NotImplementedException();
    }

    public void AddGold(uint value) {
        if(uint.MaxValue - value < gold) {
            gold = uint.MaxValue;
        } else {
            gold += value;
        }
    }

    public void SubstractGold(uint value) {
        if(value >= gold) {
            gold = 0;
        } else {
            gold -= value;
        }
    }

    internal void UpdateRegionUI() {
        if(regionGameObject)
            regionGameObject.UpdateUI();
    }
}
