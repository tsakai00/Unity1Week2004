using UnityEngine;
using System.Collections;

namespace Lib.Util
{
	/// <summary>
	/// 簡単なタイマー
	/// </summary>
	public class SimpleTimer
	{
		private float _sec = 1.0f;
		private float _max = 1.0f;

		public float GetSec()		{ return _sec; }
		public float GetMax()		{ return _max; }
		public float GetInvSec()	{ return _max - _sec; }
		public float GetNrmSec()	{ return _sec * (1.0f / _max); }
		public float GetInvNrmSec()	{ return 1.0f - GetNrmSec(); }
		public bool  IsEnd()		{ return _sec >= _max; }

		public void Init(float sec)
		{
			_sec = 0.0f;
			_max = sec;
		}

		public bool Update()
		{
			return AddSec(Time.deltaTime);
		}

		public bool FixedUpdate()
		{
			return AddSec(Time.fixedDeltaTime);
		}

		private bool AddSec(float sec)
		{
			_sec += sec;
			if(IsEnd())
			{
				_sec = _max;
				return true;
			}
			return false;
		}
	}
}
