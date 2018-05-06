using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    // respawns a non-active prefab after it has been removed (e.g. picked up)
    public class ItemRespawner : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _prefabItem;

        [HideInInspector]
        private Transform _spawnLocation;

        [HideInInspector]
        private float _spawnTime = 1f;

        private void Start()
        {
            _spawnLocation = this.GetComponent<Transform>();
            SpawnItem();
        }

        private void SpawnItem()
        {
            GameObject item = Instantiate(
                _prefabItem,
                _spawnLocation.position,
                _spawnLocation.rotation);

            var pickupItemScript = item.GetComponent<PickupItemBase>();
            _spawnTime = pickupItemScript.RespawnTime;
            pickupItemScript.ItemWasPickedUp -= ItemWasPickedUp;
            pickupItemScript.ItemWasPickedUp += ItemWasPickedUp;
        }

        private void ItemWasPickedUp(object sender, EventArgs args)
        {
            Invoke("SpawnItem", _spawnTime);
        }
    }
}
