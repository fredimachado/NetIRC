using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a TOPIC message from server or client.
    /// </summary>
    public class TopicMessage : IRCMessage, IServerMessage, IClientMessage
    {
        /// <summary>
        /// Gets the target channel name.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Gets the topic text.
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public TopicMessage(ParsedIRCMessage parsedMessage)
        {
            Channel = parsedMessage.Parameters[0];
            Topic = parsedMessage.Trailing;
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="channel">Target channel name.</param>
        /// <param name="topic">Topic text.</param>
        public TopicMessage(string channel, string topic)
        {
            Channel = channel;
            Topic = topic;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "TOPIC", Channel, Topic };
    }
}
