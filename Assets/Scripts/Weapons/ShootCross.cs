using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot in a cross pattern
public class ShootCross : BulletWeapon
{
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    protected override void Shoot()
    {
        // shoot in 4 directions in a cross pattern
        ShootAt(Vector2.up);
        ShootAt(Vector2.down);
        ShootAt(Vector2.left);
        ShootAt(Vector2.right);
    }
}
