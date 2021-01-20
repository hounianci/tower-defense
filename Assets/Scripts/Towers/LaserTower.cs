using UnityEngine;

public class LaserTower : Tower {
	[SerializeField]
	Transform turret = default, laserBeam = default;

	Vector3 laserBeamScale;

    protected override void Init0(object[] payloads)
    {
		base.Init0(payloads);
		laserBeamScale = laserBeam.localScale;
    }

	public override TowerType TowerType => TowerType.Laser;



	protected override void Shoot0 () {
		TargetAble target = targets[0];
		Vector3 point = target.GetPosition();
		turret.LookAt(point);
		laserBeam.localRotation = turret.localRotation;
		float d = Vector3.Distance(turret.position, point);
		laserBeamScale.z = d;
		laserBeam.localScale = laserBeamScale;
		laserBeam.localPosition =
			turret.localPosition + 0.5f * d * laserBeam.forward;
	}

    public override void loseTarget()
    {
		laserBeam.localScale = Vector3.zero;
		laserBeam.gameObject.SetActive(false);
    }
    protected override void lockTarget()
    {
		laserBeam.gameObject.SetActive(true);
    }
}