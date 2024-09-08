using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ExtremelySimpleLogger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Data;
using MLEM.Data.Content;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Elements;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Mods;
using TinyLife.Objects;
using TinyLife.Utilities;
using TinyLife.World;
using Action = TinyLife.Actions.Action;

namespace MoarDeco;

public class MoarDeco : Mod {

    // the logger that we can use to log info about this mod
    public static Logger Logger { get; private set; }
    public static ExampleOptions Options { get; private set; }

    // visual data about this mod
    public override string Name => "Moar Decorations";
    public override string Description => "This mod adds more decorations to Tiny Life!";
    public override TextureRegion Icon => this.uiTextures[new Point(0, 0)];
    public override string IssueTrackerUrl => "https://github.com/PrinceAllart/MoarDeco/issues";
    public override string TestedVersionRange => "[0.43.0,0.43.10]";

    private Dictionary<Point, TextureRegion> 3TierPhoto;
    private Dictionary<Point, TextureRegion> FlowerTierPhoto;
    private Dictionary<Point, TextureRegion> SmallerPaintings;
    private Dictionary<Point, TextureRegion> uiTextures;
    private Dictionary<Point, TextureRegion> wallpaperTextures;
    private Dictionary<Point, TextureRegion> tileTextures;

    public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info) {
        MoarDeco.Logger = logger;
        MoarDeco.Options = info.LoadOptions(() => new ExampleOptions());

        // loads a texture atlas with the given amount of separate texture regions in the x and y axes
        // we submit it to the texture packer to increase rendering performance. The callback is invoked once packing is completed
        // additionally, we pad all texture regions by 1 pixel, so that rounding errors during rendering don't cause visual artifacts
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("3TierPhoto"), 2, 4), r => this.3TierPhoto = r, 1, true);
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("FlowerTierPhoto"), 2, 4), r => this.FlowerTierPhoto = r, 1, true);
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("SmallerPaintings"), 6, 6), r => this.SmallerPaintings = r, 1, true);
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("UiTextures"), 1, 1), r => this.uiTextures = r, 1, true);
    }

    public override void AddGameContent(GameImpl game, ModInfo info) {
        // adding a custom furniture item
        FurnitureType.Register(new FurnitureType.TypeSettings("Photos", new Point(2, 4), ObjectCategory.Painting, 150, ColorScheme.SimpleWood) {
            // specify the type that should be constructed when this furniture type is placed
            // if this is not specified, the  Furniture class is used, which is used for furniture without special animations or data
            ConstructedType = typeof(Painting),
            // specifying icons for custom clothes and furniture is optional, but using the mod's icon helps users recognize a mod's features
            Icon = this.Icon,
        });

    public override IEnumerable<string> GetCustomFurnitureTextures(ModInfo info) {
        // tell the game about our custom furniture texture
        // this needs to be a path to a data texture atlas, relative to our "Content" directory
        // the texture atlas combines the png texture and the .atlas information
        // see https://mlem.ellpeck.de/api/MLEM.Data.DataTextureAtlas.html for more info
        yield return "SmallerPaintings";
    },

    // this method can be overridden to populate the section in the mod tab of the game's options menu where this mod's options should be displayed
    // this mod uses the ModOptions class to manage its options, though that is optional
    // in general, options should be stored in the ModInfo.OptionsFile file that is given to the mod by the game
    public override void PopulateOptions(Group group, ModInfo info) {
        group.AddChild(new Paragraph(Anchor.AutoLeft, 1, _ => $"{Localization.Get(LnCategory.Ui, "MoarDeco.DarkShirtSpeedOption")}: {MoarDeco.Options.DarkShirtSpeedIncrease}"));
        group.AddChild(new Slider(Anchor.AutoLeft, new Vector2(1, 10), 5, 5) {
            CurrentValue = MoarDeco.Options.DarkShirtSpeedIncrease,
            OnValueChanged = (_, v) => MoarDeco.Options.DarkShirtSpeedIncrease = v
        });
        group.OnRemovedFromUi += _ => info.SaveOptions(MoarDeco.Options);
    }

}