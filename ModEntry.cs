using HarmonyLib;
using JyGein.March.Cards;
using JyGein.March.External;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JyGein.March;

internal class ModEntry : SimpleMod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal Harmony Harmony;
    internal IKokoroApi.IV2 KokoroApi;
    internal IEnergyApi EnergyApi;
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
    internal ISpriteEntry March_Character_DefaultCardBackground { get; }
    internal ISpriteEntry March_Character_CardFrame { get; }
    internal ISpriteEntry March_Character_Panel { get; }
    internal ISpriteEntry March_Character_Neutral_0 { get; }
    internal ISpriteEntry March_Character_Neutral_1 { get; }
    internal ISpriteEntry March_Character_Neutral_2 { get; }
    internal ISpriteEntry March_Character_Neutral_3 { get; }
    internal ISpriteEntry March_Character_Mini_0 { get; }
    internal ISpriteEntry March_Character_Squint_0 { get; }
    internal ISpriteEntry March_Character_Squint_1 { get; }
    internal ISpriteEntry March_Character_Squint_2 { get; }
    internal ISpriteEntry March_Character_Squint_3 { get; }
    internal ISpriteEntry March_Character_Gameover_0 { get; }
    internal IDeckEntry March_Deck { get; }

    private static readonly List<Type> BaseCommonCardTypes = [
        typeof(HotPursuit),
        typeof(HeatUp),
        typeof(PlayFast),
        typeof(AndLoose)
    ];
    private static readonly List<Type> BaseUncommonCardTypes = [
    ];
    private static readonly List<Type> BaseRareCardTypes = [
    ];
    private static readonly List<Type> BaseSpecialCardTypes = [
    ];
    private static readonly IEnumerable<Type> BaseCardTypes =
        BaseCommonCardTypes
            .Concat(BaseUncommonCardTypes)
            .Concat(BaseRareCardTypes)
            .Concat(BaseSpecialCardTypes);
    private static readonly List<Type> BaseCommonArtifacts = [
    ];
    private static readonly List<Type> BaseBossArtifacts = [
    ];
    private static readonly IEnumerable<Type> BaseArtifactTypes =
        BaseCommonArtifacts
            .Concat(BaseBossArtifacts);
    private static readonly IEnumerable<Type> AllRegisterableTypes =
        BaseCardTypes
            .Concat(BaseArtifactTypes);

    internal ISpriteEntry AggressiveMoveIcon;

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;
        Harmony = new Harmony("JyGein.March");
        
        //You're probably gonna use kokoro
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;
        EnergyApi = helper.ModRegistry.GetApi<IEnergyApi>("JyGein.Energy")!;

        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
        );

        March_Character_DefaultCardBackground = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/defaultcardbackground.png"));
        March_Character_CardFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_cardframe.png"));
        March_Character_Panel = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_panel.png"));
        March_Character_Neutral_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_neutral_0.png"));
        March_Character_Neutral_1 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_neutral_1.png"));
        March_Character_Neutral_2 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_neutral_2.png"));
        March_Character_Neutral_3 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_neutral_3.png"));
        March_Character_Mini_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_mini_0.png"));
        March_Character_Squint_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_squint_0.png"));
        March_Character_Squint_1 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_squint_1.png"));
        March_Character_Squint_2 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_squint_2.png"));
        March_Character_Squint_3 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_squint_3.png"));
        March_Character_Gameover_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/March/character_gameover_0.png"));

        AggressiveMoveIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/aggressiveMove.png"));


        March_Deck = helper.Content.Decks.RegisterDeck("MarchDeck", new DeckConfiguration()
        {
            Definition = new DeckDef()
            {
                color = new Color("137bff"),

                //titleColor = new Color("FFFFFF")
            },
            DefaultCardArt = March_Character_DefaultCardBackground.Sprite,
            BorderSprite = March_Character_CardFrame.Sprite,

            Name = AnyLocalizations.Bind(["character", "March", "name"]).Localize,

            //ShineColorOverride = (shineColorOverrideArgs) => shineColorOverrideArgs.DefaultShineColor.gain(0.5)
        });

        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = March_Deck.UniqueName,

            LoopTag = "neutral",

            Frames = new[]
            {
                March_Character_Neutral_0.Sprite,
                March_Character_Neutral_1.Sprite,
                March_Character_Neutral_2.Sprite,
                March_Character_Neutral_3.Sprite
            }
        });
        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = March_Deck.UniqueName,
            LoopTag = "mini",
            Frames = new[]
            {
                March_Character_Mini_0.Sprite
            }
        });
        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = March_Deck.UniqueName,
            LoopTag = "squint",
            Frames = new[]
            {
                March_Character_Squint_0.Sprite,
                March_Character_Squint_1.Sprite,
                March_Character_Squint_2.Sprite,
                March_Character_Squint_3.Sprite,
            }
        });

        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = March_Deck.UniqueName,
            LoopTag = "gameover",
            Frames = new[]
            {
                March_Character_Gameover_0.Sprite
            }
        });
        helper.Content.Characters.V2.RegisterPlayableCharacter("March", new PlayableCharacterConfigurationV2()
        {
            Deck = March_Deck.Deck,

            Starters = new()
            {
                cards = [
                    new HotPursuit(),
                    new HeatUp()
                ]
            },

            Description = AnyLocalizations.Bind(["character", "March", "description"]).Localize,

            BorderSprite = March_Character_Panel.Sprite,
            //ExeCardType = typeof(MarchExeCard)
        });


        helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties", new SemanticVersion(1, 4, 4))?.RegisterAltStarters(
            deck: March_Deck.Deck,
            starterDeck: new StarterDeck
            {
                cards = [
                    new PlayFast(),
                    new AndLoose()
                ]
            }
        );
        foreach (var type in AllRegisterableTypes)
            AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        ModdedEnergyResource.Register();
    }
    
    public static ISpriteEntry RegisterSprite(IPluginPackage<IModManifest> package, string dir)
    {
        return Instance.Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile(dir));
    }
}

