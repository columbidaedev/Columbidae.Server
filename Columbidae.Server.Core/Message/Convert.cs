using Columbidae.Message;
using Google.Protobuf.WellKnownTypes;
using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
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
            Url = entity.ImageUrl,
            Caption = entity.ToPreviewText()
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
            Url = entity.VideoUrl,
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

    public static FaceEntity GetFaceEntity(this FrameCreator frame)
    {
        return new FaceEntity((ushort)frame.Emoji.Emoji, frame.Emoji.IsLarge);
    }

    public static ForwardEntity GetForwardEntity(this FrameCreator frame)
    {
        return new ForwardEntity
        {
            Time = frame.Reply.Time.ToDateTime(),
            Sequence = (uint)frame.Reply.ReplySeq,
            TargetUin = frame.Reply.ReplyUin
        };
    }

    public static MentionEntity GetMentionEntity(this FrameCreator frame)
    {
        return new MentionEntity
        {
            Name = frame.Mention.Nick,
            Uid = frame.Mention.Uid,
            Uin = frame.Mention.Uin
        };
    }

    public static TextEntity GetTextEntity(this FrameCreator frame)
    {
        return new TextEntity(frame.Text.Text);
    }

    public static XmlEntity GetXmlEntity(this FrameCreator frame)
    {
        return new XmlEntity(frame.Xml.Xml);
    }

    public static JsonEntity GetJsonEntity(this FrameCreator frame)
    {
        return new JsonEntity(frame.Json.Json, frame.Json.ResId);
    }

    public static async Task<ImageEntity> GetImageEntity(this FrameCreator frame, ColumbidaeContext context)
    {
        var cache = context.MessageCaches.GetPrior();
        if (cache == null)
        {
            throw new NullReferenceException("No message cache registered");
        }

        var stream = await cache.CreateResourceSender(frame.File.Token);
        return new ImageEntity(stream);
    }

    public static async Task<FileEntity> GetFileEntity(this FrameCreator frame, ColumbidaeContext context,
        MessageCreator message, BotContext bot)
    {
        var cache = context.MessageCaches.GetPrior();
        if (cache == null)
        {
            throw new NullReferenceException("No message cache registered");
        }

        var file = await cache.GetResourcePath(frame.File.Token);
        var entity = new FileEntity(file);
        switch (message.Type)
        {
            case CMsgType.Friend:
                await bot.UploadFriendFile(message.Destination, entity);
                break;
            case CMsgType.Group:
                await bot.GroupFSUpload(message.Destination, entity);
                break;
        }

        return entity;
    }

    public static async Task<VideoEntity> GetVideoEntity(this FrameCreator frame, ColumbidaeContext context)
    {
        var cache = context.MessageCaches.GetPrior();
        if (cache == null)
        {
            throw new NullReferenceException("No message cache registered");
        }

        var file = await cache.GetResourcePath(frame.Video.Token);
        return new VideoEntity(file);
    }

    public static async Task<IMessageEntity> GetMessageEntity(this FrameCreator frame, ColumbidaeContext context,
        BotContext bot, MessageCreator message)
    {
        switch (frame.Type)
        {
            case Frame.Types.FrameType.Emoji:
                return frame.GetFaceEntity();
            case Frame.Types.FrameType.File:
                return await frame.GetFileEntity(context, message, bot);
            case Frame.Types.FrameType.Reply:
                return frame.GetForwardEntity();
            case Frame.Types.FrameType.Picture:
                return await frame.GetImageEntity(context);
            case Frame.Types.FrameType.Mention:
                return frame.GetMentionEntity();
            case Frame.Types.FrameType.Forward:
                return frame.GetForwardEntity();
            case Frame.Types.FrameType.Text:
                return frame.GetTextEntity();
            case Frame.Types.FrameType.Video:
                return await frame.GetVideoEntity(context);
            case Frame.Types.FrameType.Xml:
                return frame.GetXmlEntity();
            case Frame.Types.FrameType.Json:
                return frame.GetJsonEntity();
            default:
                throw new ArgumentOutOfRangeException(nameof(frame.Type), frame.Type, "Frame type is not supported");
        }
    }

    public static async Task<MessageChain> GetMessageChain(this MessageCreator message, ColumbidaeContext context,
        BotContext bot)
    {
        var builder = MessageBuilder.Group(message.Group);
        foreach (var frameCreator in message.Frames)
        {
            builder.Add(await frameCreator.GetMessageEntity(context, bot, message));
        }

        return builder.Build();
    }
}