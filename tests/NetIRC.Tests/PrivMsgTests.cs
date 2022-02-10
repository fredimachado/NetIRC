using NetIRC.Messages;
using Xunit;

namespace NetIRC.Tests
{
    public class PrivMsgTests
    {

        [Fact]
        public void TestPrivMsgMessageWithNoSpacesAndStartingWithColon()
        {
            var target = "WiZ";
            var message = ":)";
            var privMsgMessage = new PrivMsgMessage(target, message);

            Assert.Equal($"PRIVMSG {target} :{message}", privMsgMessage.ToString());
        }

        [Fact]
        public void TestPrivMsgMessageTokens()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ?";
            var privMsgMessage = new PrivMsgMessage(target, message);

            Assert.Equal($"PRIVMSG {target} :{message}", privMsgMessage.ToString());
        }

        [Fact]
        public void TestMultiLinePrivMsgMessageTokens()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? Are you receiving this message ? testtest Are you receiving this message ?Are you receiving this message ?Are you receiving this message ?Are you receiving this message ?";
            var privMsgMessage = new PrivMsgMessage(target, message);

            var lines = privMsgMessage.ToString().Split(Constants.CrLf);

            Assert.Equal(2, lines.Length);

            Assert.StartsWith($"PRIVMSG {target} :Are", lines[0]);
            Assert.EndsWith("test", lines[0]);

