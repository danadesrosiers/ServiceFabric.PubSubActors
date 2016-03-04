﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiceFabric.PubSubActors.Interfaces;

namespace ServiceFabric.PubSubActors.SubscriberServices
{
	/// <summary>
	/// Defines a common interface for all Subscriber Services. 
	/// </summary>
	[ServiceContract]
	public interface ISubscriberService
    {
		/// <summary>
		/// Registers this Service as a subscriber for messages.
		/// </summary>
		/// <returns></returns>
		[OperationContract(IsOneWay = true)]
		Task RegisterAsync();

		/// <summary>
		/// Unregisters this Service as a subscriber for messages.
		/// </summary>
		/// <returns></returns>
		[OperationContract(IsOneWay = true)]
		Task UnregisterAsync();

		/// <summary>
		/// Receives a published message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[OperationContract(IsOneWay = true)]
		Task ReceiveMessageAsync(MessageWrapper message);
	}
}
