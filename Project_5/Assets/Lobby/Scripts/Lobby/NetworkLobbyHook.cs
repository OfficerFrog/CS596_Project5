using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Lobby.Scripts.Lobby
{
    public class NetworkLobbyHook : LobbyHook
    {
        public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayerObject, GameObject gamePlayerObject)
        {
            LobbyPlayer lobbyPlayer = lobbyPlayerObject.GetComponent<LobbyPlayer>();
            PlayerController player = gamePlayerObject.GetComponent<PlayerController>();

            player.PlayerColor = lobbyPlayer.playerColor;
            player.PlayerName = lobbyPlayer.playerName;
        }
    }
}
