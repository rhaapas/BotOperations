# BotOperations
2D Unity app where a remote operator can see the real-time location of service bots in a factory area

# Building the project
1. Clone project files
2. Open project in Unity (2020.3.34f1)
3. Build project from File -> Build Settings -> Build

#Usage
##Host
1. Run built BotOperations.exe
2. Type your IP address in the input field labeled "Connection address".
	For LAN hosting, use "Local Address" (The first found IPv4 address is shown on the menu)
	For hosting over internet, use "Public Address" (Shown in menu) - Remember to forward the chosen port in your router.
	Defaults to localhost so it can be tested on a single computer.
3. Choose operating mode (Operator or Bot)
	In Bot mode it is possible to move around the area and create/use bots.
	In Operator mode all bots are seen and the area from a better view. Messages can be sent to selected bots.
4. Press 'Esc' to open Quit menu to quit the application.
