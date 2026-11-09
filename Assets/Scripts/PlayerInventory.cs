using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    readonly Dictionary<string, int> keys = new Dictionary<string, int>();

    public bool AddKey(string keyId)
    {
        if (string.IsNullOrWhiteSpace(keyId))
            return false;

        keys.TryGetValue(keyId, out int count);
        keys[keyId] = count + 1;
        return true;
    }

    public int GetKeyCount(string keyId)
    {
        return !string.IsNullOrWhiteSpace(keyId) && keys.TryGetValue(keyId, out int count)
            ? count : 0;
    }

    public bool TryConsumeKey(string keyId)
    {
        int count = GetKeyCount(keyId);
        if (count == 0)
            return false;

        if (count == 1)
            keys.Remove(keyId);
        else
            keys[keyId] = count - 1;

        return true;
    }
}
