/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

public class Swing : AbsCustomAniCommand
{
	
	float halfAngle;
	float totAngleToMoveInOneSec;
	float dirVal;


	float changeFactor;
	float tentativeVal;

	float localAngleVal;


	void Update()
	{
		changeFactor = dirVal * (totAngleToMoveInOneSec * UnityEngine.Time.deltaTime);
		tentativeVal = localAngleVal + changeFactor;

		if((dirVal == 1)&&(tentativeVal >= halfAngle))
		{
			UnityEngine.Vector3 tmpV = transform.localEulerAngles;
			tmpV.z = halfAngle;
			transform.localEulerAngles = tmpV;
			localAngleVal = halfAngle;
			dirVal *= -1;
		}
		else if((dirVal == -1)&&(tentativeVal <= (-halfAngle)))
		{
			UnityEngine.Vector3 tmpV = transform.localEulerAngles;
			tmpV.z = (-halfAngle);
			transform.localEulerAngles = tmpV;
			localAngleVal = -halfAngle;
			dirVal *= -1;
		}
		else
		{
			UnityEngine.Vector3 tmpV = transform.localEulerAngles;
			tmpV.z = tentativeVal;
			transform.localEulerAngles = tmpV;
			localAngleVal = tentativeVal;
		}
	}

	public void init(float para_swingTotalAngle, float para_frequency)
	{
		halfAngle = para_swingTotalAngle/2f;
		totAngleToMoveInOneSec = (para_swingTotalAngle * 2f) * para_frequency;
		dirVal = 1;
		localAngleVal = transform.localEulerAngles.z;
	}

	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float p_swingTotalAngle = (float) para_prep.parameters[0];
		float p_frequency = (float) para_prep.parameters[1];
		this.init(p_swingTotalAngle,p_frequency);
		return true;
	}
}
