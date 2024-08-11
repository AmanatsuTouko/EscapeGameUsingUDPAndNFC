using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UUIDCardIDDictScriptableObject", order = 2)]
public class UUIDToCardIDScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<UUIDCardID> UuidCard;

    public CardID GetCardIDFromUUID(string uuid)
    {
        // 自身の持っているListから検索してCardIDを返す
        return CardID.ID01;
    }
}

[System.Serializable]
public class UUIDCardID
{
    public string Uuid;
    public CardID cardID;
}