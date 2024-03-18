using System.Net;
using KMBox.NET;
using KMBox.NET.Structures;

const string mac = "<YOUR MAC HERE>";

var client = new KmBoxClient(IPAddress.Parse("<YOUR IP HERE>"), 8888 /* YOUR PORT HERE */, mac);

Console.WriteLine("Connecting..");

if (!await client.Connect())
{
    Console.WriteLine("Failed to connect!");
    return;
}

Console.WriteLine("Successfully connected.");

// Time for you to switch to Notepad, lol
await Task.Delay(TimeSpan.FromSeconds(2));

// Sending next mouse/keyboard command automatically cancels previous action!
// For example, calling MouseRightClick() after MouseLeftClick() will stop pressing Left mouse button.
// Same for goes for keyboard.

try
{
    Console.WriteLine("Pressing keys: U, C, Shift+F");
    
    // Pressing keyboard buttons
    await client.KeyboardButtonDown(KeyboardButton.KeyU); // Will press 'U' key
    await Task.Delay(TimeSpan.FromMilliseconds(200));
    
    await client.KeyboardButtonDown(KeyboardButton.KeyC); // Will press 'C' key
    await Task.Delay(TimeSpan.FromMilliseconds(200));

    // Pressing keyboard buttons with modifiers
    await client.KeyboardButtonDown(KeyboardButton.KeyF, KeyboardModifiers.LeftShift); // same as Shift+F
    await Task.Delay(TimeSpan.FromMilliseconds(200));

    Console.WriteLine("Writing newline..");
    
    // Newline!
    await client.KeyboardButtonDown(KeyboardButton.KeyEnter);
    await Task.Delay(TimeSpan.FromMilliseconds(200));
    
    // Stop pressing last keyboard key
    await client.AllKeyboardButtonsUp();
    await Task.Delay(TimeSpan.FromMilliseconds(200));

    Console.WriteLine("Typing random text..");
    
    // Type some random text
    await client.TypeText($"Hello world! Random GUID: {Guid.NewGuid():D}");

    Console.WriteLine("Pressing middle click..");
    
    // Pressing mouse buttons
    //await client.MouseRightClick();
    //await client.MouseLeftClick();
    await client.MouseMiddleClick();

    // Stop pressing last mouse button
    await client.AllMouseButtonsUp();

    Console.WriteLine("Moving mouse..");
    
    // Moving mouse
    await client.MouseMoveSimple(100, 100);
    await client.MouseMoveAuto(200, 200, 1000);
    await client.MouseMoveBezier(300, 300, 2000, 400, 400, 500, 500);

    Console.WriteLine("Scrolling up and down..");
    
    // Scrolling
    await client.MouseWheel(100);
    await client.MouseWheel(-100);
    
    // Updating box's IP address and port
    // Uncomment this if you actually want to change your box's IP and port!
    // await client.SetConfig(IPAddress.Parse("192.168.2.130"), 8888);
    
    // Rebooting box
    // await client.Reboot();

    Console.WriteLine("Uploading UC's logo..");
    
    await client.SetImage(ImageArrays.UnknownCheatsLogo);

    Console.WriteLine("Press 'F' key on keyboard connected to KMBox..");
    
    var middleClickPressedEvent = new ManualResetEventSlim(false);
    var listener = client.CreateReportListener();
    
    listener.EventListener = report =>
    {
        if(report.KeyboardReport.IsButtonPressed(KeyboardButton.KeyF))
            middleClickPressedEvent.Set();
    };
    
    listener.Start();
    middleClickPressedEvent.Wait();
    listener.Stop();
    
    Console.WriteLine("'F' was pressed!");
    
    Console.WriteLine("Masking keyboard button 'F'. You should not be able to use this buttons.");

    //await client.MaskMouseInput(MouseMasks.Right);
    //await client.MaskMouseInput(MouseMasks.Left | MouseMasks.Right | MouseMasks.Middle | MouseMasks.XMovement | MouseMasks.YMovement);
    await client.MaskKeyboardButton(KeyboardButton.KeyF);
    
    Console.WriteLine("Press any key on host to continue..");
    Console.ReadKey(true);

    await client.UnmaskAllInput();
    
    Console.WriteLine("Unmasked all input. Verify that right click now works!");
    
    Console.WriteLine("\n\n--------------------");
    Console.WriteLine("Showcase finished! You can now close this window or press CTRL+C to exit.");

    await Task.Delay(-1);
}
catch (Exception exception)
{
    Console.WriteLine(exception.ToString());
    
    await client.AllMouseButtonsUp();
    await client.AllKeyboardButtonsUp();

    Console.ReadLine();
}
