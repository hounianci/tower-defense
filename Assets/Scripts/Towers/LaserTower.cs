using UnityEngine;

public class LaserTower : Tower {

	[SerializeField, Range(1f, 100f)]
	float damagePerSecond = 10f;

	[SerializeField]
	Transform turret = default, laserBeam = default;

	Vector3 laserBeamScale;

    protected override void Init0()
    {
		base.Init0();
		laserBeamScale = laserBeam.localScale;
    }

	public override TowerType TowerType => TowerType.Laser;

	public override void GameUpdate () {
		if (TrackTarget() || AcquireTarget()) {
			Shoot();
		}
		else {
			laserBeam.localScale = Vector3.zero;
		}
	}

	void Shoot () {
		if(targets==null||targets.Count==0){
			return;
		}
		TargetAble target = targets[0];
		Vector3 point = target.GetPosition();
		turret.LookAt(point);
		laserBeam.localRotation = turret.localRotation;

		float d = Vector3.Distance(turret.position, point);
		laserBeamScale.z = d;
		laserBeam.localScale = laserBeamScale;
		laserBeam.localPosition =
			turret.localPosition + 0.5f * d * laserBeam.forward;

		target.ApplyDamage(CurrentSkill.Damage);
		changeSkill();
	}

    protected override void loseTarget()
    {
		laserBeam.gameObject.SetActive(false);
    }
    protected override void lockTarget()
    {
		laserBeam.gameObject.SetActive(true);
    }
}