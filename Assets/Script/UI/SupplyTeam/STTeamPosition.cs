using Canute.SupplyTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.SupplyTeam
{
    public class STTeamPosition : MonoBehaviour
    {
        public List<STPositionPair> positionPairs;
        public STTeammark teammark;
        public ExplorationTeam SupplyTeam => Game.PlayerData.SupplyTeam;

        public void RefreshTeamMark()
        {
            if (SupplyTeam.IsOut)
            {
                MoveMark();
            }
            else HideMark();
        }

        public void HideMark()
        {
            teammark.gameObject.SetActive(false);
        }

        public void MoveMark()
        {
            var pos = positionPairs[UnityEngine.Random.Range(0, positionPairs.Count)];
            teammark.gameObject.SetActive(true);
            teammark.transform.position = pos.objPosition.transform.position;
            teammark.cityName = pos.CityName.Split(',')[0];
            teammark.countryName = pos.CityName.Split(',')[1];
        }
    }


    [Serializable]
    public class STPositionPair
    {
        public GameObject objPosition;
        public string CityName;
    }
}
