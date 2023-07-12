using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlagship : EnemyBase
{
    [Header("Guard")]
    public EnemyRed[] Guards;

    protected override void Start()
    {
        base.Start();
        foreach (var item in Guards)
        {
            item.LinkFlagship();
        }
    }

    protected override void BeginAttack(EnumDirection direction)
    {
        base.BeginAttack(direction);
        foreach (var item in Guards)
        {
            item.LaunchAttack(direction);
        }
    }

    protected override void ChangeAttackDirection()
    {
        base.ChangeAttackDirection();
        foreach (var item in Guards)
        {
            item.AttackDirection(_attackDirection);
        }
    }

    private void OnDestroy()
    {
        foreach (var item in Guards)
        {
            item.UnlinkFlagship();
        }
        GameManager.Formation.DelShip(row, col);
    }
}
