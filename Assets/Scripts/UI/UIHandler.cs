using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIHandler : MonoBehaviour
    {
        private Player _player;
        Player PlayerComponent => _player ?? (_player = gameObject.GetComponent<Player>());
        [SerializeField] ParticleSystem Stars;

        private void Update()
        {
            var velocity = Stars.velocityOverLifetime;
            velocity.speedModifierMultiplier = PlayerComponent.EngineComponent.CurrentSpeed;
        }

    }
}