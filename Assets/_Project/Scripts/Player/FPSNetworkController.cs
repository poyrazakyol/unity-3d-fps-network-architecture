using Unity.Netcode;
using UnityEngine;

namespace ArcadeNetwork.Player
{
    public class FPSNetworkController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _mouseSensitivity = 2f;

        [Header("Components")]
        [SerializeField] private Camera _playerCamera;

        private float _xRotation = 0f;
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                if (_playerCamera != null) _playerCamera.gameObject.SetActive(true);
                
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                if (_playerCamera != null) _playerCamera.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            HandleLook();
            HandleMovement();
        }

        private void HandleMovement()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            
            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            transform.position += move.normalized * (_moveSpeed * Time.deltaTime);
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            
            transform.Rotate(Vector3.up * mouseX);
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            if (_playerCamera != null)
            {
                _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }
        }
    }
}