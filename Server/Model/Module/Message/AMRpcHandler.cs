using System;

namespace ETModel
{
    /// <summary>
    /// MARK:通过where类型约束来声明泛型Request和Response为IRequest和IResponse的接口实现类
    /// </summary>
    /// <typeparam name="Request"></typeparam>
    /// <typeparam name="Response"></typeparam>
	public abstract class AMRpcHandler<Request, Response>: IMHandler where Request : class, IRequest where Response : class, IResponse 
	{
		protected abstract ETTask Run(Session session, Request request, Response response, Action reply);

        /// <summary>
        /// Rpc调用处理
        /// </summary>
        /// <param name="session">会话的抽象，Entity的一种</param>
        /// <param name="message">IRequest的实现类</param>
        /// <returns></returns>
		public async ETVoid Handle(Session session, object message)
		{
			try
			{
				Request request = message as Request;
				if (request == null)
				{
					Log.Error($"消息类型转换错误: {message.GetType().Name} to {typeof (Request).Name}");
				}

				int rpcId = request.RpcId;
				long instanceId = session.InstanceId;
				Response response = Activator.CreateInstance<Response>();

				void Reply()
				{
					// 等回调回来,session可以已经断开了,所以需要判断session InstanceId是否一样
					if (session.InstanceId != instanceId)
					{
						return;
					}

					response.RpcId = rpcId;
					session.Reply(response);
				}

				try
				{
					await this.Run(session, request, response, Reply);
				}
				catch (Exception e)
				{
					Log.Error(e);
					response.Error = ErrorCode.ERR_RpcFail;
					response.Message = e.ToString();
					Reply();
				}
				
			}
			catch (Exception e)
			{
				Log.Error($"解释消息失败: {message.GetType().FullName}\n{e}");
			}
		}

		public Type GetMessageType()
		{
			return typeof (Request);
		}
	}
}