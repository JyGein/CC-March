using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static JyGein.March.IEnergyApi;

namespace JyGein.March.Cards;

internal sealed class AndLoose : Card, IRegisterable, ISetModdedEnergyCostBaseHook
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("AndLoose", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.March_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "AndLoose", "name"]).Localize
        });

    }
    
    public AndLoose()
    {
        ModEntry.Instance.EnergyApi.SetModdedEnergyCostBaseHook(this, this);
    }

    public IDictionary<Energy, int> GetModdedEnergyCostBase(State s)
    {
        return new Dictionary<Energy, int>()
        {
            { Energy.Revenge, upgrade == Upgrade.B ? 4 : 2 }
        };
    }

    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 1,
        };
        return data;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        if (upgrade == Upgrade.B)
        {
            actions.Add(new AAttack { damage = GetDmg(s, 2) });
            actions.Add(new AAttack { damage = GetDmg(s, 2) });
        }
        actions.Add(new AAttack { damage = GetDmg(s, upgrade == Upgrade.None ? 1 : 2) });
        actions.Add(new AAttack { damage = GetDmg(s, 2) });
        return actions;
    }
}
