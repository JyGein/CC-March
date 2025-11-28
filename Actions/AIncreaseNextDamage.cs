using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.March.Actions;

internal class AIncreaseNextDamage : CardAction
{
    public int amt;

    public override List<Tooltip> GetTooltips(State s)
    {
        return [
            new GlossaryTooltip("") {
                Icon = ModEntry.Instance.AggressiveMoveIcon.Sprite,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "IncreaseNextDamage", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "IncreaseNextDamage", "description"], new object?[] {amt})
            }
        ];
    }
}
