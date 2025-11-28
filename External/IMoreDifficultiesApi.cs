using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.March
{
    public interface IMoreDifficultiesApi
    {
        void RegisterAltStarters(Deck deck, StarterDeck starterDeck);
    }
}
