using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Excerpt from the Enemy Behavior controller managing context-driven movement and path execution.
/// </summary>
public class EnemyMovementExcerpt : MonoBehaviour
{
    // Mock declarations representing the expected class state fields
    [SerializeField] private float enemyMoveSpeed = 5f;
    [SerializeField] private float enemyStopRange = 2f;
    [SerializeField] private float chargeRange = 5f;
    [SerializeField] private float midRange = 8f;
    [SerializeField] private bool isAware = true;
    
    private Rigidbody _enemyRigidBody;
    private Animator _enemyAnimator;
    private MonoBehaviour _playerController; // Replace with your exact PlayerController class type

    private bool _meleeTriggered;
    private bool _chargeTriggered;
    private bool _midRangeTriggered;

    private void Awake()
    {
        _enemyRigidBody = GetComponent<Rigidbody>();
        _enemyAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Processes target positioning data, calls path generation systems, and updates physics actuators.
    /// </summary>
    /// <param name="distance">The direct spatial distance evaluation to the target player entity.</param>
    private void HandlePursuit(float distance)
    {
        if (!_playerController) return;

        // --- 1. Target Vector Evaluation & Intercept Math ---
        Vector3 playerPos = _playerController.transform.position;
        Rigidbody playerRb = _playerController.GetComponent<Rigidbody>();
        Vector3 predictedPos = playerPos;

        if (playerRb != null)
        {
            // Intercept time estimate framework calculated using relative distances
            float travelTime = distance / enemyMoveSpeed;
            // Vector3 playerVelocity = playerRb.velocity;
            // predictedPos = playerPos + playerVelocity * travelTime;
        }

        // Project coordinate math exclusively onto the baseline horizontal grid plane
        predictedPos.y = transform.position.y;

        // --- 2. Grid Metric Mapping Extraction ---
        Vector2Int enemyCell = GridManager.Instance.GetCellCoordinatesAtWorldPos(transform.position);
        Vector2Int playerCell = GridManager.Instance.GetCellCoordinatesAtWorldPos(predictedPos);

        // --- 3. Dynamic Algorithm Execution Path Queries ---
        List<Vector2Int> path = Pathfinder.Instance.FindPath(enemyCell, playerCell);
        Vector3 targetPos = predictedPos; 

        // Extract localized waypoint nodes if valid grid path strings are returned
        if (path != null && path.Count > 1)
        {
            Vector2Int nextCell = path[1];
            targetPos = GridManager.Instance.GetWorldPosAtCell(nextCell);
            targetPos.y = transform.position.y;
        }

        // --- 4. Rotation Vector Interpolation ---
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.fixedDeltaTime * 0.8f // Consider refactoring into an accessible serialized variable
            );
        }

        // --- 5. State Processing Matrix & Actuator Updates ---
        bool shouldMove = false;

        // State A: Melee Threshold Verification
        if (isAware || distance <= enemyStopRange) 
        {
            _enemyRigidBody.MovePosition(transform.position);

            if (!_enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("canMelee"))
            {
                _enemyAnimator.SetTriggerOnce("canMelee", ref _meleeTriggered);
            }
            _enemyAnimator.SetBool("isAttacking", true);
        }

        // State B: Charge Behavior Threshold
        else if (distance > enemyStopRange && distance <= chargeRange)
        {
            _enemyRigidBody.MovePosition(transform.position);

            if (!_enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("canCharge"))
            {
                _enemyAnimator.SetTriggerOnce("canCharge", ref _chargeTriggered);
                StartCoroutine(PerformCharge()); 
            }
            _enemyAnimator.SetBool("isAttacking", true);
        }

        // State C: Mid-Range Utility Action Threshold
        else if (distance > chargeRange && distance <= midRange)
        {
            _enemyRigidBody.MovePosition(transform.position);

            if (!_enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("canMidRange"))
            {
                _enemyAnimator.SetTriggerOnce("canMidRange", ref _midRangeTriggered);
            }
            _enemyAnimator.SetBool("isAttacking", true);
        }

        // State D: Path Tracking and Physical Movement Routing
        else
        {
            if (path != null && path.Count > 1)
            {
                Vector3 moveDir = (targetPos - transform.position).normalized;
                _enemyRigidBody.MovePosition(transform.position + moveDir * enemyMoveSpeed * Time.fixedDeltaTime);
                shouldMove = true;
            }

            // Flag system cleanup routine to clear execution frames
            AnimatorExtensions.ResetTriggerFlag(ref _meleeTriggered);
            AnimatorExtensions.ResetTriggerFlag(ref _chargeTriggered);
            AnimatorExtensions.ResetTriggerFlag(ref _midRangeTriggered);
            _enemyAnimator.SetBool("isAttacking", false);
        }

        // --- 6. Blend Tree State Feedback Loops ---
        _enemyAnimator.SetBool("isMoving", shouldMove);
    }

    private IEnumerator PerformCharge()
    {
        yield return null;
    }
}

// Dummy classes to represent custom extensions present in your framework codebase
public static class AnimatorExtensions
{
    public static void SetTriggerOnce(this Animator anim, string name, ref bool triggerFlag) { }
    public static void ResetTriggerFlag(ref bool triggerFlag) { }
}