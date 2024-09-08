using Columbidae.Message;
using Google.Protobuf.WellKnownTypes;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;

namespace Columbidae.Server.Core.Message;

using CMsg = Columbidae.Message.Message;
using CMsgType = Columbidae.Message.Message.Types.MessageType;

public static class Convert
{
    public static CMsg ToCMsg(this MessageChain chain)
    {
        var msg = new CMsg
        {
            Id = chain.MessageId,
            Frames = { chain.Select(ToFrame) },
            Time = Timestamp.FromDateTime(chain.Time),
            Type = chain.Type.ToCMsgType(),
            Sender = chain.FriendUin,
            Destination = chain.TargetUin
        };
        if (chain.GroupUin.HasValue) msg.Group = chain.GroupUin.Value;

        return msg;
    }

    public static CMsgType ToCMsgType(this MessageChain.MessageType type)
    {
        return type switch
        {
            MessageChain.MessageType.Group => CMsgType.Group,
            MessageChain.MessageType.Temp => CMsgType.Temp,
            MessageChain.MessageType.Friend => CMsgType.Friend,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static EmojiFrame ToEmojiFrame(this FaceEntity entity)
    {
        return new EmojiFrame
        {
            Emoji = entity.FaceId,
            IsLarge = entity.IsLargeFace
        };
    }

    public static FileFrame ToFileFrame(this FileEntity entity)
    {
        return new FileFrame
        {
            Name = entity.FileName,
            Url = entity.FileUrl,
            Size = entity.FileSize
        };
    }

    public static ReplyFrame ToReplyFrame(this ForwardEntity entity)
    {
        return new ReplyFrame
        {
            ReplySeq = entity.Sequence,
            ReplyUin = entity.TargetUin,
            SenderUid = entity.Uid
        };
    }

    public static ImageFrame ToImageFrame(this ImageEntity entity)
    {
        return new ImageFrame
        {
            Height = (uint)entity.PictureSize.Y,
            Width = (uint)entity.PictureSize.X,
            Size = entity.ImageSize,
            Url = entity.ImageUrl
        };
    }

    public static JsonFrame ToJsonFrame(this JsonEntity entity)
    {
        return new JsonFrame
        {
            ResId = entity.ResId,
            Json = entity.Json
        };
    }

    public static MentionFrame ToMentionFrame(this MentionEntity entity)
    {
        return new MentionFrame
        {
            Nick = entity.Name,
            Uid = entity.Uid,
            Uin = entity.Uin
        };
    }

    public static ForwardFrame ToForwardFrame(this MultiMsgEntity entity)
    {
        return new ForwardFrame
        {
            Messages = { entity.Chains.Select(ToCMsg) },
            GroupUin = entity.GroupUin ?? 0
        };
    }

    public static TextFrame ToTextFrame(this TextEntity entity)
    {
        return new TextFrame
        {
            Text = entity.Text
        };
    }

    public static VideoFrame ToVideoFrame(this VideoEntity entity)
    {
        return new VideoFrame
        {
            Width = (uint)entity.Size.X,
            Height = (uint)entity.Size.Y,
            Size = (uint)entity.VideoSize,
            Url = entity.VideoUrl
        };
    }

    public static XmlFrame ToXmlFrame(this XmlEntity entity)
    {
        return new XmlFrame
        {
            Xml = entity.Xml
        };
    }

    public static Frame ToFrame(this IMessageEntity entity)
    {
        return entity switch
        {
            FaceEntity face => new Frame { Emoji = face.ToEmojiFrame() },
            ForwardEntity forward => new Frame { Reply = forward.ToReplyFrame() },
            FileEntity file => new Frame { File = file.ToFileFrame() },
            ImageEntity image => new Frame { Image = image.ToImageFrame() },
            JsonEntity json => new Frame { Json = json.ToJsonFrame() },
            MentionEntity mention => new Frame { Mention = mention.ToMentionFrame() },
            MultiMsgEntity multi => new Frame { Forward = multi.ToForwardFrame() },
            TextEntity text => new Frame { Text = text.ToTextFrame() },
            VideoEntity video => new Frame { Video = video.ToVideoFrame() },
            XmlEntity xml => new Frame { Xml = xml.ToXmlFrame() },
            _ => throw new NotImplementedException($"Type {entity.GetType().Name} is not supported")
        };
    }
}