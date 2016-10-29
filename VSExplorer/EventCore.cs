using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WinExplorer
{
    #region Event Router Helper Classes

    /// <summary>
    /// default implementation of InternalEventHandler for standard events. The signature of the HandleEvent method matches that of the
    /// EventHandler delegate.
    /// </summary>
    public class DefaultEventRouter : InternalEventRouter
    {
        /// <summary>
        /// construct and pass the event-info to the base constructor.
        /// </summary>
        /// <param name="info"></param>
        public DefaultEventRouter(EventInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// handles EventHandler type events:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void HandleEvent(object sender, EventArgs e)
        {
            // capture the event parameters:
            Object[] args = new Object[2];
            args[0] = sender;
            args[1] = e;

            // submit the event data to the event broker.
            EventBroker.SubmitEvent(sender, Info, args);
        }
    }

    /// <summary>
    /// abstract base class for the event routing helper classes.
    /// </summary>
    public abstract class InternalEventRouter
    {
        #region Fields

        /// <summary>
        /// the delegate for this event handler
        /// </summary>
        private Delegate _delegate = null;

        /// <summary>
        /// list of object's generating events that this handler is rerouting.
        /// </summary>
        private List<Object> _subscribers = new List<object>();

        #endregion Fields

        /// <summary>
        /// construct the event handler class.
        /// </summary>
        /// <param name="info"></param>
        public InternalEventRouter(EventInfo info)
        {
            Info = info;
        }

        #region Properties

        /// <summary>
        /// gets the event for which this handler is created.
        /// </summary>
        public virtual EventInfo Info { get; internal set; }

        /// <summary>
        /// gets the name of the event for which this handler is created.
        /// </summary>
        public virtual String EventName { get { return Info.Name; } }

        /// <summary>
        /// gets a delegate to the HandleEvent method implemented in event-handler specific classes derived from this class.
        /// </summary>
        /// <returns></returns>
        public virtual Delegate EventHandlerDelegate
        {
            get
            {
                // keep a reference to the delegate. this allows it to be removed.
                if (_delegate == null)
                    _delegate = Delegate.CreateDelegate(Info.EventHandlerType, this, this.GetType().GetMethod("HandleEvent"));

                // return the delegate to the HandleEvent method
                return _delegate;
            }
        }

        /// <summary>
        /// the event broker that handled the event.
        /// </summary>
        public EventCore EventBroker { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// attaches this event-handler to the event on object obj.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void AttachToEventOn(object obj)
        {
            try
            {
                // attach the handler to the event on object obj
                Info.AddEventHandler(obj, EventHandlerDelegate);

                // add to the list of subscribers:
                _subscribers.Add(obj);
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Could Not Bind to Event: " + Info.Name + " on: " + obj.ToString());
            }
        }

        /// <summary>
        /// removes this event handler from the target object.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void DetachFrom(object obj)
        {
            if (_subscribers.Contains(obj))
            {
                // remove this event handler from the object:
                Info.RemoveEventHandler(obj, EventHandlerDelegate);

                // remove the object from the list:
                _subscribers.Remove(obj);
            }
        }

        /// <summary>
        /// detatches all event handlers.
        /// </summary>
        public virtual void DetachAll()
        {
            // iterate the list of object's this event handler is attached to:
            foreach (object obj in _subscribers)
            {
                // remove this event handler delegate from the object:
                Info.RemoveEventHandler(obj, EventHandlerDelegate);
            }

            // reset the list of subscribers:
            _subscribers.Clear();
        }

        #endregion Methods
    }

    #endregion Event Router Helper Classes

    #region Common Event Arguments

    /// <summary>
    /// event arguments for a common event.
    /// </summary>
    public class CommonEventArgs : EventArgs
    {
        /// <summary>
        /// event information.
        /// </summary>
        public EventInfo EventInfo { get; set; }

        /// <summary>
        /// parameters of the event.
        /// </summary>
        public CommonEventParameter[] Parameters { get; set; }

        /// <summary>
        /// the event broker that generated the common event.
        /// </summary>
        public EventCore Broker { get; set; }

        /// <summary>
        /// builds a string representation of the event signature.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var parameter in Parameters)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(parameter.ParameterType.Name).Append(' ').Append(parameter.Name);
            }
            return EventInfo.Name + " (" + sb.ToString() + ")";
        }
    }

    /// <summary>
    /// this class holds details of an event parameter.
    /// </summary>
    public class CommonEventParameter
    {
        /// <summary>
        /// the name of the parameter (eg "sender")
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// the position in the method header of the paramter. zero based.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// the value of the parameter.
        /// </summary>
        public Object Value { get; set; }

        /// <summary>
        /// the type of the parameter (eg EventArgs)
        /// </summary>
        public Type ParameterType { get; set; }
    }

    #endregion Common Event Arguments

    #region Common Event Handler

    /// <summary>
    /// delegate for a universal event handler. passes the source object, the event-info, and all the parameters of the event.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="eventInfo"></param>
    /// <param name="arguments"></param>
    public delegate void CommonEventHandler(object source, CommonEventArgs e);

    #endregion Common Event Handler

    #region Common Event Broker

    /// <summary>
    /// instance universal event router: any object can have any number of it's events subscribed by this object, which then re-broadcasts the event.
    /// </summary>
    public class EventCore : IDisposable
    {
        #region Common Event Broker

        /// <summary>
        /// sync object
        /// </summary>
        private static object _locker = new object();

        /// <summary>
        /// holder for the static event broker.
        /// </summary>
        private static EventCore _common = null;

        /// <summary>
        /// static common event broker (singleton pattern field)
        /// </summary>
        public static EventCore CommonEventBroker
        {
            get
            {
                lock (_locker)
                {
                    if (_common == null)
                        _common = new EventCore();
                }
                return _common;
            }
        }

        #endregion Common Event Broker

        #region Construct / Destruct

        /// <summary>
        /// construct a new instance of the event broker class and start the event consumer thread.
        /// </summary>
        public EventCore()
        {
            // add in the default event handler implementation:
            if (!typeCache.ContainsKey(typeof(EventHandler)))
            {
                typeCache.Add(typeof(EventHandler), typeof(DefaultEventRouter));
            }

            // create and start the event consumer:
            _eventConsumerThread = new Thread((ThreadStart)EventConsumerTask);
            _eventConsumerThread.Start();
        }

        /// <summary>
        /// destructor.
        /// </summary>
        ~EventCore()
        {
            // stop the processing thread if required.
            Stop();
        }

        #endregion Construct / Destruct

        #region Fields

        /// <summary>
        /// dictionary of types of event-handler classes for specific event-handler delegates.
        /// </summary>
        private Dictionary<Type, Type> typeCache = new Dictionary<Type, Type>();

        /// <summary>
        /// dictionary of event-handler classes for event types.
        /// </summary>
        private Dictionary<EventInfo, InternalEventRouter> handlerCache = new Dictionary<EventInfo, InternalEventRouter>();

        /// <summary>
        /// FIFO queue of event notifications
        /// </summary>
        private static Queue<CommonEventInfo> _eventQueue = new Queue<CommonEventInfo>();

        /// <summary>
        /// locking object to synchronize access to the event-queue.
        /// </summary>
        private object _eventQueueLocker = new object();

        /// <summary>
        /// auto-reset event used to signal the event-queue consumer that a new event is on the queue.
        /// </summary>
        private AutoResetEvent _newEventEvent = new AutoResetEvent(false);

        /// <summary>
        /// thread to run the event consumer task.
        /// </summary>
        private Thread _eventConsumerThread = null;

        /// <summary>
        /// flag to control the event consumer thread.
        /// </summary>
        private bool _run = true;

        #endregion Fields

        #region Common Event

        /// <summary>
        /// this is the common event handler: any event subscribed will raise this event when it fires.
        /// </summary>
        public event CommonEventHandler CommonEvent;

        /// <summary>
        /// this class holds information from a routed event, and is used to queue the event notification.
        /// </summary>
        private class CommonEventInfo
        {
            /// <summary>
            /// construct the event-info class.
            /// </summary>
            /// <param name="source"></param>
            /// <param name="info"></param>
            /// <param name="arguments"></param>
            public CommonEventInfo(object source, EventCore eventBroker, EventInfo info, params Object[] arguments)
            {
                Source = source;
                Info = info;
                Arguments = arguments;
                TimeStamp = DateTime.Now;
                EventBroker = eventBroker;
            }

            #region Properties

            public EventCore EventBroker { get; set; }

            /// <summary>
            /// source object
            /// </summary>
            public object Source { get; set; }

            /// <summary>
            /// event information
            /// </summary>
            public EventInfo Info { get; set; }

            /// <summary>
            /// arguments of the event.
            /// </summary>
            public Object[] Arguments { get; set; }

            /// <summary>
            /// date and time the event was raised.
            /// </summary>
            public DateTime TimeStamp { get; set; }

            #endregion Properties

            #region Methods

            public static CommonEventParameter[] GetParameters(EventInfo info, Object[] arguments)
            {
                CommonEventParameter[] output = new CommonEventParameter[arguments.Length];
                int i = 0;

                // enumerate the parameters and setup the array of CommonEventParameter objects:
                foreach (var parameter in info.EventHandlerType.GetMethod("Invoke").GetParameters())
                {
                    output[i] = new CommonEventParameter();
                    output[i].Name = parameter.Name;
                    output[i].ParameterType = parameter.ParameterType;
                    output[i].Position = parameter.Position;
                    output[i].Value = arguments[i];
                    i++;
                }
                return output;
            }

            #endregion Methods

            /// <summary>
            /// method to raise the CommonEventBroker.CommonEvent
            /// </summary>
            public void RaiseEvent()
            {
                if (EventBroker.CommonEvent != null)
                    EventBroker.CommonEvent(Source, new CommonEventArgs() { EventInfo = Info, Parameters = GetParameters(Info, Arguments), Broker = EventBroker });
            }
        }

        #endregion Common Event

        #region Methods

        /// <summary>
        /// stops the consumer thread and detatches all the event routers.
        /// </summary>
        public void Stop()
        {
            // first detach any handlers so no new events can come in.
            DetachAll();

            // set the run-flag to false (this will cause the consumer loop to terminate)
            _run = false;

            // signal the auto-reset event to allow the thread to continue
            _newEventEvent.Set();

            if (_eventConsumerThread != null)
            {
                // wait for the thread to join, then dereference it.
                _eventConsumerThread.Join();
                _eventConsumerThread = null;
            }
        }

        /// <summary>
        /// detach all the handlers from a specific object:
        /// </summary>
        /// <param name="obj"></param>
        public void DetachHandlersFrom(object obj)
        {
            // detatch all the handlers from this object:
            foreach (var handler in handlerCache.Values)
            {
                handler.DetachFrom(obj);
            }
        }

        /// <summary>
        /// call detatch all for every instanced handler.
        /// </summary>
        public void DetachAll()
        {
            foreach (var handler in this.handlerCache.Values)
            {
                // detach this handler from all objects it subscribes to.
                handler.DetachAll();
            }
        }

        /// <summary>
        /// consumes events posted to the queue. decouples event processing from it's generation.
        /// </summary>
        private void EventConsumerTask()
        {
            // this will hold the dequeued event.
            CommonEventInfo info = null;

            // loop while the run flag is true
            while (_run)
            {
                // wait for a new item to be posted to the queue.
                _newEventEvent.WaitOne();

                // consume any items on the queue:
                while (_eventQueue.Count > 0)
                {
                    // syncrhonize access to the queue
                    lock (_eventQueueLocker)
                    {
                        // extract the item from the queue
                        info = _eventQueue.Dequeue();
                    }

                    // check we have an item
                    if (info != null)
                    {
                        // raise the event...this might take some time:
                        info.RaiseEvent();
                    }
                }
            }
        }

        /// <summary>
        /// public method used to raise the common-event. The event detail is added to a FIFO queue for processing by
        /// another thread.
        /// </summary>
        /// <param name="source">object raising the event</param>
        /// <param name="info">event-information of the event being raised</param>
        /// <param name="arguments">the arguments of the original event</param>
        public void SubmitEvent(object source, EventInfo info, params Object[] arguments)
        {
            // generate the event-info class:
            CommonEventInfo evi = new CommonEventInfo(source, this, info, arguments);

            // add that class to the queue:
            lock (_eventQueueLocker)
                _eventQueue.Enqueue(evi);

            // signal the auto-reset event that a new item was added to the queue.
            _newEventEvent.Set();
        }

        #endregion Methods

        #region Event Subscription

        public ArrayList events = null;

        /// <summary>
        /// subscribe to all the events of the specified object.
        /// </summary>
        /// <param name="obj"></param>
        public void SubscribeAll(object obj)
        {
            events = new ArrayList();

            foreach (EventInfo info in obj.GetType().GetEvents())
                Subscribe(obj, info.Name);
        }

        /// <summary>
        /// subscribes the specified event from the specified object to the universal event broker.
        /// when the event is raised, it will be rerouted to the CommonEventBroker.CommonEvent
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventName"></param>
        public void Subscribe(object obj, string eventName)
        {
            try
            {
                // get the event-info:
                EventInfo info = obj.GetType().GetEvent(eventName);

                // get the event-handler:
                var eventHandler = CreateEventHandler(info);

                // attach the event handler to the source object:
                eventHandler.AttachToEventOn(obj);

                events.Add(info);
            }
            catch (Exception ex) { };
        }

        #endregion Event Subscription

        #region Create Event Handler Methods

        /// <summary>
        /// returns a reference to a new event-handler for the specified event.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private InternalEventRouter CreateEventHandler(EventInfo info)
        {
            // if a handler for this event was already generated, return that:
            if (handlerCache.ContainsKey(info))
                return handlerCache[info];

            // get the type for the handler:
            Type handlerType = GenerateHandlerType(info);

            // create a new handler: instance the type, pass in the event-information:
            var handler = (InternalEventRouter)Activator.CreateInstance(handlerType, new object[] { info });

            // set the event handler:
            handler.EventBroker = this;

            // add into cache:
            handlerCache.Add(info, handler);

            // return the handler:
            return handler;
        }

        /// <summary>
        /// compile a new class that will work as an event-handler for the specified event info.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Type GenerateHandlerType(EventInfo info)
        {
            // return the handler type from cache if already generated:
            if (typeCache.ContainsKey(info.EventHandlerType))
                return typeCache[info.EventHandlerType];

            // create the code and compile to a new type:
            Type handlerType = info.GenerateEventHandlerCode().CreateType();

            // add into the type cache.
            typeCache.Add(info.EventHandlerType, handlerType);

            // return the type for the event handler:
            return handlerType;
        }

        #endregion Create Event Handler Methods

        #region IDisposable Members

        public void Dispose()
        {
            // stop event handling for disposal.
            Stop();
        }

        #endregion IDisposable Members
    }

    #endregion Common Event Broker

    #region Utility

    /// <summary>
    /// utility methods for generating event handler code.
    /// </summary>
    public static class CodeUtil
    {
        /// <summary>
        /// write an event-handler class for the given event handler type.
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public static String GenerateEventHandlerCode(this EventInfo eventInfo)
        {
            // define the class template: spots are marked for replacement (ie %NAME%)
            string baseCode = @"using System;
                                using System.Collections.Generic;
                                using System.Linq;
                                using System.Text;
                                using System.Reflection;

                                namespace WinReflector.Reflector.Utils
                                {
	                                public class %NAME%EventRouter : InternalEventRouter
	                                {
    		                                public %NAME%EventRouter(EventInfo info) : base(info)
    		                                {
    		                                }
    		                                public virtual void HandleEvent(%PARAMETERS%)
    		                                {
    			                                Object[] args = new Object[%PARAMCOUNT%];

			                                    %PARAMASSIGNMENT%

                                                // submit the event to the broker
			                                    EventBroker.SubmitEvent(sender, Info, args);
    		                                }
	                                }
                                }";

            // determine the parameters of the event:
            ParameterInfo[] handlerArgs = eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters();

            // parameter count is a replacement variable:
            int paramCount = handlerArgs.Length;

            string parameters = "";     // becommes "Object sender, EventArgs ev";
            string paramAssign = "";    // becomes args[0]=sender; args[1]=e etc.
            int i = 0;                  // current parameter id;

            // enumerate the handler arguments generating the appropriate code:
            foreach (var p in handlerArgs)
            {
                // handle comma-seperated arguments:
                if (parameters.Length > 0)
                    parameters += ", ";

                // this is the list of arguments for the HandleEvent method:
                parameters += p.ParameterType.FullName + " " + p.Name;

                // this will be the code to assign the values of the arguments to an array:
                paramAssign += "args[" + i++ + "]=" + p.Name + ";\r\n\t\t\t";
            }

            // perform string replacements to create the final class:
            baseCode = baseCode.Replace("%NAME%", eventInfo.EventHandlerType.Name);
            baseCode = baseCode.Replace("%PARAMETERS%", parameters);
            baseCode = baseCode.Replace("%PARAMCOUNT%", paramCount.ToString());
            baseCode = baseCode.Replace("%PARAMASSIGNMENT%", paramAssign);

            // return the formatted code:
            return baseCode;
        }

        #region Code Dom Util Methods

        /// <summary>
        /// gets an array of file-names of the current referenced assemblies of the executing assembly.
        /// </summary>
        /// <returns></returns>
        public static String[] GetAssemblyRefs()
        {
            // empty list:
            List<String> referencedAssemblies = new List<string>();

            // add the filename of each referenced assembly:
            foreach (var assemblyRef in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                referencedAssemblies.Add(System.Reflection.Assembly.Load(assemblyRef).Location);

            // add the current application to the list of referenced assemblies:
            referencedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // return the string-array:
            return referencedAssemblies.ToArray();
        }

        /// <summary>
        /// create and return a System.Type from the specified C# code.
        /// </summary>
        /// <param name="codeDefinition"></param>
        /// <returns></returns>
        public static Type CreateType(this String codeDefinition)
        {
            // create the provider:
            var provider = CodeDomProvider.CreateProvider("CSharp");

            // create the compilere parameters object:
            var parameters = new CompilerParameters(GetAssemblyRefs());

            // set some options: this should all be kept in RAM
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            // compile the code:
            var results = provider.CompileAssemblyFromSource(parameters, codeDefinition);

            // generate an exeption if the compilation generated errors:
            if (results.Errors.HasErrors)
            {
                Exception e = new Exception("Compiler Encountered Errors!");
                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        e.Data.Add(error.ErrorNumber, error);
                    }
                }
                throw e;
            }
            else
            {
                // get the array of types from the newly compiled assembly:
                Type[] types = results.CompiledAssembly.GetTypes();

                // return the first item in the list:
                if (types.Length > 0)
                    return types[0];
                else
                    throw new Exception("No Types Returned!");
            }
        }

        #endregion Code Dom Util Methods
    }

    #endregion Utility
}