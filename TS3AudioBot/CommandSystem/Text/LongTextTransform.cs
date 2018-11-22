// TS3AudioBot - An advanced Musicbot for Teamspeak 3
// Copyright (C) 2017  TS3AudioBot contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the Open Software License v. 3.0
//
// You should have received a copy of the Open Software License along with this
// program. If not, see <https://opensource.org/licenses/OSL-3.0>.

namespace TS3AudioBot.CommandSystem.Text
{
	using System.Collections.Generic;
	using TS3AudioBot.Helper;
	using TS3Client.Commands;

	public static class LongTextTransform
	{
		public static IEnumerable<string> Transform(string text, LongTextBehaviour behaviour)
		{
			switch (behaviour)
			{
			case LongTextBehaviour.Drop:
			case LongTextBehaviour.Trim:
			case LongTextBehaviour.SplitHard:
				int tokenCnt = 0;
				int lastSplit = 0;
				for (int i = 0; i < text.Length; i++)
				{
					var prevTokenCnt = tokenCnt;
					tokenCnt += Ts3String.IsDoubleChar(text[i]) ? 2 : 1;
					if (tokenCnt > Ts3Const.MaxSizeTextMessage) // TODO >= ??
					{
						if (behaviour == LongTextBehaviour.Drop)
							yield break;
						yield return text.Substring(lastSplit, i - lastSplit);
						if (behaviour == LongTextBehaviour.Trim)
							yield break;
						lastSplit = i;
						tokenCnt -= prevTokenCnt;
					}
				}
				yield return text;
				break;

			case LongTextBehaviour.Split:
				tokenCnt = 0;
				lastSplit = 0;
				int lastLineBreak = 0;
				int lastLineBreakTokens = 0;
				for (int i = 0; i < text.Length; i++)
				{
					var prevTokenCnt = tokenCnt;
					tokenCnt += Ts3String.IsDoubleChar(text[i]) ? 2 : 1;

					if (tokenCnt > Ts3Const.MaxSizeTextMessage) // TODO >= ??
					{
						if (lastLineBreak == 0)
						{
							yield return text.Substring(lastSplit, i - lastSplit);
							tokenCnt -= prevTokenCnt;
							lastSplit = i;
						}
						else
						{
							yield return text.Substring(lastSplit, lastLineBreak - lastSplit);
							tokenCnt -= lastLineBreakTokens;
							lastSplit = lastLineBreak;
							lastLineBreak = 0;
						}
					}
					else if (text[i] == '\n')
					{
						lastLineBreak = i;
						lastLineBreakTokens = tokenCnt;
					}
				}
				break;

			default:
				throw Util.UnhandledDefault(behaviour);
			}
		}
	}
}
