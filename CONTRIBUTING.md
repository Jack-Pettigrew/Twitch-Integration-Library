# Contributing

If you'd like to help improve or extend the library, you're more than welcome to!

Currently, there are many Twitch event subscriptions that still need creating according to Twitch's documentation. If you want to take a stab at creating some, take a look at the [`ChannelChatMessageEventSub`](https://github.com/Jack-Pettigrew/Twitch-Integration-Library/blob/main/twitch_api/event_subs/ChannelChatMessageEventSub.cs) IEventSub.

Before doing so, please ensure you've read and followed the guide below to make the process easier for everyone.

> [!NOTE]
> **Most Importantly: All skill levels are welcome!** Don't be put off if this is your first contribution or you're unsure of your own skill.
>
> This library was made by someone with the brain of a newt (me) - you might even be able to tell. Everyone is welcome!

> [!IMPORTANT]
> By contributing, you agree that your contribution has been provided legally and does not represent code of proprietary software of which you do not have rights.

## Documentation / Doc Blocks - Contribution Policy

- Any and all improvements to the documentation and code doc blocks are welcome.
- Document all new features and functions.
- Ensure all documentation is clear and accurate.

## Code - Contribution Policy

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

## Pull Requests

All Pull Requests must contain sufficient descriptions fully detailing its changes and whether it has been tested. A PR may not be actioned until it has this information.

If using GitHub Copilot description, please ensure it is accurate.