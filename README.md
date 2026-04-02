NetIRC
======
[![Build status](https://github.com/fredimachado/NetIRC/workflows/CI/badge.svg)](https://github.com/fredimachado/NetIRC/actions/workflows/ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/fredimachado/NetIRC/badge.svg)](https://coveralls.io/github/fredimachado/NetIRC)
[![NuGet](https://img.shields.io/nuget/dt/NetIRC.svg)](https://www.nuget.org/packages/NetIRC)
[![NuGet](https://img.shields.io/nuget/vpre/NetIRC.svg)](https://www.nuget.org/packages/NetIRC)
[![MyGet](https://img.shields.io/myget/netirc/v/NetIRC.svg)](https://www.myget.org/feed/netirc/package/nuget/NetIRC)

A simple, portable IRC client library for C# targeting **.NET Standard 2.0** and **.NET Framework 4.6.1**.

NetIRC gives you a clean async-first API for building IRC bots, desktop clients, and any other application that needs to talk to an IRC server.

---

## Features

- Fully **async/await** API
- **TCP** and **WebSocket** connections out of the box
- **Observable collections** for channels, queries, users, and server messages — ready for WPF / MAUI data binding
- Built-in IRC command messages (`JOIN`, `PART`, `PRIVMSG`, `NICK`, `KICK`, `QUIT`, `TOPIC`, …)
- **CTCP** support (`VERSION`, `TIME`, `PING`, `CLIENTINFO`, `ACTION`)
- Extensible **custom message handler** system
- Fluent **builder** for client configuration

---

## Installing

Via the NuGet Package Manager Console:

```
Install-Package NetIRC
```

Via the .NET CLI:

```
dotnet add package NetIRC
```

Pre-release builds are available on [MyGet](https://www.myget.org/feed/netirc/package/nuget/NetIRC).

---

## Quick Start

```csharp
using NetIRC;
using NetIRC.Messages;

// Build the client
using var client = Client.CreateBuilder()
    .WithNick("MyBot", "My IRC Bot")
    .WithServer("irc.libera.chat", 6667)
    .Build();

// Called once the server confirms registration (001 Welcome)
client.RegistrationCompleted += async (sender, e) =>
{
    if (sender is Client c)
        await c.SendAsync(new JoinMessage("#mychannel"));
};

// Called for every channel message
client.Channels.CollectionChanged += (s, e) =>
{
    foreach (Channel ch in e.NewItems ?? Array.Empty<object>())
    {
        ch.Messages.CollectionChanged += (ms, me) =>
        {
            foreach (ChannelMessage msg in me.NewItems ?? Array.Empty<object>())
                Console.WriteLine($"[{ch.Name}] <{msg.User.Nick}> {msg.Text}");
        };
    }
};

await client.ConnectAsync();
await Task.Delay(Timeout.Infinite); // keep alive
```

A fully commented, runnable example is in [`samples/NetIRC.ConsoleCli`](samples/NetIRC.ConsoleCli/Program.cs).

---

## Core Concepts

### Client

`Client` is the entry point. Create one via the fluent builder:

```csharp
var client = Client.CreateBuilder()
    .WithNick("MyNick", "My Real Name")   // sets the IRC nick and USER real name
    .WithServer("irc.libera.chat", 6667)  // host and port
    .WithPassword("serverpassword")        // optional: server password (PASS command)
    .Build();
```

Or directly via constructor if you need fine-grained control over the connection:

```csharp
var user = new User("MyNick", "My Real Name");
var connection = new TcpClientConnection("irc.libera.chat", 6667);
using var client = new Client(user, connection);
```

### User

Represents an IRC user. Implements `INotifyPropertyChanged` so the `Nick` property can be bound directly in WPF/MAUI.

```csharp
Console.WriteLine(client.User.Nick);
```

### Channel & Query

| Type | Description |
|------|-------------|
| `Channel` | An IRC channel the client has joined. Has an observable `Users` and `Messages` collection. |
| `Query` | A private conversation with another user. Has an observable `Messages` collection. |

Both are created automatically by the library when a `JOIN` or `PRIVMSG` (private) is received.

### Observable Collections

All collections implement `ObservableCollection<T>` so you can subscribe to `CollectionChanged`:

| Property | Type | Contains |
|----------|------|---------|
| `client.Channels` | `ChannelCollection` | Channels you have joined |
| `client.Queries` | `QueryCollection` | Active private conversations |
| `client.Peers` | `UserCollection` | All known users |
| `client.ServerMessages` | `ServerMessageCollection` | Server status / motd lines |

---

## Events

| Event | Signature | Fired when |
|-------|-----------|-----------|
| `RawDataReceived` | `(Client client, string rawData)` | Any raw line is received from the server |
| `IRCMessageParsed` | `(Client client, ParsedIRCMessage msg)` | A raw line has been parsed into a structured message |
| `RegistrationCompleted` | `(object sender, EventArgs e)` | Server sends `001` (Welcome) — safe to join channels |
| `CtcpReceived` | `(Client client, CtcpEventArgs ctcp)` | A CTCP request or reply is received |

```csharp
client.RawDataReceived += (c, raw) => Console.WriteLine(raw);

client.IRCMessageParsed += (c, msg) =>
{
    if (msg.IRCCommand == IRCCommand.TOPIC)
        Console.WriteLine($"Topic on {msg.Parameters[0]}: {msg.Trailing}");
};
```

---

## Sending Messages

Use `SendAsync` with any `IClientMessage` implementation:

```csharp
await client.SendAsync(new JoinMessage("#channel"));
await client.SendAsync(new PartMessage("#channel", "Goodbye!"));
await client.SendAsync(new PrivMsgMessage("#channel", "Hello, world!"));
await client.SendAsync(new NickMessage("NewNick"));
await client.SendAsync(new TopicMessage("#channel", "New topic here"));
await client.SendAsync(new KickMessage("#channel", "SomeUser", "Reason"));
await client.SendAsync(new QuitMessage("Leaving"));
```

For raw protocol strings:

```csharp
await client.SendRaw("MODE #channel +m\r\n");
```

### Built-in Message Types

| Class | IRC Command | Direction |
|-------|------------|-----------|
| `JoinMessage` | `JOIN` | Client → Server |
| `PartMessage` | `PART` | Client → Server |
| `PrivMsgMessage` | `PRIVMSG` | Both |
| `NickMessage` | `NICK` | Both |
| `TopicMessage` | `TOPIC` | Both |
| `KickMessage` | `KICK` | Both |
| `QuitMessage` | `QUIT` | Both |
| `NoticeMessage` | `NOTICE` | Server → Client |
| `ModeMessage` | `MODE` | Server → Client |
| `PingMessage` / `PongMessage` | `PING` / `PONG` | Both (auto-handled) |
| `PassMessage` | `PASS` | Client → Server |
| `UserMessage` | `USER` | Client → Server |

---

## Custom Message Handlers

Implement a custom handler by deriving from `CustomMessageHandler<TMessage>` and decorating it with `[Command]`:

```csharp
using NetIRC.Messages;

[Command(IRCCommand.PRIVMSG)]
public class PrivMsgHandler : CustomMessageHandler<PrivMsgMessage>
{
    public override Task HandleAsync(Client client, PrivMsgMessage message)
    {
        Console.WriteLine($"<{message.From}> {message.Message}");
        return Task.CompletedTask;
    }
}
```

Register it on the client:

```csharp
// Register a specific handler type
client.RegisterCustomMessageHandler<PrivMsgHandler>();

// Or auto-discover all handlers in an assembly
client.RegisterCustomMessageHandlers(typeof(Program).Assembly);
```

---

## CTCP Support

The following CTCP commands are handled automatically:

| Command | Behaviour |
|---------|-----------|
| `VERSION` | Replies with `NetIRC` |
| `TIME` | Replies with the current local time |
| `PING` | Echoes the timestamp back |
| `CLIENTINFO` | Lists supported CTCP commands |
| `ACTION` | Raises `CtcpReceived` |

Subscribe to `CtcpReceived` to handle additional or custom CTCP messages:

```csharp
client.CtcpReceived += (c, ctcp) =>
{
    Console.WriteLine($"CTCP {ctcp.Command} from {ctcp.Nick}");
};
```

---

## Connection Types

### TcpClientConnection (default)

Standard IRC over plain TCP. Used by the builder by default.

```csharp
var connection = new TcpClientConnection("irc.libera.chat", 6667);
```

### WebSocketClientConnection

For IRC servers that expose a WebSocket endpoint (e.g. KiwiIRC gateway):

```csharp
var connection = new WebSocketClientConnection(new Uri("wss://irc.example.com/socket"));
using var client = new Client(user, connection);
```

---

## WPF / UI Thread Dispatcher

If you are building a WPF or WinForms application, observable collection changes must happen on the UI thread. Pass the dispatcher before connecting:

```csharp
// WPF
client.SetDispatcherInvoker(action => Application.Current.Dispatcher.Invoke(action));
```

---

## Projects Built with NetIRC

- [NetIRC.Desktop](https://github.com/fredimachado/NetIRC.Desktop) — Simple WPF Desktop IRC Client

Feel free to open a pull request to add your project to this list.

---

## Contributing

Contributions are welcome! Open an issue to discuss a bug or feature, then submit a pull request.

1. Fork the repo and create a branch from `master`
2. Make your changes and add / update tests under `tests/NetIRC.Tests`
3. Ensure `dotnet test` passes
4. Open a pull request with a clear description
