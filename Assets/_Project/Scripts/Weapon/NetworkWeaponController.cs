using Unity.Netcode;
using UnityEngine;

namespace ArcadeNetwork.Weapon
{
    public class NetworkWeaponController : NetworkBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private float _fireRange = 100f;
        [SerializeField] private int _damageAmount = 20;

        [Header("Components")]
        [SerializeField] private Camera _playerCamera;

        private void Update()
        {
            if (!IsOwner) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            if (_playerCamera == null) return;
            
            Vector3 camPos = _playerCamera.transform.position;
            Vector3 camForward = _playerCamera.transform.forward;
            
            RequestFireServerRpc(camPos, camForward);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestFireServerRpc(Vector3 firePoint, Vector3 direction)
        {
            Debug.Log($"<color=orange>[ServerRpc]</color> Oyuncu {OwnerClientId} ateş etti. Sunucu doğrulaması başladı.");
            
            if (Physics.Raycast(firePoint, direction, out RaycastHit hit, _fireRange))
            {
                Debug.Log($"<color=yellow>[Hit]</color> Sunucu şuna vurdu: {hit.collider.name}");
                
                if (hit.collider.TryGetComponent(out Player.NetworkHealth enemyHealth))
                {
                    if (enemyHealth.OwnerClientId != OwnerClientId)
                    {
                        enemyHealth.TakeDamage(_damageAmount);
                    }
                }
            }
        }
    }
}