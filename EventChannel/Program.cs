using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventChannel
{
    public interface IEventChannel
    {
        void Publish(string topic, string data);
        void Subscribe(string topic, ISubscriber subscriber);
    }
    public interface IPublisher
    {
        void Publish(string topic);
    }
    public interface ISubscriber
    {
        void Notify(string data);
    }
    public class EventChannel : IEventChannel
    {
        private Dictionary<string, List<ISubscriber>> _topic = new Dictionary<string, List<ISubscriber>>();
        public void Publish(string topic, string data)
        {
            if (!_topic.ContainsKey(topic))
                return;
            foreach (var subscriber in _topic[topic])
                subscriber.Notify(data);
        }

        public void Subscribe(string topic, ISubscriber subscriber)
        {
            if (_topic.ContainsKey(topic))
                _topic[topic].Add(subscriber);
            else
                _topic.Add(topic, new List<ISubscriber>() { subscriber });
        }
    }

    public class Publisher : IPublisher
    {
        private string _topic;
        private IEventChannel _channel;

        public Publisher(string topic, IEventChannel channel)
        {
            _topic = topic;
            _channel = channel;
        }

        public void Publish(string data)
        {
            _channel.Publish(_topic, data);
        }
    }

    public class Subsriber : ISubscriber
    {
        private string _name;

        public Subsriber(string name)
        {
            _name = name;
        }

        public void Notify(string data)
        {
            Console.WriteLine($"Subscriber {_name} notify {data}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var channel = new EventChannel();
            var publisherA = new Publisher("#topic a", channel);
            var publisherB = new Publisher("#topic b", channel);
            var subscriberA = new Subsriber("reader 1");
            var subscriberB = new Subsriber("reader 2");

            channel.Subscribe("#topic a", subscriberA);
            channel.Subscribe("#topic a", subscriberB);
            channel.Subscribe("#topic b", subscriberB);

            publisherA.Publish("text1");
            publisherB.Publish("text2");

            Console.ReadKey();
        }
    }
}
