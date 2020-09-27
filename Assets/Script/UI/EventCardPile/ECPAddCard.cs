using Canute.Module;
using UnityEngine;

namespace Canute.UI.EventCardPile
{
    public class ECPAddCard : MonoBehaviour
    {
        public Vector3 position;

        public void Start()
        {
            GetComponent<Canvas>().overrideSorting = false;
        }

        public void OnMouseDown()
        {
            position = transform.localPosition;
            gameObject.AddComponent<FollowMouseMove>();
            GetComponent<Canvas>().overrideSorting = true;
            GetComponent<Canvas>().sortingOrder = 1000;
        }

        public void OnMouseUp()
        {
            GetComponent<Canvas>().sortingOrder = 0;
            GetComponent<Canvas>().overrideSorting = false;
            Destroy(gameObject.GetComponent<FollowMouseMove>());

            if (transform.position.y > 0)
            {
                PileAddCard();
                ECPCardScroll.instance.Reload();
                ECPPileDisplay.instance.Reload();
            }
            else
            {
                transform.localPosition = position;
                ECPCardScroll.instance.Organize();
            }
        }

        public bool PileAddCard()
        {
            return ECPPileDisplay.instance.EventCardPile.Add(GetComponent<EventCardUI>().displayingEventCard);
        }
    }

}
