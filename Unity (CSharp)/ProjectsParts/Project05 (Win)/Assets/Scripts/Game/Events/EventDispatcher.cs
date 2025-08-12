using com.ootii.Collections;
using com.ootii.Messages;
using it.Game.Events.Messages;

namespace it.Game.Events
{
  //public delegate void MessageHandler(IMessage rMessage);
  public static class EventDispatcher
  {

	 public static void SendMessage(string rType, float rDelay = 0f)
	 {
		MessageDispatcher.SendMessage(rType, rDelay);
	 }
	 public static void SendMessage(string rType, string rFilter, float rDelay = 0f)
	 {
		MessageDispatcher.SendMessage(rType, rFilter, rDelay);
	 }

	 public static void SendMessageData(string rType, object rData, float rDelay = 0f)
	 {
		MessageDispatcher.SendMessageData(rType, rData, rDelay);
	 }

	 public static void SendMessage(object rSender, string rType, object rData, float rDelay)
	 {
		MessageDispatcher.SendMessage(rSender, rType, rData, rDelay);
	 }

	 public static void SendMessage(object rSender, object rRecipient, string rType, object rData, float rDelay)
	 {
		MessageDispatcher.SendMessage(rSender, rRecipient, rType, rData, rDelay);
	 }

	 public static void SendMessage(object rSender, string rRecipient, string rType, object rData, float rDelay)
	 {
		MessageDispatcher.SendMessage(rSender, rRecipient, rType, rData, rDelay);
	 }

	 public static void SendMessage(IMessage rMessage, bool rSetUnhandledToHandled = false)
	 {
		MessageDispatcher.SendMessage(rMessage, rSetUnhandledToHandled);
	 }

	 public static void AddListener(string rMessageType, MessageHandler rHandler)
	 {
		MessageDispatcher.AddListener(rMessageType, rHandler);
	 }
	 public static void AddListener(string rMessageType, MessageHandler rHandler, bool rImmediate)
	 {
		MessageDispatcher.AddListener(rMessageType, rHandler, rImmediate);
	 }
	 public static void AddListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler)
	 {
		MessageDispatcher.AddListener(rOwner, rMessageType, rHandler);
	 }
	 public static void AddListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler, bool rImmediate)
	 {
		MessageDispatcher.AddListener(rOwner,  rMessageType,  rHandler,  rImmediate);
	 }
	 public static void AddListener(string rMessageType, string rFilter, MessageHandler rHandler)
	 {
		MessageDispatcher.AddListener(rMessageType, rFilter, rHandler);
	 }
	 public static void AddListener(string rMessageType, string rFilter, MessageHandler rHandler, bool rImmediate)
	 {
		MessageDispatcher.AddListener(rMessageType, rFilter, rHandler, rImmediate);
	 }

	 public static void RemoveListener(string rMessageType, MessageHandler rHandler)
	 {
		MessageDispatcher.RemoveListener(rMessageType, rHandler);
	 }
	 public static void RemoveListener(string rMessageType, MessageHandler rHandler, bool rImmediate)
	 {
		MessageDispatcher.RemoveListener(rMessageType, rHandler, rImmediate);
	 }
	 public static void RemoveListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler)
	 {
		MessageDispatcher.RemoveListener(rOwner, rMessageType, rHandler);
	 }
	 public static void RemoveListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler, bool rImmediate)
	 {
		MessageDispatcher.RemoveListener(rOwner, rMessageType, rHandler, rImmediate);
	 }
	 public static void RemoveListener(string rMessageType, string rFilter, MessageHandler rHandler)
	 {
		MessageDispatcher.RemoveListener(rMessageType, rFilter, rHandler);
	 }
	 public static void RemoveListener(string rMessageType, string rFilter, MessageHandler rHandler, bool rImmediate)
	 {
		MessageDispatcher.RemoveListener(rMessageType, rFilter, rHandler, rImmediate);
	 }


  }

}