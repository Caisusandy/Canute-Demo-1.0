using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class BattleScore : MonoBehaviour
    {
        public Text text;

        // Start is called before the first frame update
        void Start()
        {
            if (Game.CurrentBattle is null)
            {
                Destroy(this);
                return;
            }

            if (Game.CurrentBattle.BattleType != Battle.Type.endless)
            {
                Destroy(this);
                return;
            }

            StartCoroutine("GetScore");

        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator GetScore()
        {
            while (Game.CurrentBattle != null && this)
            {
                if (text)
                {
                    text.text = Game.CurrentBattle.ScoreBoard.GetScore().ToString();
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
}