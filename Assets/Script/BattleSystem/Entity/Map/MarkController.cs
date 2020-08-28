using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class MarkController
    {
        [SerializeField] private List<CellMark> cellMarks = new List<CellMark>();
        [SerializeField] private CellMark.Type type;

        public List<CellMark> CellMarks { get => cellMarks; set => cellMarks = value; }
        public CellMark.Type Type { get => type; set => type = value; }

        public MarkController()
        {

        }

        public MarkController(CellMark.Type type)
        {
            //Debug.Log("Generate Controller");
            this.Type = type;
            cellMarks = new List<CellMark>();
        }

        public MarkController(CellMark.Type type, params CellEntity[] cells) : this(type)
        {
            GenerateMark(cells);
        }

        public MarkController(CellMark.Type type, IEnumerable<CellEntity> cells) : this(type, cells.ToArray()) { }

        ~MarkController()
        {
            ClearDisplay();
        }

        public void Refresh(params CellEntity[] cells)
        {
            ClearDisplay();
            GenerateMark(cells);
        }
        public void Refresh(IEnumerable<CellEntity> cells) => Refresh(cells.ToArray());


        private void GenerateMark(params CellEntity[] cells)
        {
            foreach (var cell in cells)
            {
                var mark = GetMark(cell);
                mark.HasController = true;
                CellMarks.Add(mark);
            }
        }

        private CellMark GetMark(CellEntity cell)
        {
            IEnumerable<CellMark> nonUsingMark = cell.Marks.Where(item => !item.HasController);
            CellMark mark;
            if (nonUsingMark.Count() > 0)
            {
                mark = nonUsingMark.First();
                foreach (var item in nonUsingMark)
                {
                    if (mark.level > item.level)
                        mark = item;
                }
            }
            else
            {
                mark = CellMark.CreateMark(cell);
                mark.level = cell.Marks.Count - 1;
            }

            return mark;

        }

        public void Display()
        {
            foreach (var mark in CellMarks)
            {
                mark.Load(Type);
            }
        }

        public void ClearDisplay()
        {
            foreach (var mark in CellMarks)
            {
                var curMarkLvl = mark.level;
                foreach (var item in mark.CellEntity.Marks)
                {
                    if (item.level > curMarkLvl)
                    {
                        item.level--;
                    }
                }
                mark.HasController = false;
                mark.level = mark.CellEntity.Marks.Count - 1;
                mark.Unload();
            }

            cellMarks.Clear();
        }


        public override bool Equals(object obj)
        {
            if (obj is MarkController)
            {
                var b = obj as MarkController;
                if (b.CellMarks.Count != CellMarks.Count)
                {
                    return false;
                }
                else return base.Equals(obj);
            }
            return false;
        }

        public static bool operator ==(MarkController a, MarkController b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null || b is null)
            {
                if (!(a is null))
                {
                    return a.CellMarks.Count == 0;
                }
                else
                {
                    return b.CellMarks.Count == 0;
                }
            }
            return a.Equals(b);
        }
        public static bool operator !=(MarkController a, MarkController b)
        {
            return !(a == b);
        }

    }
}
