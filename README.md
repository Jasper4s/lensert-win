# lensert-win
Lensert - A simple screenshot utility, windows version

## Prerequisites
 - Windows (Vista or higher)
 - .NET 4.5 (Pre-installed on Windows 8 and higher)
 - Internet
 
## Settings
After the reorganization of Lensert, the graphical interface has been removed.
This due it wasn't great and compatible with dpi scaling. Soon a new interface
will be made, till that happens the settings are configurable through a ini file.
This ini file can be found at: `%localappdata%\lensert\Settings.ini`

At this point only the hotkeys are configurable. The part after the = is the 
value which can be of any combination of keys. The valid enumeration can be found
[on msdn.][1]

Invalid settings are overwritten automatically and settings are only loaded on
startup. Thus when changing something the application must be restarted before
it takes effect.

## Logs
Since Lensert is lacking an interface, you should look at the log files for more 
information (i.e. when an error happend.) If you'd like to make an issue please
supply the log file. The file can be found at: ``%localappdata%\lensert\log.text`


[1]: https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx
