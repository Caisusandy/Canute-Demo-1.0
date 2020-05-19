using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.UI
{
    public class PvPUI : MonoBehaviour
    {
        private void Awake()
        {
            if (!Game.Configuration.PvP)
            {
                Destroy(transform.parent.parent.gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}