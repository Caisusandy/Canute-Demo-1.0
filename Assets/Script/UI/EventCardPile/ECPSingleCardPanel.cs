using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.EventCardPile
{
    public class ECPSingleCardPanel : MonoBehaviour
    {
        public static ECPSingleCardPanel instance;
        public static EventCardItem SelectingEventCard => instance.selectingEventCard.displayingEventCard;

        public EventCardUI selectingEventCard;

        public EventCardUI eventCardCardUI;
        //public LSEventCardSkillCardUI eventCardSkillCardUI;

        [Header("Text Info")]
        public Text critRateKey;
        public Text critRate;
        public Text critBounesKey;
        public Text critBounes;

        public Text leaderName;

        [Header("Icon")]
        public Image career;
        public Image leaderIcon;
        ////public EventCardTypeIcon eventCardTypeIcon;
        //public AttackTypeIcon attackTypeIcon;
        //public StandPostionIcon standPosition;
        //public AttackPostionIcon attackPostion;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Display(EventCardItem eventCardItem)
        {
            ClearDisplay();
            Debug.Log(eventCardCardUI);
            eventCardCardUI.Display(eventCardItem);

            career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, eventCardItem.Career.ToString());
        }

        public void ClearDisplay()
        {
            eventCardCardUI.Display(EventCardItem.Empty);


            critBounes.text = "";
            critRate.text = "";

            career.sprite = null;
            leaderName.text = "";
        }

        public void ChangeEventCard()
        {

        }

        public void LeftEventCard()
        {
            Debug.Log("Left EventCard");
            ECPPileDisplay.instance.EventCardPile.Left(SelectingEventCard);
            ECPPileDisplay.instance.Reload();
            PlayerFile.SaveCurrentData();

        }

        //public void Filp()
        //{
        //    Vector3 d = new Vector3(0, 5f, 0);
        //    if (eventCardCardUI.transform.eulerAngles.y < 90)
        //    {
        //        StartCoroutine(Flip());
        //    }
        //    else
        //    {
        //        StartCoroutine(Reverse());
        //    }

        //    IEnumerator Flip()
        //    {
        //        while (true)
        //        {
        //            if (eventCardCardUI.transform.eulerAngles.y < 90)
        //            {
        //                eventCardCardUI.transform.eulerAngles += d;
        //            }
        //            else
        //            {
        //                eventCardCardUI.gameObject.SetActive(false);
        //                eventCardSkillCardUI.gameObject.SetActive(true);
        //                Debug.Log(eventCardSkillCardUI.transform.eulerAngles.y);
        //                if (Math.Abs(eventCardSkillCardUI.transform.eulerAngles.y) > 0.1)
        //                {
        //                    eventCardSkillCardUI.transform.eulerAngles -= d;
        //                }
        //                else
        //                {
        //                    eventCardSkillCardUI.transform.eulerAngles = Vector3.zero;
        //                    yield break;
        //                }
        //            }
        //            yield return new WaitForFixedUpdate();
        //            Debug.Log(eventCardCardUI.transform.eulerAngles.y);
        //        }
        //    }

        //    IEnumerator Reverse()
        //    {
        //        while (true)
        //        {
        //            if (eventCardSkillCardUI.transform.eulerAngles.y < 90)
        //            {
        //                eventCardSkillCardUI.transform.eulerAngles += d;
        //            }
        //            else
        //            {
        //                eventCardSkillCardUI.gameObject.SetActive(false);
        //                eventCardCardUI.gameObject.SetActive(true);
        //                Debug.Log(eventCardCardUI.transform.eulerAngles.y);
        //                if (Math.Abs(eventCardCardUI.transform.eulerAngles.y) > 0.1)
        //                {
        //                    eventCardCardUI.transform.eulerAngles -= d;
        //                }
        //                else
        //                {
        //                    eventCardCardUI.transform.eulerAngles = Vector3.zero;
        //                    yield break;
        //                }
        //            }
        //            yield return new WaitForFixedUpdate();
        //        }
        //    }
        //}
    }
}
