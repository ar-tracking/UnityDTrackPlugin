/* DTrackSDK in C#: DTrackHand.cs
 *
 * Data object containing DTRACK output data of one Fingertracking hand.
 *
 * Copyright (c) 2020-2022 Advanced Realtime Tracking GmbH & Co. KG
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

namespace DTrackSDK
{


public class DTrackFinger : DTrackBody
{
	public float RadiusTip  { get; }
	public float LengthPhalanxOuter  { get; }
	public float AngleOuterMiddle  { get; }
	public float LengthPhalanxMiddle  { get; }
	public float AngleMiddleInner  { get; }
	public float LengthPhalanxInner  { get; }

	public DTrackFinger( int id, float quality, float sx, float sy, float sz,
	                     float r0, float r1, float r2, float r3, float r4, float r5, float r6, float r7, float r8,
	                     float radiusTip, float lengthOuter, float angleOuterMiddle,
	                     float lengthMiddle, float angleMiddleInner, float lengthInner )
			: base( id, quality, sx, sy, sz, r0, r1, r2 ,r3, r4, r5, r6, r7, r8 )
	{
		this.RadiusTip = radiusTip;
		this.LengthPhalanxOuter = lengthOuter;
		this.AngleOuterMiddle = angleOuterMiddle;
		this.LengthPhalanxMiddle = lengthMiddle;
		this.AngleMiddleInner = angleMiddleInner;
		this.LengthPhalanxInner = lengthInner;
	}
}


public class DTrackHand : DTrackBody
{
	public int NumFingers => Fingers.Length;

	// Left (0) or right (1) hand
	public int LeftRight  { get; }
	public bool IsLeft => ( LeftRight == 0 );
	public bool IsRight => ( LeftRight != 0 );

	public DTrackFinger[] Fingers  { get; }

	public DTrackHand( int id, float quality, int leftRight, float sx, float sy, float sz,
	                   float r0, float r1, float r2, float r3, float r4, float r5, float r6, float r7, float r8,
	                   DTrackFinger[] fingers )
			: base( id, quality, sx, sy, sz, r0, r1, r2 ,r3, r4, r5, r6, r7, r8 )
	{
		this.LeftRight = leftRight;

		this.Fingers = fingers;
		if ( this.Fingers == null )
			this.Fingers = new DTrackFinger[ 0 ];
	}
}


}  // namespace DTrackSDK

