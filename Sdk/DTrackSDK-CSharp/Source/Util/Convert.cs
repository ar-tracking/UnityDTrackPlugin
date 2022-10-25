/* DTrackSDK in C#: Convert.cs
 *
 * Helper routines to convert data.
 *
 * Copyright (c) 2019-2022 Advanced Realtime Tracking GmbH & Co. KG
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

namespace DTrackSDK.Util
{


public class Convert
{

	// Convert 3x3 rotation matrix into rotation quaternion.
	//
	//       [ r00, r01, r02 ]
	//   R = [ r10, r11, r12 ]
	//       [ r20, r21, r22 ]
	//
	//   q = [ w, x, y, z ]

	public static float[] Rot2Quat( float[,] rot )
	{
		float t;
		float x, y, z, w;

		if ( rot[ 2, 2 ] < 0.0f )
		{
			if ( rot[ 0, 0 ] > rot[ 1, 1 ] )
			{
				t = 1.0f + rot[ 0, 0 ] - rot[ 1, 1 ] - rot[ 2, 2 ];
				x = t;
				y = rot[ 0, 1 ] + rot[ 1, 0 ];
				z = rot[ 2, 0 ] + rot[ 0, 2 ];
				w = rot[ 2, 1 ] - rot[ 1, 2 ];
			}
			else
			{
				t = 1.0f - rot[ 0, 0 ] + rot[ 1, 1 ] - rot[ 2, 2 ];
				x = rot[ 0, 1 ] + rot[ 1, 0 ];
				y = t;
				z = rot[ 1, 2 ] + rot[ 2, 1 ];
				w = rot[ 0, 2 ] - rot[ 2, 0 ];
			}
		}
		else
		{
			if ( rot[ 0, 0 ] < -rot[ 1, 1 ] )
			{
				t = 1.0f - rot[ 0, 0 ] - rot[ 1, 1 ] + rot[ 2, 2 ];
				x = rot[ 2, 0 ] + rot[ 0, 2 ];
				y = rot[ 1, 2 ] + rot[ 2, 1 ];
				z = t;
				w = rot[ 1, 0 ] - rot[ 0, 1 ];
			}
			else
			{
				t = 1.0f + rot[ 0, 0 ] + rot[ 1, 1 ] + rot[ 2, 2 ];
				x = rot[ 2, 1 ] - rot[ 1, 2 ];
				y = rot[ 0, 2 ] - rot[ 2, 0 ];
				z = rot[ 1, 0 ] - rot[ 0, 1 ];
				w = t;
			}
		}

		float s = 0.5f / ( float )Math.Sqrt( Math.Max( 0.0, ( double )t ) );

		float[] quat = new float[ 4 ];
		quat[ 0 ] = s * w;
		quat[ 1 ] = s * x;
		quat[ 2 ] = s * y;
		quat[ 3 ] = s * z;

		return quat;
	}
}


}  // namespace DTrackSDK.Util

