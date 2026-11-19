using BaseLib.Abstracts;
using Bellkeeper.BellkeeperCode.Extensions;
using Godot;

namespace Bellkeeper.BellkeeperCode.Character;

public class BellkeeperRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Bellkeeper.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}