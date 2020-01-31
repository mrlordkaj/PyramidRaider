using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using OpenitvnGame;
using OpenitvnGame.Helpers;

namespace PyramidRaider
{
    class AdventureMap
    {
        public static AdventureMap Instance { get; private set; }

        public short[] PyramidProcess { get; private set; }

        public byte[][] _neigbourPyramid = new byte[15][] {
            new byte[] { 1, 6 },            //0. The Pyramid of Nekhtamun...
            new byte[] { 0, 2, 6 },         //1. Tomb of Mahu...
            new byte[] { 1, 3 },            //2. Shrine of Apophis...
            new byte[] { 2, 4 },            //3. The Three Brothers...
            new byte[] { 3, 5 },            //4. Pyramid of Hunefer...
            new byte[] { 4, 8, 10 },        //5. Temple of Ra...
            new byte[] { 0, 1, 7 },         //6. Crypt of Inarus...
            new byte[] { 6, 8, 11 },        //7. Dark Temple of Horus...
            new byte[] { 5, 7, 11, 10 },    //8. The Pyramid of Senenmut...
            new byte[] { 11, 12 },          //9. Shrine of Bast...
            new byte[] { 5, 8, 12 },        //10. Tomb of the Lamp...
            new byte[] { 7, 8, 9 },         //11. Crypt of Khaemhat...
            new byte[] { 9, 10, 13 },       //12. The Palace of Necho...
            new byte[] { 12, 14 },          //13. Tomb of Nefertiti...
            new byte[] { 13 }               //14. Pharaoh's Tomb...
        };
        public byte[][] NeigbourPyramid { get { return _neigbourPyramid; } }

        Rectangle[] recPyramids = new Rectangle[] {
	        new Rectangle(535, 371, 44, 41),    //0. The Pyramid of Nekhtamun...
	        new Rectangle(545, 266, 44, 41),    //1. Tomb of Mahu...
	        new Rectangle(451, 212, 44, 41),    //2. Shrine of Apophis...
	        new Rectangle(535, 142, 44, 41),    //3. The Three Brothers...
	        new Rectangle(428, 67, 44, 41),     //4. Pyramid of Hunefer...
	        new Rectangle(342, 159, 44, 41),    //5. Temple of Ra...
	        new Rectangle(428, 385, 44, 41),    //6. Crypt of Inarus...
	        new Rectangle(303, 375, 44, 41),    //7. Dark Temple of Horus...
	        new Rectangle(358, 284, 44, 41),    //8. The Pyramid of Senenmut...
	        new Rectangle(99, 283, 44, 41),     //9. Shrine of Bast...
	        new Rectangle(234, 265, 44, 41),    //10. Tomb of the Lamp...
            new Rectangle(172, 375, 44, 41),    //11. Crypt of Khaemhat...
	        new Rectangle(79, 172, 44, 41),     //12. The Palace of Necho...
	        new Rectangle(148, 89, 44, 41),     //13. Tomb of Nefertiti...
	        new Rectangle(234, 147, 44, 41)     //14. Pharaoh's Tomb...
        };

        Vector2[] vtHeads = new Vector2[] {
            new Vector2(583, 382),  //0. The Pyramid of Nekhtamun...
            new Vector2(542, 272),  //1. Tomb of Mahu...
            new Vector2(449, 216),  //2. Shrine of Apophis...
            new Vector2(580, 146),  //3. The Three Brothers...
            new Vector2(478, 78),   //4. Pyramid of Hunefer...
            new Vector2(389, 163),  //5. Temple of Ra...
            new Vector2(473, 388),  //6. Crypt of Inarus...
            new Vector2(297, 383),  //7. Dark Temple of Horus...
            new Vector2(406, 290),  //8. The Pyramid of Senenmut...
            new Vector2(97, 286),   //9. Shrine of Bast...
            new Vector2(279, 267),  //10. Tomb of the Lamp...
            new Vector2(164, 388),  //11. Crypt of Khaemhat...
            new Vector2(123, 173),  //12. The Palace of Necho...
            new Vector2(197, 96),   //13. Tomb of Nefertiti...
            new Vector2(234, 148)   //14. Pharaoh's Tomb...
        };
        Vector2 vtHeadCenter = new Vector2(13, 12);

