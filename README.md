# LightDE
LightDE is a complete bootleg of your Windows OS Experience. It's a complete Desktop Environment with it's own panel, app switcher, App Drawer. It's material design-driven but we're making some variantions here and there ;)
We're managed  to get most of WINAPI but it's poor documentation makes it hard so we have to reverse engineer all the things!
# Current State
We're currently working on Core of the whole Desktop Environment. We're also doing some redesign to make LightDE more Eye-catching.
![Alt text](/preview.jpg?raw=true "Screenshot")
# Structure Of LightDE
LightDE consists of programs which are modules, communicating via IPC(Inter-Process Communication) on port 12345 
Core is the core of whole Environment
UI is an ui Layer, starting with Core and depends on it. It contains Desktop, Panel and App Drawer
Dock is a app switcher dock which can be replaced by your favourite dock app (like RocketDock or somethin')
Config is a config helper, but you can control settings through json file (LightDE directory/config/config.json)
# Wanna Help us?! 
For now we are not accepting any new team members, but you can help by catching bugs etc. Add an Issue and we're figure it out for you!
Remember that we're rather enthustiasts not adult developers making a consumer ready product. So Expect bugs and some mind-blows in design itself (I can't swear on Github :C ).
# Disclaimer
It's not a product for consumer use.
It's rather a call to Microsoft about how we see the future of Windows: 
Customizing your os from the Bottom to the Top. 
So, If any Microsoft developer ever will read it, this is our note: Make customizing Windows more available! No icon packs, No explorer theming?! If you guys will continue to stagnate, Linux is gonna kill you...


 # Join the team 
 Do you want to collaborate? Join the project at https://crowdforge.io/projects/260