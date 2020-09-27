using Canute.Module;
using UnityEngine;

namespace Canute.UI.EventCardPile
{
    public class ECPRemoveCard : MonoBehaviour
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
            GetComponent<Canvas>().overrideSorting = false;
            GetComponent<Canvas>().sortingOrder = 0;
            Destroy(gameObject.GetComponent<FollowMouseMove>());

            if (transform.position.y < 0)
            {
                PileRemoveCard();
                ECPCardScroll.instance.Reload();
                ECPPileDisplay.instance.Reload();
            }
            else
            {
                transform.localPosition = position;
            }
        }

        public void PileRemoveCard()
        {
            ECPPileDisplay.instance.EventCardPile.Left(GetComponent<EventCardUI>().displayingEventCard);
        }
    }

}
