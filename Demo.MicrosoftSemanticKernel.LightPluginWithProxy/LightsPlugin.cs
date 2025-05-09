using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;


namespace Demo.MicrosoftSemanticKernel.LightPluginWithProxy;

public class LightsPlugin
{
    private readonly List<LightModel> lights = new()
    {
        new LightModel { Id = 1, Name = "Table Lamp", IsOn = false, Brightness = 100, Hex = "FF0000" },
        new LightModel { Id = 2, Name = "Porch Light", IsOn = false, Brightness = 50, Hex = "00FF00" },
        new LightModel { Id = 3, Name = "Chandelier", IsOn = true, Brightness = 75, Hex = "0000FF" }
    };

    [KernelFunction("get_lights")]
    [Description("Retrieves a list of lights and their current states.")]
    [return: Description("An array of lights.")]
    public Task<List<LightModel>> GetLightsAsync()
    {
        WriteStatus("GetLightsAsync");

        return Task.FromResult(lights);
    }

    [KernelFunction("turn_on_light")]
    [Description("Turns on the specified light by ID.")]
    [return: Description("The state of the light which was turned on.")]
    public Task<string> TurnOnLightAsync([Description("This is the identifier of the light.")]int id)
    {
        var light = lights.Find(l => l.Id == id);
        if (light != null)
        {
            light.IsOn = true;
            WriteStatus("TurnOnLightsAsync");
            return Task.FromResult($"{light.Name} is now ON.");
        }
        return Task.FromResult($"Light with ID {id} not found.");
    }

    [KernelFunction("turn_off_light")]
    [Description("Turns off the specified light by ID.")]
    [return: Description("The state of the light which was turned off.")]
    public Task<string> TurnOffLightAsync([Description("This is the identifier of the light.")] int id)
    {
        var light = lights.Find(l => l.Id == id);
        if (light != null)
        {
            light.IsOn = false;
            WriteStatus("TurnOffLightsAsync");
            return Task.FromResult($"{light.Name} is now OFF.");
        }
        return Task.FromResult($"Light with ID {id} not found.");
    }


    [KernelFunction("set_light_color")]
    [Description("Sets the color of the specified light by ID.")]
    [return: Description("The state of the light.")]
    public Task<string> SetLightColorAsync(
        [Description("This is the identifier of the light.")] int id,
        [Description("This is the RGB Value of the light in HEX.")] string hex)
    {
        var light = lights.Find(l => l.Id == id);
        if (light != null)
        {
            light.Hex = hex;
            WriteStatus("SetLightColorAsync");
            return Task.FromResult($"{light.Name} is now in color {light.Hex}.");
        }
        return Task.FromResult($"Light with ID {id} not found.");
    }


    private void WriteStatus(string? s)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n\t------------------------------------------------------");
        Console.WriteLine($"\tList status ({s})");

        int maxNameLength = lights.Max(l => l.Name.Length);

        foreach (var light in lights)
        {
            string status = light.IsOn ? "ON " : "OFF";
            string paddedName = light.Name.PadRight(maxNameLength);
            Console.WriteLine($"\t{light.Id} {paddedName} {status}   {light.Brightness} {light.Hex}");
        }
        Console.WriteLine("\t------------------------------------------------------\n");
        Console.ForegroundColor = originalColor;
    }
}
