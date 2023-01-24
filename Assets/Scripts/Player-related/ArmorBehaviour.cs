using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nebuloic
{
    /// <summary>
    /// Monobehaviour handler for the Armor class
    /// </summary>
    public class ArmorBehaviour : MonoBehaviour
    {
        [SerializeField] Armor _armor;
        [SerializeField] float _invulnerabilityTime;
        public Armor Armor { get => _armor; }

        private EventHandler<int> HPListener;
        private EventHandler<bool> VulnerabilityListener;

        public UnityEvent<int> ArmorHPChanged;
        public UnityEvent<bool> ArmorVulnerabilityChanged;

        private void Awake()
        {
            Armor.Init();
            // subscribing to armor events
            HPListener = (object e, int hp) => ArmorHPChanged?.Invoke(hp);
            VulnerabilityListener = (object e, bool vulnerability) => ArmorVulnerabilityChanged?.Invoke(vulnerability);
            Armor.HPChanged += HPListener;
            Armor.VulnerabilityChanged += VulnerabilityListener;
            Armor.VulnerabilityChanged += ResetVulnerability;
        }

        

        private void ResetVulnerability(object _, bool vuln)
        {
            if (!vuln) {
                StartCoroutine(Utility.ExecuteAfterTime(() => Armor.IsVulnerable = true, _invulnerabilityTime));
            }
        }

        private void OnDestroy()
        {
            // unsubscribing from armor events
            _armor.HPChanged -= HPListener;
            _armor.VulnerabilityChanged -= VulnerabilityListener;
        }
        

    }
}