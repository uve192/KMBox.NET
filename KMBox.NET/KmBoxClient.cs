using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using JetBrains.Annotations;
using KMBox.NET.Structures;

namespace KMBox.NET;

/// <summary>
/// KMBox UDP client.
/// </summary>
[PublicAPI]
public class KmBoxClient
{
    private readonly IPAddress _remoteEndpoint;
    internal readonly int Port;
    private readonly string _mac;
    private readonly UdpClient _udpClient = new();

    private uint _currentIndexPts;
    private ReportListener? _latestReportListener;

    /// <summary>
    /// Creates new client.
    /// </summary>
    /// <param name="remoteEndpoint">KMBox's IP address.</param>
    /// <param name="port">KMBox's port.</param>
    /// <param name="mac">KMBox's UUID/MAC. Example: 417F0CD3</param>
    public KmBoxClient(IPAddress remoteEndpoint, int port, string mac)
    {
        _remoteEndpoint = remoteEndpoint;
        Port = port;
        _mac = mac;
    }

    /// <summary>
    /// Attempts to connect to KMBox.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> Connect()
    {
        _udpClient.Connect(_remoteEndpoint, Port);

        var request = NextCmdHead(KmCommand.CmdConnect);
        var response = await SendAndRecieve<CmdHeadT, CmdHeadT>(request);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Simple mouse movements. Instantly snaps cursor with defined offset.
    /// </summary>
    /// <param name="x">Offset on X axis. Can be positive or negative.</param>
    /// <param name="y">Offset on Y axis. Can be positive or negative.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseMoveSimple(short x, short y)
    {
        var request = NextCmdHead(KmCommand.CmdMouseMove);

        var currentMouseAction = new MouseAction
        {
            X = x,
            Y = y
        };

        var response = await SendAndRecieve<CmdHeadT, MouseAction, CmdHeadT>(request, currentMouseAction);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Interpolated mouse movement over specified time. <br/>
    /// That means mouse will move to <paramref name="x"/>, <paramref name="y"/> offset in <paramref name="ms"/> milliseconds.
    /// </summary>
    /// <param name="x">Offset on X axis. Can be positive or negative.</param>
    /// <param name="y">Offset on Y axis. Can be positive or negative.</param>
    /// <param name="ms">How long the movement should take.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseMoveAuto(short x, short y, uint ms)
    {
        var request = NextCmdHead(KmCommand.CmdMouseAutomove);
        request.rand = ms;

        var currentMouseAction = new MouseAction
        {
            X = x,
            Y = y
        };

        var response = await SendAndRecieve<CmdHeadT, MouseAction, CmdHeadT>(request, currentMouseAction);
        
        return CheckResponse(request, response);
    }
    
    /// <summary>
    /// Moves mouse using <a href="https://en.wikipedia.org/wiki/B%C3%A9zier_curve">Bézier curve</a>.
    /// </summary>
    /// <param name="x">Offset on X axis. Can be positive or negative.</param>
    /// <param name="y">Offset on Y axis. Can be positive or negative.</param>
    /// <param name="ms">How long the movement should take.</param>
    /// <param name="x1">X1</param>
    /// <param name="y1">Y1</param>
    /// <param name="x2">X2</param>
    /// <param name="y2">Y2</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseMoveBezier(int x, int y, uint ms, int x1, int y1, int x2, int y2)
    {
        var request = NextCmdHead(KmCommand.CmdBeizerMove);
        request.rand = ms;

        // ReSharper disable once UseObjectOrCollectionInitializer (Array's gotta be 10 elements)
        var currentMouseAction = new MouseAction
        {
            X = x,
            Y = y
        };
        
        currentMouseAction.Points[0] = x1;
        currentMouseAction.Points[1] = y1;
        currentMouseAction.Points[2] = x2;
        currentMouseAction.Points[3] = y2;

        var response = await SendAndRecieve<CmdHeadT, MouseAction, CmdHeadT>(request, currentMouseAction);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Clicks a mouse button.
    /// </summary>
    /// <param name="mouseButton">One or more mouse buttons to press.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseClick(params MouseButton[] mouseButton)
    {
        const KmCommand command = KmCommand.CmdMouseLeft; // Command doesn't seem to matter much here
        
        var request = NextCmdHead(command);

        var currentMouseAction = new MouseAction();

        foreach (var button in mouseButton)
        {
            currentMouseAction.Buttons |= (int) button;
        }
        
        var response = await SendAndRecieve<CmdHeadT, MouseAction, CmdHeadT>(request, currentMouseAction);
        
        return CheckResponse(request, response);
    }
    
    /// <summary>
    /// Clicks left mouse button.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseLeftClick()
    {
        return await MouseClick(MouseButton.MouseLeft);
    }
    
    /// <summary>
    /// Clicks right mouse button.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseRightClick()
    {
        return await MouseClick(MouseButton.MouseRight);
    }
    
    /// <summary>
    /// Clicks middle mouse button.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseMiddleClick()
    {
        return await MouseClick(MouseButton.MouseMiddle);
    }

    /// <summary>
    /// Release all mouse buttons.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> AllMouseButtonsUp()
    {
        return await MouseClick();
    }

    /// <summary>
    /// Scroll mouse wheel.
    /// </summary>
    /// <param name="wheel">Scroll amount. Can be positive or negative.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MouseWheel(int wheel)
    {
        var request = NextCmdHead(KmCommand.CmdMouseWheel);

        var currentMouseAction = new MouseAction
        {
            Wheel = wheel
        };

        var response = await SendAndRecieve<CmdHeadT, MouseAction, CmdHeadT>(request, currentMouseAction);
        
        return CheckResponse(request, response);
    }
    
    /// <summary>
    /// Pressed keyboard button.
    /// </summary>
    /// <param name="button">Keyboard button.</param>
    /// <param name="modifiers">Keyboard button modifier(s): Ctrl, Shift, Alt, etc..</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> KeyboardButtonDown(KeyboardButton button, KeyboardModifiers modifiers = 0)
    {
        var request = NextCmdHead(KmCommand.CmdKeyboardAll);

        var currentKeyboardAction = new KeyboardAction();
        
        currentKeyboardAction.SetButtons(button, modifiers);
        
        var response = await SendAndRecieve<CmdHeadT, KeyboardAction, CmdHeadT>(request, currentKeyboardAction);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Types text via keyboard.
    /// </summary>
    /// <param name="text">Text to type.</param>
    /// <param name="delay">Delay between each character.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    /// <exception cref="InvalidOperationException">This character is unsupported. You should manually add support in <see cref="KeyboardButtonMap"/>.</exception>
    public async Task<bool> TypeText(string text, int delay = 80)
    {
        char? lastCharacter = null;
        TimeSpan? delayTimeSpan = delay != 0 ? TimeSpan.FromMilliseconds(delay) : null;
        
        foreach (var character in text)
        {
            var (button, modifiers) = KeyboardButtonMap.CharToKeyboardButton.GetValueOrDefault(character);

            if (button == default && modifiers == default)
                throw new InvalidOperationException($"Character '{character}' is not supported!");

            if (lastCharacter == character)
            {
                // Two repeating characters are causing some weird behaviour.
                // We gotta send empty command before sending that letter again.
                await AllKeyboardButtonsUp();
            }
            
            if (!await KeyboardButtonDown(button, modifiers))
                return false;

            lastCharacter = character;

            if (delayTimeSpan != null)
                await Task.Delay(delayTimeSpan.Value);
        }

        await AllKeyboardButtonsUp();
        return true;
    }
    
    /// <summary>
    /// Releases all keyboard buttons.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> AllKeyboardButtonsUp()
    {
        var request = NextCmdHead(KmCommand.CmdKeyboardAll);
        var response = await SendAndRecieve<CmdHeadT, KeyboardAction, CmdHeadT>(request, new KeyboardAction());
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Sends raw mouse command. Do not use if you don't need it.
    /// </summary>
    /// <param name="command">KMBox command ID.</param>
    /// <param name="mouseAction">Mouse action.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> SendRawMouse(KmCommand command, MouseAction mouseAction)
    {
        var request = NextCmdHead(command);
        var response = await SendAndRecieve<CmdHeadT, MouseAction, CmdHeadT>(request, mouseAction);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Sends raw keyboard command. Do not use if you don't need it.
    /// </summary>
    /// <param name="command">KMBox command ID.</param>
    /// <param name="keyboardAction">Keyboard action.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> SendRawKeyboard(KmCommand command, KeyboardAction keyboardAction)
    {
        var request = NextCmdHead(command);
        var response = await SendAndRecieve<CmdHeadT, KeyboardAction, CmdHeadT>(request, keyboardAction);
        
        return CheckResponse(request, response);
    }
    
    /// <summary>
    /// Sets KMBox's IP and port. <br/>
    /// Manual reboot of KMBox or <see cref="Reboot"/> is required to apply changes!
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> SetConfig(IPAddress newIpAddress, ushort newPort)
    {
        if (newIpAddress.AddressFamily != AddressFamily.InterNetwork)
            throw new InvalidOperationException("IPv4 address expected here!");

        if (!IsInKmBoxSubnet(newIpAddress))
            throw new InvalidOperationException("Must be in KMBox's subnet! Subnet expected: 192.168.2.*");
        
        var request = NextCmdHead(KmCommand.CmdSetConfig);
        var addressAsBytes = newIpAddress.GetAddressBytes();
        var portAsBytes = BitConverter.GetBytes(newPort);

        request.rand = BitConverter.ToUInt32(addressAsBytes);

        // ReSharper disable once UseObjectOrCollectionInitializer
        var setConfig = new SetConfigCommand();
        
        setConfig.Port[0] = portAsBytes[1];
        setConfig.Port[1] = portAsBytes[0];
        
        var response = await SendAndRecieve<CmdHeadT, SetConfigCommand, CmdHeadT>(request, setConfig);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Draws image on screen. Fascinating format which I do not understand. <br/>
    /// Google is your friend and there are some clues: <br/>
    /// - Image2Lcd <br/>
    /// - C array <br/>
    /// - Horizon Scan <br/>
    /// - 16-bit True Color <br/>
    /// - 128x160 <br/>
    /// - All checkboxes off <br/>
    /// - R: 5bits, G: 6bits, B: 5bits.
    /// </summary>
    /// <param name="buff128160"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<bool> SetImage(byte[] buff128160)
    {
        if (buff128160.Length != 40960)
            throw new ArgumentException("Image array must be exactly 40960 bytes in length.", nameof(buff128160));
        
        var buffer = new ScreenLineBuffer
        {
            Buffer = new byte[1024]
        };
        
        for (var line = 0; line < 40; line++)
        {
            var request = NextCmdHead(KmCommand.CmdShowPicture);
            request.rand = (uint) (line * 4);
            
            Array.Copy(buff128160, line * 1024, buffer.Buffer, 0, 1024);
            
            var response = await SendAndRecieve<CmdHeadT, ScreenLineBuffer, CmdHeadT>(request, buffer);

            if (!CheckResponse(request, response))
                return false;
        }

        return true;
    }
    
    // Pasted this straight from ChatGPT
    // I am NOT going to figure out how subnet masks work
    private static bool IsInKmBoxSubnet(IPAddress ipAddress)
    {
        var subnetMask = IPAddress.Parse("255.255.255.0");
        var subnetAddress = IPAddress.Parse("192.168.2.0");

        // Get the bytes of the IP address and subnet mask
        var ipAddressBytes = ipAddress.GetAddressBytes();
        var subnetMaskBytes = subnetMask.GetAddressBytes();
        var subnetAddressBytes = subnetAddress.GetAddressBytes();

        // Perform bitwise AND operation between IP address and subnet mask
        var resultBytes = new byte[ipAddressBytes.Length];
        for (var i = 0; i < ipAddressBytes.Length; i++)
        {
            resultBytes[i] = (byte)(ipAddressBytes[i] & subnetMaskBytes[i]);
        }

        // Check if the result matches the subnet address
        var isInSubnet = true;
        
        for (var i = 0; i < ipAddressBytes.Length; i++)
        {
            if (resultBytes[i] == subnetAddressBytes[i])
                continue;
            
            isInSubnet = false;
            break;
        }

        return isInSubnet;
    }
    
    /// <summary>
    /// Reboots KMBox.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> Reboot()
    {
        var request = NextCmdHead(KmCommand.CmdReboot);
        var response = await SendAndRecieve<CmdHeadT, CmdHeadT>(request);
        
        return CheckResponse(request, response);
    }

    internal async Task<bool> EnableMonitor(bool enable)
    {
        var request = NextCmdHead(KmCommand.CmdMonitor);

        // I don't know what's happening here.
        // Good luck figuring that out!
        request.rand = enable ? (uint)(((ushort) Port + 1) | (0xaa55U << 16)) : 0; 
        
        var response = await SendAndRecieve<CmdHeadT, CmdHeadT>(request);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Disables (masks) certain mouse input. It is possible to combine multiple mouse masks at the same time.
    /// </summary>
    /// <param name="masks">Mouse masks.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MaskMouseInput(MouseMasks masks)
    {
        var request = NextCmdHead(KmCommand.CmdMask);

        request.rand = (uint) masks;
        
        var response = await SendAndRecieve<CmdHeadT, CmdHeadT>(request);
        
        return CheckResponse(request, response);
    }
    
    /// <summary>
    /// Disables (masks) certain keyboard button. 
    /// </summary>
    /// <param name="keyboardButton">Keyboard button to disable.</param>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> MaskKeyboardButton(KeyboardButton keyboardButton)
    {
        var request = NextCmdHead(KmCommand.CmdMask);

        request.rand = (uint) keyboardButton << 8;
        
        var response = await SendAndRecieve<CmdHeadT, CmdHeadT>(request);
        
        return CheckResponse(request, response);
    }
    
    /// <summary>
    /// Unmasks all keyboard/mouse input.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    public async Task<bool> UnmaskAllInput()
    {
        var request = NextCmdHead(KmCommand.CmdUnmaskAll);

        request.rand = 0;
        
        var response = await SendAndRecieve<CmdHeadT, CmdHeadT>(request);
        
        return CheckResponse(request, response);
    }

    /// <summary>
    /// Creates report listener. Allows to listen to mouse movements/keyboard button presses.
    /// </summary>
    /// <returns><see langword="true" /> if successful.</returns>
    /// <exception cref="InvalidOperationException">You should call <see cref="ReportListener.Stop"/> before getting another <see cref="ReportListener"/>. Only 1 can be active at the same time.</exception>
    public ReportListener CreateReportListener()
    {
        if (_latestReportListener is { Stopped: false })
            throw new InvalidOperationException("Last report listener was not stopped. Can not create a new one!");

        _latestReportListener = new ReportListener(this);
        return _latestReportListener;
    }
    
    private async Task<TResponse> SendAndRecieve<THead, TResponse>(THead request)
        where THead : struct
        where TResponse : struct
    {
        var serialized = StructHelper.StructToByteArray(request);

        await _udpClient.SendAsync(serialized);
        
        var response = await _udpClient.ReceiveAsync();

        return StructHelper.ByteArrayToStruct<TResponse>(response.Buffer);
    }
    
    private async Task<TResponse> SendAndRecieve<THead, TRequest, TResponse>(THead head, TRequest request)
        where THead : struct
        where TRequest : struct
        where TResponse : struct
    {
        var serialized = StructHelper.StructToByteArray(head, request);

        await _udpClient.SendAsync(serialized);
        
        var response = await _udpClient.ReceiveAsync();

        return StructHelper.ByteArrayToStruct<TResponse>(response.Buffer);
    }

    private CmdHeadT NextCmdHead(KmCommand command)
    {
        return new CmdHeadT
        {
            mac = HexHelper.MacToUInt(_mac),
            rand = (uint)RandomNumberGenerator.GetInt32(int.MaxValue),
            indexpts = _currentIndexPts++,
            cmd = command
        };
    }

    private static bool CheckResponse(CmdHeadT request, CmdHeadT response)
    {
        return request.cmd == response.cmd &&
               request.indexpts == response.indexpts;
    }
}