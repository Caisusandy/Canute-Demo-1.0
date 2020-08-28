using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class ColumnEntity : DecorativeEntity, IEnumerable<CellEntity>
    {
        public List<CellEntity> cellEntities;
        public override EntityData Data => new Column(this);
        public int index => MapEntity.CurrentMap.columnEntities.IndexOf(this);

        public CellEntity this[int index] => cellEntities[index];


        [ContextMenu("Column Setup")]
        public void ColumnSetup()
        {
            foreach (Transform item in transform)
            {
                CellEntity cellEntity = item.GetComponent<CellEntity>();
                cellEntities.Add(cellEntity);
                cellEntity.data.Coordinate = MapEntity.CurrentMap.GetPosition(cellEntity);
            }
        }


        public static implicit operator List<CellEntity>(ColumnEntity columnEntity)
        {
            return columnEntity.cellEntities;
        }

        public IEnumerator<CellEntity> GetEnumerator()
        {
            return ((IEnumerable<CellEntity>)cellEntities).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CellEntity>)cellEntities).GetEnumerator();
        }
    }

    [Serializable]
    public class Column : NonOwnableEntityData, IEnumerable<Cell>
    {
        public List<Cell> cells = new List<Cell>();
        public Cell this[int index] { get => cells[index]; set => cells[index] = value; }

        public Column(ColumnEntity columnEntity)
        {
            cells = new List<Cell>();
            foreach (CellEntity item in columnEntity)
            {
                cells.Add(item.data);
            }
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return ((IEnumerable<Cell>)cells).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Cell>)cells).GetEnumerator();
        }
    }
}
