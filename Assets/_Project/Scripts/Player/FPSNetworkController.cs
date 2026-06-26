using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ArcadeNetwork.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSNetworkController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _gravity = -9.81f;

        [Header("Components")]
        [SerializeField] private Camera _playerCamera;

        private CharacterController _characterController;
        private float _xRotation = 0f;
        private Vector3 _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterController.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                if (_playerCamera != null) _playerCamera.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                
                StartCoroutine(EnablePhysicsDelayed());
            }
            else
            {
                if (_playerCamera != null) _playerCamera.gameObject.SetActive(false);
                _characterController.enabled = false;
            }
            
            if (IsServer)
            {
                MoveToRandomSpawnPoint();
            }
        }

        private IEnumerator EnablePhysicsDelayed()
        {
            yield return new WaitForEndOfFrame();
            
            _characterController.enabled = true;
        }

        private void Update()
        {
            if (!IsOwner || !_characterController.enabled) return;

            HandleLook();
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            
            _characterController.Move(move.normalized * (_moveSpeed * Time.deltaTime));

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
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

        public void MoveToRandomSpawnPoint()
        {
            if (!IsServer) return;

            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            Vector3 targetPos = new Vector3(15f, 1f, 15f); 

            if (spawnPoints.Length > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                targetPos = spawnPoints[randomIndex].transform.position;
            }
            
            ExecuteRespawnOwnerRpc(targetPos);
        }
        
        [Rpc(SendTo.Owner)]
        private void ExecuteRespawnOwnerRpc(Vector3 newPosition)
        {
            Debug.Log("<color=magenta>[Respawn]</color> Öldük, yeni spawn noktasına ışınlanıyoruz.");
            
            if (_characterController != null) _characterController.enabled = false;
            
            transform.position = newPosition;
            
            if (_characterController != null) _characterController.enabled = true;
        }
    }
}