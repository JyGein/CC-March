using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static JyGein.March.IEnergyApi;
using JyGein.March.External;

namespace JyGein.March.Cards;

internal sealed class PlayFast : Card, IRegisterable, ISetModdedEnergyCostBaseHook
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("PlayFast", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.March_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PlayFast", "name"]).Localize
        });

    }
    
    public PlayFast()
    {
        ModEntry.Instance.EnergyApi.SetModdedEnergyCostBaseHook(this, this);
    }

    public IDictionary<Energy, int> GetModdedEnergyCostBase(State s)
    {
        return new Dictionary<Energy, int>()
        {
            { Energy.Sacrifice, upgrade == Upgrade.B ? 1 : 0 }
        };
    }

    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 0,
            floppable = upgrade == Upgrade.B ? false : true,
            infinite = upgrade == Upgrade.B ? true : false
        };
        return data;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        if (upgrade == Upgrade.B)
        {
            actions.Add(new AHurt
            {
                hurtAmount = 1,
                targetPlayer = true
            });
            actions.Add(new AStatus()
            {
                status = Status.evade,
                statusAmount = 1,
                targetPlayer = true
            });
        }
        else
        {
            CardAction cardAction1 = ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(
                ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(
                    ModEntry.Instance.KokoroApi.ActionCosts.EnergyResource,
                    1
                ),
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ).AsCardAction;
            cardAction1.disabled = flipped;
            actions.Add(cardAction1);
            actions.Add(new ADummyAction());
            actions.Add(new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true,
                    disabled = !flipped
                });
            CardAction cardAction3 = ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(
                ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(
                    ModEntry.Instance.KokoroApi.ActionCosts.EnergyResource,
                    1
                ),
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ).AsCardAction;
            cardAction3.disabled = !flipped;
            actions.Add(cardAction3);
            CardAction cardAction4 = ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(
                ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(
                    new ModdedEnergyResource { EnergyType = Energy.Sacrifice },
                    upgrade == Upgrade.A ? 2 : 3
                ),
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 2,
                    targetPlayer = true
                }
            ).AsCardAction;
            cardAction4.disabled = !flipped;
            actions.Add(cardAction4);
        }
        return actions;
    }
}
