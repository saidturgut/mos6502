namespace mos6502;

public class Bus
{
    private readonly Rom Rom = new();
    private readonly Ram Ram = new();
    private readonly Tty Tty = new();

    private Device[] Devices = [];
    
    public void Init()
    {
        Rom.Init();
        Devices[0] = Rom.Device;
        Ram.Init();
        Devices[1] = Ram.Device;
        Tty.Init();
        Devices[2] = Tty.Device;
    }
    
    public byte Read(ushort address)
    {
        foreach (var device in Devices)
        {
            if (address >= device.Min && address <= device.Max)
            {
                return device.Read(address);
            }
        }
        throw new Exception($"REGION TO READ \"{address}\" NOT FOUND");
    }
    
    public void Write(ushort address, byte data)
    {
        foreach (Device device in Devices)
        {
            if (address >= device.Min && address <= device.Max)
            {
                device.Write(address, data);
            }
            return;
        }
        throw new Exception($"REGION TO WRITE \"{address}\" NOT FOUND");
    }
}

public class Device
{
    public ushort Min;
    public ushort Max;
    public Func<ushort, byte> Read;
    public Action<ushort, byte> Write;
} 