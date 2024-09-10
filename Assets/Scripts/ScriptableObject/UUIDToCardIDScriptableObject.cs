using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UUIDCardIDDictScriptableObject", order = 1)]
public class UUIDToCardIDScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<UUIDCardID> UuidCard;

    // null許容型を返す
    public CardID? GetCardIDFromUUID(string uuid)
    {
        // 自身の持っているListから検索してCardIDを返す
        if (UuidCard.Exists(x => x.Uuid == uuid))
        {
            return UuidCard.Find(x => x.Uuid == uuid).CardID;
        }
        else
        {
            Debug.LogError($"エラー:登録されていないUUID{uuid}をCardIDに変換できません．");
            return null;
        }
    }
}

[System.Serializable]
public class UUIDCardID : IEquatable<UUIDCardID>
{
    public CardID CardID;
    public CardType CardType;
    public string Uuid;

    public override string ToString()
    {
        return $"UUID:{Uuid}, CardID:{CardID}";
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        UUIDCardID objAsPart = obj as UUIDCardID;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public override int GetHashCode()
    {
        return Uuid.GetHashCode();
    }

    public bool Equals(UUIDCardID other)
    {
        if (other == null) return false;
        return this.Uuid.Equals(other.Uuid) && this.CardID.Equals(other.CardID);
    }
}

public enum CardType
{
    Question,
    Hint,
}