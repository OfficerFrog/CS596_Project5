using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class NetworkManagerController : NetworkManager
    {
        [SerializeField]
        private Transform[] _playerSpawnPoints;
        [SerializeField]
        private Transform[] _enemySpawnPoints;



        //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        //{
        //    GameObject player;

        //    if (chosenClass == CharacterClasses.Ronin)
        //        player = Instantiate(Resources.Load("Ronin Prefab"), transform.position, Quaternion.identity) as GameObject;

        //    if (chosenClass == CharacterClasses.Samurai)
        //        player = Instantiate(Resources.Load("Samurai Prefab"), transform.position, Quaternion.identity) as GameObject;

        //    NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        //}

    }
}
