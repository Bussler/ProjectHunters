using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to enable ability to dash
public class MovementDash : MonoBehaviour
{
    public bool canDash = true;
    private Rigidbody2D _rb;

    [SerializeField]
    private float _dashingPower = 24f;

    [SerializeField]
    private float _dashingTime = 0.2f;

    [SerializeField]
    private float _dashingCooldown = 1f;

    private bool _isDashing;
    public delegate void IsDashingChangedHandler(bool _isDashing);
    public event IsDashingChangedHandler OnDashingChanged;
    public bool isDashing
    {
        get { return _isDashing; }
        set
        {
            if (_isDashing != value)
            {
                _isDashing = value;
                OnDashingChanged?.Invoke(_isDashing);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void setUntargetable()
    {
        StatManager playerStatManager = GetComponent<StatManager>();
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
