using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    public class ShipLogic 
    {

        private readonly ArmorLogic _armorLogic;
        private readonly EngineLogic _engineLogic;

        public EngineLogic Engine => _engineLogic;
        public ArmorLogic Armor => _armorLogic;



        public ShipLogic( ShipData _data ) {
            _armorLogic = new ArmorLogic(_data.ArmorData);
            _engineLogic = new EngineLogic(_data, _armorLogic);
        }


        public void Update(float deltaTime) { 
            _armorLogic.Update(deltaTime);
            _engineLogic.Update(deltaTime);
        }

    }
}