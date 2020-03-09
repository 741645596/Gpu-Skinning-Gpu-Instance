using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class IAnimation: MonoBehaviour
{
	public float Fps;

	public IGGAniClip[] ListAniClip;

	public bool IsRepeatPlay { get; protected set; }

	public bool IsPlaying { get; /*protected*/ set; }

	public int CurrentFrame { get; protected set; }

	// This value is multiplied to the regular frame interval.
	// 1 is regular speed. > 1 is slower, < 1 faster
	public float SpeedFactor { get; set; }

	// this should always be 1 when not playing a walk cycle
	protected float animationSpecificSpeedFactor = 1f;

	protected float frameInterval;

	protected float nextUpdate;

	private bool wasUpdateSincePlay = false;

	// last frame displayed was supposed to be at this time. next animation frame should be calculated from this value
	protected float lastUsedAnimationTime;

	protected int startFrame;

	protected int endFrame;


	private IGGAniClip m_Clip;


	protected void Awake() {
		SpeedFactor = 1;
	}


	void Start () {
	}



	public void Play(string pAnimName, bool pRepeat = true) {
		if (IsPlaying ) {
			Stop();
		}
		IsRepeatPlay = pRepeat;

		if (GetClipData(pAnimName) == true) {
			startFrame = m_Clip.StartFrame;
			endFrame = m_Clip.EndFrame;
			animationSpecificSpeedFactor = 1.0f;

			frameInterval = 1.0f / Fps;
			nextUpdate = /*Time.time + */frameInterval * SpeedFactor * animationSpecificSpeedFactor;
			CurrentFrame = startFrame;
			wasUpdateSincePlay = false;

			IsPlaying = true;
		}
	}



	public virtual void Stop() {
		IsPlaying = false;
	}


	/// </summary>
	private void UpdateMonoBehaviour(IGG.Util.ITime pTime) {
		if((!IsPlaying && wasUpdateSincePlay) || SpeedFactor == 0) {
			return;
		}
			
		if (!wasUpdateSincePlay) {
			// considering that it was played in the previous frame.
			nextUpdate += pTime.FixedTime - pTime.FixedDeltaTime;

			wasUpdateSincePlay = true;
		}
			
		DoAnim (CurrentFrame);

		float currentTime = pTime.FixedTime;
		if (currentTime > nextUpdate) {
			float modifiedSingleFrameTime = (frameInterval * SpeedFactor * animationSpecificSpeedFactor);
			int frameIncrement = (int)((currentTime - lastUsedAnimationTime) / modifiedSingleFrameTime);

			if (frameIncrement > 0) {
				CurrentFrame += frameIncrement;

				float overshoot = currentTime - nextUpdate;
				// calculate exactly when the next update should be
				nextUpdate = lastUsedAnimationTime + (frameIncrement + 1) * modifiedSingleFrameTime;

				lastUsedAnimationTime = currentTime - overshoot;
			}
		}

		if (CurrentFrame > endFrame) {
			if (IsRepeatPlay) {
				CurrentFrame = startFrame;
			} else {
				CurrentFrame = endFrame;
				Stop();

			}
		}
	}


	private bool GetClipData(string pAnimName)
	{
		if (ListAniClip == null)
			return false;
		
		for (int i = 0; i < ListAniClip.Length; i++) 
		{
			if (string.Compare (ListAniClip [i].clipName, pAnimName) == 0) {
				m_Clip = ListAniClip [i];
				return true;
			}
		}
		return false;
	}



	public virtual void DoAnim(int Frame)
	{
		//Mesh[] meshArray = meshAni.GetAniMeshData(CurrentFrame, AniCtrl.clipName);
		//UpdateAniMesh(meshArray);
	}
		
	public virtual void Update () {
		UpdateMonoBehaviour(IGG.Util.Time.Default);
	}
}

[Serializable]
public struct IGGAniClip {
	public string clipName;
	public int StartFrame;           // 动画开始帧
	public int EndFrame;             // 动画结束帧
}