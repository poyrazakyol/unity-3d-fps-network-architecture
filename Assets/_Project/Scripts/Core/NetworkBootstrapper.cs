using Unity.Netcode;
using UnityEngine;

namespace ArcadeNetwork.Core
{
    public class NetworkBootstrapper : MonoBehaviour
    {
        private void OnGUI()
        {
            if (NetworkManager.Singleton == null) return;
            
            GUILayout.BeginArea(new Rect(20, 20, 300, 300));
            
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (GUILayout.Button("Start HOST (Server + Client)", GUILayout.Height(40)))
                {
                    NetworkManager.Singleton.StartHost();
                    Debug.Log("<color=cyan>[Network]</color> Host başlatıldı. Hem sunucu hem oyuncuyuz.");
                }

                if (GUILayout.Button("Start CLIENT (Join Game)", GUILayout.Height(40)))
                {
                    NetworkManager.Singleton.StartClient();
                    Debug.Log("<color=green>[Network]</color> Sunucuya bağlanılıyor...");
                }
            }
            else
            {
                string networkMode = NetworkManager.Singleton.IsHost ? "Host" : "Client";
                
                GUILayout.Box($"=== NETWORK STATUS ===");
                GUILayout.Label($"Current Mode: {networkMode}");
                GUILayout.Label($"Connected Players: {NetworkManager.Singleton.ConnectedClients.Count}");
            }

            GUILayout.EndArea();
        }
    }
}