/* Unity DTrack Plugin: DTrackReceiverHand.cs
 *
 * Script providing DTRACK Fingertracking data to a hand mapper script.
 *
 * Copyright (c) 2021-2022 Advanced Realtime Tracking GmbH & Co. KG
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. Neither the name of copyright holder nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using UnityEngine;

using DTrackSDK;
using DTrack;
using DTrack.Util;

namespace DTrack
{


public class DTrackReceiverHand : DTrackReceiver
{
	[ Tooltip( "Hand ID as seen in DTRACK" ) ]
	public int handId;

	// Class containing absolute poses of all joints/phalanges.

	public class Finger
	{
		public Vector3 rootPosition;
		public Quaternion rootRotation;

		public Vector3 middlePosition;
		public Quaternion middleRotation;

		public Vector3 outerPosition;
		public Quaternion outerRotation;

		public Vector3 tipPosition;
	}

	// Class containing absolute pose of wrist and all available fingers.

	public class Hand
	{
		public Vector3 wristPosition;
		public Quaternion wristRotation;

		public DTrackReceiverHand.Finger[] fingers;
	}

#if UNITY_EDITOR
	private int numberOfFingers;  // number of fingers (just for debugging)
	private char handedness = '?';  // handedness (left or right) (just for debugging)
#endif

	// to convert coordinate systems
	private Quaternion rotUnityToArtHand;
	private Quaternion rotUnityToArtFingerLeft;
	private Quaternion rotUnityToArtFingerRight;

	private float indexLength;


	// Convert DTRACK Fingertracking data into Unity world.

	private DTrackReceiverHand.Hand ConvertHand( DTrackSDK.DTrackHand dtHand )
	{
		Hand hand = new Hand();
		hand.fingers = new Finger[ dtHand.NumFingers ];

		// consider differences between left and right hand
		float facArtToUnityAngle;
		Quaternion rotUnityToArtFinger;

		if ( dtHand.IsLeft )  // left hand
		{
			rotUnityToArtFinger = this.rotUnityToArtFingerLeft;
			facArtToUnityAngle = 1.0f;
		}
		else  // right hand
		{
			rotUnityToArtFinger = this.rotUnityToArtFingerRight;
			facArtToUnityAngle = -1.0f;
		}

		Quaternion dtHandRotation0 = Converter.RotationToUnity( dtHand.Quaternion );
		Quaternion dtHandRotation = dtHandRotation0 * this.rotUnityToArtHand;
		Vector3 dtHandPosition = Converter.PositionToUnity( dtHand.Loc );

		Vector3 locart, dlocart;
		Quaternion rotart, drotartInv;

		// calculate pose of phalanges:

		for ( int i = 0; i < dtHand.NumFingers; i++ )
		{
			hand.fingers[ i ] = new Finger();

			DTrackSDK.DTrackFinger dtFinger = dtHand.Fingers[ i ];
			DTrackReceiverHand.Finger finger = hand.fingers[ i ];

			// finger tip:

			locart = dtHandPosition + dtHandRotation0 * Converter.PositionToUnity( dtFinger.Loc );

			finger.tipPosition = locart;

			// outer phalanx:

			dlocart = new Vector3( 0.0f, dtFinger.LengthPhalanxOuter * Converter.MM_TO_M, 0.0f );

			rotart = dtHandRotation0 * Converter.RotationToUnity( dtFinger.Quaternion ) * rotUnityToArtFinger;
			locart -= rotart * dlocart;

			finger.outerPosition = locart;
			finger.outerRotation = rotart;

			// middle phalanx:

			dlocart = new Vector3( 0.0f, dtFinger.LengthPhalanxMiddle * Converter.MM_TO_M, 0.0f );
			drotartInv = Quaternion.AngleAxis( facArtToUnityAngle * dtFinger.AngleOuterMiddle, Vector3.back );

			rotart *= drotartInv;
			locart -= rotart * dlocart;

			finger.middlePosition = locart;
			finger.middleRotation = rotart;

			// inner (root) phalanx:

			dlocart = new Vector3( 0.0f, dtFinger.LengthPhalanxInner * Converter.MM_TO_M, 0.0f );
			drotartInv = Quaternion.AngleAxis( facArtToUnityAngle * dtFinger.AngleMiddleInner, Vector3.back );

			rotart *= drotartInv;
			locart -= rotart * dlocart;

			finger.rootPosition = locart;
			finger.rootRotation = rotart;
		}

		// estimation of wrist:

		if ( dtHand.NumFingers >= 3 )
		{
			DTrackSDK.DTrackFinger dtFinger = dtHand.Fingers[ DTrackSDK.FingerIndex.INDEX ];

			float t = dtFinger.LengthPhalanxInner + dtFinger.LengthPhalanxMiddle + dtFinger.LengthPhalanxOuter;
			if ( this.indexLength > 0.0f )
			{
				this.indexLength += ( t - this.indexLength ) / 1000;
			}
			else
			{
				this.indexLength = t;
			}

			hand.wristRotation = dtHandRotation;
			hand.wristPosition = hand.fingers[ DTrackSDK.FingerIndex.MIDDLE ].rootPosition -
			                     dtHandRotation * new Vector3( 0.0f, this.indexLength * Converter.MM_TO_M, 0.0f );
		}
		else
		{
			hand.wristRotation = dtHandRotation;
			hand.wristPosition = dtHandPosition + dtHandRotation * new Vector3( 0.0f, -0.100f, 0.025f );
		}

		return hand;
	}


	void Start()
	{
		this.rotUnityToArtHand = Quaternion.Inverse( Converter.RotationArtToUnityHand );
		this.rotUnityToArtFingerLeft = this.rotUnityToArtHand;
		this.rotUnityToArtFingerRight = new Quaternion( 1.0f, 0.0f, 0.0f, 0.0f ) * this.rotUnityToArtHand;

		this.indexLength = 0.0f;
	}

	void OnEnable()
	{
		this.Register();
	}

	void OnDisable()
	{
		this.Unregister();
	}


	// Get hand and finger data with most recent DTRACK Fingertracking data.

	public DTrackReceiverHand.Hand GetHand()
	{
		DTrackSDK.Frame frame = GetDTrackFrame();  // ensures data integrity against DTrack class
		if ( frame == null )  return null;  // no new tracking data
		if ( frame.Hands == null )  return null;

		try
		{
			DTrackSDK.DTrackHand dtHand;

			if ( frame.Hands.TryGetValue( this.handId - 1, out dtHand ) )
			{
				if ( dtHand.IsTracked )
				{
					Hand hand = ConvertHand( dtHand );

#if UNITY_EDITOR
					this.numberOfFingers = dtHand.NumFingers;
					this.handedness = dtHand.IsLeft ? 'L' : 'R';
#endif

					return hand;
				}
			}
		}
		catch ( Exception e )
		{
			Debug.Log( $"Error while moving object: {e}" );
		}

		return null;
	}
}


}  // namespace DTrack

