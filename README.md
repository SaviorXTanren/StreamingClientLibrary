[![Build Status](https://travis-ci.org/SaviorXTanren/StreamingClientLibrary.svg?branch=master)](https://travis-ci.org/SaviorXTanren/StreamingClientLibrary)
Mixer: [![NuGet](https://img.shields.io/nuget/v/StreamingClientLibrary.Mixer.svg?style=flat)](https://www.nuget.org/packages/StreamingClientLibrary.Mixer)
Twitch: [![NuGet](https://img.shields.io/nuget/v/StreamingClientLibrary.Twitch.svg?style=flat)](https://www.nuget.org/packages/StreamingClientLibrary.Twitch)
YouTube: [![NuGet](https://img.shields.io/nuget/v/StreamingClientLibrary.YouTube.svg?style=flat)](https://www.nuget.org/packages/StreamingClientLibrary.YouTube)

# StreamingClientLibrary
C# client library for Mixer, Twitch, & YouTube streaming services

## What is this?
When this project was initially created, it was made to fill a gap that existed with lack of a .NET-based library for interaction with Mixer. As time has gone on, the project has added support for additional streaming sites and strives to build a "one-stop shop" to interact with variety of different sites in a consistent manner.

## Requirements
This library uses the native WebSocket support found in Windows 8 & higher. This means that any application that uses this library must be running Windows 8 or higher.

## Current functionality
All of the most common APIs are available across all sites. We've also added support for the various other forms of connections for each site:
- Chat/Constellation/MixPlay WebSockets for Mixer
- Chat IRC for Twitch, PubSub WebSockets for Twitch

## How do I get started using it?
Download the appropriate NuGet package based on the streaming site you want to work with:

Mixer: https://www.nuget.org/packages/StreamingClientLibrary.Mixer
Twitch: https://www.nuget.org/packages/StreamingClientLibrary.Twitch
YouTube: https://www.nuget.org/packages/StreamingClientLibrary.YouTube

There are sample apps available in this repository for each of the different streaming sites that you can reference for some of the more common scenarios. Additionally, there are a large series of unit tests that go through all of the individual functionality that you can look at.

## I found a bug, who do I contact?
Just head over to the https://github.com/SaviorXTanren/StreamingClientLibrary/issues page and create a new issue.

## I have a new feature idea!
Submit feature requests at the https://github.com/SaviorXTanren/StreamingClientLibrary/issues page or feel free to develop the feature yourself and submit a pull request at https://github.com/SaviorXTanren/StreamingClientLibrary/pulls.

## License
MIT License

Copyright (c) 2017-2020 Matthew Olivo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.