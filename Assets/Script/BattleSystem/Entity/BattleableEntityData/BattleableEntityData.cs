using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{

    /// <summary> 至少能够上战场的接口 </summary>
    public interface IBattleableEntityData : IOnMapEntityData, ICareerLabled
    {
        int Anger { get; set; }
        BattleLeader ViceCommander { get; }
        HalfSkillEffect SkillPack { get; }
        BattleProperty Properties { get; }
        BattleProperty RawProperties { get; set; }
        BattleProperty.Position StandPosition { get; }

        List<CellEntity> GetMoveArea();
    }

}