using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to enable ability to dash
public class MovementDash : MonoBehaviour
{
    public bool canDash = true;
    private Rigidbody2D _rb = null;
    private StatManager _statManager = null;

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
        _statManager = GetComponentInParent<StatManager>();
        if (_statManager == null)
            Debug.Log("No Statmanger found for " + this.gameObject.name);
    }

    private void setUntargetable()
    {
        if (this.gameObject.tag == "Player") // TODO: change this to a better solution; should enemies also be untargetable?
        {
            StatManager playerStatManager = GetComponent<StatManager>();
            if (playerStatManager != null)
            {
                playerStatManager.IsTargetable = !playerStatManager.IsTargetable;
            }
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
        _rb.velocity = direction.normalized * _statManager.DashingPower;
        yield return new WaitForSeconds(_statManager.DashingTime);
        _rb.gravityScale = originalGravity;
        setUntargetable();
        isDashing = false;
        yield return new WaitForSeconds(_statManager.DashingCooldown);
        canDash = true;
    }
}
