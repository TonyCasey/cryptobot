1. Hit F5 or Right Click on the exe and select "Run as Administrator" to run as a normal console app.

2. Run the following command in the Command Prompot as an adminstrator to Create a Windows Service which the exe can run under.
> sc create eCron_CryptoBot DisplayName="eCron_CryptoBot_Service" binPath="C:\Apps\CryptoBot\CryptoBot.exe"

Ensure you update the Windows Service to AutoStart or AutoStart(Delayed) to ensure the Service/Bot is started automaticially on a Windows restart.

You can use the following command to delete the service if needed.

> sc delete eCron_CryptoBot

3. If you want to do a fresh deploy of the application, you will need to Stop the Windows Service first (if in use).