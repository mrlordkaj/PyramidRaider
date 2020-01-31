using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PyramidRaider
{
    enum Language { English, Vietnamese }

    class Localize
    {
        static Localize _instance;
        public static Localize Instance
        {
            get
            {
                if (_instance == null) _instance = new Localize();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public Language Language { get; private set; }
        public string LanguageCode { get; private set; }
        //SplashScreen.cs
        public string TouchToContinue { get; private set; }
        public string ExitGame { get; private set; }
        public string ExitGameDescription { get; private set; }
        public string Disclaimer { get; private set; }
        public string DisclaimerDescription { get; private set; }
        //LeaderboardScreen.cs
        public string FetchingRank { get; private set; }
        public string FetchingData { get; private set; }
        public string SubmittingData { get; private set; }
        public string NameEntry { get; private set; }
        public string EnterName { get; private set; }
        public string InvalidName { get; private set; }
        public string InvalidNameDescription { get; private set; }
        public string SubmitError { get; private set; }
        public string SubmitErrorDescription { get; private set; }
        public string YourRank { get; private set; }
        public string NotRanked { get; private set; }
        public string CantFetchRank { get; private set; }
        public string CantFetchLeaderboard { get; private set; }
        public string RemoveError { get; private set; }
        public string RemoveErrorDescription { get; private set; }
        //LevelManager.cs
        public string Tutorial_1_1 { get; private set; }
        public string Tutorial_1_2 { get; private set; }
        public string Tutorial_1_3 { get; private set; }
        public string Tutorial_1_4 { get; private set; }
        public string Tutorial_1_5 { get; private set; }
        public string Tutorial_1_6 { get; private set; }
        public string Tutorial_1_7 { get; private set; }
        public string Tutorial_2_1 { get; private set; }
        public string Tutorial_2_2 { get; private set; }
        public string Tutorial_2_3 { get; private set; }
        public string Tutorial_2_4 { get; private set; }
        public string Tutorial_2_5 { get; private set; }
        public string Tutorial_2_6 { get; private set; }
        public string Tutorial_2_7 { get; private set; }
        public string Tutorial_3_1 { get; private set; }
        public string Tutorial_3_2 { get; private set; }
        public string Tutorial_3_3 { get; private set; }
        public string Tutorial_3_4 { get; private set; }
        public string Tutorial_3_5 { get; private set; }
        public string Tutorial_3_6 { get; private set; }
        public string Tutorial_3_7 { get; private set; }
        public string Tutorial_3_8 { get; private set; }
        public string Tutorial_3_9 { get; private set; }
        public string Tutorial_3_10 { get; private set; }
        public string Tutorial_3_11 { get; private set; }
        public string Tutorial_3_12 { get; private set; }
        public string Tutorial_3_13 { get; private set; }
        public string Tutorial_4_1 { get; private set; }
        public string Tutorial_4_2 { get; private set; }
        public string Tutorial_4_3 { get; private set; }
        public string Tutorial_4_4 { get; private set; }
        public string Tutorial_4_5 { get; private set; }
        public string Tutorial_4_6 { get; private set; }
        public string Tutorial_4_7 { get; private set; }
        public string Tutorial_4_8 { get; private set; }
        public string Tutorial_4_9 { get; private set; }
        public string Tutorial_5_1 { get; private set; }
        public string Tutorial_5_2 { get; private set; }
        public string Tutorial_5_3 { get; private set; }
        public string Tutorial_5_4 { get; private set; }
        public string Tutorial_5_5 { get; private set; }
        public string Tutorial_5_6 { get; private set; }
        public string Tutorial_5_7 { get; private set; }
        //MainMenuScene.cs
        public string PlayTutorial { get; private set; }
        public string PlayTutorialDescription { get; private set; }
        //PlayScene.cs
        public string Loading { get; private set; }
        public string NumTurn { get; private set; }
        public string NumSec { get; private set; }
        public string ShowingSolution { get; private set; }
        public string StopTutorial { get; private set; }
        public string StopTutorialDescription { get; private set; }
        public string QuitGame { get; private set; }
        public string QuitGameDescription { get; private set; }
        public string[] Pyramid { get; private set; }
        public string[] PyramidDescription { get; private set; }
        public string AdventureMode { get; private set; }
        public string AdventureModeDescription_1 { get; private set; }
        public string AdventureModeDescription_2 { get; private set; }
        public string AdventureModeDescription_3 { get; private set; }
        //QuickMenu.cs
        public string SolutionWarning { get; private set; }
        public string SolutionWarningDescription { get; private set; }
        public string SolutionConfirm { get; private set; }
        public string SolutionConfirmDescription { get; private set; }
        public string AbandonConfirm { get; private set; }
        public string AbandonConfirmDescription { get; private set; }

        public Localize()
        {
            Pyramid = new string[15];
            PyramidDescription = new string[15];
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "vi")
            {
                Language = Language.Vietnamese;
                LanguageCode = "vi";
            }
            else
            {
                Language = Language.English;
                LanguageCode = "en";
            }
            switch (Language)
            {
                case Language.English:
                    //SplashScene.cs
                    TouchToContinue = "touch to continue";
                    ExitGame = "Exit Game";
                    ExitGameDescription = "Are you sure that you want to exit the game?\nIf true, click Yes, and remember that the mummies always hope to see you again!";
                    Disclaimer = "Disclaimer";
                    DisclaimerDescription = "This game is totally free, you can play to over the game without any paid. This upgrade just removes all advertistments and increases amount of hint points reward.";
                    //LeaderboardScene.cs
                    FetchingRank = "fetching your rank...";
                    FetchingData = "fetching data...";
                    SubmittingData = "submitting...";
                    NameEntry = "Name entry";
                    EnterName = "Enter your name";
                    InvalidName = "Invalid name";
                    InvalidNameDescription = "The length of your name must be between 3 and 14 characters, and must not contains \"Unnamed\".\nPlease update your info and try again!";
                    SubmitError = "Error";
                    SubmitErrorDescription = "Something went wrong while we uploading your score to leaderboard server.\nPlease try again later!";
                    YourRank = "your rank: ";
                    NotRanked = "you are not ranked!";
                    CantFetchRank = "can't fetch your rank!";
                    CantFetchLeaderboard = "can't fetch the leaderboard";
                    RemoveError = "Error";
                    RemoveErrorDescription = "Something went wrong while we removing your score from leaderboard server.\nPlease try again later!";
                    //LevelManager.cs
                    Tutorial_1_1 = "The goal of game is to get your Explorer to the exit of each maze without being caught by the Mummy.";
                    Tutorial_1_2 = "Move your explorer with the arrow keys. Move the explorer North now.";
                    Tutorial_1_3 = "Notice that the Mummy moved after you! In the darkness of the pyramid, he can move twice as fast as you. Carefull!";
                    Tutorial_1_4 = "You have to use the maze walls to trap and confuse the Mummy if you want to escape. Move North again now.";
                    Tutorial_1_5 = "Notice the Mummy couldn't move! It always tries to take a direct path to you. But the wall next to your left blocked it.";
                    Tutorial_1_6 = "Now proceed all the way North then West to the exit. Watch the Mummy's movement as it tries to get to you.";
                    Tutorial_1_7 = "Luckily, the Mummy is still trapped by walls. Remember, it always takes a straight line route to you.";
                    Tutorial_2_1 = "Very good! You escaped the maze! Now let's try a slightly trickier maze.";
                    Tutorial_2_2 = "Here you'll learn that sometimes the best move is no move at all.";
                    Tutorial_2_3 = "This maze doesn't offer very much cover. Any move you make will bring the Mummy straight to you with no escape.";
                    Tutorial_2_4 = "The solution is not to move at all, but to pass your turn instead. The Mummy then moves as normal.";
                    Tutorial_2_5 = "You can wait by pressing the CENTER key. Go ahead and pass your turn now.";
                    Tutorial_2_6 = "The Mummy has trapped himself in the nook to your North. Now head West to the wall, then North to escape the maze.";
                    Tutorial_2_7 = "Yikes! This could be close!";
                    Tutorial_3_1 = "Whew! You made it! Remember as you paly that you can pass your turn and just let the Mummy move.";
                    Tutorial_3_2 = "In this part of the tutorial, we'll see how you can use the Mummy's bizarre brain to your advantage.";
                    Tutorial_3_3 = "The Mummy will always try to move closer to you by the W-E direction before he moves in the N-S direction.";
                    Tutorial_3_4 = "Here, for instance, you can see that if the Mummy moved South two squares, you'd be trapped for sure.";
                    Tutorial_3_5 = "But he's not that bright. Move West now and see what the Mummy does.";
                    Tutorial_3_6 = "Now, if you move West again, the Mummy will get you for sure. But if you wait instead, he'll trap himself. Wait now..";
                    Tutorial_3_7 = "Good job! Now, as before, head West to the wall then dash North to the exit!";
                    Tutorial_3_8 = "Another close call! As you play, you'll get better judging the distance between and the Mummy.";
                    Tutorial_3_9 = "A few final things to remember... You can use the Reset Maze and Undo Move button at any time.";
                    Tutorial_3_10 = "Undo Move takes back your last action. Reset Maze restarts the maze to the original state.";
                    Tutorial_3_11 = "If a maze is too hard for you, you can paid your hint points for the solution.";
                    Tutorial_3_12 = "Your score in each maze is determined by the maze's difficult, and the amount of moves you take to solve it.";
                    Tutorial_3_13 = "Yes, this is all basic rules, now we'll learn more for know about some advanced features.";
                    Tutorial_4_1 = "Sometimes you'll encouter TWO mummies in the same maze!";
                    Tutorial_4_2 = "The Undead don't play well with others... try to lure them into a collision, only one will emerge unscathed.";
                    Tutorial_4_3 = "Move East now to see what happens when mummies collide.";
                    Tutorial_4_4 = "Sometimes a locked gate will bar your progress. When you see such a gate, look for a key nearby.";
                    Tutorial_4_5 = "Stepping on it will open or close the gate. Remember that the gates can be trigged by either you or the mummy.";
                    Tutorial_4_6 = "Step onto the key now and release the scorpion!";
                    Tutorial_4_7 = "The scorpions behave just like the mummies do, except that they only move one square per turn.";
                    Tutorial_4_8 = "They're still deadly if they catch you, though!";
                    Tutorial_4_9 = "Now move all the way to the West wall then North to exit the chamber and proceed to the next part of the tutorial.";
                    Tutorial_5_1 = "Remember how normal mummies always move by the W-E direction first, then by the N-S direction?";
                    Tutorial_5_2 = "The blood mummy does the opposite, moving by the N-S direction first, then by the W-E direction.";
                    Tutorial_5_3 = "There are only a few things left to learn about the devious mummy tombs!";
                    Tutorial_5_4 = "Those skull tiles in the Northeast corner are traps left by the pharaohs guard their pyramids.";
                    Tutorial_5_5 = "Stepping on one of these skull tiles will spell instant doom for your explorer!";
                    Tutorial_5_6 = "But remember, mummies and scorpions can cross over them without harm.";
                    Tutorial_5_7 = "That's it! You've finished the tutorial! Let's PLAY IT NOW!";
                    PlayTutorial = "Play Tutorial";
                    PlayTutorialDescription = "Did you played this game before? If not, maybe a training course will helpful for you!\nSo, do you want to play the tutorial mode first?";
                    //PlayScene
                    Loading = "loading...";
                    NumTurn = "{0} turns";
                    NumSec = "{0} secs";
                    ShowingSolution = "Showing the solution...";
                    StopTutorial = "Stop Tutorial";
                    StopTutorialDescription = "Do you want to cancel current tutorial process and return to the main menu?\nYou can start over again in the Instruction menu.";
                    QuitGame = "Quit Game";
                    QuitGameDescription = "Do you want to quit the game and return to the main menu?";
                    Pyramid[0] = "The Pyramid of Necktamun";
                    PyramidDescription[0] = "This ancient monument guards the entry to the valley of kings.";
                    Pyramid[1] = "Tomb of Mahu";
                    PyramidDescription[1] = "Legend has it that the paiceless Golden Scarab is buried in this place..";
                    Pyramid[2] = "Shrine of Apophis";
                    PyramidDescription[2] = "The mad pharaoh was said to keep an amulet of jade and topaz.";
                    Pyramid[3] = "The Three Brothers";
                    PyramidDescription[3] = "Entombed here are the 3 pharaohs, Anen, Tjety, and Wadjmose.";
                    Pyramid[4] = "Pyramid of Hunefer";
                    PyramidDescription[4] = "A holy amulet of Set was buried with this great pharaoh.";
                    Pyramid[5] = "Temple of Ra";
                    PyramidDescription[5] = "Within this pyramid's walls, there supposedly lies a priceless painting by the Phraraoh Bek..";
                    Pyramid[6] = "Crypt of Iranus";
                    PyramidDescription[6] = "This Pharaoh was supposedly buried with an intricate silver ring.";
                    Pyramid[7] = "Dark Temple of Horus";
                    PyramidDescription[7] = "Beware! No light can enter this forbidding pyramid.";
                    Pyramid[8] = "The Pyramid of Senenmut";
                    PyramidDescription[8] = "This monarch was buried with a full-size golden lion at his side.";
                    Pyramid[9] = "Grypt of Khaemhat";
                    PyramidDescription[9] = "A chalice inlaid with gems lies within these walls.";
                    Pyramid[10] = "Shrine of Bast";
                    PyramidDescription[10] = "This monument was raised to the egyptian god of cats.";
                    Pyramid[11] = "Tomb of the Lamp";
                    PyramidDescription[11] = "Supposedly the pharaoh Rahotep kept here a magic lamp made of Gold and Silver.";
                    Pyramid[12] = "The Palace of Necho";
                    PyramidDescription[12] = "This Pharaoh supposedly owned a falcon made of solid jade.";
                    Pyramid[13] = "Tomb of Nefertiti";
                    PyramidDescription[13] = "The bride of Tutankhamen lies inside this edifice.";
                    Pyramid[14] = "Pharaoh's Tomb";
                    PyramidDescription[14] = "Only when all the other pyramids have been mastered will the final resting place of Tutankhamen open.";
                    AdventureMode = "Adventure Mode";
                    AdventureModeDescription_1 = "Welcome to the Valley of Pharaohs! Your task: find your way through 15 pyramids full of the most devious, diabolical mazes we could advise!";
                    AdventureModeDescription_2 = "You must complete all the mazes in each pyramid before you can unlock the paths beyon. Collect all 15 treasures and triumph over the mystery of King Tut's Tomb!";
                    AdventureModeDescription_3 = "You have completed the first pyramid! Now, another two pyramids have been unlocked! Tap on a unlocked pyramid to travel there! Let's go and good luck!";
                    //QuickMenu.cs
                    SolutionWarning = "Warning";
                    SolutionWarningDescription = "Your hint points: {0}\nSolution cost: {1}\nYou have not enough hint points to get solution!";
                    SolutionConfirm = "Get Solution";
                    SolutionConfirmDescription = "Your hint points: {0}\nSolution cost: {1}\nDo you want to pay your hint points for the solution?";
                    AbandonConfirm = "Abandon";
                    AbandonConfirmDescription = "This will fully reset your current process.\nDo you want to continue?";
                    break;

                case Language.Vietnamese:
                    //SplashScene.cs
                    TouchToContinue = "chạm để tiếp tục";
                    ExitGame = "Thoát ứng dụng";
                    ExitGameDescription = "Bạn có chắc chắn muốn thoát khỏi ứng dụng không?\nHãy quay lại sớm nhé, lũ xác ướp sẽ nhớ bạn lắm đó!";
                    Disclaimer = "Chú ý";
                    DisclaimerDescription = "Trò chơi này là hoàn toàn miễn phí. Việc nâng cấp đơn giản là gỡ bỏ toàn bộ quảng cáo, tăng số ngọc dành cho sự trợ giúp, và trên tinh thần hỗ trợ nhà phát triển của bạn.";
                    //LeaderboardScene.cs
                    FetchingRank = "lấy thứ hạng...";
                    FetchingData = "lấy dữ liệu...";
                    SubmittingData = "gửi dữ liệu...";
                    NameEntry = "Nhập tên";
                    EnterName = "Hãy nhập vào tên của bạn";
                    InvalidName = "Không hợp lệ";
                    InvalidNameDescription = "Tên của bạn phải có độ dài từ 3 đến 14 ký tự, và không được chứa cụm từ \"Unnamed\".\nVui lòng nhập lại.";
                    SubmitError = "Có lỗi";
                    SubmitErrorDescription = "Có lỗi gì đó xảy ra khiến điểm của bạn không gửi lên máy chủ được. Vui lòng thử lại vào lúc khác.";
                    YourRank = "hạng của bạn: ";
                    NotRanked = "bạn chưa được xếp hạng!";
                    CantFetchRank = "không xem được hạng!";
                    CantFetchLeaderboard = "không xem được bảng điểm";
                    RemoveError = "Có lỗi";
                    RemoveErrorDescription = "Có lỗi gì đó xảy ra khiến yêu cầu xóa điểm của bạn chưa thực hiện được. Vui lòng thử lại vào lúc khác.";
                    //LevelManager.cs
                    Tutorial_1_1 = "Mục tiêu trò chơi là đưa nhà thám hiểm tới cửa ra mà không bị xác ướp bắt được.";
                    Tutorial_1_2 = "Di chuyển nhà thám hiểm bằng các phím ảo. Giờ hãy đi lên theo hướng Bắc!";
                    Tutorial_1_3 = "Lưu ý rằng xác ướp sẽ di chuyển sau bạn. Chúng có khả năng di chuyển nhanh gấp đôi. Hãy cẩn thận!";
                    Tutorial_1_4 = "Bạn có thể lợi dụng các bức tường để cản đường đi của xác ướp. Giờ hãy lên hướng Bắc lần nữa.";
                    Tutorial_1_5 = "Bạn có thấy xác ướp bị cản rồi chứ? Bởi nó chỉ biết đi thẳng tới bạn, nhưng đã bị một bức tường chặn lại.";
                    Tutorial_1_6 = "Giờ hãy tiến hết theo hướng Bắc, sau đó là hướng Tây, tới cửa ra. Chú ý cách di chuyển của xác ướp.";
                    Tutorial_1_7 = "May quá, xác ướp vẫn bị cản. Hãy nhớ, chúng chỉ biết đi theo đường thẳng để đến chỗ bạn.";
                    Tutorial_2_1 = "Rất tốt! Bạn đã vượt qua một mê cung! Giờ hãy làm thử với mê cung khó hơn.";
                    Tutorial_2_2 = "Đôi khi bạn sẽ thấy được rằng cách tốt nhất là không di chuyển gì cả.";
                    Tutorial_2_3 = "Mê cung này không có nhiều tường chắn. Bất kể bạn đi theo hướng nào đều không phải cách tốt nhất.";
                    Tutorial_2_4 = "Giải pháp là không di chuyển gì hết, hãy để xác ướp di chuyển trước.";
                    Tutorial_2_5 = "Bạn có thể chờ bằng cách bấm phím GIỮA. Giờ, hãy làm thế để bỏ qua lượt của bạn!";
                    Tutorial_2_6 = "Xác ướp đã tự đi vào ngõ cụt. Giờ hãy tiến thẳng theo hướng Tây, sau đó là hướng Bắc để tới cửa ra.";
                    Tutorial_2_7 = "Á chà! Tên xác ướp tiến gần quá rồi!";
                    Tutorial_3_1 = "Phù! Bạn vẫn thoát! Hãy nhớ rằng bạn có thể bỏ qua lượt của mình bất cứ lúc nào.";
                    Tutorial_3_2 = "Ở phần này, tôi sẽ chỉ bạn thấy cách tư duy của những xác ướp. Chúng thực sự rất ngốc nghếch.";
                    Tutorial_3_3 = "Xác ướp sẽ cố di chuyển tới gần bạn nhất theo hướng Đông-Tây, rồi Bắc-Nam.";
                    Tutorial_3_4 = "Đây, bạn có thể thấy nếu xác ướp di chuyển 2 bước xuống hướng Nam, bạn chắc chắn bị bắt!";
                    Tutorial_3_5 = "Nhưng không, nó không thông minh đến vậy, hãy di chuyển theo hướng Tây và quan sát nhé!";
                    Tutorial_3_6 = "Nếu bạn di chuyển theo hướng Tây nữa, bạn sẽ bị bắt. Nhưng nếu bạn đứng yên, xác ướp sẽ bị bẫy. Hãy chờ..";
                    Tutorial_3_7 = "Rất tốt! Giờ hãy tiến thẳng theo hướng Tây, rồi hướng Bắc để tới cửa ra!";
                    Tutorial_3_8 = "Lại gần rồi! Nhưng đây là lượt đi của bạn, bạn có thể thoát khỏi xác ướp ngay bây giờ.";
                    Tutorial_3_9 = "Vài điều còn lại... Bạn có thể Chơi lại hoặc Lùi bước bất cứ lúc nào.";
                    Tutorial_3_10 = "Lùi bước sẽ đưa bạn về trạng thái trước đó. Chơi lại sẽ đưa về trạng thái ban đầu.";
                    Tutorial_3_11 = "Điểm số của bạn sẽ được tính theo độ khó của màn chơi và số bước đi mà bạn thực hiện.";
                    Tutorial_3_12 = "Nếu cảm thấy câu đố quá khó, bạn có thể đổi số ngọc tích lũy để lấy lời giải.";
                    Tutorial_3_13 = "Đó là tất cả luật cơ bản, giờ chúng ta sẽ tìm hiểu tiếp một số tính năng sâu hơn nữa.";
                    Tutorial_4_1 = "Đôi khi bạn sẽ gặp HAI xác ướp trong một mê cung!";
                    Tutorial_4_2 = "Các xác ướp không hòa thuận, nếu chạm chán nhau, chúng sẽ ẩu đả tới khi chỉ còn lại một.";
                    Tutorial_4_3 = "Di chuyển theo hướng Đông để xem điều gì xảy ra khi xác ướp đụng độ nhau.";
                    Tutorial_4_4 = "Đôi khi cánh cổng khóa sẽ chắn lối bạn. Khi bạn thấy nó, hãy tìm chiếc chìa gần đó.";
                    Tutorial_4_5 = "Bước lên nó để mở hoặc đóng cổng. Nhớ rằng, bạn hay kẻ thù đều có thể kích hoạt chìa khóa.";
                    Tutorial_4_6 = "Giờ hãy bước lên chìa khóa để thả con bọ cạp ra!";
                    Tutorial_4_7 = "Bọ cạp cũng giống xác ướp, chỉ khác là chúng chỉ di chuyển được một bước mỗi lượt.";
                    Tutorial_4_8 = "Nhưng nếu để chúng tóm được, bạn cũng sẽ tạch!";
                    Tutorial_4_9 = "Giờ hãy di chuyển về hướng Tây, sau đó là hướng Bắc để tới bài hướng dẫn tiếp theo.";
                    Tutorial_5_1 = "Bạn đã ghi nhớ xác ướp trắng di chuyển theo hướng Đông-Tây trước, Bắc-Nam sau chưa?";
                    Tutorial_5_2 = "Xác ướp đỏ thì khác, chúng sẽ di chuyển hướng Bắc-Nam trước, sau đó mới là Đông-Tây.";
                    Tutorial_5_3 = "Chỉ còn lại một bí mật nữa về những lăng mộ này thôi!";
                    Tutorial_5_4 = "Những ô gạch hình đầu lâu ở góc phía bắc kia chính là những chiếc bẫy chết người!";
                    Tutorial_5_5 = "Bước lên đó thì chắc chắn nhà thám hiểm của bạn sẽ chết một cách khủng khiếp...";
                    Tutorial_5_6 = "Nhưng cũng cần nhớ, xác ướp và bọ cạp không gặp phải vấn đề gì khi vượt qua những cái bẫy này.";
                    Tutorial_5_7 = "Đó là tất cả! Bạn đã hoàn thành khóa huấn luyện rồi đó. CHƠI THÔI!";
                    PlayTutorial = "Huấn luyện";
                    PlayTutorialDescription = "Bạn đã chơi trò này bao giờ chưa? Nếu chưa, bạn nên thử qua chế độ huấn luyện trước.\nBạn có muốn được huấn luyện bây giờ không?";
                    //PlayScene
                    Loading = "nạp dữ liệu..";
                    NumTurn = "{0} lượt";
                    NumSec = "{0} giây";
                    ShowingSolution = "Trình bày lời giải...";
                    StopTutorial = "Dừng huấn luyện";
                    StopTutorialDescription = "Bạn muốn dừng khóa huấn luyện này và quay về danh mục chính?\nBạn vẫn có thể bắt đầu lại bất cứ lúc nào.";
                    QuitGame = "Dừng chơi";
                    QuitGameDescription = "Bạn chắc chắn muốn dừng chơi và quay lại danh mục chính?";
                    Pyramid[0] = "Lăng mộ Necktamun";
                    PyramidDescription[0] = "Lăng mộ này cũng chính cánh cửa phong ấn toàn bộ thung lũng của các Pharaoh.";
                    Pyramid[1] = "Lăng mộ Mahu";
                    PyramidDescription[1] = "Nơi đây chứa một bảo vật cổ quý giá, là một con bọ cánh cứng được đúc bằng vàng.";
                    Pyramid[2] = "Đền thờ Apophis";
                    PyramidDescription[2] = "Đền thờ một ác thần của Ai Cập cổ đại và là nơi cất giữ tấm bùa làm bằng ngọc quý của hắn.";
                    Pyramid[3] = "Lăng mộ Tam đế";
                    PyramidDescription[3] = "Đây là nơi chôn cất ba vị pharaoh là Anen, Tjety, và Wadjmose, cùng biểu tượng quyền lực của họ.";
                    Pyramid[4] = "Lăng mộ Hunefer";
                    PyramidDescription[4] = "Lăng một chôn cất một vị pharaoh vĩ đại cùng tấm bùa thần thánh mang tên \"Set\".";
                    Pyramid[5] = "Đền thờ Ra";
                    PyramidDescription[5] = "Đền thờ thần Ra, tương truyền nơi đây cất giữ bức điêu khắc vô giá của vị pharaoh Bek..";
                    Pyramid[6] = "Lăng mộ Iranus";
                    PyramidDescription[6] = "Có tin đồn rằng lăng mộ của vị pharaoh này cất giữ một chiếc nhẫn bạc cực kỳ quý giá.";
                    Pyramid[7] = "Lăng mộ Horus";
                    PyramidDescription[7] = "Cẩn thận! Không có thứ ánh sáng nào có thể tồn tại trong lăng mộ bị phong ấn này.";
                    Pyramid[8] = "Lăng mộ Senenmut";
                    PyramidDescription[8] = "Vị vua này được chôn cất cùng một bức tượng con sư tử bằng vàng rất lớn.";
                    Pyramid[9] = "Lăng mộ Khaemhat";
                    PyramidDescription[9] = "Lăng mộ này cất giấu một chiếc ly được làm từ vàng và đá quý.";
                    Pyramid[10] = "Đền thờ Bast";
                    PyramidDescription[10] = "Đền thờ một vị thần cổ xưa của người Ai Cập, được cho là người giữ cửa địa ngục.";
                    Pyramid[11] = "Đền thờ Đèn Thần";
                    PyramidDescription[11] = "Đây là nơi vị pharaoh Rahotep cất giữ chiếc đèn ma thuật làm bằng vàng và đá quý của ông ta.";
                    Pyramid[12] = "Lăng mộ Necho";
                    PyramidDescription[12] = "Vị pharaoh sở hữu một con chim ưng làm bằng ngọc bích, và đây là nơi ông ta cất giữ nó.";
                    Pyramid[13] = "Lăng mộ Nefertiti";
                    PyramidDescription[13] = "Lăng mộ vị hậu của pharaoh Tutankhamun, đây là phong ấn cuối cùng tới lăng mộ vị pharaoh này.";
                    Pyramid[14] = "Lăng mộ Tutankhamun";
                    PyramidDescription[14] = "Chỉ khi tất cả các kim tự tháp khác bị chinh phục, lăng mộ pharaoh Tutankhamun mới được mở.";
                    AdventureMode = "Thám hiểm";
                    AdventureModeDescription_1 = "Chào mừng bạn đến với thung lũng của các Pharaoh! Nhiệm vụ của bạn: tìm cách vượt qua thử thách của tất cả 15 kim tự tháp trên bản đồ.";
                    AdventureModeDescription_2 = "Bạn phải hoàn thành toàn bộ thử thách của mỗi kim tự tháp để có thể tìm đường đến những cái tiếp theo. Hãy thu thập đủ 15 bảo vật của toàn bộ khu lăng mộ!";
                    AdventureModeDescription_3 = "Bạn đã hoàn thành kim tự tháp đầu tiên! Giờ đã có hai kim tự tháp mới được mở. Hãy chạm vào một trong số chúng để tiếp tục khám phá!";
                    //QuickMenu.cs
                    SolutionWarning = "Thông báo";
                    SolutionWarningDescription = "Số ngọc bạn có: {0}\nGiá cho lời giải: {1}\nBạn không đủ số ngọc để đổi lấy sự trợ giúp!";
                    SolutionConfirm = "Trợ giúp";
                    SolutionConfirmDescription = "Số ngọc bạn có: {0}\nGiá cho lời giải: {1}\nBạn có muốn đổi số ngọc này để được trợ giúp?";
                    AbandonConfirm = "Bỏ cuộc";
                    AbandonConfirmDescription = "Việc này sẽ thiết lập lại toàn bộ tiến trình của bạn.\nBạn có muốn tiếp tục?";
                    break;
            }
        }
    }
}
