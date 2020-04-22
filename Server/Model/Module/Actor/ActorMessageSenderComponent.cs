using System;
using System.Net;

namespace ETModel
{
	public class ActorMessageSenderComponent: Component
	{
		public ActorMessageSender Get(long actorId)
		{
			if (actorId == 0)
			{
				throw new Exception($"actor id is 0");
			}
			//通过actorId拿到对应的内部服务端点
			IPEndPoint ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(IdGenerater.GetAppId(actorId));
			//根据actorId和内网服务端点，实例化一个消息发送者实例
			ActorMessageSender actorMessageSender = new ActorMessageSender(actorId, ipEndPoint);
			return actorMessageSender;
		}
	}
}
