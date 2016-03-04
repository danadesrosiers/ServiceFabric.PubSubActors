﻿using System;
using System.Fabric;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ServiceFabric.PubSubActors.SubscriberServices
{
	/// <summary>
	/// <see cref="ICommunicationListener"/> implementation for receiving messages in Subscriber Services.
	/// Note: implement <see cref="ISubscriberService"/> on your Reliable Service, and pass it in the ctor and
	/// return a new instance of <see cref="SubscriberCommunicationListener"/> from <see cref="StatefulService.CreateServiceReplicaListeners"/> or 
	/// <see cref="StatelessServiceBase.CreateServiceInstanceListeners"/>.
	/// </summary>
	public class SubscriberCommunicationListener : ICommunicationListener
	{
		public const string EndpointResourceName = "Subscriber";
		private readonly ISubscriberService _subscriberService;
		private readonly ServiceInitializationParameters _serviceInitializationParameters;
		private WcfCommunicationListener _wrappedCommunicationListener;

		/// <summary>
		/// Creates a new communication listener that receives published messages from 
		/// <see cref="BrokerActor"/> after being registered there.
		/// </summary>
		/// <param name="subscriberService"></param>
		/// <param name="serviceInitializationParameters"></param>
		public SubscriberCommunicationListener(ISubscriberService subscriberService, ServiceInitializationParameters serviceInitializationParameters)
		{
			if (subscriberService == null) throw new ArgumentNullException(nameof(subscriberService));
			if (serviceInitializationParameters == null)
				throw new ArgumentNullException(nameof(serviceInitializationParameters));

			_subscriberService = subscriberService;
			_serviceInitializationParameters = serviceInitializationParameters;
		}

		
		/// <summary>
		/// This method causes the communication listener to be opened. Once the Open
		///             completes, the communication listener becomes usable - accepts and sends messages.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// A <see cref="T:System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
		///             the endpoint string.
		/// </returns>
		public Task<string> OpenAsync(CancellationToken cancellationToken)
		{
			_wrappedCommunicationListener = new WcfCommunicationListener(_serviceInitializationParameters,
			   typeof(ISubscriberService), _subscriberService)
			{
				EndpointResourceName = EndpointResourceName,
				Binding = CreateBinding(),
			};

			
			return _wrappedCommunicationListener.OpenAsync(cancellationToken);
		}

		/// <summary>
		/// This method causes the communication listener to close. Close is a terminal state and 
		///             this method allows the communication listener to transition to this state in a
		///             graceful manner.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// A <see cref="T:System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
		/// </returns>
		public Task CloseAsync(CancellationToken cancellationToken)
		{
			return _wrappedCommunicationListener.CloseAsync(cancellationToken);
		}

		/// <summary>
		/// This method causes the communication listener to close. Close is a terminal state and
		///             this method causes the transition to close ungracefully. Any outstanding operations
		///             (including close) should be canceled when this method is called.
		/// </summary>
		public void Abort()
		{
			_wrappedCommunicationListener.Abort();
		}

		/// <summary>
		/// Creates a new Binding to use for the <see cref="_wrappedCommunicationListener"/>.
		/// </summary>
		/// <returns></returns>
		protected virtual Binding CreateBinding()
		{
			return BindingFactory.CreateBinding();
		}
	}
}
