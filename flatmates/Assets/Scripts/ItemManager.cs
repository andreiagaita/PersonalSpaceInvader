using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public List<ItemSpawnpoint> AvailableItemSpawnLocations;
    public List<GameObject> ItemTemplates;

    public Dictionary<int, PickupItem> ItemDatabase = new Dictionary<int, PickupItem>();
    private int itemIndexer = 0;

    // Use this for initialization
    void Start()
    {
        // fill available item spawn locations from the level
        AvailableItemSpawnLocations = GameObject.FindObjectsOfType<ItemSpawnpoint>().ToList();

        // fill available item sprites for the level
    }

    void Update()
    {
        // this handles the rendering of all the items - show the items which are up for grabs and hide the ones already picked by player
    }

    void InitItems(List<PlayerInfo> players)
    {
        // lets create (2 x playercount) items in the level
        for (int i = 0; i < players.Count; i++)
        {
            // one item closest to the player location
            int closestIndex = GetClosestItemLocation(players[i].Position);
            if (closestIndex != -1)
            {
                int randomIndex = Random.Range(0, ItemTemplates.Count);
                GameObject go = GameObject.Instantiate(ItemTemplates[randomIndex], AvailableItemSpawnLocations[closestIndex].transform.position, Quaternion.identity) as GameObject;
                PickupItem item = go.GetComponent<PickupItem>();
                item.ID = itemIndexer;
                item.Owner = players[i].ID;
                ItemDatabase.Add(itemIndexer, item);
                itemIndexer++;
                ItemTemplates.RemoveAt(randomIndex);
                AvailableItemSpawnLocations.RemoveAt(closestIndex);
            }

            // second item farthest from the player location
            int farthestIndex = GetClosestItemLocation(players[i].Position);
            if (farthestIndex != -1)
            {
                int randomIndex = Random.Range(0, ItemTemplates.Count);
                GameObject go = GameObject.Instantiate(ItemTemplates[randomIndex], AvailableItemSpawnLocations[closestIndex].transform.position, Quaternion.identity) as GameObject;
                PickupItem item = go.GetComponent<PickupItem>();
                item.ID = itemIndexer;
                item.Owner = players[i].ID;
                ItemDatabase.Add(itemIndexer, item);
                itemIndexer++;
                ItemTemplates.RemoveAt(randomIndex);
                AvailableItemSpawnLocations.RemoveAt(farthestIndex);
            }
        }

        Dictionary<int, int> objectiveAssignments = new Dictionary<int, int>();
        players.ForEach(x => objectiveAssignments.Add(x.ID, 0));
        // lets give 2 items as objectives to each player
        foreach(PickupObject pickObject in ItemDatabase.Values)
        {
            int playerPick = GetRandomPlayer(objectiveAssignments.Where(x => x.Value < 2).ToArray());
            pickObject.ObjectiveForPlayer = playerPick;
            pickObject.ObjectiveIndex = objectiveAssignments[playerPick];
            objectiveAssignments[playerPick]++;
        }
    }

    int GetRandomPlayer(params KeyValuePair<int, int>[] playerIds)
    {
        return playerIds[Random.Range(0, playerIds.Length)].Key;
    }

    int GetClosestItemLocation(Vector3 pos)
    {
        int index = -1;
        float distanceValue = float.MaxValue;
        for (int i = 0; i < AvailableItemSpawnLocations.Count; i++)
        {
            float distance = Vector3.Distance(AvailableItemSpawnLocations[i].transform.position, pos);
            if (distance < distanceValue)
            {
                distanceValue = distance;
                index = i;
            }
        }
        return index;
    }

    int GetFarthestItemLocation(Vector3 pos)
    {
        int index = -1;
        float distanceValue = float.MinValue;
        for (int i = 0; i < AvailableItemSpawnLocations.Count; i++)
        {
            float distance = Vector3.Distance(AvailableItemSpawnLocations[i].transform.position, pos);
            if (distance >= distanceValue)
            {
                distanceValue = distance;
                index = i;
            }
        }
        return index;
    }
}
