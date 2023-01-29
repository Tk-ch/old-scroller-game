using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    public class ShipLogic 
    {

        private ArmorLogic _armorLogic;
        private EngineLogic _engineLogic;


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