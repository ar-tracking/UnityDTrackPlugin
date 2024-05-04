/* DTrackSDK in C#: Frame.cs
 * 
 * Data object containing DTRACK output data of one frame.
 *
 * Copyright (c) 2019-2024 Advanced Realtime Tracking GmbH & Co. KG
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

using System.Collections.Generic;

namespace DTrackSDK
{


public class Frame
{
	// frame counter, timestamp
	public uint FrameCounter  { get; set; }
	public double TimeStamp  { get; set; }
	public uint TimeStampSec  { get; set; }
	public uint TimeStampUsec  { get; set; }
	public uint LatencyUsec  { get; set; }

	// Standard Bodies
	public Dictionary< int, DTrackBody > Bodies  { get; set; }
	public int NumBodies  { get; set; }  // number of calibrated Standard Bodies (as far as known)

	// Flysticks
	public Dictionary< int, DTrackFlystick > Flysticks  { get; set; }
	public int NumFlysticks  { get; set; }  // number of calibrated Flysticks

	// Measurement Tools
	public Dictionary< int, DTrackMeaTool > MeaTools  { get; set; }
	public int NumMeaTools  { get; set; }  // number of calibrated Measurement Tools

	// Fingertracking hands
	public Dictionary< int, DTrackHand > Hands  { get; set; }
	public int NumHands  { get; set; }  // number of calibrated Fingertracking hands (as far as known)


	public Frame()
	{
		this.FrameCounter = 0;
		this.TimeStamp = -1.0;
		this.TimeStampSec = 0;
		this.TimeStampUsec = 0;
		this.LatencyUsec = 0;

		this.NumBodies = 0;
		this.NumFlysticks = 0;
		this.NumMeaTools = 0;
		this.NumHands = 0;
	}


	// Get Standard Body data.

	public DTrackBody GetBody( int id )
	{
		if ( this.Bodies == null )  return null;

		DTrackBody body;
		if ( this.Bodies.TryGetValue( id, out body ) )  return body;

		return null;
	}

	// Get Flystick data.

	public DTrackFlystick GetFlystick( int id )
	{
		if ( this.Flysticks == null )  return null;

		DTrackFlystick flystick;
		if ( this.Flysticks.TryGetValue( id, out flystick ) )  return flystick;

		return null;
	}

	// Get Measurement Tool data.

	public DTrackMeaTool GetMeaTool( int id )
	{
		if ( this.MeaTools == null )  return null;

		DTrackMeaTool meatool;
		if ( this.MeaTools.TryGetValue( id, out meatool ) )  return meatool;

		return null;
	}

	// Get Fingertracking hand data.

	public DTrackHand GetHand( int id )
	{
		if ( this.Hands == null )  return null;

		DTrackHand hand;
		if ( this.Hands.TryGetValue( id, out hand ) )  return hand;

		return null;
	}
}


}  // namespace DTrackSDK

