# Twitch-Integration-Library

An open source Twitch Integration Library for C# based Video Games and Apps.

Connect your game or app to Twitch and create engaging experiences. Embed chat features or react to Twitch events in game!

- Built, tested and utilised within my own project
- Engine-agnostic - use with Godot .NET, Unity or any other modern .NET engine
- Designed for simplicity, extensibility and plug-and-play
- Event-driven subscription
- Authentication built-in
  - Device Code Grant Flow with polling
  - Automatic token refresh

## Table of Contents

- Getting Started
- Issues
- Contributing
- Suggestions

## 💻 Getting Started

### Requirements

#### Technical

While the library is engine-agnostic uses no external dependencies, it does have extremely minimum requirements:

- .NET 8.0+
- C# 10+

As long as your engine or app supports the above, you're good to go!

#### Twitch

Before you can use this library, you will need to register a new app on [Twitch's Developer Console](https://dev.twitch.tv/).

Registering allows you to utilise Twitch's API as a known application, while obtaining your super secret `CLIENT_ID` and `CLIENT_SECRET` (the latter is dependant on your use case).

For more information on setting this up, please refer to [Twitch's Developer Documentation](https://dev.twitch.tv/docs/authentication/register-app/).

### Installing / Updating

1. Clone the repo, or download the [most recent release here](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/releases).
2. Move the downloaded code into your project.

If you're updating, **remember to backup any details in the `Env`** (as these will be removed) and simply overwrite to contents of the library within your project.

### Using the Library

This is dependant on your game engine or app, but it's use is largely the same.

Below is an example from Godot 4.6 .NET, but the general idea is universal.

[CODE EXAMPLES]

## 🔴 Issues

Should you find any issues with the library, feel free to commit changes or create an Issue on the repository with the relevant information.

Issues will be picked up as and when there's capacity, of which, myself and contributors will do our best to address them when we can.

> [!IMPORTANT]
> Issues not labelled correctly will not be picked up.

### External Service Changes

As this library works with an external service, changes made on _that_ service will, of course, not be reflect in this repository. Changes that mirror updated functionality of that service will need to be made by contributors.

If you identify this is the case, please create an Issue with the `Service Change` label.

## 🙌🏻 Contributing

If you'd like to help improve or extend the library, you're more than welcome to!

Before doing so, please ensure you've read and follow the guide below to make the process easier for everyone.

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
  - Try your best not to over engineer solutions - sometimes context demands complexity and that's okay
- You Ain't Gonna Need It ([YAGNI](https://martinfowler.com/bliki/Yagni.html))
  - Don't build something just because you _think_ it'll be needed
  - This does not apply to foundations of planned work
- Please keep code style consistent with existing code
  - Everyone is opinionated but being stubborn can get in the way of making cool stuff
- Variables names must be sensible and descriptive
  - Not overly descriptive but enough to 'get it'
  - Single letter variable names will see your PR slipped into a shredder
- Hardcoding & Magic Numbers
  - Steer away from these two things unless it is absolutely necessary
- Sensitive Information
  - When contributing code, ensure not to commit any tokens or identifiers
  - This is a library working with an external service and can be common to do so by accident
  - If you have committed sensitive information, **regenerate all those with apply to ensure maximum security**

### Pull Requests

All Pull Requests must contain sufficient descriptions fully detailing it's changes and whether it has been tested. A PR may not be actioned until it has this information.

If using GitHub Copilot description, please ensure it is accurate.

## 💡 Suggestions

Suggestions are welcome!

You don't need to be able make code changes to help. Implementing and using the library is enough for you to identify quality of life improvements.

Keep in mind, not all suggestions _will_ be implemented. This will be down to capacity or being out-of-scope.

To create a suggestion, create a new Issue on the repository with the `Suggestion` tag.

> [!IMPORTANT]
> Issues not labelled correctly will not be picked up.
