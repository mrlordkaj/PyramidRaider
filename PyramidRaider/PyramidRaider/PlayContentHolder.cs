using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;
#if WINDOWS
using System.IO;
using Microsoft.Xna.Framework;
#endif

namespace PyramidRaider
{
    class PlayContentHolder
    {
        public static PlayContentHolder Instance { get; private set; }

        private ContentManager content;

        public Model[] ModelFloor { get; private set; }
        public Model ModelWall { get; private set; }
        public Model ModelStair { get; private set; }
        public Model ModelTrap { get; private set; }
        public Model ModelRock { get; private set; }
        public Model ModelArrow { get; private set; }
        public Model ModelCenter { get; private set; }
        public Model ModelGate { get; private set; }
        public Model ModelGateWall { get; private set; }
        public Model ModelGateKey { get; private set; }
        public Model ModelExplorer { get; private set; }
        public Model[] ModelScorpions { get; private set; }
        public Model[] ModelMummies { get; private set; }
        public Model[] ModelBorder { get; private set; }
        public Model[] ModelTile { get; private set; }

        public Texture2D TextureExplorer { get; private set; }
        public Texture2D TextureExplorerFear { get; private set; }
        public Texture2D TextureExplorerPoisoned { get; private set; }
        public Texture2D TextureDust { get; private set; }
        public Texture2D[] TextureFloorLight { get; private set; }
        public Texture2D[] TextureFloorMask { get; private set; }

        public Texture2D[] ImageUp { get; private set; }
        public Texture2D[] ImageRight { get; private set; }
        public Texture2D[] ImageDown { get; private set; }
        public Texture2D[] ImageLeft { get; private set; }
        public Texture2D[] ImageCenter { get; private set; }
        public Texture2D ImageCompass { get; private set; }
        public Texture2D ImageNeedle { get; private set; }
        public Texture2D ImageGalaxy { get; private set; }
        public Texture2D[] ImageUndo { get; private set; }
        public Texture2D[] ImageWorldMap { get; private set; }
        public Texture2D ImageLevelCompleted { get; private set; }
        public Texture2D ImagePyramidCompleted { get; private set; }
        public Texture2D ImageLongScroll { get; private set; }
        public Texture2D ImageWhiteScreen { get; private set; }
        public Texture2D ImageCurrentChamber { get; private set; }
        public Texture2D ImageCrackedChamber { get; private set; }
        public Texture2D ImageSmallTreasures { get; private set; }
        public Texture2D ImageNiceTreasures { get; private set; }
        public Texture2D ImageBigTreasures { get; private set; }
        public Texture2D ImageAdventureMap { get; private set; }
        public Texture2D ImageAdventureHead { get; private set; }
        public Texture2D ImageAdventurePyramid { get; private set; }
        public Texture2D ImageStar { get; private set; }
        public Texture2D[] ImageSummarySelect { get; private set; }
        public Texture2D[] ImageSummaryNext { get; private set; }
        public Texture2D[] ImageSummaryRestart { get; private set; }
#if WINDOWS
        public Texture2D ImageRotateCW { get; private set; }
        public Texture2D ImageRotateCCW { get; private set; }
#endif

        public SoundEffect SoundGateToggle { get; private set; }
        public SoundEffect SoundExplorerWalk { get; private set; }
        public SoundEffect SoundMummyWalk { get; private set; }
        public SoundEffect SoundScorpionWalk { get; private set; }
        public SoundEffect SoundEscaped { get; private set; }
        public SoundEffect SoundFight { get; private set; }
        public SoundEffect SoundRock { get; private set; }
        public SoundEffect SoundPit { get; private set; }
        public SoundEffect SoundPoisoned { get; private set; }
        public SoundEffect SoundMummyHowl { get; private set; }
        public SoundEffect SoundVictory { get; private set; }
        public SoundEffect SoundRate { get; private set; }

        public Song SongIngame { get; private set; }

        public PlayContentHolder()
        {
            Instance = this;
            content = Main.Instance.Content;
        }

