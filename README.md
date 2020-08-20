[![Build Status](https://travis-ci.org/SaviorXTanren/StreamingClientLibrary.svg?branch=master)](https://travis-ci.org/SaviorXTanren/StreamingClientLibrary)
# StreamingClientLibrary
C# client library for Twitch, YouTube, and other streaming services

## What is this?
When this project was initially created, it was made to fill a gap that existed with lack of a .NET-based library for interaction with Mixer. As time has gone on, the project has added support for additional streaming sites and strives to build a "one-stop shop" to interact with variety of different sites in a consistent manner.

## Requirements
This library uses the native WebSocket support found in Windows 8 & higher. This means that any application that uses this library must be running Windows 8 or higher.

## Current functionality
All of the most common APIs are available across all sites. We've also added support for the various other forms of connections for each site:
- Chat IRC for Twitch, PubSub WebSockets for Twitch
- Automated Chat Web Call for YouTube

## How do I get started using it?
Download the appropriate NuGet package based on the streaming site you want to work with:

Twitch: [![NuGet](https://img.shields.io/nuget/v/StreamingClientLibrary.Twitch.svg?style=flat)](https://www.nuget.org/packages/StreamingClientLibrary.Twitch)

YouTube: [![NuGet](https://img.shields.io/nuget/v/StreamingClientLibrary.YouTube.svg?style=flat)](https://www.nuget.org/packages/StreamingClientLibrary.YouTube)

There are sample apps available in this repository for each of the different streaming sites that you can reference for some of the more common scenarios. Additionally, there are a large series of unit tests that go through all of the individual functionality that you can look at.

## I found a bug, who do I contact?
Just head over to the https://github.com/SaviorXTanren/StreamingClientLibrary/issues page and create a new issue.

## I have a new feature idea!
Submit feature requests at the https://github.com/SaviorXTanren/StreamingClientLibrary/issues page or feel free to develop the feature yourself and submit a pull request at https://github.com/SaviorXTanren/StreamingClientLibrary/pulls.

## License
https://github.com/SaviorXTanren/StreamingClientLibrary/blob/master/LICENSE.txt
