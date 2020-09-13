using UnityEngine;

namespace Canute.UI.SupplyTeam
{
    public class STTeammark : MonoBehaviour
    {
        public string cityName;
        public string countryName;
        public Label label;

        public void OnMouseDown()
        {
            label = Label.GetLabel(transform);
            string cityName = ("Canute.World.City." + this.cityName + ".name").Lang();
            string countryName = ("Canute.World.Country." + this.countryName + ".name").Lang();
            label.text.text = ("Canute.SupplyTeam.TeamPositionInfo").Lang().Replace("@city", cityName).Replace("@country", countryName);
        }

        public void OnMouseUp()
        {
            Destroy(label.gameObject);
        }
    }
}
