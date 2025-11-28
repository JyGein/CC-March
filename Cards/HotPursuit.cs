using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static JyGein.March.IEnergyApi;
using JyGein.March.Actions;
using JyGein.March.External;

namespace JyGein.March.Cards;

internal sealed class HotPursuit : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("HotPursuit", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.March_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HotPursuit", "name"]).Localize
        });

    }

    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.A ? 0 : 1
        };
        return data;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions =
        [
            new AAggressiveMove()
            {
                dist = 2
            },
            ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ModdedEnergyResource { EnergyType = Energy.Thermal }, upgrade == Upgrade.B ? 1 : 2), new AAttack { damage = GetDmg(s, upgrade == Upgrade.B ? 1 : 2) }).AsCardAction
        ];
        if (upgrade == Upgrade.B)
        {
            actions.Add(ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ModdedEnergyResource { EnergyType = Energy.Thermal }, 1), new AAttack { damage = GetDmg(s, 2) }).AsCardAction);
        }
        return actions;
    }
}
