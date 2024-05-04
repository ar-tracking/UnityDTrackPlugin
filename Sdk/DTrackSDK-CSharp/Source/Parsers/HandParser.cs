/* DTrackSDK in C#: HandParser.cs
 * 
 * Parsing Fingertracking hands of DTRACK output data.
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
using System.Collections.Generic;
using System.Globalization;

namespace DTrackSDK.Parsers
{


public static class HandParser
{
	public static Dictionary< int, DTrackHand > Parse( string raw, out int num )
	{
		string[] handsSplit = raw.Split( Statics.NumberSplit, 3 );

		int handsCount = Convert.ToInt32( handsSplit[ 1 ] );
		if ( handsCount <= 0 )
		{
			num = 0;
			return null;
		}

		var hands = new Dictionary< int, DTrackHand >();

		string trimmed = handsSplit[ 2 ].Trim( Statics.SectionTrim );
		string[] handsData = trimmed.Split( Statics.SectionSplit, StringSplitOptions.None );

		int blk = 0;
		int id = -1;
		for ( int gl = 0; gl < handsCount; gl++ )
		{
			string[] meta = handsData[ blk++ ].Split( ' ' );
			id = Convert.ToInt32( meta[ 0 ] );
			float qu = Convert.ToSingle( meta[ 1 ], CultureInfo.InvariantCulture );
			int lr = Convert.ToInt32( meta[ 2 ] );
			int nf = Convert.ToInt32( meta[ 3 ] );

			string[] s = handsData[ blk++ ].Split( ' ' );
			float sx = Convert.ToSingle( s[ 0 ], CultureInfo.InvariantCulture );
			float sy = Convert.ToSingle( s[ 1 ], CultureInfo.InvariantCulture );
			float sz = Convert.ToSingle( s[ 2 ], CultureInfo.InvariantCulture );

			string[] r = handsData[ blk++ ].Split( ' ' );
			float r0 = Convert.ToSingle( r[ 0 ], CultureInfo.InvariantCulture );
			float r1 = Convert.ToSingle( r[ 1 ], CultureInfo.InvariantCulture );
			float r2 = Convert.ToSingle( r[ 2 ], CultureInfo.InvariantCulture );
			float r3 = Convert.ToSingle( r[ 3 ], CultureInfo.InvariantCulture );
			float r4 = Convert.ToSingle( r[ 4 ], CultureInfo.InvariantCulture );
			float r5 = Convert.ToSingle( r[ 5 ], CultureInfo.InvariantCulture );
			float r6 = Convert.ToSingle( r[ 6 ], CultureInfo.InvariantCulture );
			float r7 = Convert.ToSingle( r[ 7 ], CultureInfo.InvariantCulture );
			float r8 = Convert.ToSingle( r[ 8 ], CultureInfo.InvariantCulture );

			DTrackFinger[] fingers = new DTrackFinger[ nf ];
			for ( int fi = 0; fi < nf; fi++ )
			{
				string[] fs = handsData[ blk++ ].Split( ' ' );
				float fsx = Convert.ToSingle( fs[ 0 ], CultureInfo.InvariantCulture );
				float fsy = Convert.ToSingle( fs[ 1 ], CultureInfo.InvariantCulture );
				float fsz = Convert.ToSingle( fs[ 2 ], CultureInfo.InvariantCulture );

				string[] fr = handsData[ blk++ ].Split( ' ' );
				float fr0 = Convert.ToSingle( fr[ 0 ], CultureInfo.InvariantCulture );
				float fr1 = Convert.ToSingle( fr[ 1 ], CultureInfo.InvariantCulture );
				float fr2 = Convert.ToSingle( fr[ 2 ], CultureInfo.InvariantCulture );
				float fr3 = Convert.ToSingle( fr[ 3 ], CultureInfo.InvariantCulture );
				float fr4 = Convert.ToSingle( fr[ 4 ], CultureInfo.InvariantCulture );
				float fr5 = Convert.ToSingle( fr[ 5 ], CultureInfo.InvariantCulture );
				float fr6 = Convert.ToSingle( fr[ 6 ], CultureInfo.InvariantCulture );
				float fr7 = Convert.ToSingle( fr[ 7 ], CultureInfo.InvariantCulture );
				float fr8 = Convert.ToSingle( fr[ 8 ], CultureInfo.InvariantCulture );

				string[] fgeom = handsData[ blk++ ].Split( ' ' );
				float ro = Convert.ToSingle( fgeom[ 0 ], CultureInfo.InvariantCulture );
				float lo = Convert.ToSingle( fgeom[ 1 ], CultureInfo.InvariantCulture );
				float alpha_om = Convert.ToSingle( fgeom[ 2 ], CultureInfo.InvariantCulture );
				float lm = Convert.ToSingle( fgeom[ 3 ], CultureInfo.InvariantCulture );
				float alpha_mi = Convert.ToSingle( fgeom[ 4 ], CultureInfo.InvariantCulture );
				float li = Convert.ToSingle( fgeom[ 5 ], CultureInfo.InvariantCulture );

				fingers[ fi ] = new DTrackFinger( fi, qu, fsx, fsy, fsz, fr0, fr1, fr2, fr3, fr4, fr5, fr6, fr7, fr8,
				                                  ro, lo, alpha_om, lm, alpha_mi, li );
			}

			hands.Add( id,
			           new DTrackHand( id, qu, lr, sx, sy, sz, r0, r1, r2, r3, r4, r5, r6, r7, r8, fingers ) );
		}

		num = id + 1;
		return hands;
	}
}


}  // namespace DTrackSDK.Parsers

