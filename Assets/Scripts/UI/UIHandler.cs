using Nebuloic;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIHandler : MonoBehaviour
    {
        private ShipLogic _ship;
        ShipLogic Ship => _ship ?? (_ship = gameObject.GetComponent<ShipBehaviour>().Logic);
        [SerializeField] ParticleSystem Stars;
        [SerializeField] ParticleSystem Thruster;
        [SerializeField] GUIHandler guiHandler;


        private void Update()
        {
            var velocity = Stars.velocityOverLifetime;
            velocity.speedModifierMultiplier = Ship.Engine.CurrentSpeed;

            velocity = Thruster.velocityOverLifetime;
            velocity.speedModifierMultiplier = Ship.Engine.CurrentSpeed;
            var m = Thruster.main;
            m.startColor = guiHandler.GearColorsSelected[Ship.Engine.CurrentGear];
            var emission = Thruster.emission;
            emission.rateOverTimeMultiplier = Ship.Engine.SpeedPercentage * 20;
        }

    }
}