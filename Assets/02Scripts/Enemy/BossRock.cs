using System.Collections;
using DUS;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody ridgid;
    float scaleValue = 0.3f;
    bool isShoot;

    private void Awake()
    {
        ridgid = GetComponent<Rigidbody>();
        StartCoroutine(PowerTimer());
        StartCoroutine(GainPower());
    }

    private void Update()
    {
        Timer();
    }

    private IEnumerator PowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            speed += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            ridgid.AddTorque(transform.right * speed, ForceMode.Impulse);
            yield return null;
        }
    }

}
