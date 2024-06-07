using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageableEntity {
    public void Hurt(int damage);
    public void Die();
}
