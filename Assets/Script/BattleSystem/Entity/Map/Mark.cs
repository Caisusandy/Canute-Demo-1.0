using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{

    public class Mark : MonoBehaviour
    {
        [Flags]
        public enum Type
        {
            none = 0,
            moveRange = 1,
            attackRange = 2,
            ranges,
            select = 4,
            owner = 5,
        }

        public Type type;
        public CellEntity cellEntity;
        public SpriteRenderer markRenderer;

        public float Time { get; set; }

        public void OnEnable()
        {
            Time = 0;
        }

        // Start is called before the first frame update
        public void Awake()
        {
            markRenderer = GetComponent<SpriteRenderer>();
            cellEntity = transform.parent.GetComponent<CellEntity>();
            cellEntity.mark = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (cellEntity.data.hide)
            {
                if (markRenderer.enabled)
                {
                    markRenderer.enabled = false;
                }
                return;
            }
            else
            {
                markRenderer.enabled = true;
            }

            Color color = Color.white;
            switch (type)
            {
                case Type.attackRange:
                    color = Color.red;
                    transform.localScale = new Vector3(1, 1);
                    break;
                case Type.moveRange:
                    color = Color.green;
                    transform.localScale = new Vector3(1, 1);
                    break;
                case Type.ranges:
                    color = new Color(1, 0.5f, 0, 1);
                    transform.localScale = new Vector3(1, 1);
                    break;
                case Type.select:
                    transform.localScale = new Vector3(1, 1);
                    break;
                case Type.owner:
                    if (cellEntity.HasArmyStandOn)
                    {
                        color = cellEntity.HasArmyStandOn.Owner == Game.CurrentBattle.Player ? Color.blue : Color.red;
                        transform.localScale = new Vector3(0.8f, 0.8f);
                    }
                    else
                    {
                        cellEntity.Unhighlight(Type.owner);
                    }
                    break;
            }
            AutoFade(color);
        }

        public void AutoFade(Color color)
        {
            Time += UnityEngine.Time.deltaTime;
            color.a = 0.50f + (0.15f * Mathf.Sin(2 * Time));
            markRenderer.color = color;
        }

        public void Load(Type openBy)
        {
            if (type != Type.none && openBy != type)
            {
                if ((type == Type.attackRange && openBy == Type.moveRange) || (type == Type.moveRange && openBy == Type.attackRange))
                {
                    openBy = Type.ranges;
                }
                else return;
            }
            type = openBy;

            //Debug.Log("Mark loaded");
            markRenderer.gameObject.SetActive(true);

        }

        public void Unload(Type closeBy)
        {
            if (type == closeBy || closeBy == Type.none || type == Type.none)
            {
                type = Type.none;
                markRenderer?.gameObject.SetActive(false);
            }
            else if (type == Type.ranges)
            {
                if (closeBy == Type.moveRange)
                {
                    type = Type.attackRange;
                }
                else if (closeBy == Type.attackRange)
                {
                    type = Type.moveRange;
                }
            }
        }

        public static void Load(Type MarkType, IEnumerable<CellEntity> cell)
        {
            if (cell is null)
            {
                return;
            }

            foreach (CellEntity cellEntity in cell)
            {
                cellEntity.Highlight(MarkType);
            }
        }

        public static void Unload(Type MarkType, IEnumerable<CellEntity> cell)
        {
            if (cell is null)
            {
                return;
            }
            foreach (CellEntity cellEntity in cell)
            {
                cellEntity.Unhighlight(MarkType);
            }
        }
    }
}