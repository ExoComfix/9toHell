using UnityEngine;
public static class EnemyPlayerTarget
{
    private static Transform cachedPlayerTransform;

    public static Transform Transform
    {
        get
        {
            if (cachedPlayerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) cachedPlayerTransform = player.transform;
            }
            return cachedPlayerTransform;
        }
    }
}
