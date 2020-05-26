using Canute.BattleSystem.UI;
using Canute.Languages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem
{
    public class ArmyCardEntity : CardEntity
    {
        [Header("Carrying Army")]
        public BattleArmy battleArmy;

        public static List<ArmyCardEntity> armyCards = new List<ArmyCardEntity>();

        public override void Awake()
        {
            base.Awake();
            armyCards.Add(this);
        }


        public override void OnDestroy()
        {
            armyCards.Remove(this);

            if (armyCards.Count == 0)
            {
                Game.CurrentBattle.Start();
            }
            else if (Owner == Game.CurrentBattle.Player && Owner.Entity.Cards.Count == 0 && Game.Configuration.PvP)
            {
                BattleUI.SendMessage("Open Card Pile For Second Player");
                Reorganize(Game.CurrentBattle.Enemy.Entity.Cards, BattleUI.HandCardBar);
            }
            base.OnDestroy();
        }

        public override void Display()
        {
            if (isDraging && SelectingEntity)
            {
                if (SelectingEntity is IBattleableEntity && SelectingEntity.Owner == Owner)
                {
                    if (data.Effect.Type == Effect.Types.enterMove)
                    {
                        data.Effect.Parameter = (SelectingEntity as IBattleableEntity).Data.Properties.MoveRange;
                    }
                }
            }

            int layer = !IsSelected ? id * 2 : 1000;

            infoDisplayer.text = data.Effect.Type.Lang();
            infoDisplayer.canvas.sortingLayerName = "Card";
            infoDisplayer.canvas.sortingOrder = layer + 1;

            careerPicture.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, data.Career.ToString());
            GetComponent<Image>().canvas.sortingLayerName = "Card";
            GetComponent<Image>().canvas.sortingOrder = layer;

        }

        public static CardEntity Create(Card card, BattleArmy battleArmy)
        {
            Debug.Log("Generating armycard");

            ArmyCardEntity cardEntity;
            GameObject cardObject;
            GameObject cardPrefab = GameData.Prefabs.ArmyCard;


            cardObject = Instantiate(cardPrefab, BattleUI.ClientHandCardBar.transform);
            cardEntity = cardObject.GetComponent<ArmyCardEntity>();

            cards.Add(cardEntity);

            cardEntity.data = card;
            cardEntity.battleArmy = battleArmy;
            cardObject.name = "Card";
            cardObject.transform.SetParent(BattleUI.ClientHandCardBar.transform);
            cardObject.transform.Find("Text").GetComponent<Canvas>().overrideSorting = true;

            Debug.Log(cardEntity.Owner);
            Reorganize(battleArmy.Owner.Entity.Cards, BattleUI.ClientHandCardBar);
            return cardEntity;

        }
    }
}
