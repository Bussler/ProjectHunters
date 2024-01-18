using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDash : MonoBehaviour
{
    public bool canDash = true;
    public bool isDashing;
    private Rigidbody2D _rb;

    [SerializeField]
    private float _dashingPower = 24f;

    [SerializeField]
    private float _dashingTime = 0.2f;

    [SerializeField]
    private float _dashingCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void setUntargetable()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        if (playerStatManager != null)
        {
            playerStatManager.IsTargetable = !playerStatManager.IsTargetable;
        }
    }

    public IEnumerator Dash(Vector2 direction)
    {
        if(!canDash || _rb == null)
        {
            yield break;
        }

        canDash = false;
        isDashing = true;
        setUntargetable();

        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        _rb.velocity = direction.normalized * _dashingPower;
        yield return new WaitForSeconds(_dashingTime);
        _rb.gravityScale = originalGravity;
        setUntargetable();
        isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        canDash = true;
    }
}
