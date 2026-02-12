# Twitch-Integration-Library

An open source Twitch Integration Library for C# based Video Games and Apps.

Connect your game or app to Twitch and create engaging experiences. Embed chat features or react to Twitch events in-game!

- Built, tested and utilised within my own project
- Engine-agnostic - use with Godot .NET, Unity or any other modern .NET engine
- Designed for simplicity, extensibility and plug-and-play
- Event-driven subscription
- Authentication built-in
  - Device Code Grant Flow with polling
  - Automatic token refresh

## Table of Contents

- [Getting Started](#getting-started)
- [Issues](#issues)
- [Contributing](#contributing)
- [Suggestions](#suggestions)

## Getting Started

### Requirements

#### Technical

While the library is engine-agnostic and uses no external dependencies, it does have extremely minimal requirements:

- .NET 8.0+
- C# 10+

As long as your engine or app supports the above, you're good to go!

#####  Using with Godot and GDScript

You can use this library even if you're primarily programming with GDScript. There are two requirements:
- You're using Godot 4.0+ .NET
- You convert the events in the [`EventRegistry`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/events/EventRegistry.cs) class to [C# Godot Signals](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html)
	- This is the only way for GDScript to 'communicate' with C# events

I'm looking to make this more convenient in the future, but with the interest of prioritising making this engine-agnostic, this is how to integrate with GDScript.

#### Twitch

Before you can use this library, you will need to register a new app on [Twitch's Developer Console](https://dev.twitch.tv/).

Registering allows you to utilise Twitch's API as a known application, while obtaining your super secret `CLIENT_ID` and `CLIENT_SECRET` (the latter is dependent on your use case).

For more information on setting this up, please refer to [Twitch's Developer Documentation](https://dev.twitch.tv/docs/authentication/register-app/).

### Installing / Updating

1. Clone the repo, or download the [most recent release here](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/releases).
2. Move the downloaded code into your project.

If you're updating, **remember to backup any details in the `Env`** (as these will be removed) and simply overwrite to contents of the library within your project.

### Using the Library

#### Connecting/Disconnecting from Twitch

The majority of client functionality is handled through [`TwitchClient`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/client/TwitchClient.cs). Using this class depends on your game engine or app, but it's largely universal.

Below is an example of using the client in a custom Godot Node to connect to Twitch.

**1.** Create a `TwitchClient` reference and initialise with your details and subscriptions

  ```c#
  public partial class TwitchClientNode : Node
  {
	// Handle to our TwitchClient object
  	private TwitchClient twitchClient;
  
  	public override void _Ready()
  	{
  		// Initialise Twitch Client with details and subscriptions
  		twitchClient = new TwitchClient(Env.CLIENT_ID, Env.USER_ID, new IEventSub[]
  		{
  			new ChannelChatMessage()
  		});

      // ...
  	}
  }
  ```

**2.** Call [`TwitchClient::ConnectToTwitchAsync()`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/client/TwitchClient.cs) to connect to Twitch

```c#
public override void _Ready()
{
	// Initialise Twitch Client with details and subscriptions
	twitchClient = new TwitchClient(Env.CLIENT_ID, Env.USER_ID, new IEventSub[]
	{
		new ChannelChatMessage()
	});

	// Defer to an async function for more control over initialisation (optional)
	_ = StartTwitchAsync();
}

public async Task StartTwitchAsync()
{
	// Catch Exceptions
	try
	{
		// Connect to Twitch
		bool success = await twitchClient.ConnectToTwitchAsync();

		// ...
	}
	catch (Exception e)
	{
		// Print any errors to the Godot console
		GD.PrintErr(e.Message);
		GD.PrintErr(e.StackTrace);
	}
}
```

**3.** Implement disconnect functionality (optional if you never want to disconnect ever again)

```c#
public override void _ExitTree()
{
	// Disconnect from Twitch
	_ = twitchClient?.DisconnectFromTwitchAsync();
}

public async Task StopTwitchAsync()
{
	// Catch Exceptions
	try
	{
		// Disconnect from Twitch
		await twitchClient.DisconnectFromTwitchAsync();

		// ...
	}
	catch (Exception e)
	{
		// Print any errors to the Godot console
		GD.PrintErr(e.Message);
		GD.PrintErr(e.StackTrace);
	}
}
```

**4.** All done!

##### Full Code
```c#
using Godot;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TIL;
using TIL.Client;
using TIL.Events;
using TIL.TwitchAPI.EventSubs;

public partial class TwitchClientNode : Node
{
	// Handle to our TwitchClient object
	private TwitchClient twitchClient;

	public override void _Ready()
	{
		// Initialise Twitch Client with details and subscriptions
		twitchClient = new TwitchClient(Env.CLIENT_ID, Env.USER_ID, new IEventSub[]
		{
			new ChannelChatMessage()
		});

		_ = StartTwitchAsync();
	}

	public override void _ExitTree()
	{
		// Disconnect from Twitch
		_ = twitchClient?.DisconnectFromTwitchAsync();
	}

	public async Task StartTwitchAsync()
	{
		// Catch Exceptions
		try
		{
			// Connect to Twitch
			bool success = await twitchClient.ConnectToTwitchAsync();

			// ...
		}
		catch (Exception e)
		{
			// Print any errors to the Godot console
			GD.PrintErr(e.Message);
			GD.PrintErr(e.StackTrace);
		}
	}

	public async Task StopTwitchAsync()
	{
		// Catch Exceptions
		try
		{
			// Disconnect from Twitch
			await twitchClient.DisconnectFromTwitchAsync();

			// ...
		}
		catch (Exception e)
		{
			// Print any errors to the Godot console
			GD.PrintErr(e.Message);
			GD.PrintErr(e.StackTrace);
		}
	}
}
```

#### Event Subscriptions

Event Subscriptions are the cornerstone of how the library tells Twitch which events we'd like to receive.

Each class intended for subscribe to events implements the `IEventSub` interface, creating the an event sub 'context.

For example, see [`ChannelChatMessage`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/twitch_api/event_subs/ChannelChatMessage.cs). This class defines:
- the payload of the subscription network request for that event subscription
- the logic that is ran in the processing of this event

The purpose of these classes is to decouple unique event logic to their events and away from the rest of the library, _or your code_.

> [!NOTE]
> There are only so many of these events as only a handful of them have been used so far.
>
> If you're like to contribute more of these according to the Twitch documentation, please see the Contribution section.

#### Reacting to Twitch Events

To hook your project up to events Twitch sends, simply subscribe to one of the many events found within the [`EventRegistry`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/events/EventRegistry.cs) class with a valid function:

```c#
public override void _Ready()
{
	EventRegistry.ChatMessageReceived += ProcessChatter;
}

public override void _ExitTree()
{
	EventRegistry.ChatMessageReceived -= ProcessChatter;
}
```

Below is an example implementation of the above function handling the result of an event subscription from the `ChatMessageReceived` event:

```c#
public void ProcessChatter(ChatMessage chatMessage)
{
	GD.Print($"{chatMessge.UserName} said: {chatMessage.Message}");
}

```

> [!NOTE]
> To use with **GDScript** - convert the events in the [`EventRegistry`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/events/EventRegistry.cs) class to [C# Godot Signals](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html).
>
> This is the only way for GDScript to 'communicate' with C# events.
>
> I'm looking to make this more convenient in the future, but with the interest of prioritising making this engine-agnostic, this is how to integrate with GDScript.

## 🔴 Issues

Should you find any issues with the library, feel free to commit changes or create an Issue on the repository with the relevant information.

Issues will be picked up as and when there's capacity, of which, myself and contributors will do our best to address them when we can.

> [!IMPORTANT]
> Issues not labelled correctly will not be picked up.

### External Service Changes

As this library works with an external service, changes made to _that_ service will, of course, not be reflected in this repository. Changes that mirror the updated functionality of that service will need to be made by contributors.

If you identify this is the case, please create an Issue with the `Service Change` label.

## 🙌🏻 Contributing

If you'd like to help improve or extend the library, you're more than welcome to!

Currently, there are many Twitch event subscriptions that still need creating according to Twitch's documentation. If you want to take a stab at creating some, take a look at the [`ChannelChatMessage`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/twitch_api/event_subs/ChannelChatMessage.cs) IEventSub.

Before doing so, please ensure you've read and followed the guide below to make the process easier for everyone.

> [!NOTE]
> **Most Importantly: All skill levels are welcome!** Don't be put off if this is your first contribution or you're unsure of your own skill.
>
> This library was made by someone with the brain of a newt (me) - you might even be able to tell. Everyone is welcome!

### Documentation / Doc Blocks - Contribution Policy

- Any and all improvements to the documentation and code doc blocks are welcome.
- Document all new features and functions.
- Ensure all documentation is clear and accurate.

### Code - Contribution Policy

- **AI Code**
  - AI-assisted code **is not** currently accepted for feature development or architectural changes
  - While AI tools can be helpful, they often:
    - produce code that does not fully align with the project's design goals
    - introduce subtle inconsistencies
    - bypass documented architectural decisions
    - generate surface-level implementations without full context
- Keep It Simple Stupid ([KISS](https://en.wikipedia.org/wiki/KISS_principle))
  - Try your best not to over-engineer solutions - sometimes context demands complexity and that's okay
- You Ain't Gonna Need It ([YAGNI](https://martinfowler.com/bliki/Yagni.html))
  - Don't build something just because you _think_ it'll be needed
  - This does not apply to the foundations of planned work
- Please keep code style consistent with existing code
  - Everyone is opinionated, but being stubborn can get in the way of making cool stuff
- Variable names must be sensible and descriptive
  - Not overly descriptive but enough to 'get it'
  - Single-letter variable names will see your PR slipped into a shredder
- Hardcoding & Magic Numbers
  - Steer away from these two things unless it is absolutely necessary
- Sensitive Information
  - When contributing code, ensure not to commit any tokens or identifiers
  - This is a library working with an external service, and it can be common to do so by accident
  - If you have committed sensitive information, **regenerate all those that apply to ensure maximum security**

### Pull Requests

All Pull Requests must contain sufficient descriptions fully detailing its changes and whether it has been tested. A PR may not be actioned until it has this information.

If using GitHub Copilot description, please ensure it is accurate.

## 💡 Suggestions

Suggestions are welcome!

You don't need to be able make code changes to help. Implementing and using the library is enough for you to identify quality of life improvements.

Keep in mind, not all suggestions _will_ be implemented. This will be down to capacity or being out-of-scope.

To create a suggestion, create a new Issue on the repository with the `Suggestion` tag.

> [!IMPORTANT]
> Issues not labelled correctly will not be picked up.
