using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRed : EnemyBase
{
    private bool _linkToFlagship = false;

    public void LaunchAttack(EnumDirection direction) 
    {
        base.BeginAttack(direction);
    }

    public void AttackDirection(EnumDirection direction)
    {
        _attackDirection = direction;
    }

    protected override void ChangeAttackDirection()
    {
        if (_linkToFlagship == false)
        {
            _attackDirection = 1 - _attackDirection;  // cambia la direccion de izquierda (0) a derecha (1)
        }
    }
                       
    protected override void BeginAttack(EnumDirection direction)
    {
        if (!_linkToFlagship)
        {
            base.BeginAttack(direction);
        }
    }

    public void LinkFlagship()
    {
        _linkToFlagship = true;
    }

    public void UnlinkFlagship()
    {
        _linkToFlagship = false;
    }
}