            Assert.StartsWith($"PRIVMSG {target} :test", lines[1]);
            Assert.EndsWith("?", lines[1]);
        }

        [Fact]
        public void TestMultiLinePrivMsgMessageTokens_With2ByteEncodedCharacters()
        {
            var target = "WiZ";
            var message = "Я помню чудное мгновенье: Передо мной явилась ты, Как мимолетное виденье, Как гений чистой красоты. В томленьях грусти безнадежной, В тревогах шумной суеты, Звучал мне долго  голос нежный И снились милые черты. Выхожу один я на дорогу; Сквозь туман кремнистый путь блестит; Ночь тиха. Пустыня внемлет богу, И звезда с звездою говорит. В небесах торжественно и чудно! Спит земля в сиянье голубом... Что же мне так больно и так трудно? Жду ль чего? жалею ли о чем?";
            var privMsgMessage = new PrivMsgMessage(target, message);
            var preMessage = $"PRIVMSG {target} :";

            var lines = privMsgMessage.ToString().Split(Constants.CrLf);

            Assert.Equal(3, lines.Length);

            Assert.StartsWith($"{preMessage}Я помню", lines[0]);
            Assert.EndsWith(" оди", lines[0]);

            Assert.StartsWith($"{preMessage}н я на", lines[1]);
            Assert.EndsWith("чего?", lines[1]);

            Assert.StartsWith($"{preMessage} жалею", lines[2]);
            Assert.EndsWith("чем?", lines[2]);
        }

        [Fact]
        public void TestMultiLinePrivMsgMessageTokens_With3ByteEncodedCharacters()
        {
            var target = "WiZ";
            var message = "大晦日、大勢の善男善女で賑わった除夜の鐘。一般には近江八景の「三井の晩鐘」として知られるこの梵鐘は、 「天下の三銘鐘」の一つにも数えられています。 姿の立派な宇治平等院の鐘、由緒の正しい高雄神護寺と、音色の美しさで選ばれた三井寺。 つまり、三井の晩鐘は、日本一の響きであると認められているのです。耳を澄ますと、この銘鐘、ドレミの「ラ」の音の、四分の一ほど低い音で鳴り響いています。 西洋の音楽がド（C）の音を基準音とするのに対して、東洋ではラ（A）が基本になります。 永い梵鐘づくりの経験を経て、鐘の形や大きさ、厚さ、銅の配合といったものから、 美しいラの音が出るように完成させたのかもしれません。三井の晩鐘には、哀しい民話が伝わります。村の子どもらにイジメられる一匹の蛇を助けたことで、 里の漁師は竜宮の王女をめとることになります。 間もなく、二人の間には子どもが産まれますが、 自分が竜女であることを知られた女は、琵琶湖底に呼び戻されてしまいます。 残された子どもは母親を恋しがり、毎日、激しく泣き叫びます。 でも母親にもらった目玉をなめると、不思議と、泣きやむのです。 しかし、その目玉も、やがて小さくなり、ついに竜女の両方の目玉はなめ尽くされてしまいました。 盲（めいし）になった竜女は、漁師に、三井寺の鐘をついて、 二人が達者でいることを知らせてくれるように頼みます。 鐘が湖に響くのを聴いて、竜女は心安らがせたといいます。";
            var privMsgMessage = new PrivMsgMessage(target, message);
            var preMessage = $"PRIVMSG {target} :";

            var lines = privMsgMessage.ToString().Split(Constants.CrLf);

            Assert.Equal(5, lines.Length);

            Assert.StartsWith($"{preMessage}大晦日", lines[0]);
            Assert.EndsWith("きである", lines[0]);

            Assert.StartsWith($"{preMessage}と認められ", lines[1]);
            Assert.EndsWith("ものから、", lines[1]);

            Assert.StartsWith($"{preMessage} 美しいラ", lines[2]);
            Assert.EndsWith("れた女は", lines[2]);

            Assert.StartsWith($"{preMessage}、琵琶湖", lines[3]);
            Assert.EndsWith("）になった", lines[3]);

            Assert.StartsWith($"{preMessage}竜女は、", lines[4]);
            Assert.EndsWith("といいます。", lines[4]);
        }

        [Fact]
        public void TestMultiLinePrivMsgMessageTokens_With4ByteEncodedCharacters()
        {
            var target = "WiZ";
            var message = "😀😃😄😁😆😅🤣😂🙂🙃😉😊😇🥰😍🤩😘😗😚😙😋😛😜🤪😝🤑🤗🤭🤫🤔🤐🤨😐😑😶😶‍🌫😶‍🌫😏😒🙄😬😮‍💨🤥😌😔😪🤤😴😷🤒🤕🤢🤮🤧🥵🥶🥴😵😵‍💫🤯🤠🥳😎🤓🧐😕😟🙁☹️😮😯😲😳🥺😦😧😨😰😥😢😭😱😖😣😞😓😩😫🥱😤😡😠🤬😈👿💀☠️☠💩🤡👹👺👻👽👾🤖😺😸😹😻😼😽🙀😿😾🙈🙉🙊💋💌💘💝💖💗💓💞💕💟❣️💔❤️‍🔥❤‍🔥❤️‍🩹❤‍🩹❤️❤🧡💛💚💙💜🤎🖤🤍💯💢💥💫💦💨🕳️🕳💣💬👁️‍🗨️👁‍🗨️👁️‍🗨👁‍🗨🗨️🗨🗯️🗯💭💤👋👋🏻👋🏼👋🏽👋🏾👋🏿🤚🤚🏻🤚🏼🤚🏽🤚🏾🤚🏿🖐️🖐🖐🏻🖐🏼🖐🏽🖐🏾🖐🏿✋✋🏻✋🏼✋🏽✋🏾✋🏿🖖🖖🏻🖖🏼🖖🏽🖖🏾🖖🏿👌👌🏻👌🏼👌🏽👌🏾👌🏿🤏🤏🏻🤏🏼🤏🏽🤏🏾🤏🏿✌️✌✌🏻✌🏼✌🏽✌🏾✌🏿🤞🤞🏻🤞🏼🤞🏽🤞🏾🤞🏿🤟🤟🏻🤟🏼🤟🏽🤟🏾🤟🏿🤘🤘🏻🤘🏼🤘🏽🤘🏾🤘🏿🤙🤙🏻🤙🏼🤙🏽🤙🏾🤙🏿👈👈🏻👈🏼👈🏽👈🏾👈🏿👉👉🏻👉🏼👉🏽👉🏾👉🏿👆👆🏻👆🏼👆🏽👆🏾👆🏿👇👇🏻👇🏼👇🏽👇🏾👇🏿☝️☝☝🏻☝🏼☝🏽☝🏾☝🏿👍👍🏻👍🏼👍🏽👍🏾👍🏿";
            var privMsgMessage = new PrivMsgMessage(target, message);

            var lines = privMsgMessage.ToString().Split(Constants.CrLf);

            Assert.Equal(4, lines.Length);
        }
    }
}