        public void LoadContent()
        {
            ModelCenter = content.Load<Model>("Models/center");
            ModelArrow = content.Load<Model>("Models/arrow");
            ModelExplorer = content.Load<Model>("Models/explorer");
            ModelScorpions = new Model[] {
                content.Load<Model>("Models/normalScorpion"),
                content.Load<Model>("Models/bloodScorpion")
            };
            ModelMummies = new Model[] {
                content.Load<Model>("Models/normalMummy"),
                content.Load<Model>("Models/bloodMummy")
            };
            ModelFloor = new Model[] {
                content.Load<Model>("Models/floor6"),
                content.Load<Model>("Models/floor8"),
                content.Load<Model>("Models/floor10")
            };
            ModelGate = content.Load<Model>("Models/gate");
            ModelGateWall = content.Load<Model>("Models/gateWall");
            ModelGateKey = content.Load<Model>("Models/gateKey");
            ModelTrap = content.Load<Model>("Models/trap");
            ModelRock = content.Load<Model>("Models/rock");
            ModelWall = content.Load<Model>("Models/wall");
            ModelStair = content.Load<Model>("Models/stair");
            ModelBorder = new Model[] {
                content.Load<Model>("Models/border6"),
                content.Load<Model>("Models/border8"),
                content.Load<Model>("Models/border10")
            };
            ModelTile = new Model[] {
                content.Load<Model>("Models/tileDark"),
                content.Load<Model>("Models/tileLight"),
                content.Load<Model>("Models/tileBlack")
            };

            TextureExplorer = content.Load<Texture2D>("Textures/explorer");
            TextureExplorerFear = content.Load<Texture2D>("Textures/explorerFear");
            TextureExplorerPoisoned = content.Load<Texture2D>("Textures/explorerPoisoned");
            TextureDust = content.Load<Texture2D>("Textures/dust");
            TextureFloorLight = new Texture2D[] {
                content.Load<Texture2D>("Textures/floorLight6"),
                content.Load<Texture2D>("Textures/floorLight8"),
                content.Load<Texture2D>("Textures/floorLight10")
            };
            TextureFloorMask = new Texture2D[] {
                content.Load<Texture2D>("Textures/floorMask6"),
                content.Load<Texture2D>("Textures/floorMask8"),
                content.Load<Texture2D>("Textures/floorMask10")
            };

            ImageUp = new Texture2D[] {
                content.Load<Texture2D>("Images/upActive"),
                content.Load<Texture2D>("Images/upInactive")
            };
            ImageRight = new Texture2D[] {
                content.Load<Texture2D>("Images/rightActive"),
                content.Load<Texture2D>("Images/rightInactive")
            };
            ImageDown = new Texture2D[] {
                content.Load<Texture2D>("Images/downActive"),
                content.Load<Texture2D>("Images/downInactive")
            };
            ImageLeft = new Texture2D[] {
                content.Load<Texture2D>("Images/leftActive"),
                content.Load<Texture2D>("Images/leftInactive")
            };
            ImageCenter = new Texture2D[] {
                content.Load<Texture2D>("Images/centerActive"),
                content.Load<Texture2D>("Images/centerInactive")
            };
            ImageCompass = content.Load<Texture2D>("Images/compass");
            ImageNeedle = content.Load<Texture2D>("Images/needle");
            ImageGalaxy = content.Load<Texture2D>("Images/galaxy");
            ImageUndo = new Texture2D[] {
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnUndoLight"),
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnUndo")
            };
            ImageWorldMap = new Texture2D[] {
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnWorldMapLight"),
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnWorldMap")
            };
            ImageLevelCompleted = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/levelCompleted");
            ImagePyramidCompleted = content.Load<Texture2D>("Images/pyramidCompleted");
            ImageLongScroll = content.Load<Texture2D>("Images/longScroll");
            ImageWhiteScreen = content.Load<Texture2D>("Images/whiteScreen");
            ImageCurrentChamber = content.Load<Texture2D>("Images/currentChamber");
            ImageCrackedChamber = content.Load<Texture2D>("Images/crackedChamber");
            ImageSmallTreasures = content.Load<Texture2D>("Images/smallTreasure");
            ImageNiceTreasures = content.Load<Texture2D>("Images/niceTreasure");
            ImageBigTreasures = content.Load<Texture2D>("Images/bigTreasure");
            ImageAdventureMap = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/adventureMap");
            ImageAdventureHead = content.Load<Texture2D>("Images/adventureHead");
            ImageAdventurePyramid = content.Load<Texture2D>("Images/adventurePyramid");
            ImageStar = content.Load<Texture2D>("Images/star");
            ImageSummarySelect = new Texture2D[] {
                content.Load<Texture2D>("Images/btnSummarySelectLight"),
                content.Load<Texture2D>("Images/btnSummarySelect")
            };
            ImageSummaryNext = new Texture2D[] {
                content.Load<Texture2D>("Images/btnSummaryNextLight"),
                content.Load<Texture2D>("Images/btnSummaryNext")
            };
            ImageSummaryRestart = new Texture2D[] {
                content.Load<Texture2D>("Images/btnSummaryRestartLight"),
                content.Load<Texture2D>("Images/btnSummaryRestart")
            };
#if WINDOWS
            ImageRotateCCW = Texture2D.FromStream(Main.Instance.GraphicsDevice, TitleContainer.OpenStream("Images/rotateCCW.png"));
            ImageRotateCW = Texture2D.FromStream(Main.Instance.GraphicsDevice, TitleContainer.OpenStream("Images/rotateCW.png"));
#endif

            SoundGateToggle = content.Load<SoundEffect>("Sounds/gateToggle");
            SoundExplorerWalk = content.Load<SoundEffect>("Sounds/explorerWalk");
            SoundMummyWalk = content.Load<SoundEffect>("Sounds/mummyWalk");
            SoundScorpionWalk = content.Load<SoundEffect>("Sounds/scorpionWalk");
            SoundEscaped = content.Load<SoundEffect>("Sounds/escaped");
            SoundFight = content.Load<SoundEffect>("Sounds/fight");
            SoundRock = content.Load<SoundEffect>("Sounds/rock");
            SoundPit = content.Load<SoundEffect>("Sounds/pit");
            SoundPoisoned = content.Load<SoundEffect>("Sounds/poisoned");
            SoundMummyHowl = content.Load<SoundEffect>("Sounds/mummyHowl");
            SoundVictory = content.Load<SoundEffect>("Sounds/victory");
            SoundRate = content.Load<SoundEffect>("Sounds/rate");

            SongIngame = content.Load<Song>("Musics/ingame");

            Thread.Sleep(2000);
        }
    }
}
