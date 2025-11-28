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

namespace JyGein.March.Cards;

internal sealed class HeatUp : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("HeatUp", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.March_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HeatUp", "name"]).Localize
        });
    }

    public HeatUp()
    {
        ModEntry.Instance.EnergyApi.SetModdedEnergyCostBase(this, new Dictionary<Energy, int> { { Energy.Core, 1 } });
    }

    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 0,
            buoyant = upgrade == Upgrade.A,
            infinite = upgrade == Upgrade.B
        };
        return data;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions =
        [
            new AStatus()
            {
                status = Status.heat,
                statusAmount = upgrade == Upgrade.B ? 1 : 2,
                targetPlayer = true
            }
        ];
        if (upgrade != Upgrade.B)
        {
            actions.Add(new AStatus()
            {
                status = Status.tempShield,
                statusAmount = 2,
                targetPlayer = true
            });
        }
        return actions;
    }
}
