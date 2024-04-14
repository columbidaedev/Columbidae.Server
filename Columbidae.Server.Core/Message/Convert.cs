using Columbidae.Message;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;

namespace Columbidae.Server.Core.Message;

using CMsg = Columbidae.Message.Message;

public static class Convert
{
    public static CMsg ToCMsg(this MessageChain chain)
    {
        return new CMsg
        {
            Id = chain.MessageId,
            Frames = { chain.Select(ToFrame) }
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
            Size = entity.FileSize,
        };
    }

    public static ReplyFrame ToReplyFrame(this ForwardEntity entity)
    {
        return new ReplyFrame
        {
            ReplySeq = entity.Sequence,
            ReplyUin = entity.TargetUin,
            SenderUid = entity.Uid,
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
        if (entity is FaceEntity face)
        {
            return new Frame
            {
                Emoji = face.ToEmojiFrame()
            };
        }

        if (entity is ForwardEntity forward)
        {
            return new Frame
            {
                Reply = forward.ToReplyFrame()
            };
        }

        if (entity is FileEntity file)
        {
            return new Frame
            {
                File = file.ToFileFrame()
            };
        }

        if (entity is ImageEntity image)
        {
            return new Frame
            {
                Image = image.ToImageFrame()
            };
        }

        if (entity is JsonEntity json)
        {
            return new Frame
            {
                Json = json.ToJsonFrame()
            };
        }

        if (entity is MentionEntity mention)
        {
            return new Frame
            {
                Mention = mention.ToMentionFrame()
            };
        }

        if (entity is MultiMsgEntity multi)
        {
            return new Frame
            {
                Forward = multi.ToForwardFrame()
            };
        }

        if (entity is TextEntity text)
        {
            return new Frame
            {
                Text = text.ToTextFrame()
            };
        }

        if (entity is VideoEntity video)
        {
            return new Frame
            {
                Video = video.ToVideoFrame()
            };
        }

        if (entity is XmlEntity xml)
        {
            return new Frame
            {
                Xml = xml.ToXmlFrame()
            };
        }

        throw new NotImplementedException($"Type {entity.GetType().Name} is not supported");
    }
}