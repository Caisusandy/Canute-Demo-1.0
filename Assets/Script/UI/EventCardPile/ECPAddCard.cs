using Canute.Module;
using UnityEngine;

namespace Canute.UI.EventCardPile
{
    public class ECPAddCard : MonoBehaviour
    {
        public Vector3 position;


        public void OnMouseDown()
        {
            position = transform.localPosition;
            gameObject.AddComponent<FollowMouseMove>();
            GetComponent<Canvas>().sortingOrder = 1000;
        }

        public void OnMouseUp()
        {
            GetComponent<Canvas>().sortingOrder = 0;
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
