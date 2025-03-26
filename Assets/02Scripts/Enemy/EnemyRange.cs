using System.Collections;
using DUS;
using UnityEngine;

public class EnemyRange : Enemy
{
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform bulletPos;

    protected override IEnumerator Attack()
    {
        yield return StartCoroutine(base.Attack());
        GameObject bullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        yield return new WaitForSeconds(attackDelay);

        StartCoroutine(OutAttack());
    }
}
