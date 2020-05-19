using Canute.BattleSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class CellBorder : MonoBehaviour
    {
        public CellEntity cellEntity;
        public List<GameObject> Border;

        public float Time { get; set; } = 0;

        // Start is called before the first frame update
        private void Start()
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            AutoFade(Color.white);
        }

        public void AutoFade(Color color)
        {
            Time += UnityEngine.Time.deltaTime;
            foreach (GameObject gameObject in Border)
            {
                color.a = 0.6f + 0.30f * Mathf.Sin(2 * Time);
                gameObject.GetComponent<SpriteRenderer>().color = color;
            }
        }

        public void SetBorderClose(CellEntity ToCell)
        {
            //Debug.Log("设置边境:" + CellEntity.Column + "," + CellEntity.Cell);
            if (cellEntity.NearByCells.Contains(ToCell))
            {
                if (ToCell.HexCoord.x == cellEntity.HexCoord.x)
                {
                    if (ToCell.HexCoord.y == cellEntity.HexCoord.y + 1)
                    {
                        Border[0].SetActive(true);
                    }
                    else
                    {
                        Border[3].SetActive(true);
                    }
                }
                else if (ToCell.HexCoord.y == cellEntity.HexCoord.y)
                {
                    if (ToCell.HexCoord.x == cellEntity.HexCoord.x + 1)
                    {
                        Border[1].SetActive(true);
                    }
                    else
                    {
                        Border[4].SetActive(true);
                    }
                }
                else if (ToCell.HexCoord.z == cellEntity.HexCoord.z)
                {
                    if (ToCell.HexCoord.y == cellEntity.HexCoord.y + 1)
                    {
                        Border[5].SetActive(true);
                    }
                    else
                    {
                        Border[2].SetActive(true);
                    }
                }
            }
        }
        public void UnloadAllBorder()
        {
            foreach (GameObject border in Border)
            {
                border.SetActive(false);
            }
        }

        ///<summary>战斗范围的加载</summary>
        public static void CreateBorder(List<CellEntity> Field)
        {
            foreach (CellEntity cell in Field)
            {
                foreach (CellEntity Nearby in cell.NearByCells)
                {
                    if (!Field.Contains(Nearby))
                    {
                        //cell.CreateBorder(Nearby);
                    }
                }
            }
        }

        public static void DeleteBorder(List<CellEntity> Field)
        {
            foreach (CellEntity cell in Field)
            {
                //cell.DeleteBorder();
            }
        }
    }
}