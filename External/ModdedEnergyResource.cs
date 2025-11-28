using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JyGein.March.IEnergyApi;
using static JyGein.March.External.IKokoroApi.IV2.IActionCostsApi;
using Nickel;
using System.Diagnostics;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Reflection;

namespace JyGein.March.External;

public sealed class ModdedEnergyResource : IResource
{
    public static void Register()
    {
        foreach (Energy energy in Enum.GetValues(typeof(Energy)))
        {
            ModEntry.Instance.KokoroApi.ActionCosts.RegisterResourceCostIcon(new ModdedEnergyResource { EnergyType = energy }, ModEntry.Instance.EnergyApi.ModdedEnergyTooltipInfo.ResourceCostSatisfiedIcon(energy, 1), ModEntry.Instance.EnergyApi.ModdedEnergyTooltipInfo.ResourceCostUnsatisfiedIcon(energy, 1));
        }
        ModEntry.Instance.EnergyApi.RegisterInUseEnergyHook(new EnergiesInUseHook());
    }

    public Energy EnergyType { get; init; }

    public string ResourceKey
    {
        get
        {
            ResourceKeyStorage ??= $"actioncost.resource.energy.{EnergyType}";
            return ResourceKeyStorage;
        }
    }

    private string? ResourceKeyStorage;

    public int GetCurrentResourceAmount(State state, Combat combat)
    {
        return ModEntry.Instance.EnergyApi.GetCombatModdedEnergy(combat).ToDictionary().GetValueOrDefault(EnergyType, 0);
    }

    public IReadOnlyList<Tooltip> GetTooltips(State state, Combat combat, int amount)
    {
        if (amount <= 0)
            return [];
        IEnergyInfoApi energyInfo = ModEntry.Instance.EnergyApi.GetEnergyInfo(EnergyType);

        return [
            new GlossaryTooltip(ResourceKey)
                    {
                        Icon = ModEntry.Instance.KokoroApi.ActionCosts.GetResourceCostIcons(this, amount)[0].CostSatisfiedIcon,
                        TitleColor = energyInfo.GetColor(),
                        Title = ModEntry.Instance.EnergyApi.ModdedEnergyTooltipInfo.ResourceCostName(EnergyType),
                        Description = ModEntry.Instance.EnergyApi.ModdedEnergyTooltipInfo.ResourceCostDescription(EnergyType, amount),
                    }
        ];
    }

    public void Pay(State state, Combat combat, int amount)
    {
        Dictionary<Energy, int> energyBlock = ModEntry.Instance.EnergyApi.GetCombatModdedEnergy(combat).ToDictionary();
        energyBlock[EnergyType] = energyBlock.GetValueOrDefault(EnergyType, 0) - amount;
        ModEntry.Instance.EnergyApi.SetCombatModdedEnergy(combat, energyBlock);
    }

    public sealed class EnergiesInUseHook : IInUseEnergyHook
    {
        public IReadOnlyList<Energy> MoreEnergiesInUse(State s, Combat c)
        {
            List<Energy> inUseEnergy = [];
            foreach (Card card in s.deck.Concat(c.hand).Concat(c.discard).Concat(c.exhausted))
            {
                List<CardAction> actions = card.GetActions(s, c);
                foreach (CardAction action in actions)
                {
                    ICostAction? costAction = ModEntry.Instance.KokoroApi.ActionCosts.AsCostAction(action);
                    if (costAction != null)
                    {
                        IResourceCost? resourceCost = ModEntry.Instance.KokoroApi.ActionCosts.AsResourceCost(costAction.Cost);
                        if (resourceCost != null) foreach (IResource resource in resourceCost.PotentialResources) if (resource is ModdedEnergyResource moddedEnergyResource && !inUseEnergy.Contains(moddedEnergyResource.EnergyType)) inUseEnergy.Add(moddedEnergyResource.EnergyType);
                    }
                }
            }
            return inUseEnergy;
        }

        public IReadOnlyList<Energy> MoreEnergiesInUseOutOfCombat(State s)
        {
            List<Energy> inUseEnergy = [];
            foreach (Card card in s.deck)
            {
                List<CardAction> actions = card.GetActions(s, DB.fakeCombat);
                foreach (CardAction action in actions)
                {
                    ICostAction? costAction = ModEntry.Instance.KokoroApi.ActionCosts.AsCostAction(action);
                    if (costAction != null)
                    {
                        IResourceCost? resourceCost = ModEntry.Instance.KokoroApi.ActionCosts.AsResourceCost(costAction.Cost);
                        if (resourceCost != null) foreach (IResource resource in resourceCost.PotentialResources) if (resource is ModdedEnergyResource moddedEnergyResource && !inUseEnergy.Contains(moddedEnergyResource.EnergyType)) inUseEnergy.Add(moddedEnergyResource.EnergyType);
                    }
                }
            }
            return inUseEnergy;
        }
    }
}
