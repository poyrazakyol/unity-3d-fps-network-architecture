using Unity.Netcode;
using UnityEngine;

namespace ArcadeNetwork.Player
{
    public class NetworkHealth : NetworkBehaviour
    {
        public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(100, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            CurrentHealth.OnValueChanged += OnHealthChanged;
        }

        public override void OnNetworkDespawn()
        {
            CurrentHealth.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int oldHealth, int newHealth)
        {
            Debug.Log($"<color=red>[Damage]</color> Oyuncu {OwnerClientId} Canı Düştü: {newHealth}");
            
            if (newHealth <= 0)
            {
                HandleDeath();
            }
        }

        
        public void TakeDamage(int damageAmount)
        {
            if (!IsServer) return; 
            
            CurrentHealth.Value = Mathf.Max(0, CurrentHealth.Value - damageAmount);
        }

        private void HandleDeath()
        {
            Debug.Log($"<color=darkred>[DEATH]</color> Oyuncu {OwnerClientId} öldü!");
    
            if (IsServer)
            {
                if (TryGetComponent(out FPSNetworkController controller))
                {
                    controller.MoveToRandomSpawnPoint();
                }
        
                CurrentHealth.Value = 100;
            }
        }
    }
}