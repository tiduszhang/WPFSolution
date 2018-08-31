using System;

namespace MVVM.Messaging
{
	/// <summary>
	/// The Messenger is a class allowing objects to exchange messages.
	/// </summary>
	public interface IMessenger
	{
		/// <summary>
		/// Registers a recipient for a type of message TMessage. The action
		/// parameter will be executed when a corresponding message is sent.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		void Register<TMessage>(object recipient, Action<TMessage> action);

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent. See the receiveDerivedMessagesToo parameter
		/// for details on how messages deriving from TMessage (or, if TMessage is an interface,
		/// messages implementing TMessage) can be received too.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="receiveDerivedMessagesToo">If true, message types deriving from
		/// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.
		/// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
		/// transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.</para>
		/// </param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action);

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach all recipients that registered for this message type
		/// using one of the Register methods.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		void Send<TMessage>(TMessage message);

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach only recipients that registered for this message type
		/// using one of the Register methods, and that are
		/// of the targetType.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <typeparam name="TTarget">The type of recipients that will receive
		/// the message. The message won't be sent to recipients of another type.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		void Send<TMessage, TTarget>(TMessage message);

		/// <summary>
		/// Unregisters a messager recipient completely. After this method
		/// is executed, the recipient will not receive any messages anymore.
		/// </summary>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		void Unregister(object recipient);

		/// <summary>
		/// Unregisters a message recipient for a given type of messages only. 
		/// After this method is executed, the recipient will not receive messages
		/// of type TMessage anymore, but will still receive other message types (if it
		/// registered for them previously).
		/// </summary>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		void Unregister<TMessage>(object recipient);

		/// <summary>
		/// Unregisters a message recipient for a given type of messages and for
		/// a given action. Other message types will still be transmitted to the
		/// recipient (if it registered for them previously). Other actions that have
		/// been registered for the message type TMessage and for the given recipient (if
		/// available) will also remain available.
		/// </summary>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		/// <param name="action">The action that must be unregistered for
		/// the recipient and for the message type TMessage.</param>
		void Unregister<TMessage>(object recipient, Action<TMessage> action);
	}
}