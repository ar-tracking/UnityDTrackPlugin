/* DTrackSDK in C#: Parser.cs
 * 
 * Parsing a frame of DTRACK output data.
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

using System;

namespace DTrackSDK.Parsers
{


public class Parser
{
	private int _numBodies = 0;
	private int _numHands = 0;

	public Frame Parse( string packet )
	{
		var frame = new Frame();

		int calNumBodies = -1;
		int calNumHands = -1;

		string[] lines = packet.Split( Statics.LineSplit, StringSplitOptions.RemoveEmptyEntries );

		foreach ( string line in lines )
		{
			try
			{
				if ( line.StartsWith( Statics.Prefix_fr ) )
				{
					frame.FrameCounter = FrameCounterParser.Parse( line );
				}
				else if ( line.StartsWith( Statics.Prefix_ts ) )
				{
					frame.TimeStamp = TimestampParser.Parse( line );
				}
				else if ( line.StartsWith( Statics.Prefix_ts2 ) )
				{
					uint tssec, tsusec, lat;
					frame.TimeStamp = Timestamp2Parser.Parse( line, out tssec, out tsusec, out lat );
					frame.TimeStampSec = tssec;
					frame.TimeStampUsec = tsusec;
					frame.LatencyUsec = lat;
				}
				else if ( line.StartsWith( Statics.Prefix_6dcal ) )
				{
					calNumBodies = CalibratedBodiesParser.Parse( line );
				}
				else if ( line.StartsWith( Statics.Prefix_6d ) )
				{
					int num;
					frame.Bodies = BodyParser.Parse( line, out num );
					frame.NumBodies = num;
				}
				else if ( line.StartsWith( Statics.Prefix_6df2 ) )
				{
					int num;
					frame.Flysticks = FlystickParser.Parse( line, out num );
					frame.NumFlysticks = num;
				}
				else if ( line.StartsWith( Statics.Prefix_6dmt2 ) )
				{
					int num;
					frame.MeaTools = MeaToolParser.Parse( line, out num );
					frame.NumMeaTools = num;
				}
				else if ( line.StartsWith( Statics.Prefix_glcal ) )
				{
					calNumHands = CalibratedHandsParser.Parse( line );
				}
				else if ( line.StartsWith( Statics.Prefix_gl ) )
				{
					int num;
					frame.Hands = HandParser.Parse( line, out num );
					frame.NumHands = num;
				}
			}
			catch ( Exception e )
			{
				throw new Exception( $"Error parsing line '{line.Substring( 0, 6 )}': {e.Message}" );
			}
		}

		if ( calNumBodies >= 0 )
		{
			frame.NumBodies = calNumBodies;
		}
		else
		{
			if ( frame.NumBodies > _numBodies )
			{
				_numBodies = frame.NumBodies;
			}
			else
			{
				frame.NumBodies = _numBodies;
			}
		}

		if ( calNumHands >= 0 )
		{
			frame.NumHands = calNumHands;
		}
		else
		{
			if ( frame.NumHands > _numHands )
			{
				_numHands = frame.NumHands;
			}
			else
			{
				frame.NumHands = _numHands;
			}
		}

		return frame;
	}
}


}  // namespace DTrackSDK.Parsers

