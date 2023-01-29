using System;
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

        public event Action OnRollChanged;

        private readonly float _rollDurationInSeconds;
        private Timer rollingTimer;

        private bool _isRolling;
        public bool IsRolling
        {
            get { return _isRolling; }
            set
            {
                _isRolling = value;
                OnRollChanged?.Invoke();
            }
        }

        public void Roll()
        {
            if (!IsRolling)
            {
                IsRolling = true;
                rollingTimer = new Timer(_rollDurationInSeconds);
                rollingTimer.OnTimerEnd += ResetRoll;
            }
        }
        void ResetRoll() => IsRolling = false;

        public ShipLogic( ShipData _data, float rollDurationInSeconds) {
            _rollDurationInSeconds = rollDurationInSeconds;
            _armorLogic = new ArmorLogic(_data.ArmorData);
            _engineLogic = new EngineLogic(_data, _armorLogic);
        }


        public void Update(float deltaTime) { 
            if (rollingTimer != null) rollingTimer.UpdateTimer(deltaTime); 
            _armorLogic.Update(deltaTime);
            _engineLogic.Update(deltaTime);
        }

    }
}