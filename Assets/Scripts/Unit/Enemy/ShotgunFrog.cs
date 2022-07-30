using UnityEngine;

public class ShotgunFrog : UnitShooter
{
    //Declare public float to edit in unity editor
    public float maxSpread;
    public int spreadAmount;
    public float pelletFireVel;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            ShootInSpread();
    }

    void ShootInSpread()
    {
        GameObject newBullet;
        for (int i = 0; i < spreadAmount; i++)
        {
            newBullet = InstantiateBullet(Target, bullet);
            //Add randomness to every bullet direction
            Vector3 dir = transform.forward + new Vector3(Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread));
            newBullet.GetComponent<Rigidbody2D>().AddForce(dir * pelletFireVel);
            newBullet.transform.position = bulletPosition.position;
        }
    }

    protected override void Shoot(GameObject target)
    {
        //base.Shoot(target);
        ShootInSpread();
    }
}
