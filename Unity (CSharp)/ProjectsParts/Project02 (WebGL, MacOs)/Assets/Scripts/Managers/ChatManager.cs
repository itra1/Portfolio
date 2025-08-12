using System.Collections;
using UnityEngine;
using it.Api;
using System.Collections.Generic;
using it.Network.Rest;

namespace Garilla.Managers
{
	public class ChatManager
	{
		public List<UserBlock> UserBlockList => _userBlock;

		private List<UserBlock> _userBlock = new List<UserBlock>();

		public void LoadBlockList()
		{
			TableApi.ChatBlockListGet((result) =>
			{
				if (result.IsSuccess)
				{
					_userBlock = result.Result;
					com.ootii.Messages.MessageDispatcher.SendMessage(StringConstants.CHAT_BLOCKLOAD);
				}
			});
		}

		public void ChatBlockListBlockChange(ulong userId, bool isBlock, UnityEngine.Events.UnityAction onComplete)
		{
			TableApi.ChatBlockListBlockChange(userId, isBlock, (result) =>
			{
				if (result.IsSuccess)
					LoadBlockList();
				onComplete();
				//TODO добавить сообщение об ошибке
			});
		}

		public bool IsUserBlock(ulong userId)
		{
			return _userBlock.Exists(x => x.blocked_user.id == userId);
		}

	}
}