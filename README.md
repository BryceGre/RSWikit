# RuneScape Wikit

Unnoficial RuneScape client that utilizes the RuneScape Wiki. (http://www.runescape.wiki)
This client is not owned or endorsed by Jagex or the RuneScape Wiki.

I don't really expect anyone to use this (in fact you shouldn't use random clients without checking the code for spyware) but I decided to upload it to GitHub anyway after sitting on it for a few years.

## Features

* Docks the official RuneScape client (RS3 or OSRS) into the RSWikit window, adding a sidebar and tabs:
  * This client in no way interacts with the official RuneScape client, therefore it is not against the rules.
  * I've been using a prototype of this client for years now, and haven't gotten banned.
* Adds a Toolbox to the sidebar
  * Time since last Grand Exchance update.
  * Calculator with memory and K and M buttons.
  * Stopwatch (with lap function) and counter.
  * Notepad that saves notes.
* Adds an option to open RuneScape Wiki pages to the sidebar:
  * Can open any number of pages.
  * Pages use a Chromium plugin (hence the dlls) so everything works correctly.
  * Pages display in mobile view for readability.
  * Right-click menu for:
    * Opening links in new tabs (in the client).
	* Copying link addresses.
	* Back and Forward.
	* View source.

## Building
* I'm not including a built binary, since you really should check the code before using any random client you find on the internet.
* Download everything and open the solution in VisualStudio. It should build and run fine.
* If not, check that the dll files, along with the "lib" and "ico" folders, are properly placed in the output directory.

## Requirements
* VisualStudio 2017 or newer.
* An Official RuneScape client (RS3 or OSRS) installed on your system.

## Installation
* Copy the items in the output directory (including the "lib" and "ico" folders, along with the CefSharp and MdiTabControl dlls) to a folder.
* Send RSWikit.exe to Desktop as a shortcut.

## Usage
1. Run RSWikit.exe. It should start the RuneScape client, and once that loads it should dock it into it's own client window.
2. You can play RuneScape through it's own client (which is now docked inside RSWikit) like normal.
3. You can use the toolbox in the sidebar right away.
4. You can open a new RuneScape Wiki tab in the sidebar by clicking the "+" button in the upper-right.
5. You can switch between normal and fullscreen using the fullscreen button in the upper-right.
6. When you're done, you may either close RSWikit or exit the RuneScape client. Closing one will close the other.

## Troubleshooting
* The client comes with the RuneScape 3 NZXT startup command that works as of this time.
* If you want to use Old-School RuneScape, or the client doesn't launch properly:
  * Find your RS3 or OSRS client shortcut on your Desktop.
  * Drag and Drop the shortcut over either the RSWikit client window.
  * If the RSWikit client window doesn't show, Drop the shortcut on the window in the upper-left of your screen.
  * If all else fails, there's a config.ini file where you can manually set the launch url.
    * If RSWikit.exe is in a writable directory, config.ini will be in the same directory as RSWikit.exe
    * IF RSWikit is in a protected directory (like ProgramFiles) then config.ini will be in C:\Users\{Username}\AppData\Local\VirtualStore\{Path to RSWikit.exe}\