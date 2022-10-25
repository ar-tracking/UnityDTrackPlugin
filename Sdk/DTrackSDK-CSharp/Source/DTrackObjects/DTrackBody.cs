/* DTrackSDK in C#: DTrackBody.cs
 *
 * Data object containing DTRACK output data of one 6DOF object,
 * (e.g. of a Standard Body).
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

using DTrackSDK.Interfaces;
using DTrackSDK.Util;

namespace DTrackSDK
{


public class DTrackBody : DTrackObject, IDTrackLocation, IDTrackRotation
{
	public float[] Loc  { get; }
	public float[,] Rot  { get; }
	public float[] Quaternion  => Convert.Rot2Quat( this.Rot );

	public DTrackBody( int id, float quality, float sx, float sy, float sz,
	                   float r0, float r1, float r2, float r3, float r4, float r5, float r6, float r7, float r8 )
			: base( id, quality )
	{
		this.Loc = new float[ 3 ];
		this.Loc[ 0 ] = sx;
		this.Loc[ 1 ] = sy;
		this.Loc[ 2 ] = sz;

		this.Rot = new float[ 3, 3 ];
		this.Rot[ 0, 0 ] = r0;
		this.Rot[ 1, 0 ] = r1;
		this.Rot[ 2, 0 ] = r2;
		this.Rot[ 0, 1 ] = r3;
		this.Rot[ 1, 1 ] = r4;
		this.Rot[ 2, 1 ] = r5;
		this.Rot[ 0, 2 ] = r6;
		this.Rot[ 1, 2 ] = r7;
		this.Rot[ 2, 2 ] = r8;
	}
}


}  // namespace DTrackSDK

