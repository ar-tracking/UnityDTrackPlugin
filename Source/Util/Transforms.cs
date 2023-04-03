/* ART DTRACK Plugin for Unity Game Engine: Transform.cs
 *
 * Helper routines to convert from ART to Unity world.
 *
 * Copyright (c) 2019-2023 Advanced Realtime Tracking GmbH & Co. KG
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

using UnityEngine;

namespace DTrack.Util
{


public class Converter
{
	// Use default coordinate system conversion; old conversion otherwise
	private static bool isDefault = true;

	public static readonly float MM_TO_M = 0.001f;


	// Set coordinate system conversion.

	public static void SetDefaultCoordinates()  // default conversion (Z <-> -Z)
	{
		isDefault = true;
	}
	public static void SetOldCoordinates()  // old conversion (Y <-> Z)
	{
		isDefault = false;
	}


	// Convert ART position vector to Unity position vector.

	public static Vector3 PositionToUnity( float x, float y, float z )
	{
		// change axes as DTRACK uses right-handed world space, convert unit to 'meter'
		if ( isDefault )
		{
			return new Vector3( x * MM_TO_M, y * MM_TO_M, -z * MM_TO_M );
		}
		else
		{
			return new Vector3( x * MM_TO_M, z * MM_TO_M, y * MM_TO_M );
		}
	}


	// Convert ART position vector to Unity position vector.
	//   p = [ x, y, z ]

	public static Vector3 PositionToUnity( float[] p )
	{
		// change axes as DTRACK uses right-handed world space, convert unit to 'meter'
		if ( isDefault )
		{
			return new Vector3( p[ 0 ] * MM_TO_M, p[ 1 ] * MM_TO_M, -p[ 2 ] * MM_TO_M );
		}
		else
		{
			return new Vector3( p[ 0 ] * MM_TO_M, p[ 2 ] * MM_TO_M, p[ 1 ] * MM_TO_M );
		}
	}


	// Convert ART rotation quaternion to Unity rotation quaternion.
	//   q = [ w, x, y, z ]

	public static Quaternion RotationToUnity( float[] q )
	{
		// change axes and direction as DTRACK uses right-handed world space
		if ( isDefault )
		{
			return new Quaternion( q[ 1 ], q[ 2 ], -q[ 3 ], -q[ 0 ] );
		}
		else
		{
			return new Quaternion( q[ 1 ], q[ 3 ], q[ 2 ], -q[ 0 ] );
		}
	}


	// Rotation to convert ART hand coordinate system into 'Unity' hand coordinate system (all left-handed).

	public static Quaternion RotationArtToUnityHand =>
			( isDefault ? new Quaternion( 0.5f, 0.5f, 0.5f, 0.5f )
			            : new Quaternion( 0.0f, 0.0f, Mathf.Sqrt( 0.5f ), Mathf.Sqrt( 0.5f ) ) );
}


}  // namespace DTrack.Util

