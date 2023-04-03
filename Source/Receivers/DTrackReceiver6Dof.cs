/* ART DTRACK Plugin for Unity Game Engine: DTrackReceiver6Dof.cs
 *
 * Script providing DTRACK Standard Body data to a GameObject.
 *
 * Copyright (c) 2020-2023 Advanced Realtime Tracking GmbH & Co. KG
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


public class DTrackReceiver6Dof : DTrackReceiver
{
	[ Tooltip( "Standard Body ID as seen in DTRACK" ) ]
	public int bodyId;

	[ Tooltip( "Update position of this GameObject" ) ]
	public bool applyPosition = true;
	[ Tooltip( "Update rotation of this GameObject" ) ]
	public bool applyRotation = true;


	void OnEnable()
	{
		this.Register();
	}

	void OnDisable()
	{
		this.Unregister();
	}


	void Update()
	{
		DTrackSDK.Frame frame = GetDTrackFrame();  // ensures data integrity against DTrack class
		if ( frame == null )  return;  // no new tracking data
		if ( frame.Bodies == null )  return;

		try
		{
			DTrackSDK.DTrackBody dtBody;

			if ( frame.Bodies.TryGetValue( this.bodyId - 1, out dtBody ) )
			{
				if ( dtBody.IsTracked )
				{
					if ( this.applyPosition )
					{
						this.transform.position = Converter.PositionToUnity( dtBody.Loc );
					}

					if ( this.applyRotation )
					{
						this.transform.rotation = Converter.RotationToUnity( dtBody.Quaternion );
					}
				}
			}
		}
		catch ( Exception e )
		{
			Debug.Log( $"Error while moving object: {e}" );
		}
	}
}


}  // namespace DTrack