        Rectangle[] recTreasures = new Rectangle[] {
            new Rectangle(0, 0, 52, 51),
            new Rectangle(52, 0, 52, 51),
            new Rectangle(0, 51, 52, 51),
            new Rectangle(52, 51, 52, 51),
            new Rectangle(0, 102, 52, 51),
            new Rectangle(52, 102, 52, 51),
            new Rectangle(0, 153, 52, 51),
            new Rectangle(52, 153, 52, 51),
            new Rectangle(0, 204, 52, 51),
            new Rectangle(52, 204, 52, 51),
            new Rectangle(0, 255, 52, 51),
            new Rectangle(52, 255, 52, 51),
            new Rectangle(0, 306, 52, 51),
            new Rectangle(52, 306, 52, 51),
            new Rectangle(0, 357, 104, 83),
        };

        PlayContentHolder contentHolder;
        Sprite2D sprPyramid;
        short currentPyramid;
        Dictionary<string, Vector2[]> ptPaths;

        byte numOpenedPyramid;
        bool moving;
        List<Vector2> vtPaths;
        List<short> trackBack;

        PlayScene _parent;

        public AdventureMap(PlayScene parent)
        {
            Instance = this;

            _parent = parent;

            contentHolder = PlayContentHolder.Instance;

            sprPyramid = new Sprite2D(contentHolder.ImageAdventurePyramid, 44, 41);

            ptPaths = new Dictionary<string, Vector2[]>();
            ptPaths.Add(
                "0|1",
                new Vector2[] { new Vector2(575, 387), new Vector2(564, 391), new Vector2(554, 392), new Vector2(544, 386), new Vector2(534, 382), new Vector2(526, 375), new Vector2(518, 367), new Vector2(514, 355), new Vector2(519, 342), new Vector2(526, 331), new Vector2(535, 323), new Vector2(545, 313), new Vector2(551, 298), new Vector2(549, 287) }
            );
            ptPaths.Add(
                "1|2",
                new Vector2[] { new Vector2(537, 264), new Vector2(526, 256), new Vector2(516, 249), new Vector2(504, 246), new Vector2(491, 242), new Vector2(478, 240), new Vector2(462, 240), new Vector2(450, 237), new Vector2(448, 224) }
            );
            ptPaths.Add(
                "2|3",
                new Vector2[] { new Vector2(458, 208), new Vector2(468, 199), new Vector2(476, 192), new Vector2(484, 184), new Vector2(494, 178), new Vector2(504, 172), new Vector2(515, 166), new Vector2(527, 161), new Vector2(541, 159), new Vector2(553, 156), new Vector2(566, 155), new Vector2(575, 152) }
            );
            ptPaths.Add(
                "3|4",
                new Vector2[] { new Vector2(581, 136), new Vector2(577, 125), new Vector2(570, 113), new Vector2(563, 103), new Vector2(555, 93), new Vector2(542, 86), new Vector2(529, 79), new Vector2(516, 75), new Vector2(503, 75), new Vector2(491, 75) }
            );
            ptPaths.Add(
                "4|5",
                new Vector2[] { new Vector2(468, 84), new Vector2(459, 91), new Vector2(451, 99), new Vector2(444, 110), new Vector2(437, 120), new Vector2(428, 127), new Vector2(418, 137), new Vector2(411, 145), new Vector2(402, 157) }
            );
            ptPaths.Add(
                "0|6",
                new Vector2[] { new Vector2(576, 388), new Vector2(564, 394), new Vector2(552, 392), new Vector2(541, 387), new Vector2(532, 383), new Vector2(522, 378), new Vector2(512, 374), new Vector2(502, 373), new Vector2(490, 376), new Vector2(482, 381) }
            );
            ptPaths.Add(
                "1|6",
                new Vector2[] { new Vector2(547, 283), new Vector2(549, 294), new Vector2(546, 304), new Vector2(539, 315), new Vector2(530, 324), new Vector2(524, 332), new Vector2(517, 343), new Vector2(514, 352), new Vector2(506, 361), new Vector2(497, 364), new Vector2(490, 371), new Vector2(482, 377) }
            );
            ptPaths.Add(
                "6|7",
                new Vector2[] { new Vector2(468, 395), new Vector2(461, 401), new Vector2(453, 407), new Vector2(443, 412), new Vector2(433, 416), new Vector2(421, 421), new Vector2(410, 425), new Vector2(397, 427), new Vector2(386, 427), new Vector2(374, 428), new Vector2(360, 425), new Vector2(348, 420), new Vector2(337, 414), new Vector2(328, 408), new Vector2(320, 400), new Vector2(313, 393), new Vector2(307, 387) }
            );
            ptPaths.Add(
                "7|8",
                new Vector2[] { new Vector2(297, 372), new Vector2(297, 360), new Vector2(302, 347), new Vector2(313, 338), new Vector2(324, 331), new Vector2(336, 326), new Vector2(348, 321), new Vector2(363, 316), new Vector2(377, 311), new Vector2(388, 306), new Vector2(399, 299) }
            );
            ptPaths.Add(
                "7|11",
                new Vector2[] { new Vector2(289, 379), new Vector2(277, 375), new Vector2(265, 374), new Vector2(254, 374), new Vector2(243, 375), new Vector2(233, 378), new Vector2(224, 381), new Vector2(215, 386), new Vector2(206, 390), new Vector2(197, 392), new Vector2(187, 393), new Vector2(179, 393), new Vector2(172, 390) }
            );
            ptPaths.Add(
                "8|11",
                new Vector2[] { new Vector2(399, 299), new Vector2(387, 306), new Vector2(376, 310), new Vector2(365, 314), new Vector2(352, 318), new Vector2(344, 321), new Vector2(332, 327), new Vector2(320, 333), new Vector2(310, 341), new Vector2(304, 350), new Vector2(293, 357), new Vector2(288, 364), new Vector2(275, 368), new Vector2(262, 370), new Vector2(248, 372), new Vector2(235, 373), new Vector2(220, 378), new Vector2(210, 385), new Vector2(198, 390), new Vector2(188, 394), new Vector2(176, 394) }
            );
            ptPaths.Add(
                "5|8",
                new Vector2[] { new Vector2(383, 172), new Vector2(376, 183), new Vector2(371, 193), new Vector2(368, 208), new Vector2(367, 220), new Vector2(362, 232), new Vector2(362, 244), new Vector2(370, 251), new Vector2(381, 257), new Vector2(393, 263), new Vector2(402, 268), new Vector2(408, 279) }
            );
            ptPaths.Add(
                "5|10",
                new Vector2[] { new Vector2(384, 171), new Vector2(378, 182), new Vector2(374, 193), new Vector2(372, 208), new Vector2(368, 222), new Vector2(362, 232), new Vector2(353, 243), new Vector2(339, 251), new Vector2(322, 252), new Vector2(309, 254), new Vector2(297, 257), new Vector2(287, 260) }
            );
            ptPaths.Add(
                "8|10",
                new Vector2[] { new Vector2(407, 279), new Vector2(403, 269), new Vector2(392, 264), new Vector2(380, 260), new Vector2(366, 254), new Vector2(350, 252), new Vector2(339, 252), new Vector2(326, 251), new Vector2(313, 253), new Vector2(299, 256), new Vector2(290, 262) }
            );
            ptPaths.Add(
                "9|11",
                new Vector2[] { new Vector2(102, 294), new Vector2(105, 303), new Vector2(110, 316), new Vector2(114, 332), new Vector2(119, 344), new Vector2(128, 356), new Vector2(135, 367), new Vector2(145, 376), new Vector2(155, 384) }
            );
            ptPaths.Add(
                "9|12",
                new Vector2[] { new Vector2(98, 276), new Vector2(100, 266), new Vector2(102, 250), new Vector2(102, 234), new Vector2(103, 220), new Vector2(108, 208), new Vector2(112, 197), new Vector2(119, 186) }
            );
            ptPaths.Add(
                "10|12",
                new Vector2[] { new Vector2(269, 275), new Vector2(260, 282), new Vector2(253, 286), new Vector2(242, 289), new Vector2(232, 289), new Vector2(223, 289), new Vector2(213, 284), new Vector2(202, 279), new Vector2(194, 271), new Vector2(184, 266), new Vector2(174, 261), new Vector2(163, 253), new Vector2(156, 247), new Vector2(146, 240), new Vector2(138, 232), new Vector2(128, 222), new Vector2(126, 211), new Vector2(124, 196), new Vector2(125, 183) }
            );
            ptPaths.Add(
                "12|13",
                new Vector2[] { new Vector2(117, 161), new Vector2(112, 153), new Vector2(116, 139), new Vector2(123, 128), new Vector2(129, 120), new Vector2(138, 114), new Vector2(151, 108), new Vector2(162, 103), new Vector2(175, 100), new Vector2(186, 99) }
            );
            ptPaths.Add(
                "13|14",
                new Vector2[] { new Vector2(205, 101), new Vector2(214, 109), new Vector2(221, 118), new Vector2(228, 127), new Vector2(235, 138) }
            );

            moving = false;
            currentPyramid = (short)(SettingHelper.GetSetting<short>(PlayScene.RECORD_ADVENTURE_PYRAMID_CURRENT, 1) - 1);
            vtPaths = new List<Vector2>();

            PyramidProcess = new short[15];
            string strOpenedPyramid = SettingHelper.GetSetting<string>(PlayScene.RECORD_ADVENTURE_PYRAMID_PROCESS, PlayScene.DEFAULT_ADVENTURE_PYRAMID_PROCESS);
            //strOpenedPyramid = "15,15,0,-1,-1,0,15,15,15,-1,0,0,-1,-1,-1";
            //strOpenedPyramid = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
            string[] arrOpenedPyramid = strOpenedPyramid.Split(',');
            numOpenedPyramid = 0;
            for (byte i = 0; i < 15; i++)
            {
                PyramidProcess[i] = short.Parse(arrOpenedPyramid[i]);
                if (PyramidProcess[i] >= 0) numOpenedPyramid++;
            }
        }

