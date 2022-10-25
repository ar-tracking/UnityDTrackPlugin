/* DTrackSDK in C#: Statics.cs
 * 
 * DTrackSDK: Constants.
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

namespace DTrackSDK
{


public class FingerIndex
{
	public const int THUMB = 0;
	public const int INDEX = 1;
	public const int MIDDLE = 2;
	public const int RING = 3;
	public const int PINKY = 4;
}


public class Statics
{
	// properties for parsers
	public static readonly char[] LineSplit = { ( char )0x0d, ( char )0x0a };
	public static readonly char[] NumberSplit = { ' ' };
	public static readonly char[] SectionTrim = { '[', ']' };
	public static readonly string[] SectionSplit = { "][", "] [" };

	// DTRACK output (ASCII): prefixes of data types
	public const string Prefix_fr = "fr ";
	public const string Prefix_ts = "ts ";
	public const string Prefix_6dcal = "6dcal ";
	public const string Prefix_6d = "6d ";
	public const string Prefix_6df2 = "6df2 ";
	public const string Prefix_glcal = "glcal ";
	public const string Prefix_gl = "gl ";
}


}  // namespace DTrackSDK

