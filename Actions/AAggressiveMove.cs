using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.March.Actions;

internal class AAggressiveMove : AMove
{
    public int dist;

    int GetMoveDirection(State s, Combat c)
    {
        float playerShipMidpoint = s.ship.x + s.ship.parts.Count / 2f;
        float enemyShipMidpoint = c.otherShip.x + c.otherShip.parts.Count / 2f;
        return enemyShipMidpoint > playerShipMidpoint ? 1 : playerShipMidpoint > enemyShipMidpoint ? -1 : 0;
    }

    public override void Begin(G g, State s, Combat c)
    {
        dir = GetMoveDirection(s, c) * dist;
        //no "enemy aggressive move" support yet
        targetPlayer = true;
        base.Begin(g, s, c);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        int moveDir = 0;
        string combatHint = "";
        if (s.route is Combat c)
        {
            moveDir = GetMoveDirection(s, c);
            combatHint = moveDir > 0 ? $"Right <c=boldPink>{dist}</c>" : moveDir < 0 ? $"Left <c=boldPink>{dist}</c>" : "No move";
        }
        return [
            new GlossaryTooltip("") {
                Icon = ModEntry.Instance.AggressiveMoveIcon.Sprite,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "AggressiveMove", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "AggressiveMove", s.route is Combat ? "inCombatDescription" : "description"]),
                vals = [dist, combatHint]
                
            }
        ];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(ModEntry.Instance.AggressiveMoveIcon.Sprite, dist, Colors.textMain);
    }
}
