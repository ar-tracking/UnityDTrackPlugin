/* Unity DTrack Plugin: LMRealisticHandMapper.cs
 *
 * Hand mapper to 'Leap Motion Realistic Male/Female Hands' and simlilar models.
 *
 * Copyright (c) 2022 Advanced Realtime Tracking GmbH & Co. KG
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
 *
 * Works for (at least):
 *  - 'Leap Motion Realistic Male Hands' (by Storkplay),
 *    https://assetstore.unity.com/packages/3d/characters/humanoids/leap-motion-realistic-male-hands-109961
 *  - 'Leap Motion Realistic Female Hands' (by Storkplay),
 *    https://assetstore.unity.com/packages/3d/characters/humanoids/leap-motion-realistic-female-hands-211090
 */

using System;
using UnityEngine;

using DTrack;

namespace DTrack
{


[ RequireComponent( typeof( DTrackReceiverHand ) ) ]
public class LMRealisticHandMapper : MonoBehaviour
{
	[ Tooltip( "Wrist GameObject" ) ]
	public Transform wrist;

	[ Tooltip( "Finger GameObjects (in order: thumb, ... pinky)" ) ]
	public Transform[] fingers = new Transform[ 5 ];

	[ Tooltip( "Try automatic scaling of hand model" ) ]
	public bool automaticScale = true;

 	private DTrackReceiverHand receiverHand;  // DTrackReceiverHand object this hand mapper is associated to

	private Vector3 wristToLocalPosition;  // pose between this game object and the hand model's wrist
	private Quaternion wristToLocalRotationInverse;
	private Quaternion wristToReceiver;  // rotations between the hand model and DTrackReceiverHand 
	private Quaternion fingerToReceiver;

	private float handModelWidth;
	private bool isSetAutomaticScale = false;

	private const float shiftThumbRootToWrist = 0.1f;
	private const float shiftThumbRootToIndexRoot = 0.2f;
	private const float shiftThumbRootToThumbMiddle = 0.2f;


	void Awake()
	{
		this.receiverHand = GetComponent< DTrackReceiverHand >();

		if ( this.automaticScale && ( this.fingers.Length == 5 ) )
		{
			this.isSetAutomaticScale = false;

			Vector3 oldScale = this.transform.localScale;
			this.transform.localScale = Vector3.one;

			float hw = 0.0f;  // hand width of Unity hand model
			for ( int i = 1; i < 4; i++ )
			{
				if ( ( this.fingers[ i ] != null ) && ( this.fingers[ i + 1 ] != null ) )
				{
					hw += Vector3.Distance( this.fingers[ i ].position, this.fingers[ i + 1 ].position );
				}
				else
				{
					this.isSetAutomaticScale = true;
				}
			}
			this.handModelWidth = hw;

			this.transform.localScale = oldScale;
		}
		else
		{
			this.isSetAutomaticScale = true;
		}

		// check for extra rotations:

		Vector3 dv = Quaternion.Inverse( this.wrist.rotation ) * ( this.fingers[ 3 ].position - this.fingers[ 1 ].position );
		this.wristToReceiver = Quaternion.identity;
		if ( dv.z < 0 )  this.wristToReceiver = new Quaternion( 0.0f, 1.0f, 0.0f, 0.0f );

		dv = Quaternion.Inverse( this.fingers[ 2 ].rotation ) * ( this.fingers[ 3 ].position - this.fingers[ 1 ].position );
		this.fingerToReceiver = Quaternion.identity;
		if ( dv.z < 0 )  this.fingerToReceiver = new Quaternion( 0.0f, 1.0f, 0.0f, 0.0f );

		updateWristToLocal();
	}

	// Update pose between this game object and the hand model's wrist.

	private void updateWristToLocal()
	{
		if ( this.wrist == null )
		{
			this.wristToLocalPosition = Vector3.zero;
			this.wristToLocalRotationInverse = Quaternion.identity;
			return;
		}

		Quaternion R0T = Quaternion.Inverse( this.transform.rotation );

		this.wristToLocalPosition = R0T * ( this.wrist.position - this.transform.position );
		this.wristToLocalRotationInverse = Quaternion.Inverse( R0T * this.wrist.rotation );

		this.wristToLocalRotationInverse = this.wristToReceiver * this.wristToLocalRotationInverse;  // optimization
	}


	void Update()
	{
		DTrackReceiverHand.Hand recHand = this.receiverHand.GetHand();  // get most recent hand and finger data
		if ( recHand == null )  return;  // no new tracking data

		if ( ! this.isSetAutomaticScale )
		{
			if ( this.automaticScale && ( recHand.fingers.Length == 5 ) )
			{
				float hw = 0.0f;  // hand width of DTRACK FINGERTRACKING hand
				for ( int i = 1; i < 4; i++ )
				{
					hw += Vector3.Distance( recHand.fingers[ i ].rootPosition, recHand.fingers[ i + 1 ].rootPosition );
				}

				this.transform.localScale = Vector3.one * ( hw / this.handModelWidth );
				updateWristToLocal();
			}

			this.isSetAutomaticScale = true;
		}

		this.transform.rotation = recHand.wristRotation * this.wristToLocalRotationInverse;
		this.transform.position = recHand.wristPosition - this.transform.rotation * this.wristToLocalPosition;

		for ( int i = 0; i < recHand.fingers.Length; i++ )
		{
			if ( this.fingers.Length <= i )  break;  // break out since all mappable fingers are mapped

			Transform t = this.fingers[ i ];
			if ( t == null )  continue;  // skip in case this position of fingers is empty

			DTrackReceiverHand.Finger recFinger = recHand.fingers[ i ];

			try
			{
				if ( i == 0 )  // thumb: modify root joint
				{
					Vector3 rootPosNew = recFinger.rootPosition +
					                     ( recHand.wristPosition - recFinger.rootPosition ) * shiftThumbRootToWrist;
					rootPosNew += ( recHand.fingers[ 1 ].rootPosition - rootPosNew ) * shiftThumbRootToIndexRoot;
					rootPosNew += ( recFinger.middlePosition - rootPosNew ) * shiftThumbRootToThumbMiddle;

					Quaternion dq = Quaternion.FromToRotation( ( recFinger.middlePosition - recFinger.rootPosition ),
					                                           ( recFinger.middlePosition - rootPosNew ) );

					t.position = rootPosNew;
					t.rotation = dq * recFinger.rootRotation;

					t = t.GetChild( 0 );
					t.position = recFinger.middlePosition;
					t.rotation = recFinger.middleRotation;

					t = t.GetChild( 0 );
					t.position = recFinger.outerPosition;
					t.rotation = recFinger.outerRotation;
				}
				else  // other fingers: no modifications
				{
					t.position = recFinger.rootPosition;
					t.rotation = recFinger.rootRotation * fingerToReceiver;

					t = t.GetChild( 0 );
					t.position = recFinger.middlePosition;
					t.rotation = recFinger.middleRotation * fingerToReceiver;

					t = t.GetChild( 0 );
					t.position = recFinger.outerPosition;
					t.rotation = recFinger.outerRotation * fingerToReceiver;
				}
			}
			catch ( Exception e )  // assigned fingers aren't as deep as expected
			{
				Debug.Log( $"Error while moving finger object: {e}" );
			}
		}
	}
}


}  // namespace DTrack