        public void Update()
        {
            if (moving)
            {
                if (vtPaths.Count > 1) vtPaths.RemoveAt(0);
                else
                {
                    if (trackBack.Count > 0) setPath();
                    else
                    {
                        _parent.OpenPyramidMap(currentPyramid);
                        moving = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(contentHolder.ImageAdventureMap, Vector2.Zero, Color.White);

            for (byte i = 0; i < 15; i++)
            {
                if (PyramidProcess[i] >= 0)
                {
                    //draw pyramids
                    sprPyramid.SetPosition(recPyramids[i].X, recPyramids[i].Y);
                    sprPyramid.SetFrame(PyramidProcess[i]);
                    sprPyramid.Draw(spriteBatch);
                }
                if (PyramidProcess[i] == 15)
                {
                    //draw treasures
                    Rectangle srcRect = recTreasures[i];
                    Rectangle desRect = new Rectangle();
                    desRect.X = 695 + srcRect.X;
                    desRect.Y = 20 + srcRect.Y;
                    desRect.Width = srcRect.Width;
                    desRect.Height = srcRect.Height;
                    spriteBatch.Draw(contentHolder.ImageBigTreasures, desRect, srcRect, Color.White);
                }
            }

            //draw explorer's head
            if (moving)
            {
                spriteBatch.Draw(contentHolder.ImageAdventureHead, vtPaths[0], null, Color.White, 0, vtHeadCenter, 1, SpriteEffects.None, 1);
            }
            else
            {
                spriteBatch.Draw(contentHolder.ImageAdventureHead, vtHeads[currentPyramid], null, Color.White, 0, vtHeadCenter, 1, SpriteEffects.None, 1);
            }
        }

        public void PointerReleased(int x, int y)
        {
            if (moving) return;

            if (recPyramids[currentPyramid].Contains(x, y))
            {
                _parent.OpenPyramidMap(currentPyramid);
            }
            else
            {
                for (byte i = 0; i < 15; i++)
                {
                    if (PyramidProcess[i] >= 0 && i != currentPyramid && recPyramids[i].Contains(x, y)) findPath(i);
                }
            }
        }

        private void findPath(short nextPyramid)
        {
            Dictionary<short, short> track = new Dictionary<short, short>();
            List<short> open = new List<short>();
            List<short> close = new List<short>();
            open.Add(currentPyramid);
            while (open.Count > 0)
            {
                short pyramid = open[0];
                for (short j = 0; j < NeigbourPyramid[pyramid].Length; j++)
                {
                    short neigbour = NeigbourPyramid[pyramid][j];
                    if (!close.Contains(neigbour) && !open.Contains(neigbour) && PyramidProcess[neigbour] >= 0)
                    {
                        if (!track.ContainsKey(neigbour)) track.Add(neigbour, pyramid);
                        else track[neigbour] = pyramid;
                        open.Add(neigbour);
                    }
                }
                open.RemoveAt(0);
                close.Add(pyramid);
                if (open.Contains(nextPyramid) || close.Count == numOpenedPyramid) break;
            }

            //track back
            if (!open.Contains(nextPyramid)) return;
            trackBack = new List<short>();
            trackBack.Add(nextPyramid);
            while (trackBack[0] != currentPyramid)
            {
                trackBack.Insert(0, track[trackBack[0]]);
            }
            trackBack.RemoveAt(0);
            setPath();
            moving = true;
        }

        private void setPath()
        {
            short to = trackBack[0];
            trackBack.RemoveAt(0);
            vtPaths.Clear();
            Vector2[] ptPath;
            if (currentPyramid < to)
            {
                ptPath = ptPaths[currentPyramid + "|" + to];
                vtPaths.Add(vtHeads[currentPyramid]);
                for (short i = 0; i < ptPath.Length; i++)
                {
                    vtPaths.Add(ptPath[i]);
                }
            }
            else
            {
                ptPath = ptPaths[to + "|" + currentPyramid];
                vtPaths.Add(vtHeads[currentPyramid]);
                for (short i = (short)(ptPath.Length - 1); i >= 0; i--)
                {
                    vtPaths.Add(ptPath[i]);
                }
            }
            currentPyramid = to;
        }
    }
}
