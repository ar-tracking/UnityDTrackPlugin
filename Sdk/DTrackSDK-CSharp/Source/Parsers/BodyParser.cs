/* DTrackSDK in C#: BodyParser.cs
 * 
 * Parsing Standard Bodies of DTRACK output data.
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
using System.Collections.Generic;
using System.Globalization;

namespace DTrackSDK.Parsers
{


public static class BodyParser
{
	public static Dictionary< int, DTrackBody > Parse( string raw )
	{
		string[] bodyCountSplit = raw.Split( Statics.NumberSplit, 3 );

		int bodyCount = Convert.ToInt32( bodyCountSplit[ 1 ] );
		if ( bodyCount <= 0 )
			return null;

		var bodies = new Dictionary< int, DTrackBody >();

		string trimmed = bodyCountSplit[ 2 ].Trim( Statics.SectionTrim );
		string[] sectionSplit = trimmed.Split( Statics.SectionSplit, StringSplitOptions.None );

		int iblk = 0;
		for ( int ibody = 0; ibody < bodyCount; ibody++ )
		{
			string[] m = sectionSplit[ iblk++ ].Split( ' ' );
			string[] s = sectionSplit[ iblk++ ].Split( ' ' );
			string[] r = sectionSplit[ iblk++ ].Split( ' ' );

			int id = Convert.ToInt32( m[ 0 ] );
			float qu = Convert.ToSingle( m[ 1 ], CultureInfo.InvariantCulture );

			float sx = Convert.ToSingle( s[ 0 ], CultureInfo.InvariantCulture );
			float sy = Convert.ToSingle( s[ 1 ], CultureInfo.InvariantCulture );
			float sz = Convert.ToSingle( s[ 2 ], CultureInfo.InvariantCulture );

			float r0 = Convert.ToSingle( r[ 0 ], CultureInfo.InvariantCulture );
			float r1 = Convert.ToSingle( r[ 1 ], CultureInfo.InvariantCulture );
			float r2 = Convert.ToSingle( r[ 2 ], CultureInfo.InvariantCulture );
			float r3 = Convert.ToSingle( r[ 3 ], CultureInfo.InvariantCulture );
			float r4 = Convert.ToSingle( r[ 4 ], CultureInfo.InvariantCulture );
			float r5 = Convert.ToSingle( r[ 5 ], CultureInfo.InvariantCulture );
			float r6 = Convert.ToSingle( r[ 6 ], CultureInfo.InvariantCulture );
			float r7 = Convert.ToSingle( r[ 7 ], CultureInfo.InvariantCulture );
			float r8 = Convert.ToSingle( r[ 8 ], CultureInfo.InvariantCulture );

			bodies.Add( id,
			            new DTrackBody( id, qu, sx, sy, sz, r0, r1, r2, r3, r4, r5, r6, r7, r8 ) );
		}

		return bodies;
	}
}


}  // namespace DTrackSDK.Parsers

