using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{

    public class CellMark : MonoBehaviour
    {
        public enum Type
        {
            none,
            moveRange,
            attackRange,
            select,
            path,
            owner,
            protectCell,
        }

        public Type type;
        public int level;
        public bool hasController;
        public SpriteRenderer markRenderer;
        public CellEntity CellEntity => transform.parent.GetComponent<CellEntity>();


        private Color Color
        {
            get
            {
                Color color = Color.gray;
                transform.localScale = Vector3.one;
                switch (type)
                {
                    case Type.attackRange:
                        color = new Color(0.9f, 0, 0);
                        break;
                    case Type.moveRange:
                        color = new Color(1f, 0.9f, 0f);
                        break;
                    case Type.select:
                        color = new Color(1f, 1f, 1, 0.8f);
                        break;
                    case Type.path:
                        color = Color.green;
                        break;
                    case Type.protectCell:
                        color = new Color(0, 0.9f, 1);
                        break;
                    case Type.owner:
                        if (CellEntity.HasArmyStandOn)
                        {
                            color = CellEntity.HasArmyStandOn.Owner == Game.CurrentBattle.Player ? Color.blue : Color.red;
                            transform.localScale = Vector3.one * 0.6f;
                        }
                        else
                            Unload();
                        break;
                }

                return color;
            }
        }
        public float Time { get; set; }
        public bool HasController { get => hasController; set => hasController = value; }

        public void OnEnable()
        {
            Time = 0;
        }

        // Start is called before the first frame update
        public void Awake()
        {
            markRenderer = GetComponent<SpriteRenderer>();
            markRenderer.size = GetMarkSize();
            markRenderer.sortingLayerName = CellEntity.GetComponent<SpriteRenderer>().sortingLayerName;
            markRenderer.sortingOrder = CellEntity.GetComponent<SpriteRenderer>().sortingOrder;
            CellEntity.Marks.Add(this);
        }

        private void Update()
        {
            Color color = Color;
            if (type != Type.owner) CheckSize();
            AutoFade(color);
        }

        /// <summary>
        /// allow marks to connect each other better
        /// not so sure it is a good idea
        /// </summary>
        private void CheckSize()
        {
            transform.localScale = Vector3.one * (10 - level) / 10;
        }

        public void AutoFade(Color color)
        {
            Time += UnityEngine.Time.deltaTime;
            color.a = 0.50f + (0.15f * Mathf.Sin(2 * Time));
            markRenderer.color = color;
        }

        private Vector2 GetMarkSize()
        {
            Vector2 size = CellEntity.GetComponent<SpriteRenderer>().size;
            Vector2 vector2 = new Vector2(size.x, size.x / 0.866f);
            return vector2;
        }

        public void Load(Type type)
        {
            //Debug.Log("mark type:" + type);
            this.type = type;
            enabled = true;
            markRenderer.enabled = true;
        }

        public void Unload()
        {
            markRenderer.enabled = false;
            enabled = false;
        }

        public void OnDestroy()
        {
            CellEntity.Marks.Remove(this);
        }

        public static CellMark CreateMark(CellEntity cellEntity)
        {
            //Debug.Log("create mark on: " + cellEntity, cellEntity);
            CellMark cellMark = Instantiate(GameData.Prefabs.Get("cellMark"), cellEntity.transform).GetComponent<CellMark>();
            cellMark.name = "Mark";
            return cellMark;
        }
    }
}